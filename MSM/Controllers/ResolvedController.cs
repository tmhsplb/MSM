using MSM.DAL;
using MSM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MSM.Controllers
{
    public class ResolvedController : ApiController
    {
        
        [HttpGet]
        public List<Check> GetResolvedChecks()
        {
            List<DispositionRow> importRows = DataManager.GetUpdatedRows();

            if (importRows != null)
            {
                PrepareImportFile(importRows);
            }

            return DataManager.GetResolvedChecks();
        }

        private static void PrepareImportHeader()
        {
            string pathToDispositionHeader = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Private/Check Disposition Header.csv"));
            string pathToImportMeFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/importme.csv"));
            var retainedLines = File.ReadAllLines(pathToDispositionHeader);
            File.WriteAllLines(pathToImportMeFile, retainedLines);
        }

        private static void PrepareImportFile(List<DispositionRow> updatedRows)
        {
            // Create file importme.csv and write 2 header lines from Check Disposition Header.csv
            PrepareImportHeader();
            string pathToImportMeFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/importme.csv"));

            // Append lines to file importme.csv
            //  using (StreamWriter writer = new StreamWriter(@"C:\\Methodist\\OPID\\Linq\\importme.csv", true))
            using (StreamWriter writer = new StreamWriter(pathToImportMeFile, true))
            {
                foreach (DispositionRow d in updatedRows)
                {
                    string csvRow = string.Format(",{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        d.InterviewRecordID,
                        d.LBVDCheckNum,
                        d.LBVDCheckDisposition,
                        d.TIDCheckNum,
                        d.TIDCheckDisposition,
                        d.TDLCheckNum,
                        d.TDLCheckDisposition,
                        d.MBVDCheckNum,
                        d.MBVDCheckDisposition,
                        d.SDCheckNum,
                        d.SDCheckDisposition);

                    writer.WriteLine(csvRow);
                }
            }
        }
    }
}