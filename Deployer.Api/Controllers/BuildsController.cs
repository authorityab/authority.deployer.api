using System.Web.Http;
using Authority.Deployer.Api.Services.Contracts;
using Newtonsoft.Json;

namespace Authority.Deployer.Api.Controllers
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
            var latestFailedBuuild = _teamCityService.GetLatestFailedBuild();
            
            return JsonConvert.SerializeObject(latestFailedBuuild);
        }
    }
}
