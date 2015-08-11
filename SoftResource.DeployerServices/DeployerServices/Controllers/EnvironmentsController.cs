using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class EnvironmentsController : ApiController
    {
        public string Get(string projectId, string releaseId)
        {
            var octopusService = new OctopusService();
            var environmentPage = octopusService.GetEnvironmentPage(projectId, releaseId);

            return JsonConvert.SerializeObject(environmentPage);
        }
    }
}