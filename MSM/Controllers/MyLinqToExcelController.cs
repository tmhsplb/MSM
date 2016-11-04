using LinqToExcel;
using MSM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MSM.Controllers
{
    public class MyLinqToExcelController : ApiController
    {
        [HttpGet]
        public ExcelQueryFactory GetFactory(string fileName, string fileType)
        {
            try
            {
                string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", fileName, fileType));
                var eqf = new ExcelQueryFactory(filePath);

                // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
                eqf.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

                return eqf;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void PrepareApricotMapping(ExcelQueryFactory eqf)
        {
            eqf.AddMapping("RecordID", "Record ID");
            eqf.AddMapping("Lname", "Last Name");
            eqf.AddMapping("Fname", "First Name");
            eqf.AddMapping("InterviewRecordID", "Interview Record ID");
            eqf.AddMapping("Date", "OPID Interview Date");
            eqf.AddMapping("LBVDCheckNum", "LBVD Check Number");
            eqf.AddMapping("LBVDCheckDisposition", "LBVD Check Disposition");

            eqf.AddMapping("TIDCheckNum", "TID Check Number");
            eqf.AddMapping("TIDCheckDisposition", "TID Check Disposition");

            eqf.AddMapping("TDLCheckNum", "TDL Check Number");
            eqf.AddMapping("TDLCheckDisposition", "TDL Check Disposition");

            eqf.AddMapping("MBVDCheckNum", "MBVD Check Number");
            eqf.AddMapping("MBVDCheckDisposition", "MBVD Check Disposition");

            eqf.AddMapping("SDCheckNum", "SD Check Number");
            eqf.AddMapping("SDCheckDisposition", "SD Check Disposition");
        }

        [HttpGet]
        public List<DispositionRow> GetDispositionRows(string fileName, string fileType)
        {
            try
            {
                ExcelQueryFactory eqf = GetFactory(fileName, fileType);
                PrepareApricotMapping(eqf);

                var rows = from c in eqf.Worksheet<DispositionRow>("Sheet1") select c;

                return rows.ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}