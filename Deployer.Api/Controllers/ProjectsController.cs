using System.Web.Http;
using Authority.Deployer.Api.Services.Contracts;
using Newtonsoft.Json;

namespace Authority.Deployer.Api.Controllers
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
