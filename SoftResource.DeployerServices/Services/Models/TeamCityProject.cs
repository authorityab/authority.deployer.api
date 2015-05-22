using Newtonsoft.Json;

namespace DeployerServices.Models
{
    public class TeamCityProject
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }
        //[JsonProperty("parentProjectId")]
        //public string ParentProjectId { get; set; }S
    }
}