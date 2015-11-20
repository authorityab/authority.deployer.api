using System.Collections.Generic;

namespace Authority.Deployer.Api.Models
{
    public class Projects
    {
        public Projects()
        {
            Items = new List<Project>();
        }

        public List<Project> Items { get; set; }
    }
}