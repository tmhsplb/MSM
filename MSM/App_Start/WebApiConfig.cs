using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace MSM
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            
            // This route is used by datatableController.js
            config.Routes.MapHttpRoute(
             name: "GetQBFile",
             routeTemplate: "api/qbfile",
                  defaults: new { controller = "Inspect"}
            );

            // This route is used by datatableController.js
            config.Routes.MapHttpRoute(
            name: "GetVCFile",
            routeTemplate: "api/vcfile",
                 defaults: new { controller = "Inspect" }
           );

          // This route is used by datatableController.js
          config.Routes.MapHttpRoute(
           name: "GetAPFile",
           routeTemplate: "api/apfile",
                defaults: new { controller = "Inspect" }
          );

          config.Routes.MapHttpRoute(
             name: "GetEmptyFile",
             routeTemplate: "api/emptyfile",
             defaults: new { controller = "Inspect" }
          );

          config.Routes.MapHttpRoute(
             name: "Merge",
             routeTemplate: "api/merge",
             defaults: new { controller = "Merge", action="PerformMerge"}
          );

          config.Routes.MapHttpRoute(
                name: "Resolved",
                routeTemplate: "api/resolved",
                defaults: new { controller = "Resolved" }
             );

          config.Routes.MapHttpRoute(
               name: "Unmatched",
               routeTemplate: "api/research",
               defaults: new { controller = "Research" }
            );

          config.Routes.MapHttpRoute(
                  name: "Upload",
                  routeTemplate: "api/upload/{action}",
                  defaults: new { controller = "FileUploader" }
              );

          config.Routes.MapHttpRoute(
                 name: "Download",
                 routeTemplate: "api/download",
                 defaults: new {  controller = "FileUploader"} 
             );

          config.Routes.MapHttpRoute(
               name: "ValidFile",
               routeTemplate: "api/checkvalidity",
               defaults: new { controller = "FileUploader" }
           );

            // Used when uploading a file
            /*
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            */

            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(
              config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml"));

           // GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new  QueryStringMapping("json", "true", "application/json"));

        }
    }
}
