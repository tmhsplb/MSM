using MSM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MSM.Controllers
{
    public class ResearchController : ApiController
    {
         // This method returns the table displayed on the Research tab.
        [HttpGet]
        public List<Check> GetLongUnmatched()
        {
            List<Check> longUnmatchedChecks = new List<Check>();

            using (var dbCtx = new MSMEntities1())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();
                
                foreach (LongUnmatched lu in longUnmatched)
                {
                    longUnmatchedChecks.Add(new Check
                    {
                        RecordID = lu.RecordID,
                        InterviewRecordID = lu.InterviewRecordID,
                        Name = lu.Name,
                        Date = lu.Date,
                        Num = lu.Num,
                        Service = lu.Service,
                        Matched = lu.Matched
                    });
                }
            }

            return longUnmatchedChecks;
        }
    }
}
