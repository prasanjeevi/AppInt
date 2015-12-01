using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restinfinity.Net.Models
{
    public class Method
    {
        public Method()
        {
            Request = new Request();
            Response = new Response();
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public MethodType Type { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }

        public string Path { get; set; }
    }
}