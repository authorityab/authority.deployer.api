using System;
using System.Collections.Generic;
using Deployer.Api.Models;

namespace Deployer.Api.Services
{
    public class NodeService : INodeService
    {
        public bool SetBuilds(List<Build> builds)
        {
            throw new NotImplementedException();
        }

        public bool SetLatestBuild(Build builds)
        {
            throw new NotImplementedException();
        }

        public bool SetLatestFailedBuild(Build builds)
        {
            throw new NotImplementedException();
        }
    }
}