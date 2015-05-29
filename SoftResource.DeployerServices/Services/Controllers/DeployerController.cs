using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using DeployerServices.Models.ViewModels;
using DeployerServices.Services;
using Newtonsoft.Json;
using Octopus.Client;

namespace DeployerServices.Controllers
{
    public class DeployerController : ApiController
    {
        public string GetBuildStatus()
        {
            var tcService = new TeamCityService();
            var buildConfigIds = tcService.GetBuildConfigIdsFromConfig();
            var buildList = new List<TeamCityBuildViewModel>();
            foreach (var configId in buildConfigIds)
            {
                var buildInfo = tcService.GetBuildInfo(configId);
                if (buildInfo != null)
                {
                    var teamCityBuild = new TeamCityBuildViewModel
                    {
                        BuildStatus = buildInfo.Status,
                        ProjectId = buildInfo.BuildType.Id,
                        ProjectName = buildInfo.BuildType.ProjectName,
                        LastChangedBy = buildInfo.LastChanges.Count > 0
                            ? string.Join(", ", buildInfo.LastChanges.Changes.Select(x => x.Username))
                            : string.Empty
                    };
                    buildList.Add(teamCityBuild);
                }  
            }
            return JsonConvert.SerializeObject(buildList);
        }

        public string GetProjects()
        {
            var octopusService = new OctopusService();

            var projects =  octopusService.GetAllProjects();

            return JsonConvert.SerializeObject(projects);
        }

        public string GetDashboard()
        {
            var octopusService = new OctopusService();

            var dashboard = octopusService.GetDashboardDynamic();

            return JsonConvert.SerializeObject(dashboard);
        }

        public string GetTaskProgress(string taskId)
        {
            var octopusService = new OctopusService();

            return JsonConvert.SerializeObject(octopusService.GetTaskProgress(taskId));
        }

        public string TriggerDeploy(string projectId)
        {
            var octopusService = new OctopusService();
            var taskId = octopusService.ReleaseTheCracken(projectId);

            return taskId;
        }

    }
}
