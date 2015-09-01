using System.Configuration;

namespace Deployer.Api.Models
{
    public class Project
    {
        public Project(string id)
        {
            var serverUrl = ConfigurationManager.AppSettings["OctopusServerUrl"];

            Id = id;
            Logo = string.Format("{0}/api/projects/{1}/logo", serverUrl, id);
        }

        public string Id;

        public string Logo;

        public string Name { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }
    }
}