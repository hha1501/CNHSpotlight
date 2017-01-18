using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace CNHSpotlight.WordPress.Models
{
    [DebuggerDisplay("Id = {Id}, Title = {Title.Rendered}")]
    public class Post
    {
        #region Wordpress properties
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("date_gmt")]
        public DateTime DateGmt { get; set; }

        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        [JsonProperty("modified_gmt")]
        public DateTime ModifiedGmt { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("title")]
        public Title Title { get; set; }

        [JsonProperty("content")]
        public Content Content { get; set; }

        [JsonProperty("excerpt")]
        public Excerpt Excerpt { get; set; }

        [JsonProperty("author")]
        public int Author { get; set; }

        [JsonProperty("featured_media")]
        public int FeaturedMedia { get; set; }

        [JsonProperty("comment_status")]
        public string CommentStatus { get; set; }

        [JsonProperty("ping_status")]
        public string PingStatus { get; set; }

        [JsonProperty("sticky")]
        public bool Sticky { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("meta")]
        public object[] Meta { get; set; }

        [JsonProperty("categories")]
        public int[] Categories { get; set; }

        [JsonProperty("tags")]
        public object[] Tags { get; set; }

        [JsonProperty("_embedded")]
        public Embedded Embedded { get; set; } 
        #endregion
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