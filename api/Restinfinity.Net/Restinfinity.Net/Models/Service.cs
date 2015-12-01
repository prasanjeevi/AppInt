using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restinfinity.Net.Models
{
    public class Service
    {
        public Service()
        {
            Methods = new List<Method>();
        }
        
        public string Name { get; set; }
        public string Project { get; set; }
        public List<Method> Methods { get; set; }

        public string Content { get; set; }
    }
}