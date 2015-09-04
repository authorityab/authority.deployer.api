using System.Collections.Generic;
using Authority.Deployer.Api.Models;

namespace Authority.Deployer.Api.Services.Contracts
{
    public interface INodeService
    {
        bool PostBuilds(List<Build> builds);
        bool PostLatestBuild(Build build);
        bool PostLatestFailedBuild(Build build);
    }
}
