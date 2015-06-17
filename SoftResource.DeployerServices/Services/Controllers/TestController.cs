using System.Linq;
using System.Web.Mvc;
using DeployerServices.Models.ViewModels;
using DeployerServices.Services;

namespace DeployerServices.Controllers
{
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            var tcService = new TeamCityService();
            var octopusService = new OctopusService();

            var projectsIds = tcService.GetProjectsIdsFromConfig();
            var buildConfigIds = tcService.GetBuildConfigIdsFromConfig();

            var debugViewModel = new DebugViewModel();
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
                    debugViewModel.TeamCityBuilds.Add(teamCityBuild);
                }
            }

            debugViewModel.OctopusProjects = octopusService.GetAllProjects();
            
            return View(debugViewModel);
        }
    }
}
