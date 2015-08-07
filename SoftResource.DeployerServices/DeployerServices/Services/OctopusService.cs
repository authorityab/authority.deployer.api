using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using DeployerServices.Classes;
using DeployerServices.Models;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Environment = DeployerServices.Models.Environment;

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
        private readonly CacheManager _cahManager = new CacheManager();

        private readonly ILog _log = LogManager.GetLogger(typeof(OctopusService));

        public OctopusService()
        {
            ServerUrl = ConfigurationManager.AppSettings["OctopusServerUrl"];
            ApiKey = ConfigurationManager.AppSettings["OctopusApiKey"];

            var endpoint = new OctopusServerEndpoint(ServerUrl, ApiKey);
            _repository = new OctopusRepository(endpoint);
        }

        #region Private Methods

        private static string GetProLifecycleId()
        {
            return WebConfigurationManager.AppSettings["OctopusProLifecycleId"];
        }

        private static string GetDevLifecycleId()
        {
            return WebConfigurationManager.AppSettings["OctopusDevLifecycleId"];
        }

        private IEnumerable<ReleaseResource> GetReleasesFromProject(string projectId)
        {
            try
            {
                //TODO: Maybe do a Take() or paginate?
                var key = string.Format("{0}_{1}", CacheKeys.ReleasesFromProject, projectId);
                var releases = _cahManager.GetAndCache(
                    key,
                    900,
                    () => _repository.Releases.FindMany(x => x.ProjectId == projectId));

                return releases;
            }
            catch (Exception e)
            {
                _log.Error("Get releases from project failed.", e);
            }

            return null;
        }

        private IEnumerable<string> GetEnvironmentIdsFromLifecycle()
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

        private List<EnvironmentResource> GetEnvironmentsFromLifecycle()
        {
            try
            {
                var key = string.Format(CacheKeys.LifecycleEnvironments);
                var envList = _cahManager.GetAndCache(
                    key,
                    900,
                    () =>
                    {
                        var phases = _repository.Lifecycles.Get(GetDevLifecycleId()).Phases;
                        var envIds = new List<string>();
                        foreach (var phase in phases.Where(phase => phase.OptionalDeploymentTargets != null))
                        {
                            envIds.AddRange(phase.OptionalDeploymentTargets.ToList());
                        }

                        var environments = _repository.Environments.FindAll();
                        return environments.Where(env => envIds.Any(x => x == env.Id)).ToList();
                    });

                return envList;
            }
            catch (Exception e)
            {
                _log.Error("Get environments from lifecycle failed.", e);
            }

            return null;
        }

        #endregion Private Methods

        #region Public Methods

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

        public ProjectResource GetProject(string projectId)
        {
            try
            {
                return _repository.Projects.Get(projectId);
            }
            catch (Exception e)
            {
                _log.Error(string.Format("Get project failed. Id: {0}", projectId),  e);
            }

            return null;
        }

        public ReleaseResource GetRelease(string releaseId)
        {
            try
            {
                return _repository.Releases.Get(releaseId);
            }
            catch (Exception e)
            {
                _log.Error(string.Format("Get release failed. ReleaseId: {0}", releaseId), e);
            }

            return null;
        }

        public List<ProjectResource> GetProjectsFromLifecycle()
        {
            try
            {
                var key = string.Format(CacheKeys.LifecycleProjects);
                var projects = _cahManager.GetAndCache(
                    key,
                    900,
                    () =>
                    {
                        var lifecycleId = GetDevLifecycleId();


                        return _repository.Projects.FindAll()
                            .Where(x => x.LifecycleId == lifecycleId)
                            .ToList(); ;
                    });

                return projects;
            }
            catch (Exception e)
            {
                _log.Error("Get projects from lifecycle failed.", e);
            }

            return null;
        }

        public string ReleaseTheCracken(string projectId, string releaseId, string environmentId)
        {
            try
            {
                var deploymentResource = new DeploymentResource
                {
                    ProjectId = projectId,
                    EnvironmentId = environmentId,
                    ReleaseId = releaseId
                };

                var deployment = _repository.Deployments.Create(deploymentResource);

                return deployment.TaskId;
            }
            catch (Exception e)
            {
                _log.Error(string.Format("The release of the Cracken failed. ProjectId: {0}, ReleaseId: {1}, environmentId: {2}", projectId, releaseId, environmentId), e);
            }

            return null;
        }

        public List<DeploymentResource> GetLatestDeploys(string projectId)
        {
            try
            {
                var key = string.Format("{0}_{1}", CacheKeys.LatestDeployTaskFromProject, projectId);
                var deploys = _cahManager.GetAndCache(
                    key,
                    900,
                    () =>
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

                        return latestDeploys;
                    });

                return deploys;

            }
            catch (Exception e)
            {
                _log.Error(string.Format("Get latest deploys failed. ProjectId: {0}", projectId), e);
            }

            return null;
        }

        public List<TaskResource> GetTasksFromLatestDeploys(string projectId)
        {
            try
            {
                var key = string.Format("{0}_{1}", CacheKeys.LatestDeployTaskFromProject, projectId);
                var taskList = _cahManager.GetAndCache(
                    key,
                    900,
                    () =>
                    {
                        var deploys = GetLatestDeploys(projectId);


                        var tasks = new List<TaskResource>();
                        foreach (var deploy in deploys)
                        {
                            var task = _repository.Tasks.Get(deploy.TaskId);
                            tasks.Add(task);
                        }

                        return tasks;
                    });

                return taskList;
            }
            catch (Exception e)
            {
                _log.Error(string.Format("Get tasks from latest deploys failed. ProjectId: {0}", projectId), e);
            }

            return null;
        }

        public DeployTask GetTaskProgress(string taskId)
        {
            try
            {
                var task = _repository.Tasks.Get(taskId);

                var deployTask = new DeployTask
                {
                    Id = taskId,
                    CompletedTime =
                        !task.CompletedTime.HasValue
                            ? string.Empty
                            : string.Format("Last Deploy: {0}", task.CompletedTime.Value.ToString("dd MMMM yyyy HH:mm")),
                    State = task.State.ToString(),
                    FinishedSuccessfully = task.FinishedSuccessfully,
                    HasWarningOrErrors = task.HasWarningsOrErrors,
                    IsCompleted = task.IsCompleted
                };

                return deployTask;
            }
            catch (Exception e)
            {
                _log.Error(string.Format("Polling of task failed. TaskId: {0}", taskId), e);
            }

            return null;
        }

        public ReleasePage GetReleasePage(string projectId)
        {
            try
            {
                var project = GetProject(projectId);
                var environments = GetEnvironmentsFromLifecycle();
                var releases = GetReleasesFromProject(projectId);
                var latestDeploys = GetLatestDeploys(projectId);

                var releasePage = new ReleasePage
                {
                    ProjectName = project.Name,
                    ProjectDescription = project.Description
                };

                foreach (var release in releases)
                {
                    var r = new Release
                    {
                        Id = release.Id,
                        Version = release.Version,
                        Assembled =
                            string.Format("Assembled: {0}, {1}", release.Assembled.ToString("dd MMMM yyyy HH:mm"), release.LastModifiedBy),
                        ReleaseNotes = release.ReleaseNotes
                    };

                    foreach (var deploy in latestDeploys)
                    {
                        if (deploy.ReleaseId == release.Id)
                        {
                            foreach (var env in environments)
                            {
                                if (deploy.EnvironmentId == env.Id)
                                {
                                    r.DeployedTo.Add(env.Name);
                                }
                            }
                        }
                    }

                    releasePage.Releases.Add(r);
                }

                return releasePage;
            }
            catch (Exception e)
            {
                _log.Error(string.Format("Get release page failed. ProjectId: {0}", projectId), e);
            }

            return null;
        }

        public EnvironmentPage GetEnvironmentPage(string projectId, string releaseId)
        {
            try
            {
                var selectedRelease = GetRelease(releaseId);

                var project = GetProject(projectId);
                var environments = GetEnvironmentsFromLifecycle();
                var latestDeploys = GetLatestDeploys(projectId);
                var tasks = GetTasksFromLatestDeploys(projectId);

                var environmentPage = new EnvironmentPage
                {
                    ProjectName = project.Name,
                    ReleaseVersion = selectedRelease.Version
                };

                foreach (var env in environments)
                {
                    var environment = new Environment()
                    {
                        Id = env.Id,
                        Name = env.Name
                    };

                    if (latestDeploys.Any(x => x.EnvironmentId == env.Id))
                    {
                        var deploy = latestDeploys.Find(x => x.EnvironmentId == env.Id);

                        environment.LastDeploy = string.Format("Last Deploy: {0}",
                            deploy.Created.ToString("dd MMMM yyyy HH:mm"));

                        var release = _repository.Releases.Get(deploy.ReleaseId);
                        environment.ReleaseVersion = release.Version;

                        foreach (var task in tasks)
                        {
                            if (deploy.TaskId == task.Id)
                            {
                                if (task.IsCompleted && task.FinishedSuccessfully && !task.HasWarningsOrErrors)
                                {
                                    environment.Status = DeployStatus.Success;
                                }
                                else
                                {
                                    environment.Status = DeployStatus.Fail;
                                }
                            }
                        }
                    }
                    else
                    {
                        environment.LastDeploy = "The selected project has never been deployed to this environment";
                        environment.ReleaseVersion = string.Empty;
                        environment.Status = DeployStatus.NotDeployed;
                    }

                    environmentPage.Environments.Add(environment);
                }

                return environmentPage;

            }
            catch (Exception e)
            {
                _log.Error(string.Format("Get environment page failed. ProjectId: {0}, ReleaseId: {1}", projectId, releaseId), e);
            }

            return null;
        }

        #endregion Public Methods

       


    }
}