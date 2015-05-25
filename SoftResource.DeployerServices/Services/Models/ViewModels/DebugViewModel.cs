using System.Collections.Generic;
using Octopus.Client.Model;

namespace DeployerServices.Models.ViewModels
{
    public class DebugViewModel
    {
        public DebugViewModel()
        {
            TeamCityBuilds = new List<TeamCityBuildViewModel>();
            OctopusProjects = new List<ProjectResource>();
        }

        public List<TeamCityBuildViewModel> TeamCityBuilds { get; set; }

        public List<ProjectResource> OctopusProjects { get; set; } 
    }
}