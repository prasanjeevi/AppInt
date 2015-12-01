using System;
using System.Web.Http;
using Restinfinity.Net.Areas.HelpPage.ModelDescriptions;
using Restinfinity.Net.Areas.HelpPage.Models;
using System.Collections.Generic;

namespace Restinfinity.Net.Areas.HelpPage.Controllers
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

        //public ActionResult Index()
        //{
        //    ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
        //    return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        //}

        // GET: api/Help
        public IEnumerable<System.Web.Http.Description.ApiDescription> Get()
        {
            var documentationProvider = Configuration.Services.GetDocumentationProvider();
            var apiDesc = Configuration.Services.GetApiExplorer().ApiDescriptions;            
            return apiDesc;
        }

        // GET: api/Help/5
        public System.Web.Http.Description.ApiDescription Get(int id)
        {
            HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel("GETapiProject");            
            return apiModel.ApiDescription;
        }

        //public ActionResult Api(string apiId)
        //{
        //    if (!String.IsNullOrEmpty(apiId))
        //    {
        //        HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
        //        if (apiModel != null)
        //        {
        //            return View(apiModel);
        //        }
        //    }

        //    return View(ErrorViewName);
        //}

        //public ActionResult ResourceModel(string modelName)
        //{
        //    if (!String.IsNullOrEmpty(modelName))
        //    {
        //        ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
        //        ModelDescription modelDescription;
        //        if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
        //        {
        //            return View(modelDescription);
        //        }
        //    }

        //    return View(ErrorViewName);
        //}
    }
}