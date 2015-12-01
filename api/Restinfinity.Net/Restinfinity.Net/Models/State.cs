using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Restinfinity.Net.Models
{

    public class State
    {

        [JsonProperty("opened")]
        public bool Opened { get; set; }

        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("selected")]
        public bool Selected { get; set; }
    }

}
