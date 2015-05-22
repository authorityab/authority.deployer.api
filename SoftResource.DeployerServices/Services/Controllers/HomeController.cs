using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeployerServices.Models.ViewModels;
using DeployerServices.Services;

namespace DeployerServices.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var tcService = new TeamCityService();

            var projectsIds = tcService.GetProjectsIdsFromConfig();

            var debugList = new List<DebugViewModel>();
            foreach (var projectId in projectsIds)
            {
                var debugItem = new DebugViewModel();

                var latestBuild = tcService.GetLatestBuild(projectId);
                var buildInfo = tcService.GetBuildInfo(latestBuild.Id);

                debugItem.ProjectId = projectId;
                debugItem.BuildStatus = latestBuild.Status;
                debugItem.ProjectName = buildInfo.BuildType.ProjectName;
                debugItem.LastChangedBy = buildInfo.LastChanges.Count > 0
                    ? string.Join(", ", buildInfo.LastChanges.Changes.Select(x => x.Username))
                    : string.Empty;

                debugList.Add(debugItem);
            }

            return View(debugList);
        }
        //// GET: api/Home
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Home/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Home
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Home/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Home/5
        //public void Delete(int id)
        //{
        //}
    }
}
