using System.Web.Http;
using Deployer.Api.Services;
using Deployer.Api.Services.Contracts;
using Newtonsoft.Json;

namespace Deployer.Api.Controllers
{
    public class EnvironmentsController : ApiController
    {
        private readonly IOctopusService _octopusService;

        public EnvironmentsController(IOctopusService octopusService)
        {
            _octopusService = octopusService;
        }

        public string Get(string projectId, string releaseId)
        {
            var environmentPage = _octopusService.GetEnvironmentPage(projectId, releaseId);

            return JsonConvert.SerializeObject(environmentPage);
        }
    }
}