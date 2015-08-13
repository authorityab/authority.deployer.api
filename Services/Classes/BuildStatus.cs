using Newtonsoft.Json;

namespace Deployer.Services.Classes
{
    public enum BuildStatus
    {
        [JsonProperty("SUCCESS")]
        Success = 1,

        [JsonProperty("FAILURE")]
        Failure = 0
    }
}