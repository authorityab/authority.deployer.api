using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using Authority.Deployer.Api.Services.Contracts;
using log4net;
using TeamCitySharp;
using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;
using TeamCitySharp.Locators.TeamCitySharp.Locators;
using BuildStatus = TeamCitySharp.Locators.BuildStatus;

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
                var builds = new List<Models.Build>();
                foreach (var project in projects)
                {
                    var p = _client.Projects.ById(project.Id);
                    if (p.BuildTypes != null)
                    {
                        foreach (var buildId in p.BuildTypes.BuildType.Select(x => x.Id))
                        {
                            var tcBuilds = _client.Builds.ByBuildLocator(new FluidBuildLocator()
                                .WithBuildType(FluidBuildTypeLocator.WithId(buildId))
                                .WithBranch(new FluidBranchLocator().WithDefault(BranchLocatorFlag.Any)));

                            if (tcBuilds != null && tcBuilds.Any())
                            {
                                var tcBuild = tcBuilds.First();
                                if (tcBuild != null)
                                {
                                    var build = PopulateBuild(tcBuild);
                                    builds.Add(build);
                                }
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
            try
            {
                var projects = _client.Projects.All();

                var failedBuilds = new List<Build>();
                foreach (var project in projects)
                {
                    var p = _client.Projects.ById(project.Id);
                    if (p.BuildTypes != null)
                    {
                        foreach (var buildId in p.BuildTypes.BuildType.Select(x => x.Id))
                        {
                            failedBuilds.AddRange(_client.Builds.ByBuildLocator(new FluidBuildLocator()
                                .WithBuildType(FluidBuildTypeLocator.WithId(buildId))
                                .WithBranch(new FluidBranchLocator().WithDefault(BranchLocatorFlag.Any))
                                .WithSinceDate(DateTime.Now.AddDays(-14))
                                .WithStatus(BuildStatus.FAILURE)));
                        }
                    }
                }

                if (failedBuilds.Any())
                {
                    var buildList = new List<Models.Build>();
                    foreach (var failedBuild in failedBuilds)
                    {
                        var build = PopulateBuild(failedBuild);
                        buildList.Add(build);
                    }

                    var lastFailedBuild = buildList.OrderByDescending(x => x.FinishDate).First();
                    
                    return lastFailedBuild;
                }
               
            }
            catch (Exception ex)
            {
                _log.Error("Get latest failed build from TC failed.", ex);
            }

            return null;
        }

        private Models.Build PopulateBuild(Build tcBuild)
        {
            var build = new Models.Build();

            var buildConfig = _client.BuildConfigs.ByConfigurationId(tcBuild.BuildTypeId);

            build.Id = tcBuild.Id;
            build.Number = tcBuild.Number;
            build.ProjectId = buildConfig.ProjectId;
            build.ProjectName = buildConfig.ProjectName;
            build.StepName = buildConfig.Name;
            build.Status = tcBuild.Status;
            build.WebUrl = tcBuild.WebUrl;
            build.Href = tcBuild.Href;
            build.BuildConfigWebUrl = buildConfig.WebUrl;
            build.BuildConfigId = buildConfig.Id;
            build.BuildTypeId = tcBuild.BuildTypeId;

            var buildInfo = _client.Builds.ByBuildId(tcBuild.Id);
            if (buildInfo != null)
            {
                build.FinishDate = buildInfo.FinishDate;
                build.FinishDateFormat = buildInfo.FinishDate.ToString("dd MMMM yyyy HH:mm");
            }

            FillLastChange(build);

            return build;
        }

        private void FillLastChange(Models.Build build)
        {
            var lastChanges = _client.Changes.ByBuildId(int.Parse(build.Id));

            if (lastChanges != null && lastChanges.Any())
            {
                var lastChange = lastChanges.First();

                build.LastModifiedBy = lastChange.Username;
                if (lastChange.User != null)
                {
                    build.LastModifiedBy = lastChange.User.Name;
                }

                build.Comment = lastChange.Comment?.Trim();
            }
            else
            {
                var lastChange = _client.Changes.LastChangeDetailByBuildConfigId(build.BuildConfigId);
                if (lastChange != null)
                {
                    build.LastModifiedBy = lastChange.Username;
                    if (lastChange.User != null)
                    {
                        build.LastModifiedBy = lastChange.User.Name;
                    }

                    build.Comment = lastChange.Comment?.Trim();
                }
            }

            build.LastBuild = $"Last Build: {build.FinishDateFormat}, {build.LastModifiedBy}";
        }
    }
}