using System.Web.Http;
using Deployer.Services.Services;
using Newtonsoft.Json;

namespace Deployer.Services.Controllers
{
    public class BuildsController : ApiController
    {
        private readonly ITeamCityService _teamCityService;

        public BuildsController(ITeamCityService teamCityService)
        {
            _teamCityService = teamCityService;
        }

        public string Get()
        {
            var builds = _teamCityService.GetAllBuilds();

            return JsonConvert.SerializeObject(builds);
        }

        [HttpGet]
        public string LatestFailed()
        {
            string buildDestroyer;
            var latestFailed = _teamCityService.GetLatestFailedBuild(out buildDestroyer);

            var build = new
            {
                Build = latestFailed,
                BuildDestroyer = buildDestroyer
            };


            return JsonConvert.SerializeObject(build);
        }
    }
}
