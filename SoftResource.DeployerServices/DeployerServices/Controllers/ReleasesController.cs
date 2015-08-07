using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class ReleasesController : ApiController
    {
        public string Get(string projectId)
        {
            var octopusService = new OctopusService();
            var release = octopusService.GetReleasePage(projectId);

            return JsonConvert.SerializeObject(release);
        }


        
    }
}