using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using log4net;
using Newtonsoft.Json;
using TeamCitySharp;
using TeamCitySharp.DomainEntities;

namespace DeployerServices.Services
{
    public class TeamCityService
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

        private static IEnumerable<string> GetProjectsIdsFromConfig()
        {
            return ConfigurationManager.AppSettings["TeamCityMonitorProjects"].Split(',');
        }

        public List<Project> GetAllProjects()
        {
            try
            {
                var projectsIds = GetProjectsIdsFromConfig();
                var projects = _client.Projects.All();

                return (from projectId in projectsIds 
                        from project in projects 
                        where project.Id == projectId select project)
                        .ToList();
            }
            catch (JsonException ex)
            {
                _log.Error("Get all projects failed.", ex);
            }

            return null;
        }

        public List<Build> GetAllBuilds()
        {
            try
            {
                var projects = GetAllProjects();
                var builds = new List<Build>();
                foreach (var project in projects)
                {
                    var p = _client.Projects.ById(project.Id);
                    if (p.BuildTypes != null)
                    {
                        foreach (var buildId in p.BuildTypes.BuildType.Select(x => x.Id))
                        {
                            var build = _client.Builds.LastBuildByBuildConfigId(buildId);
                            if (build != null)
                            {

                                var buildConfig = _client.BuildConfigs.ByConfigurationId(build.BuildTypeId);
                                build.BuildConfig = buildConfig;

                                var buildDestroyer = "Anonymous";
                                var lastChange = _client.Changes.LastChangeDetailByBuildConfigId(build.BuildTypeId);
                                if (lastChange != null && lastChange.User != null)
                                {
                                    buildDestroyer = lastChange.User.Name;

                                   
                                    //buildDestroyer = lastChange.User.Name;
                                }
                                build.Changes.Change = new List<Change> {new Change {Username = buildDestroyer}};

                                builds.Add(build);
                            }
                        }
                    }
                }
                return builds;
            }
            catch (JsonException ex)
            {
                _log.Error("Get all projects failed.", ex);
            }

            return new List<Build>();
        }


        public Build GetLatestFailedBuild(out string buildDestroyer)
        {
            buildDestroyer = "Anonymous";

            try
            {
                var projects = GetAllProjects();

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
            catch (JsonException ex)
            {
                _log.Error(string.Format("Get latest build failed."), ex);
            }

            return null;
        }
    }
}