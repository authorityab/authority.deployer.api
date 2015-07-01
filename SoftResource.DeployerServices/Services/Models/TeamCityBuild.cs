using System.Globalization;
using DeployerServices.Classes;
using Newtonsoft.Json;

namespace DeployerServices.Models
{
    public class TeamCityBuild
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("buildTypeId")]
        public string BuildTypeId { get; set; }

        [JsonProperty("status")]
        public BuildStatus Status { get; set; }

       


    }
}