using System.Collections.Generic;
using Octopus.Client.Model;

namespace Deployer.Api.Models.ViewModels
{
    public class DebugViewModel
    {
        public DebugViewModel()
        {
            TeamCityBuilds = new List<Build>();
            OctopusProjects = new List<DashboardProjectResource>();
        }

        public List<Build> TeamCityBuilds { get; set; }

        public List<DashboardProjectResource> OctopusProjects { get; set; }

        public Build LatestBuild { get; set; }

        public TeamCitySharp.DomainEntities.Build LatestFailedBuild { get; set; }

        public string BuildDestroyer { get; set; } 
    }

}