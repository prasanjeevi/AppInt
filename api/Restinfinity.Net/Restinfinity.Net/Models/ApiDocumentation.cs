using Restinfinity.Utility.HelpPage.ModelDescriptions;
using Restinfinity.Utility.HelpPage.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace Restinfinity.Net.Models
{
    public class ApiDocumentation
    {
        public string Service { get; set; }
        public string ApiId { get; set; }
        public string Method { get; set; }
    }

    public class ApiDetailedDocumentation : ApiDocumentation
    {
        public string RelativePath { get; set; }
        public string Documentation { get; set; }

        public string ResponseDescription { get; set; }

        public Collection<ParameterDescription> UriParameters { get; set; }
        public string RequestDocumentation { get; set; }
        public ModelDescription RequestModelDescription { get; set; }
        public IList<Parameter> RequestBodyParameters { get; set; }
        public IDictionary<MediaTypeHeaderValue, object> SampleRequests { get; set; }
        public IList<Parameter> ResourceProperties { get; set; }
        public IDictionary<MediaTypeHeaderValue, object> SampleResponses { get; set; }
    }
}