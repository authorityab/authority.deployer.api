using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class ProjectsController : ApiController
    {
        public string Get()
        {
            var octopusService = new OctopusService();

            var projects = octopusService.GetProjectsFromLifecycle();

            return JsonConvert.SerializeObject(projects);
        }

        [HttpGet]
        public string Info(string projectId)
        {
            var octopusService = new OctopusService();

            var project = octopusService.GetProject(projectId);

            return JsonConvert.SerializeObject(project);
        }
    }
}
