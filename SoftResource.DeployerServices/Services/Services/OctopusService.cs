using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Configuration;
using DeployerServices.Models;
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

        public string ReleaseTheCracken(string projectId)
        {
            //TODO : Remove hardcode string
            projectId = "projects-33";
            var endpoint = new OctopusServerEndpoint(ServerUrl, ApiKey);
            var repository = new OctopusRepository(endpoint);

            var items = GetDashboardDynamic().Items;
            if (items.Any())
            {
                var item = GetDashboardDynamic().Items.FirstOrDefault(x => x.EnvironmentId == "Environments-1" && x.ProjectId == projectId);
                if (item != null)
                {
                    var deploymentResource = new DeploymentResource
                    {
                        ProjectId = item.ProjectId, 
                        EnvironmentId = "Environments-2",
                        ReleaseId = item.ReleaseId
                    };

                    var deployment = repository.Deployments.Create(deploymentResource);

                    return deployment.TaskId;
                }
            }

            return string.Empty;
        }

        public TaskResource GetTaskProgress(string taskId)
        {
            TaskResource task = null;
            try
            {
                var endpoint = new OctopusServerEndpoint(ServerUrl, ApiKey);
                var repository = new OctopusRepository(endpoint);
                task = repository.Tasks.Get(taskId);
            }
            catch (Exception ex)
            {
            }

            return task;
        }
    }
}