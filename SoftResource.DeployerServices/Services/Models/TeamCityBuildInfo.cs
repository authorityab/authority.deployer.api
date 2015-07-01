using System;
using System.Collections.Generic;
using DeployerServices.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        [JsonProperty("startDate", ItemConverterType = typeof(JavaScriptDateTimeConverter))]
        public DateTime StartDate { get; set; }

        [JsonProperty("finishDate", ItemConverterType = typeof(JavaScriptDateTimeConverter))]
        public DateTime FinishDate { get; set; }

        [JsonProperty("status")]
        public BuildStatus Status { get; set; }

        [JsonProperty("number")]
        public string Version { get; set; }


    }

   

   

    
}