﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using Authority.Deployer.Api.Classes;
using Authority.Deployer.Api.Services.Contracts;
using log4net;
using TeamCitySharp;
using TeamCitySharp.DomainEntities;

namespace Authority.Deployer.Api.Services
{
    public class TeamCityService : ITeamCityService
    {
        private string Username { get; }
        private string Password { get; }
        private string TeamCityUrl { get; }

        private readonly TeamCityClient _client;

        private readonly ILog _log = LogManager.GetLogger(typeof(TeamCityService));

        public TeamCityService()
        {
            Username = WebConfigurationManager.AppSettings["TeamCityUsername"];
            Password = WebConfigurationManager.AppSettings["TeamCityPassword"];
            TeamCityUrl = WebConfigurationManager.AppSettings["TeamCityUrl"];

            _client = new TeamCityClient(TeamCityUrl); ;
            _client.Connect(Username, Password);
        }

        public TeamCityService(string username, string password, string tcUrl)
        {
            _client = new TeamCityClient(tcUrl);
            _client.Connect(username, password);
        }

        public List<Models.Build> GetAllBuilds()
        {
            try
            {
                var projects = _client.Projects.All();
                var builds = new List<Api.Models.Build>();
                foreach (var project in projects)
                {
                    var p = _client.Projects.ById(project.Id);
                    if (p.BuildTypes != null)
                    {
                        foreach (var buildId in p.BuildTypes.BuildType.Select(x => x.Id))
                        {
                            var tcBuild = _client.Builds.LastBuildByBuildConfigId(buildId);
                            if (tcBuild != null)
                            {
                                var build = new Models.Build();

                                var buildConfig = _client.BuildConfigs.ByConfigurationId(tcBuild.BuildTypeId);
                                
                                build.Id = tcBuild.Id;
                                build.Number = tcBuild.Number;
                                build.ProjectId = buildConfig.ProjectId;
                                build.ProjectName = buildConfig.ProjectName;
                                build.StepName = buildConfig.Name;
                                build.Status = tcBuild.Status;
                                build.FinishDate = tcBuild.FinishDate;
                                build.WebUrl = tcBuild.WebUrl;
                                build.Href = tcBuild.Href;
                                build.BuildConfigWebUrl = buildConfig.WebUrl;
                                build.BuildConfigId = buildConfig.Id;
                                build.BuildTypeId = tcBuild.BuildTypeId;

                                var comment = "";
                                var lastModifiedBy = "Anonymous";
                                var lastChange = _client.Changes.LastChangeDetailByBuildConfigId(tcBuild.BuildTypeId);
                                if (lastChange != null)
                                {
                                    build.Comment = lastChange.Comment ?? "";
                                    if (lastChange.User != null)
                                    {
                                        lastModifiedBy = lastChange.User.Name;
                                    }
                                    var change = _client.Changes.ByChangeId(lastChange.Id);
                                    if (change != null)
                                    {
                                        comment = change.Comment;
                                    }
                                }

                                build.LastBuild =
                                    $"Last Build: {tcBuild.FinishDate.ToString("dd MMMM yyyy HH:mm")}, {lastModifiedBy}";

                                build.LastModifiedBy = lastModifiedBy;
                                build.Comment = comment;
                                
                                builds.Add(build);
                            }
                        }
                    }
                }
                return builds.OrderByDescending(x => x.FinishDate).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Get all builds from TC failed.", ex);
            }

            return new List<Models.Build>();
        }

        public Models.Build GetLatestFailedBuild()
        {
            var buildDestroyer = "Anonymous";
            var comment = "";

            try
            {
                var projects = _client.Projects.All();

                var failedBuilds = new List<Build>();
                foreach (var project in projects)
                {
                    var p = _client.Projects.ById(project.Id);
                    if (p.BuildTypes != null)
                    {
                        failedBuilds
                            .AddRange(p.BuildTypes.BuildType.Select(x => x.Id)
                            .Select(buildId => _client.Builds.LastFailedBuildByBuildConfigId(buildId))
                            .Where(failedBuild => failedBuild != null));
                    }
                }

                var lastFailedBuild = failedBuilds.OrderByDescending(x => x.FinishDate).First();

                var buildConfig = _client.BuildConfigs.ByConfigurationId(lastFailedBuild.BuildTypeId);
                lastFailedBuild.BuildConfig = buildConfig;

           
                
                var lastChange = _client.Changes.LastChangeDetailByBuildConfigId(lastFailedBuild.BuildTypeId);
                if (lastChange != null)
                {
                    if (lastChange.User != null)
                    {
                        buildDestroyer = lastChange.User.Name;
                    }
                    var change = _client.Changes.ByChangeId(lastChange.Id); 
                    if (change != null)
                    {
                        comment = change.Comment;
                    }
                }

                var build = new Models.Build
                {
                    Id = lastFailedBuild.Id,
                    Number = lastFailedBuild.Number,
                    FinishDate = lastFailedBuild.FinishDate,
                    LastModifiedBy = buildDestroyer,
                    ProjectId = lastFailedBuild.BuildConfig.ProjectId,
                    ProjectName = lastFailedBuild.BuildConfig.ProjectName,
                    StepName = lastFailedBuild.BuildConfig.Name,
                    WebUrl = lastFailedBuild.WebUrl,
                    Href = lastFailedBuild.Href,
                    BuildConfigId = lastFailedBuild.BuildConfig.Id,
                    BuildTypeId = lastFailedBuild.BuildTypeId,
                    Comment = comment,
                    Status = BuildStatus.Failure.ToString()
                };

                return build;
            }
            catch (Exception ex)
            {
                _log.Error("Get latest failed build from TC failed.", ex);
            }

            return null;
        }
    }
}