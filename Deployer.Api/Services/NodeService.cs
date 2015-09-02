using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using Deployer.Api.Models;
using Deployer.Api.Services.Contracts;
using log4net;
using Newtonsoft.Json;

namespace Deployer.Api.Services
{
    public class NodeService : INodeService
    {
        private readonly string _baseUrl;
        private readonly ILog _log = LogManager.GetLogger(typeof(NodeService));

        public NodeService()
        {
            _baseUrl = WebConfigurationManager.AppSettings["NodeUrl"];
        }

        public NodeService(string url)
        {
            _baseUrl = url;
        }

        public bool PostBuilds(List<Build> builds)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(string.Format("{0}/{1}", _baseUrl, "setbuilds"),
                        new StringContent(JsonConvert.SerializeObject(builds), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic content = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                        var isSuccess = bool.Parse(content.success.ToString());

                        return isSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Post builds failed.", ex);
            }

            return false;
        }

        public bool PostLatestBuild(Build build)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(string.Format("{0}/{1}", _baseUrl, "setlatestbuild"),
                        new StringContent(JsonConvert.SerializeObject(build), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic content = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                        var isSuccess = bool.Parse(content.success.ToString());

                        return isSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Post latest build failed.", ex);
            }

            return false;
        }

        public bool PostLatestFailedBuild(Build build)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(string.Format("{0}/{1}", _baseUrl, "setlatestfailedbuild"),
                        new StringContent(JsonConvert.SerializeObject(build), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic content = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                        var isSuccess = bool.Parse(content.success.ToString());

                        return isSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Post latest failed failed.", ex);
            }

            return false;
        }
    }
}