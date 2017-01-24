using Newtonsoft.Json;

namespace CNHSpotlight.WordPress.Models
{

    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("avatar_urls")]
        public AvatarUrls AvatarUrls { get; set; }

        [JsonProperty("meta")]
        public object[] Meta { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }


}