using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DeployerServices.Models.ViewModels;
using DeployerServices.Services;
using Newtonsoft.Json;

namespace DeployerServices.Controllers
{
    public class BuildStatusController : ApiController
    {
        public string GetBuildStatus()
        {
            var tcService = new TeamCityService();
            var buildConfigIds = tcService.GetBuildConfigIdsFromConfig();
            var buildList = new List<TeamCityBuildViewModel>();
            foreach (var configId in buildConfigIds)
            {
                var buildInfo = tcService.GetBuildInfo(configId);
                if (buildInfo != null)
                {
                    var teamCityBuild = new TeamCityBuildViewModel
                    {
                        BuildStatus = buildInfo.Status,
                        ProjectId = buildInfo.BuildType.Id,
                        ProjectName = buildInfo.BuildType.ProjectName,
                        LastChangedBy = buildInfo.LastChanges.Count > 0
                            ? string.Join(", ", buildInfo.LastChanges.Changes.Select(x => x.Username))
                            : string.Empty
                    };
                    buildList.Add(teamCityBuild);
                }
            }
            return JsonConvert.SerializeObject(buildList);
        }
    }
}
