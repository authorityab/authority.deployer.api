using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Deployer.Api.Classes
{
    /// <summary>
    /// Contains methods to make caching easier.
    /// </summary>
    public class CacheManager : ICacheManager
    {
        // Check out: http://stackoverflow.com/questions/5578744/doing-locking-in-asp-net-correctly
        public static readonly ConcurrentDictionary<string, object> MiniLocks = new ConcurrentDictionary<string, object>();

        //private static readonly Logger Log = new Logger(typeof(CacheManager));

        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns>Returns true or false</returns>
        public bool Exists(string key)
        {
            return HttpRuntime.Cache.Get(key) != null;
        }

        /// <summary>
        /// Find all cache items (of type T) whose key contains the supplied key
        /// </summary>
        /// <typeparam name="T">Type of item to get from cache</typeparam>
        /// <param name="search">the key to search for in cache keys</param>
        /// <param name="result">items are delivered in a dictionary</param>
        /// <returns>returns true if any items are found, false otherwise</returns>
        public bool Find<T>(string search, out Dictionary<string, T> result)
        {
            result = new Dictionary<string, T>();

            foreach (DictionaryEntry de in HttpRuntime.Cache)
            {
                if (de.Key.ToString().Contains(search))
                {
                    result.Add(de.Key.ToString(), (T)de.Value);
                }
            }

            return result.Count > 0;
        }

        /// <summary>
        /// Finds all cache keys that contain the specified search term.
        /// </summary>
        /// <param name="search">The search term.</param>
        /// <returns>A list of all cache keys containing the search term.</returns>
        public List<string> FindKeysLike(string search)
        {
            List<string> result;
            FindKeysLike(search, out result);

            return result;
        }

        /// <summary>
        /// Finds all cache keys containing the specified search term.
        /// </summary>
        /// <param name="search">The search term.</param>
        /// <param name="result">A list of the found cache keys.</param>
        /// <returns>True if any keys were found, otherwise false.</returns>
        public bool FindKeysLike(string search, out List<string> result)
        {
            result = new List<string>();

            foreach (DictionaryEntry de in HttpRuntime.Cache)
            {
                if (de.Key.ToString().Contains(search))
                {
                    result.Add(de.Key.ToString());
                }
            }

            return result.Count > 0;
        }

        /// <summary>
        /// Gets a value from the cache based on its key. If it does not exist, the provided
        /// function is used to retrieve the value instead, and then the value is put in the
        /// cache and returned.
        /// </summary>
        /// <typeparam name="TResult">The type of value that is cached.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="seconds">The number of seconds the value is cached.</param>
        /// <param name="retrieverDelegate">The function that is used to retrieve the value if it's not already cached.</param>
        /// <param name="prio">CacheItemPriority for the item.</param>
        /// <returns>The value retrieved either from cache or using the provided function.</returns>
        public TResult GetAndCache<TResult>(string key, int seconds, Func<TResult> retrieverDelegate, CacheItemPriority prio = CacheItemPriority.Normal)
        {
            TResult results;
            Get(key, out results);

            if (object.Equals(results, default(TResult)))
            {
                object miniLock = MiniLocks.GetOrAdd(key, k => new object());

                lock (miniLock)
                {
                    Get(key, out results);

                    if (object.Equals(results, default(TResult)))
                    {
                        results = retrieverDelegate();

                        // Possibly check if TResult is nullable (default value check won't work for referencetypes, 0 could be a return value we want cached) and null instead of Try-statement
                        try
                        {
                            Add(key, results, seconds, prio);
                        }
                        catch (Exception ex)
                        {
                            //Log.AddLogentry(LogLevel.Info, "Cant add null value to cache " + ex);
                        }
                    }

                    // Saving some space
                    object temp1;

                    if (MiniLocks.TryGetValue(key, out temp1) && (temp1 == miniLock))
                    {
                        object temp2;
                        MiniLocks.TryRemove(key, out temp2);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        public void Remove(string key)
        {
            Remove(new[] { key });
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        public void Remove(string[] key)
        {
            key.ToList().ForEach(k => HttpRuntime.Cache.Remove(k));
        }

        /// <summary>
        /// Insert value into the cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of item</param>
        /// <param name="item">Item to be cached</param>
        /// <param name="seconds">Seconds to cache the item</param>
        /// <param name="prio">CacheItemPriority for item</param>
        private static void Add<T>(string key, T item, int seconds, CacheItemPriority prio = CacheItemPriority.Normal)
        {
            HttpRuntime.Cache.Insert(
                key,
                item,
                null,
                DateTime.Now.AddSeconds(seconds),
                System.Web.Caching.Cache.NoSlidingExpiration,
                prio,
                null);
        }

        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <param name="value">Cached value. Default(T) if
        /// item doesn't exist.</param>
        /// <returns>Cached item as type</returns>
        private bool Get<T>(string key, out T value)
        {
            try
            {
                if (!Exists(key))
                {
                    value = default(T);
                    return false;
                }

                value = (T)HttpRuntime.Cache.Get(key);
            }
            catch
            {
                value = default(T);
                return false;
            }

            return true;
        }
    }
}