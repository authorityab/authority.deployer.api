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

            var dashboard = octopusService.GetDashboardDynamic();

            return JsonConvert.SerializeObject(dashboard);
        }
    }
}
