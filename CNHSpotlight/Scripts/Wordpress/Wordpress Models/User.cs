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

    public class AvatarUrls
    {
        [JsonProperty("24")]
        public string Size24 { get; set; }

        [JsonProperty("48")]
        public string Size48 { get; set; }

        [JsonProperty("96")]
        public string Size96 { get; set; }
    }

    public class Links
    {

        [JsonProperty("self")]
        public Self[] Self { get; set; }

        [JsonProperty("collection")]
        public Collection[] Collection { get; set; }
    }

    public class Self
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class Collection
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }

}