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
   
    public class MergeController : ApiController
    {
        private static List<int> recorded = new List<int>();
        private static List<int> matched = new List<int>();

        private static List<Check> unmatched;

        private static string QB = "QB";
        private static string AP = "AP";

        private static string GetDispositionFromCheck(Check check)
        {
            switch (check.Clr)
            {
                case "C":
                    return "Cleared";
                case "V":
                    return "Voided";
                default:
                    return "Voided";
            }
        }

        private static void UpdateDisposition(bool lbvd, bool tid, bool tdl, bool mbvd, bool sd, DispositionRow d, Check check)
        {
            if (lbvd)
            {
                d.LBVDCheckDisposition = GetDispositionFromCheck(check);
                matched.Add(d.LBVDCheckNum);
            }

            if (tid)
            {
                d.TIDCheckDisposition = GetDispositionFromCheck(check);
                matched.Add(d.TIDCheckNum);
            }

            if (tdl)
            {
                d.TDLCheckDisposition = GetDispositionFromCheck(check);
                matched.Add(d.TDLCheckNum);
            }

            if (mbvd)
            {
                d.MBVDCheckDisposition = GetDispositionFromCheck(check);
                matched.Add(d.MBVDCheckNum);
            }

            if (sd)
            {
                d.SDCheckDisposition = GetDispositionFromCheck(check);
                matched.Add(d.SDCheckNum);
            }

            if (lbvd || tid || tdl || mbvd || sd)
            {
                recorded.Add(check.Num);
            }
        }

        private static bool IsRecorded(int checkNum)
        {
            foreach (int cnum in recorded)
            {
                if (checkNum == cnum)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsMatched(int checkNum)
        {
            foreach (int cnum in matched)
            {
                if (checkNum == cnum)
                {
                    return true;
                }
            }

            return false;
        }

        private static void UpdateUnmatched(IQueryable<DispositionRow> originalRows, IQueryable<Check> checks)
        {
            unmatched = new List<Check>();

            foreach (DispositionRow row in originalRows)
            {
                if (row.LBVDCheckNum != 0 && !IsMatched(row.LBVDCheckNum))
                {
                    unmatched.Add(new Check
                    {
                        InterviewRecordID = row.RecordID,
                        Num = row.LBVDCheckNum,
                        Date = row.Date,
                        Type = AP,
                        Service = "LBVD"
                    });
                }

                if (row.TIDCheckNum != 0 && !IsMatched(row.TIDCheckNum))
                {
                    unmatched.Add(new Check
                    {
                        InterviewRecordID = row.RecordID,
                        Num = row.TIDCheckNum,
                        Date = row.Date,
                        Type = AP,
                        Service = "TID"
                    });
                }

                if (row.TDLCheckNum != 0 && !IsMatched(row.TDLCheckNum))
                {
                    unmatched.Add(new Check
                    {
                        InterviewRecordID = row.RecordID,
                        Num = row.TDLCheckNum,
                        Date = row.Date,
                        Type = AP,
                        Service = "TDL"
                    });
                }

                if (row.MBVDCheckNum != 0 && !IsMatched(row.MBVDCheckNum))
                {
                    unmatched.Add(new Check
                    {
                        InterviewRecordID = row.RecordID,
                        Num = row.MBVDCheckNum,
                        Date = row.Date,
                        Type = AP,
                        Service = "MBVD"
                    });
                }

                if (row.SDCheckNum != 0 && !IsMatched(row.SDCheckNum))
                {
                    unmatched.Add(new Check
                    {
                        InterviewRecordID = row.RecordID,
                        Num = row.SDCheckNum,
                        Date = row.Date,
                        Type = AP,
                        Service = "SD"
                    });
                }
            }

            foreach (Check check in checks)
            {
                if (check.Num > 0 && !IsRecorded(check.Num))
                {
                    unmatched.Add(new Check
                    {
                        Num = check.Num,
                        Date = check.Date,
                        Clr = check.Clr,
                        Type = QB
                    });
                }
            }
        }

        private static void PrepareImportHeader()
        {
          //  var retainedLines = File.ReadAllLines(@"C:\\Methodist\\OPID\\Linq\\Check Disposition Template.csv");
          //  File.WriteAllLines(@"C:\\Methodist\\OPID\\Linq\\importme.csv", retainedLines);

            string pathToDispositionTemplate = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Check Disposition Template.csv"));
            string pathToImportMeFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/importme.csv"));
            var retainedLines = File.ReadAllLines(pathToDispositionTemplate);
            File.WriteAllLines(pathToImportMeFile, retainedLines);
        }

        private static void PrepareImportFile(List<DispositionRow> updatedRows)
        {
            // Create file importme.csv and write 2 header lines from Check Disposition Template.csv
            PrepareImportHeader();
            string pathToImportMeFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/importme.csv"));

            // Append lines to file importme.csv
            //  using (StreamWriter writer = new StreamWriter(@"C:\\Methodist\\OPID\\Linq\\importme.csv", true))
            using (StreamWriter writer = new StreamWriter(pathToImportMeFile, true))
            {
                foreach (DispositionRow d in updatedRows)
                {
                    string csvRow = string.Format(",{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        d.RecordID,
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

        private static void ProcessChecks(IEnumerable<Check> checks, IEnumerable<DispositionRow> originalRows, List<DispositionRow> updatedRows)
        {
            foreach (var check in checks)
            {
                if (check.Num > 0)
                {
                    DispositionRow d = (from r in updatedRows
                                        where r.LBVDCheckNum == check.Num
                                              || r.TIDCheckNum == check.Num
                                              || r.TDLCheckNum == check.Num
                                              || r.MBVDCheckNum == check.Num
                                              || r.SDCheckNum == check.Num
                                        select r).FirstOrDefault();

                    if (d == null)
                    {
                        d = (from r in originalRows
                             where r.LBVDCheckNum == check.Num
                                   || r.TIDCheckNum == check.Num
                                   || r.TDLCheckNum == check.Num
                                   || r.MBVDCheckNum == check.Num
                                   || r.SDCheckNum == check.Num
                             select r).FirstOrDefault();

                        if (d != null)
                        {
                            UpdateDisposition(check.Num == d.LBVDCheckNum, check.Num == d.TIDCheckNum, check.Num == d.TDLCheckNum, check.Num == d.MBVDCheckNum, check.Num == d.SDCheckNum, d, check);
                            updatedRows.Add(d);
                        }
                    }

                    else
                    {
                        // Found row among already updated rows. There is more than one check
                        // check number on this row. In other words, the client had more than
                        // one check written for the visit this row corresponds to.
                        UpdateDisposition(check.Num == d.LBVDCheckNum, check.Num == d.TIDCheckNum, check.Num == d.TDLCheckNum, check.Num == d.MBVDCheckNum, check.Num == d.SDCheckNum, d, check);
                    }
                }
            }
        }


        [HttpGet]
        public MergeStats GetMerge(string vcFileName, string vcFileType, string apFileName, string apFileType, string qbFileName, string qbFileType)
        {
            string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", (vcFileName == "unknown" ? "VCEmpty" : vcFileName), vcFileType));
            string pathToApricotReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", apFileName, apFileType));
            string pathToQuickbooksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", (qbFileName == "unknown" ? "QBEmpty" : qbFileName), qbFileType));

            string sheetName = "Sheet1";
            int z;

            var voidedChecksFile = new ExcelQueryFactory(pathToVoidedChecksFile);
            var apricotReportFile = new ExcelQueryFactory(pathToApricotReportFile);
            var quickbooksFile = new ExcelQueryFactory(pathToQuickbooksFile);


            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
            quickbooksFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;
            apricotReportFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;
            voidedChecksFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

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

            var checks = from c in quickbooksFile.Worksheet<Check>(sheetName) select c;
            var originalRows = from d in apricotReportFile.Worksheet<DispositionRow>(sheetName) select d;
            var voidedChecks = from vc in voidedChecksFile.Worksheet<Check>(sheetName) select vc;

            matched = new List<int>();
            List<DispositionRow> updatedRows = new List<DispositionRow>();

            ProcessChecks(voidedChecks, originalRows, updatedRows);
            ProcessChecks(checks, originalRows, updatedRows);

            /*
            foreach (var check in checks)
            {
                if (check.Num > 0)
                {
                    DispositionRow d = (from r in updatedRows
                                        where r.LBVDCheckNum == check.Num
                                              || r.TIDCheckNum == check.Num
                                              || r.TDLCheckNum == check.Num
                                              || r.MBVDCheckNum == check.Num
                                              || r.SDCheckNum == check.Num
                                        select r).FirstOrDefault();

                    if (d == null)
                    {
                        d = (from r in originalRows
                             where r.LBVDCheckNum == check.Num
                                   || r.TIDCheckNum == check.Num
                                   || r.TDLCheckNum == check.Num
                                   || r.MBVDCheckNum == check.Num
                                   || r.SDCheckNum == check.Num
                             select r).FirstOrDefault();

                        if (d != null)
                        {
                            UpdateDisposition(check.Num == d.LBVDCheckNum, check.Num == d.TIDCheckNum, check.Num == d.TDLCheckNum, check.Num == d.MBVDCheckNum, check.Num == d.SDCheckNum, d, check);
                            updatedRows.Add(d);
                        }
                    }

                    else
                    {
                        // Found row among already updated rows. There is more than one check
                        // check number on this row. In other words, the client had more than
                        // one check written for the visit this row corresponds to.
                        UpdateDisposition(check.Num == d.LBVDCheckNum, check.Num == d.TIDCheckNum, check.Num == d.TDLCheckNum, check.Num == d.MBVDCheckNum, check.Num == d.SDCheckNum, d, check);
                    }
                }
            }
            */

            PrepareImportFile(updatedRows);
            UpdateUnmatched(originalRows, checks);

            MergeStats ms = new MergeStats
            {
                Matched = matched.Count,
                Unmatched = unmatched
            };

            z = 2;

            return ms;
        }

        [HttpGet]
        public List<Check> GetUnmatched(string recent)
        {
            return unmatched;
        }
    }
}