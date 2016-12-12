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
        public List<Check> GetResearchChecks()
        {
            List<Check> researchChecks = new List<Check>();

            using (var dbCtx = new MSMEntities())
            {
                var longUnmatched = dbCtx.Set<ResearchCheck>();
                
                foreach (ResearchCheck lu in longUnmatched)
                {
                    researchChecks.Add(new Check
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

            return researchChecks;
        }

        [HttpGet]
        public string ResolveCheck(int checkNum)
        {
             
            string status = DataManager.ResolveCheck(checkNum);

            return status;
        }

        [HttpGet]
        public string GetTimestamp()
        {
            // Set timestamp when researchController is loaded. This allows
            // the timestamp to be made part of the page title, which allows
            // the timestamp to appear in the printed file and also as part
            // of the Excel file name of the angular datatable.

            // This compensates for the fact that DateTime.Now on the AppHarbor server returns
            // the the time in the timezone of the server.
            // Here we convert UTC to Central Standard Time to get the time in Houston.
            // This is supposed to handle daylight savings time also. We will have to
            // wait and see about this.
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime cst = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, "UTC", "Central Standard Time");
           
            return cst.ToString("dd-MM-yy-hhmm"); 
        }
    }
}
