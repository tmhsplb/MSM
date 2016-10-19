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
   
    public class MergeController : ApiController
    {
        private static List<int> usedChecks = new List<int>();
        private static List<int> matchedChecks = new List<int>();
        private static List<int> knownDisposition = new List<int>();

        private static List<Check> unmatchedChecks = new List<Check>();

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
                    if (check.Clr != null && check.Clr[0] == 0xD6)
                    {
                        // Check mark in Quickbooks is character 0xD6
                        return "Cleared";
                    }
                    return "Voided";
            }
        }

        private static void UpdateDisposition(bool lbvd, bool tid, bool tdl, bool mbvd, bool sd, DispositionRow d, Check check)
        {
            if (lbvd)
            {
                d.LBVDCheckDisposition = GetDispositionFromCheck(check);
                matchedChecks.Add(d.LBVDCheckNum);
            }

            if (tid)
            {
                d.TIDCheckDisposition = GetDispositionFromCheck(check);
                matchedChecks.Add(d.TIDCheckNum);
            }

            if (tdl)
            {
                d.TDLCheckDisposition = GetDispositionFromCheck(check);
                matchedChecks.Add(d.TDLCheckNum);
            }

            if (mbvd)
            {
                d.MBVDCheckDisposition = GetDispositionFromCheck(check);
                matchedChecks.Add(d.MBVDCheckNum);
            }

            if (sd)
            {
                d.SDCheckDisposition = GetDispositionFromCheck(check);
                matchedChecks.Add(d.SDCheckNum);
            }

            if (lbvd || tid || tdl || mbvd || sd)
            {
                // Parameter check belongs to the set of used checks if it
                // provided the disposition for some check along the DispositionRow 
                // in parameter d. After all checks have been processed, those whose
                // check number is not among the set of used checks will be
                // added to the set of unmatched checks. See the end of method
                // UpdateUnmatched.
                if (check.Num == 70510)
                {
                    int z;
                    z = 2;
                }
                usedChecks.Add(check.Num);
            }
        }

        private static bool IsUsed(int checkNum)
        {
            bool has = usedChecks.Any(cnum => cnum == checkNum);
            return has;
        }

        private static bool IsMatched(int checkNum)
        {
            bool has = matchedChecks.Any(cnum => cnum == checkNum);
            return has;
        }

        private static bool IsUnmatched(int checkNum)
        {
            bool has = unmatchedChecks.Any(c => c.Num == checkNum);
            return has;
        }
        private static bool IsKnownDisposition(int checkNum)
        {
            bool has = knownDisposition.Any(cnum => cnum == checkNum);
            return has;
        }

        private static void UpdateLBVD(DispositionRow row)
        {
            if (row.LBVDCheckNum != 0 && !IsMatched(row.LBVDCheckNum) && !IsUnmatched(row.LBVDCheckNum))
            {
                if (row.LBVDCheckDisposition != null)
                {
                    knownDisposition.Add(row.LBVDCheckNum);
                }
                else
                {
                    unmatchedChecks.Add(new Check
                    {
                        InterviewRecordID = row.RecordID,
                        Num = row.LBVDCheckNum,
                        Date = row.Date,
                        Type = AP,
                        Service = "LBVD"
                    });
                }
            }
        }

        private static void UpdateTID(DispositionRow row)
        {
            if (row.TIDCheckNum != 0 && !IsMatched(row.TIDCheckNum) && !IsUnmatched(row.TIDCheckNum))
            {
                if (row.TIDCheckDisposition != null)
                {
                    knownDisposition.Add(row.TIDCheckNum);
                }
                else
                {
                    unmatchedChecks.Add(new Check
                    {
                        InterviewRecordID = row.RecordID,
                        Num = row.TIDCheckNum,
                        Date = row.Date,
                        Type = AP,
                        Service = "TID"
                    });
                }
            }
        }

        private static void UpdateTDL(DispositionRow row)
        {
            if (row.TDLCheckNum != 0 && !IsMatched(row.TDLCheckNum) && !IsUnmatched(row.TDLCheckNum))
                {
                    if (row.TDLCheckDisposition != null)
                    {
                        knownDisposition.Add(row.TDLCheckNum);
                    }
                    else
                    {
                        unmatchedChecks.Add(new Check
                        {
                            InterviewRecordID = row.RecordID,
                            Num = row.TDLCheckNum,
                            Date = row.Date,
                            Type = AP,
                            Service = "TDL"
                        });
                    }
                }
        }

        private static void UpdateMBVD(DispositionRow row)
        {
            if (row.MBVDCheckNum != 0 && !IsMatched(row.MBVDCheckNum) && !IsUnmatched(row.MBVDCheckNum))
            {
                if (row.MBVDCheckDisposition != null)
                {
                    knownDisposition.Add(row.MBVDCheckNum);
                }
                else
                {
                    unmatchedChecks.Add(new Check
                    {
                        InterviewRecordID = row.RecordID,
                        Num = row.MBVDCheckNum,
                        Date = row.Date,
                        Type = AP,
                        Service = "MBVD"
                    });
                }
            }
        }

        private static void UpdateSD(DispositionRow row)
        {
            if (row.SDCheckNum != 0 && !IsMatched(row.SDCheckNum) && !IsUnmatched(row.SDCheckNum))
            {
                if (row.SDCheckDisposition != null)
                {
                    knownDisposition.Add(row.SDCheckNum);
                }
                else
                {
                    unmatchedChecks.Add(new Check
                    {
                        InterviewRecordID = row.RecordID,
                        Num = row.SDCheckNum,
                        Date = row.Date,
                        Type = AP,
                        Service = "SD"
                    });
                }
            }
        }

        private static void UpdateUnmatchedChecks(IQueryable<DispositionRow> originalRows, IQueryable<Check> checks)
        {
            foreach (DispositionRow row in originalRows)
            {
                UpdateLBVD(row);
                UpdateTID(row);
                UpdateTDL(row);
                UpdateMBVD(row);
                UpdateSD(row);
            }

            // If a check number is not among the set of used checks or its disposition in not known,
            // then it is an unmatched check.
            foreach (Check check in checks)
            {
                if (check.Num > 0 && !(IsUsed(check.Num) || IsKnownDisposition(check.Num)))
                {
                    string service = (check.Clr != null ? check.Clr : "V");

                    unmatchedChecks.Add(new Check
                    {
                        Num = check.Num,
                        Date = check.Date,
                        Clr = check.Clr,
                        Type = QB,
                        Service = service
                    });
                }
            }
        } 

        private static void PrepareImportHeader()
        {
          //  var retainedLines = File.ReadAllLines(@"C:\\Methodist\\OPID\\Linq\\Check Disposition Template.csv");
          //  File.WriteAllLines(@"C:\\Methodist\\OPID\\Linq\\importme.csv", retainedLines);

            string pathToDispositionTemplate = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Check Disposition Header.csv"));
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
                        // number on this row. In other words, the client had more than
                        // one check written for the visit this row corresponds to.
                        UpdateDisposition(check.Num == d.LBVDCheckNum, check.Num == d.TIDCheckNum, check.Num == d.TDLCheckNum, check.Num == d.MBVDCheckNum, check.Num == d.SDCheckNum, d, check);
                    }
                }
            }
        }

        private static void ProcessLongUnmatched()
        {
            using (var dbCtx = new MSMEntities())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (Check check in unmatchedChecks)
                {
                    LongUnmatched existing = (from c in longUnmatched
                                              where c.InterviewRecordID == check.InterviewRecordID
                                              select c).FirstOrDefault();

                    if (existing == null)
                    {
                        LongUnmatched unm = new LongUnmatched
                        {
                            InterviewRecordID = check.InterviewRecordID,
                            Num = check.Num,
                            Date = check.Date,
                            Type = check.Type,
                            Service = check.Service,
                            Matched = false
                        };

                        longUnmatched.Add(unm);
                        // dbCtx.SaveChanges();

                    }
                }

                dbCtx.SaveChanges();
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

           // var voidedChecksFile = new ExcelQueryFactory(pathToVoidedChecksFile);
           // var quickbooksFile = new ExcelQueryFactory(pathToQuickbooksFile);


            // From: http://stackoverflow.com/questions/15741303/64-bits-alternatives-to-linq-to-excel
           // quickbooksFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;
           // voidedChecksFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;

            var voidedChecksFile = Linq2Excel.GetFactory(pathToVoidedChecksFile);
            var quickbooksFile = Linq2Excel.GetFactory(pathToQuickbooksFile);

          //  var apricotReportFile = new ExcelQueryFactory(pathToApricotReportFile);

            var apricotReportFile = Linq2Excel.GetFactory(pathToApricotReportFile);
            Linq2Excel.PrepareApricotMapping(apricotReportFile);

            /*
            apricotReportFile.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;
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
            */

            var qbChecks = from c in quickbooksFile.Worksheet<Check>(sheetName) select c;
            var originalRows = from d in apricotReportFile.Worksheet<DispositionRow>(sheetName) select d;
            var voidedChecks = from vc in voidedChecksFile.Worksheet<Check>(sheetName) select vc;

            matchedChecks = new List<int>();  // initialize the gloabal variable matchedChecks
            usedChecks = new List<int>();
            knownDisposition = new List<int>();
            unmatchedChecks = new List<Check>();
            List<DispositionRow> updatedRows = new List<DispositionRow>();
           
            

            ProcessChecks(voidedChecks, originalRows, updatedRows);
            ProcessChecks(qbChecks, originalRows, updatedRows);

            PrepareImportFile(updatedRows);

            UpdateUnmatchedChecks(originalRows, voidedChecks);
            UpdateUnmatchedChecks(originalRows, qbChecks);

            ProcessLongUnmatched();

            MergeStats ms = new MergeStats
            {
                Matched = matchedChecks.Count,
                Unmatched = unmatchedChecks
            };

            z = 2;

            return ms;
        }

        [HttpGet]
        public List<Check> GetUnmatchedChecks(string recent)
        {
            return unmatchedChecks;
        }
    }
}