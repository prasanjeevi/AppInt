﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Restinfinity.Net.Models
{

    public class LiAttr
    {

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("isLeaf")]
        public bool IsLeaf { get; set; }
    }

}
