using Authority.Deployer.Api.Classes;

namespace Authority.Deployer.Api.Models
{
    public class Environment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ReleaseVersion { get; set; }
        public string LastDeploy { get; set; }
        public DeployStatus Status { get; set; }
    }
}