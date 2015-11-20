using System.Collections.Generic;
using Authority.Deployer.Api.Models;

namespace Authority.Deployer.Api.Services.Contracts
{
    public interface IOctopusService
    {
        List<Project> GetAllProjects();
        Releases GetReleases(string projectId);
        Environments GetEnvironments(string projectId, string releaseId);
        DeployTask GetTaskProgress(string taskId);
        string ReleaseTheCracken(string projectId, string releaseId, string environmentId);
    }
}
