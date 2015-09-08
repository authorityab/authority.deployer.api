
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Authority.Deployer.Api.Models;
using Authority.Deployer.Api.Services.Contracts;

namespace Authority.Deployer.Api.Controllers
{
    public class BuildsController : ApiController
    {
        private readonly ITeamCityService _teamCityService;

        public BuildsController(ITeamCityService teamCityService)
        {
            _teamCityService = teamCityService;
        }

        public JsonResult<List<Build>> Get()
        {
            var builds = _teamCityService.GetAllBuilds();

            return Json(builds);
        }

        [System.Web.Http.HttpGet]
        public JsonResult<Build> LatestFailed()
        {
            var latestFailedBuuild = _teamCityService.GetLatestFailedBuild();

            return Json(latestFailedBuuild);
        }
    }
}
