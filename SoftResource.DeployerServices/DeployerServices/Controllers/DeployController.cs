using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class DeployController : ApiController
    {
        [HttpPost]
        public string Post([FromBody]dynamic data)
        {
            string projectId = data.projectId;
            string releaseId = data.releaseId;
            string environmentId = data.environmentId;


            var octopusService = new OctopusService();
            var taskId = octopusService.ReleaseTheCracken(projectId, releaseId, environmentId);

            return taskId;
        }

        [HttpGet]
        public string Status(string taskId)
        {
            var octopusService = new OctopusService();

            var task = octopusService.GetTaskProgress(taskId);

            return JsonConvert.SerializeObject(task);
        }
    }
}
