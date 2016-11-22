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
            timestamp = DateTime.Now.ToString("dd-MM-yy-hhmm");
            // Create file importme.csv and write 2 header lines from Check Disposition Header.csv
            PrepareImportHeader();
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
            return timestamp;
        }
    }
}