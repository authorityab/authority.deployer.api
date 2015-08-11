using System.Collections.Generic;

namespace DeployerServices.Models
{
    public class ProjectPage
    {
        public ProjectPage()
        {
            Projects = new List<Project>();
        }

        public List<Project> Projects { get; set; }
    }
}