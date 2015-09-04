using System.Collections.Generic;

namespace Authority.Deployer.Api.Services.Contracts
{
    public interface ITeamCityService
    {
        List<Models.Build> GetAllBuilds();

        Models.Build GetLatestFailedBuild();
    }
}
