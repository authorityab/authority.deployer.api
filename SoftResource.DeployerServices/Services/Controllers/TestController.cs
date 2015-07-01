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


            tcService.GetAllProjects();

            var octopusService = new OctopusService();

            var buildConfigIds = tcService.GetBuildConfigIdsFromConfig();

            var debugViewModel = new DebugViewModel();
            foreach (var configId in buildConfigIds)
            {
                var buildInfo = tcService.GetBuildTypeInfo(configId);
                if (buildInfo != null)
                {
                    //var buildInfo = tcService.GetBuildInfo(latestBuild.Id);
                    var teamCityBuild = new TeamCityBuildViewModel
                    {
                        Version =  buildInfo.Version,
                        BuildStatus = buildInfo.Status,
                        ProjectId = buildInfo.BuildType.Id,
                        ProjectName = buildInfo.BuildType.ProjectName,
                        StartDate = buildInfo.StartDate,
                        FinishDate = buildInfo.FinishDate,
                        LastChangedBy = buildInfo.LastChanges.Count > 0
                            ? string.Join(", ", buildInfo.LastChanges.Changes.Select(x => x.Username))
                            : string.Empty
                    };
                    debugViewModel.TeamCityBuilds.Add(teamCityBuild);
                }
            }

            var latestFailed = tcService.GetLatestFailedBuild();
            var failedBuildInfo = tcService.GetBuildTypeInfo(latestFailed.Id);

            var latestFailedViewModel = new TeamCityBuildViewModel
            {
                Version = failedBuildInfo.Version,
                BuildStatus = failedBuildInfo.Status,
                ProjectId = failedBuildInfo.BuildType.Id,
                ProjectName = failedBuildInfo.BuildType.ProjectName,
                StartDate = failedBuildInfo.StartDate,
                FinishDate = failedBuildInfo.FinishDate,
                LastChangedBy = failedBuildInfo.LastChanges.Count > 0
                    ? string.Join(", ", failedBuildInfo.LastChanges.Changes.Select(x => x.Username))
                    : string.Empty
            };

            debugViewModel.LatestFailedBuild = latestFailedViewModel;
            debugViewModel.OctopusProjects = octopusService.GetAllProjects();
            
            return View(debugViewModel);
        }
    }
}
