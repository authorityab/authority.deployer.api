using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using log4net;
using TeamCitySharp;
using TeamCitySharp.DomainEntities;

namespace DeployerServices.Services
{
    public class TeamCityService : ITeamCityService
    {
        private string Username { get; set; }
        private string Password { get; set; }
        private string TeamCityUrl { get; set; }
        private string ApiUrl { get; set; }
        private readonly TeamCityClient _client;

        private readonly ILog _log = LogManager.GetLogger(typeof(TeamCityService));

        public TeamCityService()
        {
            Username = WebConfigurationManager.AppSettings["TeamCityUsername"];
            Password = WebConfigurationManager.AppSettings["TeamCityPassword"];
            ApiUrl = WebConfigurationManager.AppSettings["TeamCityApiUrl"];
            TeamCityUrl = WebConfigurationManager.AppSettings["TeamCityUrl"];

            _client = new TeamCityClient(TeamCityUrl); ;
            _client.Connect(Username, Password);
        }

        public List<Models.Build> GetAllBuilds()
        {
            try
            {
                var projects = _client.Projects.All();
                var builds = new List<Models.Build>();
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

                                build.ProjectName = buildConfig.ProjectName;
                                build.StepName = buildConfig.Name;
                                build.Status = tcBuild.Status;
                                build.FinishDate = tcBuild.FinishDate;

                                var lastModifiedBy = "Anonymous";
                                var lastChange = _client.Changes.LastChangeDetailByBuildConfigId(tcBuild.BuildTypeId);
                                if (lastChange != null && lastChange.User != null)
                                {
                                    lastModifiedBy = lastChange.User.Name;
                                }

                                build.LastBuild = string.Format("Last Build: {0}, {1}", tcBuild.FinishDate.ToString("dd MMMM yyyy HH:mm"),
                                    lastModifiedBy);

                                build.LastModifiedBy = lastModifiedBy;

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

        public Build GetLatestFailedBuild(out string buildDestroyer)
        {
            buildDestroyer = "Anonymous";

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
                if (lastChange != null && lastChange.User != null)
                {
                    buildDestroyer = lastChange.User.Name;
                }

                return lastFailedBuild;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Get latest failed build from TC failed."), ex);
            }

            return null;
        }
    }
}