using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class ReleasesController : ApiController
    {
        private readonly IOctopusService _octopusService;

        public ReleasesController(IOctopusService octopusService)
        {
            _octopusService = octopusService;
        }

        public string Get(string projectId)
        {
            var release = _octopusService.GetReleasePage(projectId);

            return JsonConvert.SerializeObject(release);
        }
    }
}