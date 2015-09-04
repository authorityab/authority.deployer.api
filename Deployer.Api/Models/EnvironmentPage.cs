using System.Collections.Generic;

namespace Authority.Deployer.Api.Models
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