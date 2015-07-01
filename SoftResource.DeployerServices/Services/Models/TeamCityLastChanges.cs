using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DeployerServices.Models
{
    public class TeamCityLastChanges
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("change")]
        public List<TeamCityChange> Changes { get; set; }
    }
}