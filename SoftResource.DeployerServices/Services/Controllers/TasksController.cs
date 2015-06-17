using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class TasksController : ApiController
    {
        public string GetTask(string id)
        {
            var octopusService = new OctopusService();

            var task = octopusService.GetTaskProgress(id);

            return JsonConvert.SerializeObject(task);
        }
    }
}
