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
            var releases = octopusService.GetReleasesFromProject(projectId);

            return JsonConvert.SerializeObject(releases);
        }
        
    }
}