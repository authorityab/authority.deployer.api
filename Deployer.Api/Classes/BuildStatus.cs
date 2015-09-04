using Newtonsoft.Json;

namespace Authority.Deployer.Api.Classes
{
    public enum BuildStatus
    {
        [JsonProperty("SUCCESS")]
        Success = 1,

        [JsonProperty("FAILURE")]
        Failure = 0
    }
}