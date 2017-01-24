using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Newtonsoft.Json;

namespace CNHSpotlight.WordPress.Models
{
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

    public class Guid
    {
        [JsonProperty("rendered")]
        public string Rendered { get; set; }
    }

    public class Title
    {
        [JsonProperty("rendered")]
        public string Rendered { get; set; }
    }

    public class Content
    {
        [JsonProperty("rendered")]
        public string Rendered { get; set; }

        [JsonProperty("_protected")]
        public string Protected { get; set; }
    }

    public class Excerpt
    {
        [JsonProperty("rendered")]
        public string Rendered { get; set; }

        [JsonProperty("_protected")]
        public string Protected { get; set; }
    }

    public class Embedded
    {
        [JsonProperty("author")]
        public MediaAuthor[] Author { get; set; }

        [JsonProperty("wp:featuredmedia")]
        public WpFeaturedMedia[] WpFeaturedMedia { get; set; }
    }

    public class MediaAuthor
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }

    public class WpFeaturedMedia
    {
        [JsonProperty("source_url")]
        public string SourceUrl { get; set; }
    }
}