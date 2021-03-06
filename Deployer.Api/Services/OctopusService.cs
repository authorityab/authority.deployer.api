﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Authority.Deployer.Api.Classes;
using Authority.Deployer.Api.Models;
using Authority.Deployer.Api.Services.Contracts;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Environment = Authority.Deployer.Api.Models.Environment;

namespace Authority.Deployer.Api.Services
{
    public class OctopusService : IOctopusService
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
         *  Authority.Web - Develop: projects-33 | DEV - TST
         *  Authority.Web - Master: projects-225 | UAT - PRO
         *  
         */

        private string ServerUrl { get; set; }
        private string ApiKey { get; set; }
        private readonly OctopusRepository _repository;
        private readonly ICacheManager _cahManager;

        private readonly ILog _log = LogManager.GetLogger(typeof(OctopusService));

        public OctopusService(ICacheManager cacheManager)
        {
            ServerUrl = ConfigurationManager.AppSettings["OctopusServerUrl"];
            ApiKey = ConfigurationManager.AppSettings["OctopusApiKey"];

            var endpoint = new OctopusServerEndpoint(ServerUrl, ApiKey);
            _repository = new OctopusRepository(endpoint);

            _cahManager = cacheManager;
        }

        #region Public Methods

        public List<Project> GetAllProjects()
        {
            try
            {
                var projectGroups = GetAllProjectGroups();
                var key = string.Format(CacheKeys.AllProjects);
                return _cahManager.GetAndCache(
                     key,
                     900,
                     () =>
                     {
                         var projectList = new List<Project>();
                         var projects = _repository.Projects.FindAll();
                         foreach (var project in projects)
                         {
                             var p = new Project(project.Id)
                             {
                                 Name = project.Name,
                                 Description = project.Description,
                             };

                             var p2 = project;
                             var projectGroup = projectGroups.FirstOrDefault(x => x.Id == p2.ProjectGroupId);

                             p.GroupName = projectGroup != null ? projectGroup.Name : string.Empty;

                             projectList.Add(p);
                         }

                         return projectList.OrderBy(x => x.GroupName).ThenBy(x => x.Name).ToList();
                     });
            }
            catch (Exception e)
            {
                _log.Error("Get all projects failed.", e);
            }

            return null;
        }

        public Releases GetReleases(string projectId)
        {
            try
            {
                var project = GetProject(projectId);
                
                var environments = GetEnvironmentsFromProject(projectId);
                var latestDeploys = GetLatestDeploys(projectId);
                var releases = GetReleasesFromProject(project);

                var releasePage = new Releases
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
                            $"Assembled: {release.Assembled.ToString("dd MMMM yyyy HH:mm")}{(string.IsNullOrEmpty(release.LastModifiedBy) ? string.Empty : ", " + release.LastModifiedBy)}",
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

                    releasePage.Items.Add(r);
                }

                return releasePage;
            }
            catch (Exception e)
            {
                _log.Error($"Get release page failed. ProjectId: {projectId}", e);
            }

