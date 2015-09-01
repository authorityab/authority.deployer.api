namespace Deployer.Api.Models
{
    public class DeployTask
    {
        public string Id { get; set; }
        public string State { get; set; }
        public string CompletedTime { get; set; }
        public bool IsCompleted { get; set; }
        public bool FinishedSuccessfully { get; set; }
        public bool HasWarningOrErrors { get; set; }
    }
}