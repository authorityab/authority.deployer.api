﻿using System.Web.Mvc;
using log4net;

namespace Authority.Deployer.Api.Controllers
{
    public class TestController : Controller
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(TestController));

        public ActionResult Index()
        {
            //var tcService = new TeamCityService();
            //var octopusService = new OctopusService();

            //var a = octopusService.GetReleasesFromProject("projects-33");


            //var b = octopusService.GetEnvironmentsFromLifecycle();

            //var allBuilds = tcService.GetAllBuilds();
            //var latestBuild = allBuilds.OrderByDescending(x => x.FinishDate).FirstOrDefault();

            //var debugViewModel = new DebugViewModel
            //{
            //    OctopusProjects = octopusService.GetDashboardDynamic().Projects,
            //    TeamCityBuilds = allBuilds,
            //    LatestBuild = latestBuild
            //};

            //string buildDestroyer;
            //var latestFailedBuild = tcService.GetLatestFailedBuild(out buildDestroyer);

            //debugViewModel.LatestFailedBuild = latestFailedBuild;
            //debugViewModel.BuildDestroyer = buildDestroyer;

            return View();
        }
    }
}
