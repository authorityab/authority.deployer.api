using System;
using System.Collections.Generic;
using System.Web.Caching;

namespace DeployerServices.Classes
{
    public interface ICacheManager
    {
        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns>Returns true or false</returns>
        bool Exists(string key);

        /// <summary>
        /// Find all cache items (of type T) whose key contains the supplied key
        /// </summary>
        /// <typeparam name="T">Type of item to get from cache</typeparam>
        /// <param name="search">the key to search for in cache keys</param>
        /// <param name="result">items are delivered in a dictionary</param>
        /// <returns>returns true if any items are found, false otherwise</returns>
        bool Find<T>(string search, out Dictionary<string, T> result);

        /// <summary>
        /// Finds all cache keys that contain the specified search term.
        /// </summary>
        /// <param name="search">The search term.</param>
        /// <returns>A list of all cache keys containing the search term.</returns>
        List<string> FindKeysLike(string search);

        /// <summary>
        /// Finds all cache keys containing the specified search term.
        /// </summary>
        /// <param name="search">The search term.</param>
        /// <param name="result">A list of the found cache keys.</param>
        /// <returns>True if any keys were found, otherwise false.</returns>
        bool FindKeysLike(string search, out List<string> result);

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
        TResult GetAndCache<TResult>(string key, int seconds, Func<TResult> retrieverDelegate,
            CacheItemPriority prio = CacheItemPriority.Normal);

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        void Remove(string key);

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        void Remove(string[] key);
    }
}
