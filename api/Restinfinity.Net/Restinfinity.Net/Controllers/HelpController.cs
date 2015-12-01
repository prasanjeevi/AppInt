using System;
using System.Web.Http;
using Restinfinity.Utility.HelpPage.ModelDescriptions;
using Restinfinity.Utility.HelpPage.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Restinfinity.Net.Models;
using System.Net.Http.Headers;

namespace Restinfinity.Net.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : ApiController
    {
        private const string ErrorViewName = "Error";

        public HelpController()
            : this(GlobalConfiguration.Configuration)
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
        //public ApiDetailedDocumentation Get(string id)
        //{
        //    HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(id);            
        //    var api = apiModel.ApiDescription;
        //    var apiDoc = new ApiDetailedDocumentation();
        //    apiDoc.Service = api.ActionDescriptor.ControllerDescriptor.ControllerName;
        //    apiDoc.ApiId = api.GetFriendlyId();
        //    apiDoc.Method = api.HttpMethod.Method;
        //    apiDoc.RelativePath = api.RelativePath;
        //    apiDoc.Documentation = api.Documentation;
        //    apiDoc.UriParameters = apiModel.UriParameters;
        //    apiDoc.RequestBodyParameters = Parameter.GetParameters(apiModel.RequestModelDescription);
        //    apiDoc.SampleRequests = apiModel.SampleRequests;
        //    apiDoc.ResponseDescription = api.ResponseDescription.Documentation;
        //    apiDoc.ResourceProperties = Parameter.GetParameters(apiModel.ResourceDescription);
        //    apiDoc.SampleResponses = apiModel.SampleResponses;

        //    return apiDoc;
        //}

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
            //apiDoc.Documentation = api.Documentation;
            mtd.Request.UriParameters = Parameter.GetParameters(apiModel.UriParameters);
            mtd.Request.BodyParameters = Parameter.GetParameters(apiModel.RequestModelDescription);
            mtd.Request.SampleJson = GetSampleJson(apiModel.SampleRequests);
            //apiDoc.ResponseDescription = api.ResponseDescription.Documentation;
            mtd.Response.BodyParameters = Parameter.GetParameters(apiModel.ResourceDescription);
            mtd.Response.SampleJson = GetSampleJson(apiModel.SampleResponses);

            service.Methods.Add(mtd);

            return service;
        }

        string GetSampleJson(IDictionary<MediaTypeHeaderValue, object> dict)
        {
            if (dict.Count == 0) return string.Empty;
            return dict[MediaTypeHeaderValue.Parse("application/json")].ToString();
        }
    }
}