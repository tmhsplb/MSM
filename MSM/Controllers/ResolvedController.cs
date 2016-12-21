using MSM.DAL;
using MSM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MSM.Controllers
{
    public class ResolvedController : ApiController
    {
        [HttpGet]
        public List<Check> GetResolvedChecks()
        {
            var session = HttpContext.Current.Session;
            List<Check> resolved;

            if (session != null)
            {
                if (session["Resolved"] == null)
                {
                    session["Resolved"] = DataManager.GetResolvedChecks();
                }
                else
                {
                   List<Check> reslved = (List<Check>)session["Resolved"];
                   int z;
                   z = 3;
                }

                
                resolved = (List<Check>)session["Resolved"];

                return resolved;
            }

            return null;

           // return DataManager.GetResolvedChecks();
        }
    }
}