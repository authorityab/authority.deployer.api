using System.Collections.Generic;

namespace Deployer.Services.Models
{
    public class EnvironmentPage
    {
        public EnvironmentPage()
        {
            Environments = new List<Environment>();
        }

        public string ProjectName { get; set; }

        public string ReleaseVersion { get; set; }

        public List<Environment> Environments { get; set; }
    }
}