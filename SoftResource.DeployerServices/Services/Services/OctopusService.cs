using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Platform.Model;
using Octopus.Platform.Variables;
using Octopus.Platform.Web;

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
        private readonly OctopusRepository _repository;

        private readonly ILog _log = LogManager.GetLogger(typeof (OctopusService));
        
        public OctopusService()
        {
            ServerUrl = ConfigurationManager.AppSettings["OctopusServerUrl"];
            ApiKey = ConfigurationManager.AppSettings["OctopusApiKey"];

            var endpoint = new OctopusServerEndpoint(ServerUrl, ApiKey);
            _repository = new OctopusRepository(endpoint);
        }

        public string[] GetProjectIdsFromConfig()
        {
            return WebConfigurationManager.AppSettings["OctopusProjectsIds"].Split(',');
        }

        public string GetDevEnvironmentId()
        {
            return WebConfigurationManager.AppSettings["OctopusDevEnvironmentId"];
        }

        public string GetTstEnvironmentId()
        {
            return WebConfigurationManager.AppSettings["OctopusTstEnvironmentId"];
        }

        public string GetUatEnvironmentId()
        {
            return WebConfigurationManager.AppSettings["OctopusUatEnvironmentId"];
        }

        public string GetProEnvironmentId()
        {
            return WebConfigurationManager.AppSettings["OctopusProEnvironmentId"];
        }

        public string GetProLifecycleId()
        {
            return WebConfigurationManager.AppSettings["OctopusProLifecycleId"];
        }

        public string GetDevLifecycleId()
        {
            return WebConfigurationManager.AppSettings["OctopusDevLifecycleId"];
        }

        public List<ProjectResource> GetAllProjects()
        {
            try
            {
                return _repository.Projects.FindAll();
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
                
                var projectsIds = GetProjectIdsFromConfig();
                var envIds = GetDevEnvironmentId();

                var dash = _repository.Dashboards.GetDynamicDashboard(projectsIds, new[] { envIds });

                return dash;
            }
            catch (Exception e)
            {
                _log.Error("Get dashboard dynamic failed.", e);
            }

            return null;
        }

        public List<ProjectResource> GetProjectsFromLifecycle()
        {
            try
            {
                var lifecycleId = GetDevLifecycleId();

                var projects = _repository.Projects.FindAll()
                    .Where(x => x.LifecycleId == lifecycleId)
                    .ToList();

                return projects;
            }
            catch (Exception e)
            {
                _log.Error("Get projects from lifecycle failed.", e);
            }

            return null;
        }


        public List<ReleaseResource> GetReleasesFromProject(string projectId)
        {
            try
            {
                //TODO: Maybe do a Take() or paginate?
                var releases = _repository.Releases.FindMany(x => x.ProjectId == projectId);

                return releases;
            }
            catch (Exception e)
            {
                _log.Error("Get releases from project failed.", e);
            }

            return null;
        }

        public List<string> GetEnvironmentIdsFromLifecycle()
        {
            try
            {
                var phases = _repository.Lifecycles.Get(GetDevLifecycleId()).Phases;
                var environments = new List<string>();
                foreach (var phase in phases.Where(phase => phase.OptionalDeploymentTargets != null))
                {
                    environments.AddRange(phase.OptionalDeploymentTargets.ToList());
                }

                return environments;
            }
            catch (Exception e)
            {
                _log.Error("Get environments from lifecycle failed.", e);
            }

            return null;
        }


        public List<EnvironmentResource> GetEnvironmentsFromLifecycle()
        {
            try
            {
                var phases = _repository.Lifecycles.Get(GetDevLifecycleId()).Phases;
                var envIds = new List<string>();
                foreach (var phase in phases.Where(phase => phase.OptionalDeploymentTargets != null))
                {
                    envIds.AddRange(phase.OptionalDeploymentTargets.ToList());
                }

                var environments = _repository.Environments.FindAll();
                var envList = environments.Where(env => envIds.Any(x => x == env.Id)).ToList();

                return envList;
            }
            catch (Exception e)
            {
                _log.Error("Get environments from lifecycle failed.", e);
            }

            return null;
        }

        public string ReleaseTheCracken(string projectId, string releaseId, string environmentId)
        {
            try
            {
                // TODO: Find a way to not have this two environments hard coded!
                //var sourceEnvId = GetDevEnvironmentId();
                //var destinationEnvId = GetProEnvironmentId();

                //var deployments = _repository.Deployments.FindAll(new[] {projectId}, new[] {sourceEnvId}).Items;

                //var items = GetDashboardDynamic().Items;
                //if (deployments.Any())
                //{
                    //var item = deployments.FirstOrDefault(); //(x => x.EnvironmentId == sourceEnvId && x.ProjectId == projectId);
                    //if (item != null)
                    //{
                        var deploymentResource = new DeploymentResource
                        {
                            ProjectId = projectId,
                            EnvironmentId = environmentId,
                            ReleaseId = releaseId
                        };

                        var deployment = _repository.Deployments.Create(deploymentResource);

                        return deployment.TaskId;
                    //}
                //}
            }
            catch (Exception e)
            {
                _log.Error("The release of the Cracken failed.", e);
            }

            return null;
        }


        public List<DeploymentResource> GetLatestDeploys(string projectId)
        {
            try
            {
                var environments = GetEnvironmentIdsFromLifecycle();
                var latestDeploys = new List<DeploymentResource>();
                foreach (var env in environments)
                {
                    var deployments = _repository.Deployments.FindAll(new[] { projectId }, new[] { env });
                    if (deployments != null && deployments.TotalResults > 0)
                    {
                        var deploy = deployments.Items.FirstOrDefault();
                        latestDeploys.Add(deploy);
                    }
                } 

                //var devDeployments = _repository.Deployments.FindAll(new[] { projectId }, new[] { GetDevEnvironmentId() });
                //if (devDeployments != null && devDeployments.TotalResults > 0)
                //{
                //    var deploy = devDeployments.Items.FirstOrDefault();
                //    var environment = _repository.Environments.Get(deploy.EnvironmentId);
                //    latestDeploys.Add(string.Format("{0} - {1}", environment.Name, environment.Description), deploy);
                //}

                //var tstDeployments = _repository.Deployments.FindAll(new[] { projectId }, new[] { GetTstEnvironmentId() });
                //if (tstDeployments != null && tstDeployments.TotalResults > 0)
                //{
                //    var deploy = tstDeployments.Items.FirstOrDefault();
                //    var environment = _repository.Environments.Get(deploy.EnvironmentId);
                //    latestDeploys.Add(string.Format("{0} - {1}", environment.Name, environment.Description), deploy);
                //}

                return latestDeploys;
            }
            catch (Exception e)
            {
                _log.Error("Get latest deploys failed.", e);
            }

            return null;

        }

        public TaskResource GetTaskProgress(string taskId)
        {
            try
            {
                var task = _repository.Tasks.Get(taskId);

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