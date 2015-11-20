using System.Collections.Generic;

namespace Authority.Deployer.Api.Models
{
    public class Environments
    {
        public Environments()
        {
            Items = new List<Environment>();
        }

        public string ProjectName { get; set; }

        public string ReleaseVersion { get; set; }

        public List<Environment> Items { get; set; }
    }
}