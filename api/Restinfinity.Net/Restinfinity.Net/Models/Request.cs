using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Restinfinity.Net.Models
{
    public class Request
    {
        public Request()
        {
            UriParameters = new List<Parameter>();
            Headers = new List<Parameter>();
            BodyParameters = new List<Parameter>();
        }
        public IList<Parameter> UriParameters { get; set; }
        public IList<Parameter> Headers { get; set; }
        public IList<Parameter> BodyParameters { get; set; }
        public string SampleJson { get; set; }
    }
}