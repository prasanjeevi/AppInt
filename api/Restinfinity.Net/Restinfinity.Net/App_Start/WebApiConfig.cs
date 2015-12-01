using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Restinfinity.Net
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //context.MapRoute(
            //    "HelpPage_Default",
            //    "Help/{action}/{apiId}",
            //    new { controller = "Help", action = "Index", apiId = UrlParameter.Optional });

            //HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}
