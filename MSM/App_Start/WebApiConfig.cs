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
                  defaults: new { controller = "Inspect", action="GetQuickbooksFile"}
            );

            // This route is used by datatableController.js
            config.Routes.MapHttpRoute(
            name: "GetVCFile",
            routeTemplate: "api/vcfile",
                 defaults: new { controller = "Inspect", action="GetVoidedchecksFile"}
           );

          // This route is used by datatableController.js
          config.Routes.MapHttpRoute(
           name: "GetAPFile",
           routeTemplate: "api/apfile",
                defaults: new { controller = "Inspect", action = "GetApricotFile" }
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
                defaults: new { controller = "Resolved", action="GetResolvedChecks" }
             );

          config.Routes.MapHttpRoute(
               name: "Unmatched",
               routeTemplate: "api/research",
               defaults: new { controller = "Research", action="GetLongUnmatched" }
            );

          config.Routes.MapHttpRoute(
                  name: "Upload",
                  routeTemplate: "api/upload/{action}",
                  defaults: new { controller = "FileUploader" }
              );

          config.Routes.MapHttpRoute(
                 name: "Download",
                 routeTemplate: "api/downloadimportme",
                 defaults: new {  controller = "FileDownloader", action="DownloadImportMe"} 
             );

          config.Routes.MapHttpRoute(
                   name: "Resolve",
                   routeTemplate: "api/resolvecheck/{checkNum}",
                   defaults: new { controller = "Research", action = "ResolveCheck" }
               );

          config.Routes.MapHttpRoute(
               name: "ValidFile",
               routeTemplate: "api/checkvalidity",
               defaults: new { controller = "FileUploader" }
           );

          config.Routes.MapHttpRoute(
                name: "HaveResolvedChecks",
                routeTemplate: "api/haveresolvedchecks",
                defaults: new { controller = "Resolved", action="HaveResolvedChecks" }
            );

          config.Routes.MapHttpRoute(
            name: "ConnectionString",
            routeTemplate: "api/cs/{id}",
            defaults: new { controller = "Database" }
          );

          config.Routes.MapHttpRoute(
             name: "Linq2ExcelFactory",
             routeTemplate: "api/factory",
             defaults: new { controller = "MyLinqToExcel", action="GetFactory" }
           );

          config.Routes.MapHttpRoute(
               name: "Linq2ExcelDispositionRows",
               routeTemplate: "api/dispositionrows",
               defaults: new { controller = "MyLinqToExcel", action = "GetDispositionRows" }
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
