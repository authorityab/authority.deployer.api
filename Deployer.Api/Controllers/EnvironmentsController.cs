using System.Web.Http;
using System.Web.Http.Results;
using Authority.Deployer.Api.Models;
using Authority.Deployer.Api.Services.Contracts;

namespace Authority.Deployer.Api.Controllers
{
    public class EnvironmentsController : ApiController
    {
        private readonly IOctopusService _octopusService;

        public EnvironmentsController(IOctopusService octopusService)
        {
            _octopusService = octopusService;
        }

        public JsonResult<EnvironmentPage> Get(string projectId, string releaseId)
        {
            var environmentPage = _octopusService.GetEnvironmentPage(projectId, releaseId);

            return Json(environmentPage);
        }
    }
}