using MSM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace MSM
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            RegisterRoutes(RouteTable.Routes);
        }

        
        public static void RegisterRoutes(RouteCollection routes)
        {
            // This route will have access to the session context becuase
            // of the way its route handler is implemented.
            var route = routes.MapHttpRoute(
                name: "Resolved",
                routeTemplate: "api/resolved",
                defaults: new { controller = "Resolved", action = "GetResolvedChecks" }
                );

            route.RouteHandler = new MyHttpControllerRouteHandler();
        }
         
    }

    public class MyHttpControllerHandler
        : HttpControllerHandler, IRequiresSessionState
    {
        public MyHttpControllerHandler(RouteData routeData): base(routeData)
        {
           var session = HttpContext.Current.Session;
           if (session != null)
           {
               session.Timeout = 1;
               session["Resolved"] = null;
               DataManager.firstCall = true;
           }
        }
    }

    public class MyHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MyHttpControllerHandler(requestContext.RouteData);
        }
    } 
     
}
