using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DeployerServices.Models.ViewModels;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class DeployerController : ApiController
    {
        //public const string DeployerUrl 

        public string GetBuildStatus()
        {
            var tcService = new TeamCityService();
            //var octopusService = new OctopusService();

            //var projectsIds = tcService.GetProjectsIdsFromConfig();
            var buildConfigIds = tcService.GetBuildConfigIdsFromConfig();

            var buildList = new List<TeamCityBuildViewModel>();
            foreach (var configId in buildConfigIds)
            {
                var buildInfo = tcService.GetBuildInfo(configId);
                if (buildInfo != null)
                {
                    //var buildInfo = tcService.GetBuildInfo(latestBuild.Id);
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

            //debugViewModel.OctopusProjects = octopusService.GetAllProjects();S
            return JsonConvert.SerializeObject(buildList);

            //return View(debugViewModel);
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
    }
}
