using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Api.Models;

namespace Deployer.Api.Services
{
    public interface INodeService
    {
        bool SetBuilds(List<Build> builds);
        bool SetLatestBuild(Build builds);
        bool SetLatestFailedBuild(Build builds);
    }
}
