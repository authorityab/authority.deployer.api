using System.Collections.Generic;
using TeamCitySharp.DomainEntities;

namespace Deployer.Services.Services
{
    public interface ITeamCityService
    {
        List<Models.Build> GetAllBuilds();

        Build GetLatestFailedBuild(out string buildDestroyer);
    }
}
