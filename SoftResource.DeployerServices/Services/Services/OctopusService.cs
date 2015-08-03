using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;

namespace DeployerServices.Services
{
    public class OctopusService
    {
        /*
         * Octopus Environment ID
         * 
         *  DEV: Environments-1
         *  TST: Environments-2
         *  UAT: Environments-3
         *  PRO: Environments-4
         *  LAB: Environments-33
         *  
         */

        /*
         * Ocotpus Projects ID
         *  
         *  SoftResource.Web - Develop: projects-33 | DEV - TST
         *  SoftResource.Web - Master: projects-225 | UAT - PRO
         *  
         */

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

        public string GetDeployFromEnvironmentId()
        {
            return WebConfigurationManager.AppSettings["OctopusDeployFromEnvironmentId"];
        }

        public string GetDeployToEnvironmentId()
        {
            return WebConfigurationManager.AppSettings["OctopusDeployToEnvironmentId"];
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
                var envIds = GetDeployFromEnvironmentId();

                var dash = repository.Dashboards.GetDynamicDashboard(projectsIds, new[] { envIds });

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
            try
            {
                var endpoint = new OctopusServerEndpoint(ServerUrl, ApiKey);
                var repository = new OctopusRepository(endpoint);

                var sourceEnvId = GetDeployFromEnvironmentId();
                var destinationEnvId = GetDeployToEnvironmentId();

                var items = GetDashboardDynamic().Items;
                if (items.Any())
                {
                    var item = items.FirstOrDefault(x => x.EnvironmentId == sourceEnvId && x.ProjectId == projectId);
                    if (item != null)
                    {
                        var deploymentResource = new DeploymentResource
                        {
                            ProjectId = item.ProjectId,
                            EnvironmentId = destinationEnvId,
                            ReleaseId = item.ReleaseId
                        };

                        var deployment = repository.Deployments.Create(deploymentResource);

                        return deployment.TaskId;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error("The release of the Cracken failed.", e);
            }


            return null;
        }

        public TaskResource GetTaskProgress(string taskId)
        {
            
            try
            {
                var endpoint = new OctopusServerEndpoint(ServerUrl, ApiKey);
                var repository = new OctopusRepository(endpoint);
                var task = repository.Tasks.Get(taskId);

                return task;
            }
            catch (Exception e)
            {
                _log.Error("Polling of task failed.", e);
            }

            return null;

        }
    }
}