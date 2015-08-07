using Newtonsoft.Json;

namespace DeployerServices.Classes
{
    public enum BuildStatus
    {
        [JsonProperty("SUCCESS")]
        Success = 1,

        [JsonProperty("FAILURE")]
        Failure = 0
    }
}