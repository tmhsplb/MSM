﻿using LinqToExcel;
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
    public class InspectController : ApiController
    {
    
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

            /*
            var apricotFile = new ExcelQueryFactory(filePath);

            apricotFile.AddMapping("RecordID", "Interview Record ID");
            apricotFile.AddMapping("Date", "OPID Interview Date");
            apricotFile.AddMapping("LBVDCheckNum", "LBVD Check Number");
            apricotFile.AddMapping("LBVDCheckDisposition", "LBVD Check Disposition");

            apricotFile.AddMapping("TIDCheckNum", "TID Check Number");
            apricotFile.AddMapping("TIDCheckDisposition", "TID Check Disposition");

            apricotFile.AddMapping("TDLCheckNum", "TDL Check Number");
            apricotFile.AddMapping("TDLCheckDisposition", "TDL Check Disposition");

            apricotFile.AddMapping("MBVDCheckNum", "MBVD Check Number");
            apricotFile.AddMapping("MBVDCheckDisposition", "MBVD Check Disposition");

            apricotFile.AddMapping("SDCheckNum", "SD Check Number");
            apricotFile.AddMapping("SDCheckDisposition", "SD Check Disposition");

            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
            apricotFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

            var rows = from c in apricotFile.Worksheet<DispositionRow>("Sheet1") select c;

            return rows.ToArray();
            */

            return Linq2Excel.GetDispositionRows(filePath);
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