using LinqToExcel;
using MSM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
  

namespace MSM.Controllers
{
    public class FileUploaderController : ApiController
    {
    
        [HttpPost]
        public HttpResponseMessage UploadFile()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                List<string> docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var ftype = httpRequest.Form["ftype"];
                   
                    string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}", postedFile.FileName));
                    postedFile.SaveAs(filePath);
                    
                    docfiles.Add(filePath);
                }
              
                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);  
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }

        [HttpGet]
        public HttpResponseMessage DownloadFile(string fileName, string fileType)
        {
            Byte[] bytes = null;
            if (fileName != null)
            {
               // string filePath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), fileName));
                string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", fileName, fileType));
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
        public bool GetCheckvalidity(string ftype, string fname, string fext)
        {
            string fpath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", fname, fext));
           
            switch(ftype)
            {
                case "VC":
                    return ValidateVCFile(fpath);
                case "AP":
                    return ValidateAPFile(fpath);
                case "QB":
                    return ValidateQBFile(fpath);
                default:
                    return false;
            }
        }

        private bool ValidateVCFile(string fpath)
        {
           bool valid = true;

           var voidedChecksFile = new ExcelQueryFactory(fpath);

            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
            voidedChecksFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

            try
            {
                var checks = from vc in voidedChecksFile.Worksheet<Check>("Sheet1") select vc;

                var clr = (from c in checks
                           where (c.Clr != null && c.Clr != "*")
                           select c).FirstOrDefault();

                if (clr != null)
                {
                    valid = false;
                }
            }
            catch (Exception e)
            {
                valid = false;
            }

            return valid;
        }

        private bool ValidateAPFile(string fpath)
        {
            bool valid = true;

            var apricotReportFile = new ExcelQueryFactory(fpath);

            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
            apricotReportFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;
            try
            {
                apricotReportFile.AddMapping("RecordID", "Interview Record ID");
                apricotReportFile.AddMapping("Date", "OPID Interview Date");
                apricotReportFile.AddMapping("LBVDCheckNum", "LBVD Check Number");
                apricotReportFile.AddMapping("LBVDCheckDisposition", "LBVD Check Disposition");

                apricotReportFile.AddMapping("TIDCheckNum", "TID Check Number");
                apricotReportFile.AddMapping("TIDCheckDisposition", "TID Check Disposition");

                apricotReportFile.AddMapping("TDLCheckNum", "TDL Check Number");
                apricotReportFile.AddMapping("TDLCheckDisposition", "TDL Check Disposition");

                apricotReportFile.AddMapping("MBVDCheckNum", "MBVD Check Number");
                apricotReportFile.AddMapping("MBVDCheckDisposition", "MBVD Check Disposition");

                apricotReportFile.AddMapping("SDCheckNum", "SD Check Number");
                apricotReportFile.AddMapping("SDCheckDisposition", "SD Check Disposition");

                var records = from d in apricotReportFile.Worksheet<DispositionRow>("Sheet1") select d;  

                int zeroRecords = records.Count(r => r.RecordID == 0);

                valid = (zeroRecords == 0);

            } catch (Exception e)
            {
                valid = false;
            }

            return valid;
        }

        private bool ValidateQBFile(string fpath)
        {
            bool valid = true;

            var quickbooksFile = new ExcelQueryFactory(fpath);

            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
            quickbooksFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

            try
            {
                var checks = from check in quickbooksFile.Worksheet<Check>("Sheet1") select check;

                int starredRecords = checks.Count(c => c.Clr == "*");

                valid = starredRecords == 0;
            }
            catch (Exception e)
            {
                valid = false;
            }

            return valid;
        }

        [HttpGet]
        // The parameter names matter here because if the first parameter of this method is called fileName
        // and the first parameter of GetApricotFile is also called fileName, then the routing system will
        // say there are multiple methods that match the call
        //  http://localhost/msm/api/qbfile?fileName=QB&fileType=XLSX
        // even though there are 2 distinct route templates
        //    routeTemplate: "api/qbfile"
        //    routeTemplate: "api/apfile
        // on WebAPIConfig.cs. The routing system is very hard to work with!
        // Use Postman when debugging routing.
        // This method is used to return the Quickbooks file for inspection on the Inspect tab.
        public Check[] GetQuickbooksFile(string quickbooksFile, string fileType)
        {
            
            string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", quickbooksFile, fileType));

            var qwickbooksFile = new ExcelQueryFactory(filePath);

            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
            qwickbooksFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

            var checks = from c in qwickbooksFile.Worksheet<Check>("Sheet1") select c;

            return checks.ToArray();
        }

        [HttpGet]
        // This method is used to return the Voided Checks file for inspection on the Inspect tab.
        public Check[] GetVoidedchecksFile(string voidedchecksFile, string fileType)
        {
            string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", voidedchecksFile, fileType));
            var voidchecksFile = new ExcelQueryFactory(filePath);

            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
           voidchecksFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

            var checks = from c in voidchecksFile.Worksheet<Check>("Sheet1") select c;

            //  Checks = checks.ToList();

            return checks.ToArray();
        }

        
        [HttpGet]
        // This method is used to return the Apricot Report File for inspection on the Inspect tab.
        public DispositionRow[] GetApricotFile(string apricotFile, string fileType)
        {
            string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", apricotFile, fileType));

            var aprikotFile = new ExcelQueryFactory(filePath);

            aprikotFile.AddMapping("RecordID", "Interview Record ID");
            aprikotFile.AddMapping("Date", "OPID Interview Date");
            aprikotFile.AddMapping("LBVDCheckNum", "LBVD Check Number");
            aprikotFile.AddMapping("LBVDCheckDisposition", "LBVD Check Disposition");

            aprikotFile.AddMapping("TIDCheckNum", "TID Check Number");
            aprikotFile.AddMapping("TIDCheckDisposition", "TID Check Disposition");

            aprikotFile.AddMapping("TDLCheckNum", "TDL Check Number");
            aprikotFile.AddMapping("TDLCheckDisposition", "TDL Check Disposition");

            aprikotFile.AddMapping("MBVDCheckNum", "MBVD Check Number");
            aprikotFile.AddMapping("MBVDCheckDisposition", "MBVD Check Disposition");

            aprikotFile.AddMapping("SDCheckNum", "SD Check Number");
            aprikotFile.AddMapping("SDCheckDisposition", "SD Check Disposition");

            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
            aprikotFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

            var rows = from c in aprikotFile.Worksheet<DispositionRow>("Sheet1") select c;

            return rows.ToArray();
        }

        [HttpGet]
        // This method is used to make sure that angular datatables don't crash if no file is supplied:
        // at least supply an empty file!
        public EmptyCol[] GetEmptyFile(string emptyFile, string fileType)
        {

            string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", emptyFile, fileType));

            var nodataFile = new ExcelQueryFactory(filePath);

            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
            nodataFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

            var rows = from c in nodataFile.Worksheet<EmptyCol>("Sheet1") select c;

            return rows.ToArray();
        }
    }
        
}