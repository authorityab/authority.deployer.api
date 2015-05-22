using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DeployerServices.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeployerServices.Services
{
    public class TeamCityService
    {
        public const string UserName = "TeamCityServiceUser";
        public const string Password = "superhemligt#3";
        public const string Url = "http://tc.softresourcehosting.com/httpAuth/app/rest";

        public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static string Request(string url)
        {
            var credentials = GetCredentials(UserName, Password);
            HttpWebResponse response = null;

            var sbSource = string.Empty;
            ServicePointManager.ServerCertificateValidationCallback = Validator;
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                if (request != null)
                {
                    request.MaximumAutomaticRedirections = 1;
                    request.AllowAutoRedirect = true;
                    // 2. It's important that both the Accept and ContentType headers are
                    // set in order for this to be interpreted as an API request.
                    request.Accept = "application/json";
                    request.ContentType = "application/json";
                    //request.UserAgent = "harvest_api_sample.cs";
                    // 3. Add the Basic Authentication header with username/password string.
                    request.Headers.Add("Authorization", "Basic " + credentials);

                    using (response = request.GetResponse() as HttpWebResponse)
                    {
                        if (request.HaveResponse && response != null)
                        {
                            var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                            sbSource = new StringBuilder(reader.ReadToEnd()).ToString();
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        return string.Format(
                              "The server returned '{0}' with the status code {1} ({2:d}).",
                              errorResponse.StatusDescription, errorResponse.StatusCode,
                              errorResponse.StatusCode);
                    }
                }
                else
                {
                    return wex.ToString();
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return sbSource;
        }

        private static string GetCredentials(string username, string password)
        {
            return Convert.ToBase64String(new ASCIIEncoding().GetBytes(username + ":" + password));
        }

        public string[] GetProjectsIdsFromConfig()
        {
            return ConfigurationManager.AppSettings["TeamCityMonitorProjects"].Split(',');
        }


        public List<TeamCityProject> GetAllProjects()
        {
            try
            {
                var uri = string.Format("{0}/projects", Url);
                var result = Request(uri);
                var root = JObject.Parse(result);
                var serializer = new JsonSerializer();

                return serializer.Deserialize<List<TeamCityProject>>(root["project"].CreateReader());
            }
            catch (JsonException ex)
            {
                return null;
            }
        }

        public TeamCityBuild GetLatestBuild(string projectId)
        {
            try
            {
                var uri = string.Format("{0}/builds/?locator=project:(id:{1})&count=1", Url, projectId);
                var result = Request(uri);
                var root = JObject.Parse(result);
                var serializer = new JsonSerializer();


                var build = serializer.Deserialize<List<TeamCityBuild>>(root["build"].CreateReader()).FirstOrDefault();

                return build;
            }
            catch (JsonException ex)
            {
                return null;
            }
        }

        public TeamCityBuildInfo GetBuildInfo(int buildId)
        {
            try
            {
                var uri = string.Format("{0}/builds/id:{1}", Url, buildId);
                var result = Request(uri);
                var root = JObject.Parse(result);
                var serializer = new JsonSerializer();


                var build = serializer.Deserialize<TeamCityBuildInfo>(root.CreateReader());

                return build;
            }
            catch (JsonException ex)
            {
                return null;
            }
        } 

    }
}