using System.Collections.Generic;
using Octopus.Client.Model;
using TeamCitySharp.DomainEntities;

namespace DeployerServices.Models.ViewModels
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

        public Build LatestFailedBuild { get; set; }

        public string BuildDestroyer { get; set; } 
    }

}