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

            /*
            config.Routes.MapHttpRoute(
               name: "QuickbooksFile",
               routeTemplate: "api/{controller}/{action}",
               defaults: new {  action = "GetQuickbooksFile" }
           );
            */

            

            config.Routes.MapHttpRoute(
             name: "GetQBFile",
             routeTemplate: "api/qbfile",
                  defaults: new { controller = "FileUploader"}
            );

            config.Routes.MapHttpRoute(
            name: "GetVCFile",
            routeTemplate: "api/vcfile",
                 defaults: new { controller = "FileUploader" }
           );

          config.Routes.MapHttpRoute(
           name: "GetAPFile",
           routeTemplate: "api/apfile",
                defaults: new { controller = "FileUploader" }
          );

          config.Routes.MapHttpRoute(
             name: "GetEmptyFile",
             routeTemplate: "api/emptyfile",
             defaults: new { controller = "FileUploader" }
          );

          config.Routes.MapHttpRoute(
             name: "Merge",
             routeTemplate: "api/merge",
             defaults: new { controller = "Merge" }
          );

          config.Routes.MapHttpRoute(
               name: "Unmatched",
               routeTemplate: "api/unmatched",
               defaults: new { controller = "Merge" }
            );

            /*
            config.Routes.MapHttpRoute(
              name: "GetFile",
              routeTemplate: "api/{controller}/{action}/{fileName}/{fileType}"
            //  defaults: new { controller = "FileUploader", action = "QuickbooksFile" },
             // constraints: new {  action = "QuickbooksFile|ApricotFile"}
          );
          */

            /*
           config.Routes.MapHttpRoute(
              name: "GetApricotFile",
              routeTemplate: "api/{controller}/{action}/{fileName}/{fileType}",
              defaults: new { controller = "FileUploader", action = "ApricotFile"},
              constraints: new {  action = "QuickbooksFile|ApricotFile"}
          );
*/
            /*
            config.Routes.MapHttpRoute(
               name: "ApricotFile",
               routeTemplate: "api/ap/{controller}/{fileName}/{fileType}/{dummy1}/{dummy2}"
           );


            config.Routes.MapHttpRoute(
              name: "Merge",
              routeTemplate: "api/merge/{controller}/{qbFileName}/{qbFileType}/{apFileName}/{apFileType}"
          );
            */

            /*
            config.Routes.MapHttpRoute(
                name: "Download",
                routeTemplate: "api/{controller}/{fileName}/{fileType}"
            );
            */

            
            // Used when uploading a file
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            

            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(
              config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml"));

           // GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new  QueryStringMapping("json", "true", "application/json"));

        }
    }
}
