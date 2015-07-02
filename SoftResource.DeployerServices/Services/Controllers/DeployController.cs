using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class DeployController : ApiController
    {
        public string Post(string id)
        {
            var octopusService = new OctopusService();
            var taskId = octopusService.ReleaseTheCracken(id);

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
