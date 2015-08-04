using System.Web.Http;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class EnvironmentsController : ApiController
    {
        public string Get()
        {
            var octopusService = new OctopusService();
            var environments = octopusService.GetEnvironmentsFromLifecycle();

            return JsonConvert.SerializeObject(environments);
        }
    }
}