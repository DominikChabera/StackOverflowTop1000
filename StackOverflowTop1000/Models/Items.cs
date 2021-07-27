using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflowTop1000.Models
{
    public class Items
    {
        [JsonProperty("has_synonyms")]
        public bool has_synonyms { get; set; }

        [JsonProperty("is_moderator_only")]
        public bool is_moderator_only { get; set; }

        [JsonProperty("is_required")]
        public bool is_required { get; set; }

        [JsonProperty("count")]
        public int count { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        public decimal popularityPercent { get; set; }
    }
}