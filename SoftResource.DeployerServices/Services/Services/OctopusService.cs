using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;

namespace DeployerServices.Services
{
    public class OctopusService
    {
        private string ServerUrl { get; set; } 
        private string ApiKey { get; set; }

        private readonly ILog _log = LogManager.GetLogger(typeof (OctopusService));
        
        public OctopusService()
        {
            ServerUrl = ConfigurationManager.AppSettings["OctopusServerUrl"];
            ApiKey = ConfigurationManager.AppSettings["OctopusApiKey"];
        }

        public string[] GetProjectIdsFromConfig()
        {
            return WebConfigurationManager.AppSettings["OctopusProjectsIds"].Split(',');
        }

        public string[] GetEnvironmentIdsFromConfig()
        {
            return WebConfigurationManager.AppSettings["OctopusEnvironmentIds"].Split(',');
        }

        public List<ProjectResource> GetAllProjects()
        {

            try
            {
                var endpoint = new OctopusServerEndpoint(ServerUrl, ApiKey);
                var repository = new OctopusRepository(endpoint);

                return repository.Projects.FindAll();
            }
            catch (Exception e)
            {
                _log.Error("Get all projects failed.", e);
            }

            return null;
        }

        public DashboardResource GetDashboardDynamic()
        {
            try
            {
                var endpoint = new OctopusServerEndpoint(ServerUrl, ApiKey);
                var repository = new OctopusRepository(endpoint);

                var projectsIds = GetProjectIdsFromConfig();
                var envIds = GetEnvironmentIdsFromConfig();

                var dash = repository.Dashboards.GetDynamicDashboard(projectsIds, envIds);

                return dash;
            }
            catch (Exception e)
            {
                _log.Error("Get dashboard dynamic failed.", e);
            }

            return null;
        }
    }
}