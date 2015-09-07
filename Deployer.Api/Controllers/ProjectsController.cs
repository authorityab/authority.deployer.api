using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Authority.Deployer.Api.Models;
using Authority.Deployer.Api.Services.Contracts;

namespace Authority.Deployer.Api.Controllers
{
    public class ProjectsController : ApiController
    {
        private readonly IOctopusService _octopusService;

        public ProjectsController(IOctopusService octopusService)
        {
            _octopusService = octopusService;
        }

        public JsonResult<List<Project>> Get()
        {
            var projects = _octopusService.GetAllProjects();

            return Json(projects);
        }
    }
}
