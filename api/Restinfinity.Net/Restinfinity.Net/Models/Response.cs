using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restinfinity.Net.Models
{
    public class Response
    {
        public Response()
        {
            Headers = new List<Parameter>();
            BodyParameters = new List<Parameter>();
        }
        public IList<Parameter> Headers { get; set; }
        public IList<Parameter> BodyParameters { get; set; }
        public string SampleJson { get; set; }
        public string Type { get; set; }
    }
}