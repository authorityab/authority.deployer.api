using System.Collections.Generic;

namespace Deployer.Services.Models
{
    public class Release
    {
        public Release()
        {
            DeployedTo = new List<string>();
        }

        public string Id { get; set; }
        public string Version { get; set; }
        public string ReleaseNotes { get; set; }
        public string Assembled { get; set; }
        public List<string> DeployedTo { get; set; }
    }
}