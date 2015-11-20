using System.Collections.Generic;

namespace Authority.Deployer.Api.Models
{
    public class Releases
    {
        public Releases()
        {
            Items = new List<Release>();
        }

        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public List<Release> Items { get; set; }
        
    }
}