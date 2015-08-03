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
            string id = data.id;
            var octopusService = new OctopusService();
            var taskId = octopusService.ReleaseTheCracken(id);

            return taskId;
        }

        [HttpGet]
        public string Status(string id)
        {
            var octopusService = new OctopusService();

            var task = octopusService.GetTaskProgress(id);

            return JsonConvert.SerializeObject(task);
        }
    }
}
