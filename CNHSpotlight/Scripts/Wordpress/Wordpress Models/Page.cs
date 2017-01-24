using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace CNHSpotlight.WordPress.Models
{


    public class Page
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public Title Title { get; set; }

        [JsonProperty("content")]
        public Content Content { get; set; }

    }



}