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

            var allBuilds = tcService.GetAllBuilds();
            var latestBuild = allBuilds.OrderByDescending(x => x.FinishDate).FirstOrDefault();

            var debugViewModel = new DebugViewModel
            {
                OctopusProjects = octopusService.GetDashboardDynamic().Projects,
                TeamCityBuilds = allBuilds,
                LatestBuild = latestBuild
            };

            string buildDestroyer;
            var latestFailedBuild = tcService.GetLatestFailedBuild(out buildDestroyer);

            debugViewModel.LatestFailedBuild = latestFailedBuild;
            debugViewModel.BuildDestroyer = buildDestroyer;

            return View(debugViewModel);
        }
    }
}
