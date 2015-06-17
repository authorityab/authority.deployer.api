using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Configuration;
using DeployerServices.Models;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeployerServices.Services
{
    public class TeamCityService
    {
        private string Username { get; set; }
        private string Password { get; set; }
        private string ApiUrl { get; set; }

        private readonly ILog _log = LogManager.GetLogger(typeof(TeamCityService));


        public TeamCityService()
        {
            Username = WebConfigurationManager.AppSettings["TeamCityUsername"];
            Password = WebConfigurationManager.AppSettings["TeamCityPassword"];
            ApiUrl = WebConfigurationManager.AppSettings["TeamCityApiUrl"];
        }

        public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public string Request(Uri uri)
        {
            try
            {


                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        GetCredentials());
                    httpClient.BaseAddress = uri;
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    var response = httpClient.GetAsync(uri).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }

                    _log.Error(string.Format("Something went wrong with the request. {0}", response.RequestMessage));
                }
            }
            catch (Exception e)
            {
                _log.Error(string.Format("Something went wrong with the request. {0}", e));
            }
            return null;
        }
        private string GetCredentials()
        {
            return Convert.ToBase64String(new ASCIIEncoding().GetBytes(Username + ":" + Password));
        }

        public string[] GetProjectsIdsFromConfig()
        {
            return ConfigurationManager.AppSettings["TeamCityMonitorProjects"].Split(',');
        }

        public string[] GetBuildConfigIdsFromConfig()
        {
            return ConfigurationManager.AppSettings["TeamCityMonitorBuilds"].Split(',');
        }

        public List<TeamCityProject> GetAllProjects()
        {
            try
            {
                var uri = new Uri(string.Format("{0}/projects", ApiUrl));
                var result = Request(uri);
                var root = JObject.Parse(result);
                var serializer = new JsonSerializer();

                return serializer.Deserialize<List<TeamCityProject>>(root["project"].CreateReader());
            }
            catch (JsonException ex)
            {
                _log.Error("Get all projects failed.", ex);
            }

            return null;
        }

        public TeamCityBuild GetLatestBuildByProjectId(string projectId)
        {
            try
            {
                var uri = new Uri(string.Format("{0}/builds/?locator=project:(id:{1})&count=1", ApiUrl, projectId));
                var result = Request(uri);
                var root = JObject.Parse(result);
                var serializer = new JsonSerializer();


                var build = serializer.Deserialize<List<TeamCityBuild>>(root["build"].CreateReader()).FirstOrDefault();

                return build;
            }
            catch (JsonException ex)
            {
                _log.Error(string.Format("Get latest build by project id {0} failed.", projectId), ex);
            }

            return null;
        }
        public TeamCityBuildInfo GetBuildInfo(string buildConfigId)
        {
            try
            {
                var uri = new Uri(string.Format("{0}/buildTypes/id:{1}/builds/running:false?count=1&start=0", ApiUrl, buildConfigId));
                var result = Request(uri);
                var root = JObject.Parse(result);
                var serializer = new JsonSerializer();


                var build = serializer.Deserialize<TeamCityBuildInfo>(root.CreateReader());

                return build;
            }
            catch (JsonException ex)
            {
                _log.Error(string.Format("Get build info with build config id {0} failed.", buildConfigId), ex);
            }

            return null;
        }
    }
}