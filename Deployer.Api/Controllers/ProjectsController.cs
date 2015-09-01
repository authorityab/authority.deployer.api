using System.Web.Http;
using Deployer.Api.Services;
using Newtonsoft.Json;

namespace Deployer.Api.Controllers
{
    public class ProjectsController : ApiController
    {
        private readonly IOctopusService _octopusService;

        public ProjectsController(IOctopusService octopusService)
        {
            _octopusService = octopusService;
        }

        public string Get()
        {
            var projects = _octopusService.GetAllProjects();

            return JsonConvert.SerializeObject(projects);
        }
    }
}
