using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Restinfinity.Net.Models
{

    public class File
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("li_attr")]
        public LiAttr LiAttr { get; set; }

        [JsonProperty("children")]
        public bool Children { get; set; }
        
        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public string Parent { get; set; }
    }

}
