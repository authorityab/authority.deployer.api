using System.Web.Http;
using System.Web.Http.Results;
using Authority.Deployer.Api.Models;
using Authority.Deployer.Api.Services.Contracts;

namespace Authority.Deployer.Api.Controllers
{
    public class ReleasesController : ApiController
    {
        private readonly IOctopusService _octopusService;

        public ReleasesController(IOctopusService octopusService)
        {
            _octopusService = octopusService;
        }

        public JsonResult<Releases> Get(string projectId)
        {
            var release = _octopusService.GetReleases(projectId);

            return Json(release);
        }
    }
}