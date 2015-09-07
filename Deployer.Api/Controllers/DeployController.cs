
using System.Web.Http;
using System.Web.Http.Results;
using Authority.Deployer.Api.Models;
using Authority.Deployer.Api.Services.Contracts;
using Newtonsoft.Json;

namespace Authority.Deployer.Api.Controllers
{
    public class DeployController : ApiController
    {
        private readonly IOctopusService _octopusService;

        public DeployController(IOctopusService octopusService)
        {
            _octopusService = octopusService;
        }

        [HttpPost]
        public string Post([FromBody]dynamic data)
        {
            string projectId = data.projectId;
            string releaseId = data.releaseId;
            string environmentId = data.environmentId;

            var taskId = _octopusService.ReleaseTheCracken(projectId, releaseId, environmentId);

            return taskId;
        }

        [HttpGet]
        public JsonResult<DeployTask> Status(string taskId)
        {
            var task = _octopusService.GetTaskProgress(taskId);

            return Json(task);
        }
    }
}