            return null;
        }

        public Environments GetEnvironments(string projectId, string releaseId)
        {
            try
            {
                var selectedRelease = GetRelease(releaseId);

                var project = GetProject(projectId);
                var environments = GetEnvironmentsFromProject(projectId);
                var latestDeploys = GetLatestDeploys(projectId);
                var tasks = GetTasksFromLatestDeploys(projectId);

                var environmentPage = new Environments
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

                        environment.LastDeploy = $"Last Deploy: {deploy.Created.ToString("dd MMMM yyyy HH:mm")}";

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
                        environment.LastDeploy = "The project has never been deployed to this environment";
                        environment.ReleaseVersion = string.Empty;
                        environment.Status = DeployStatus.NotDeployed;
                    }

                    environmentPage.Items.Add(environment);
                }

                return environmentPage;

            }
            catch (Exception e)
            {
                _log.Error($"Get environment page failed. ProjectId: {projectId}, ReleaseId: {releaseId}", e);
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
                            : $"Last Deploy: {task.CompletedTime.Value.ToString("dd MMMM yyyy HH:mm")}",
                    State = task.State.ToString(),
                    FinishedSuccessfully = task.FinishedSuccessfully,
                    HasWarningOrErrors = task.HasWarningsOrErrors,
                    IsCompleted = task.IsCompleted
                };

                return deployTask;
            }
            catch (Exception e)
            {
                _log.Error($"Polling of task failed. TaskId: {taskId}", e);
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

                _cahManager.Remove($"{CacheKeys.LatestDeploysFromProject}_{projectId}");

                return deployment.TaskId;
            }
            catch (Exception e)
            {
                _log.Error(
                    $"The release of the Cracken failed. ProjectId: {projectId}, ReleaseId: {releaseId}, environmentId: {environmentId}", e);
            }

            return null;
        }

        #endregion Public Methods

        #region Private Methods

        private ProjectResource GetProject(string projectId)
        {
            try
            {
                return _repository.Projects.Get(projectId);
            }
            catch (Exception e)
            {
                _log.Error($"Get project failed. Id: {projectId}", e);
            }

            return null;
        }

        private ReleaseResource GetRelease(string releaseId)
        {
            try
            {
                return _repository.Releases.Get(releaseId);
            }
            catch (Exception e)
            {
                _log.Error($"Get release failed. ReleaseId: {releaseId}", e);
            }

            return null;
        }

        private IEnumerable<ReleaseResource> GetReleasesFromProject(ProjectResource project)
        {
            try
            {
                //TODO: Maybe do a Take() or paginate?
                return _repository.Projects.GetReleases(project).Items;
            }
            catch (Exception e)
            {
                _log.Error($"Get releases from project failed. ProjectId: {project.Id}", e);
            }

            return null;
        }

        private List<EnvironmentResource> GetEnvironmentsFromProject(string projectId)
        {
            try
            {
                var key = $"{CacheKeys.EnvironmentsFromProject}_{projectId}";
                var envList = _cahManager.GetAndCache(
                    key,
                    900,
                    () =>
                    {
                        var environments = _repository.Environments.FindAll();
                        var lifecycleId = _repository.Projects.Get(projectId).LifecycleId;

                        var phases = _repository.Lifecycles.Get(lifecycleId).Phases;
                        var envIds = new List<string>();
                        foreach (var phase in phases.Where(phase => phase.OptionalDeploymentTargets != null))
                        {
                            envIds.AddRange(phase.OptionalDeploymentTargets.ToList());
                        }

                        return environments.Where(env => envIds.Any(x => x == env.Id)).ToList();
                    });

                return envList;
            }
            catch (Exception e)
            {
                _log.Error($"Get environments from project failed. ProjectId: {projectId}", e);
            }

            return null;
        }

        private List<ProjectGroupResource> GetAllProjectGroups()
        {
            try
            {
                var key = string.Format(CacheKeys.AllProjectGroups);
                var projectGroups = _cahManager.GetAndCache(
                    key,
                    900,
                    () => _repository.ProjectGroups.FindAll());

                return projectGroups;
            }
            catch (Exception e)
            {
                _log.Error("Get all project groups failed.", e);
            }

            return null;
        }

        private List<DeploymentResource> GetLatestDeploys(string projectId)
        {
            try
            {
                var key = $"{CacheKeys.LatestDeployTaskFromProject}_{projectId}";
                var deploys = _cahManager.GetAndCache(
                    key,
                    900,
                    () =>
                    {
                        var environments = GetEnvironmentsFromProject(projectId);
                        var latestDeploys = new List<DeploymentResource>();
                        foreach (var env in environments)
                        {
                            var deployments = _repository.Deployments.FindAll(new[] { projectId }, new[] { env.Id });
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
                _log.Error($"Get latest deploys failed. ProjectId: {projectId}", e);
            }

            return null;
        }

        private List<TaskResource> GetTasksFromLatestDeploys(string projectId)
        {
            try
            {
                var key = $"{CacheKeys.LatestDeployTaskFromProject}_{projectId}";
                var taskList = _cahManager.GetAndCache(
                    key,
                    900,
                    () =>
                    {
                        var deploys = GetLatestDeploys(projectId);
                        return deploys.Select(deploy => _repository.Tasks.Get(deploy.TaskId)).ToList();
                    });

                return taskList;
            }
            catch (Exception e)
            {
                _log.Error($"Get tasks from latest deploys failed. ProjectId: {projectId}", e);
            }

            return null;
        }

        #endregion Private Methods
    }
}