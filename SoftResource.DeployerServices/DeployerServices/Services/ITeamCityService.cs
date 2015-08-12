using System.Collections.Generic;
using TeamCitySharp.DomainEntities;

namespace DeployerServices.Services
{
    public interface ITeamCityService
    {
        List<Models.Build> GetAllBuilds();

        Build GetLatestFailedBuild(out string buildDestroyer);
    }
}
