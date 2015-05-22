using System.Collections.Generic;
using Newtonsoft.Json;

namespace DeployerServices.Models
{
    public class TeamCityBuildInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("buildType")]
        public TeamCityBuildType BuildType { get; set; }

        [JsonProperty("lastChanges")]
        public TeamCityLastChanges LastChanges { get; set; }
    }

    public class TeamCityBuildType
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }
    }

    public class TeamCityLastChanges
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("change")]
        public List<TeamCityChange> Changes { get; set; }
    }

    public class TeamCityChange
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}