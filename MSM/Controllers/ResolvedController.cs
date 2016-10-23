using MSM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MSM.Controllers
{
    public class ResolvedController : ApiController
    {
        private static List<Check> resolvedChecks = null;

        public static void SetResolved(List<Check> resolved)
        {
            resolvedChecks = resolved;
        }

        [HttpGet]
        public List<Check> GetResolvedChecks()
        {
            return resolvedChecks;
        }
    }
}