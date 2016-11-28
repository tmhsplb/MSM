using MSM.DAL;
using MSM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace MSM.Controllers
{
    public class FileDownloaderController : ApiController
    {
        private static string timestamp;

        [HttpGet]
        public HttpResponseMessage DownloadFile(string fileName, string fileType)
        {
            Byte[] bytes = null;
            if (fileName != null)
            {
                string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Downloads/{0}.{1}", fileName, fileType));
                // string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}", fileName));
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                bytes = br.ReadBytes((Int32)fs.Length);
                br.Close();
                fs.Close();
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            System.IO.MemoryStream stream = new MemoryStream(bytes);
            result.Content = new StreamContent(stream);
            // result.Content.Headers.ContentType = new MediaTypeHeaderValue(fileType);
            //  result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/force-download");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            return (result);
        }

        [HttpGet]
        public HttpResponseMessage DownloadImportMe()
        {
            List<DispositionRow> importRows = DataManager.GetUpdatedRows();

            if (importRows != null)
            {
                PrepareImportFile(importRows);
            }

            Byte[] bytes = null;
           
           // string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/{0}.{1}", "ImportMe", "csv"));

            string fname = string.Format("importme-{0}", timestamp);
            string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Downloads/{0}.csv", fname));
            
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            bytes = br.ReadBytes((Int32)fs.Length);
            br.Close();
            fs.Close();
            
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            System.IO.MemoryStream stream = new MemoryStream(bytes);
            result.Content = new StreamContent(stream);
            // result.Content.Headers.ContentType = new MediaTypeHeaderValue(fileType);
            //  result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/force-download");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fname
            };
            return (result);
        }

        private static void PrepareImportHeader()
        {
            string pathToDispositionHeader = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Import Me Header.csv"));
            string pathToImportMeFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Downloads/importme-{0}.csv", timestamp));
            var retainedLines = File.ReadAllLines(pathToDispositionHeader);
            File.WriteAllLines(pathToImportMeFile, retainedLines);
        }

        private static void PrepareImportFile(List<DispositionRow> updatedRows)
        {
            // Create file importme.csv and write 2 header lines from Check Disposition Header.csv
            PrepareImportHeader();

            // Static variable timestamp will be set by this point, because GetTimestamp will have been called.
            string pathToImportMeFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Downloads/importme-{0}.csv", timestamp));

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

        [HttpGet]
        public string GetTimestamp()
        {
            // Set timestamp when resolvedController is loaded. This allows
            // the timestamp to be made part of the page title, which allows
            // the timestamp to appear in the printed file and also as part
            // of the Excel file name of both the angular datatable and
            // the importme file.

            // This compensates for the fact that DateTime.Now on the AppHarbor server returns
            // the the time in the timezone of the server.
            // Here we convert UTC to Central Standard Time to get the time in Houston.
            // This is supposed to handle daylight savings time also. We will have to
            // wait and see about this.
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime cst = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, "UTC", "Central Standard Time");
            timestamp = cst.ToString("dd-MM-yy-hhmm");

            return timestamp;
        }
    }
}