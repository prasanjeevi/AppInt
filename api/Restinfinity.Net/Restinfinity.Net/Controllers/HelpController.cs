using System;
using System.Web.Http;
using Restinfinity.Utility.HelpPage.Models;
using System.Collections.Generic;
using Restinfinity.Net.Models;
using System.Net.Http.Headers;
using System.Web.Http.Cors;

namespace Restinfinity.Net.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    [EnableCors(origins: Config.Orgins, headers: "*", methods: "*")]
    public class HelpController : ApiController
    {
        private const string ErrorViewName = "Error";

        public HelpController() : this(GlobalConfiguration.Configuration)
        {
        }

        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        public HttpConfiguration Configuration { get; private set; }
        public object Model { get; private set; }

        // GET: api/Help
        public IEnumerable<ApiDocumentation> Get()
        {
            //var documentationProvider = Configuration.Services.GetDocumentationProvider();
            var apiDesc = Configuration.Services.GetApiExplorer().ApiDescriptions;
            var apiDocs = new List<ApiDocumentation>();
            foreach (var api in apiDesc)
            {
                apiDocs.Add(new ApiDocumentation() { Service = api.ActionDescriptor.ControllerDescriptor.ControllerName, ApiId = api.GetFriendlyId(), Method = api.HttpMethod.Method });
            }
            return apiDocs;
        }
        
        // GET: api/Help/5
        public Service Get(string id)
        {
            HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(id);
            var api = apiModel.ApiDescription;
            
            var service = new Service();
            service.Name = api.ActionDescriptor.ControllerDescriptor.ControllerName;
            service.Project = ""; //Todo

            var mtd = new Method();
            mtd.Name = api.HttpMethod.Method;
            mtd.Id = id;
            mtd.Type = (MethodType)Enum.Parse(typeof(MethodType), api.HttpMethod.Method);
            mtd.Path = api.RelativePath;
            mtd.Request.UriParameters = Parameter.GetParameters(apiModel.UriParameters);
            mtd.Request.BodyParameters = Parameter.GetParameters(apiModel.RequestModelDescription);
            mtd.Request.SampleJson = getSampleJson(apiModel.SampleRequests);
            mtd.Response.BodyParameters = Parameter.GetParameters(apiModel.ResourceDescription);
            mtd.Response.SampleJson = getSampleJson(apiModel.SampleResponses);

            service.Methods.Add(mtd);

            return service;
        }

        string getSampleJson(IDictionary<MediaTypeHeaderValue, object> dict)
        {
            if (dict.Count == 0) return string.Empty;
            return dict[MediaTypeHeaderValue.Parse("application/json")].ToString();
        }
    }
}