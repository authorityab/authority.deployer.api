using System.Collections.Generic;
using Deployer.Services.Models;

namespace Deployer.Services.Services
{
    public interface IOctopusService
    {
        List<Project> GetAllProjects();
        ReleasePage GetReleasePage(string projectId);
        EnvironmentPage GetEnvironmentPage(string projectId, string releaseId);
        DeployTask GetTaskProgress(string taskId);
        string ReleaseTheCracken(string projectId, string releaseId, string environmentId);
    }
}
