using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class BuildsController : ApiController
    {
        public string Get()
        {
            var tcService = new TeamCityService();

            var builds = tcService.GetAllBuilds();

            return JsonConvert.SerializeObject(builds);
        }

        [HttpGet]
        public string LatestFailed()
        {
            var tcService = new TeamCityService();

            string buildDestroyer;
            var latestFailed = tcService.GetLatestFailedBuild(out buildDestroyer);

            var build = new
            {
                Build = latestFailed,
                BuildDestroyer = buildDestroyer
            };


            return JsonConvert.SerializeObject(build);
        }
    }
}
