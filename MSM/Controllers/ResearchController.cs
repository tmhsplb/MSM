using MSM.DAL;
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

            using (var dbCtx = new MSMEntities())
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

        [HttpGet]
        public string ResolveCheck(int checkNum)
        {
             
            string status;

            int z;
            z = 2;

            status = DataManager.ResolveCheck(checkNum);
             

            return status;
        }
    }
}
