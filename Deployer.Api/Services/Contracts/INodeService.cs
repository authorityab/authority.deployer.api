using System.Collections.Generic;
using Deployer.Api.Models;

namespace Deployer.Api.Services.Contracts
{
    public interface INodeService
    {
        bool PostBuilds(List<Build> builds);
        bool PostLatestBuild(Build build);
        bool PostLatestFailedBuild(Build build);
    }
}
