using LinqToExcel;
using MSM.Models;
using MSM.Utils;
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
                   
                   // string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/{0}", postedFile.FileName));

                    string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", postedFile.FileName));
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
        public bool GetCheckvalidity(string ftype, string fname, string fext)
        {
          //  string fpath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/{0}.{1}", fname, fext));

            string fpath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}.{1}", fname, fext));
           
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

            var voidedChecksFile = Linq2Excel.GetFactory(fpath);

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

           // return valid;
            return true;
        }

        private bool ValidateAPFile(string fpath)
        {
            bool valid = true;
 
            try
            {
                List<DispositionRow> rows = Linq2Excel.GetDispositionRows(fpath);
                int zeroRecords = rows.Count(r => r.RecordID == 0);
 
                valid = (zeroRecords == 0);

            } catch (Exception e)
            {
                valid = false;
            }

           // return valid;
            return true;
        }

        private bool ValidateQBFile(string fpath)
        {
            bool valid = true;
 
            var quickbooksFile = Linq2Excel.GetFactory(fpath);

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

            // return valid;
            return true;
        }
    }   
}