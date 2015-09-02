using System.Collections.Generic;

namespace Deployer.Api.Services.Contracts
{
    public interface ITeamCityService
    {
        List<Models.Build> GetAllBuilds();

        Models.Build GetLatestFailedBuild();
    }
}
