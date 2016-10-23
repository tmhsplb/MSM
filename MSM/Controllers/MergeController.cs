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
        private static List<Check> resolvedChecks = new List<Check>();

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
                        RecordID = row.RecordID,
                        InterviewRecordID = row.InterviewRecordID,
                        Num = row.LBVDCheckNum,
                        // Name = row.Name,
                        Name = string.Format("{0}, {1}", row.Lname, row.Fname),
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
                        RecordID = row.RecordID,
                        InterviewRecordID = row.InterviewRecordID,
                        Num = row.TIDCheckNum,
                        //  Name = row.Name,
                        Name = string.Format("{0}, {1}", row.Lname, row.Fname),
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
                        RecordID = row.RecordID,
                        InterviewRecordID = row.InterviewRecordID,
                        Num = row.TDLCheckNum,
                        // Name = row.Name,
                        Name = string.Format("{0}, {1}", row.Lname, row.Fname),
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
                        RecordID = row.RecordID,
                        InterviewRecordID = row.InterviewRecordID,
                        Num = row.MBVDCheckNum,
                        // Name = row.Name,
                        Name = string.Format("{0}, {1}", row.Lname, row.Fname),
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
                        RecordID = row.RecordID,
                        InterviewRecordID = row.InterviewRecordID,
                        Num = row.SDCheckNum,
                        //  Name = row.Name,
                        Name = string.Format("{0}, {1}", row.Lname, row.Fname),
                        Date = row.Date,
                        Type = AP,
                        Service = "SD"
                    });
                }
            }
        }

        private static void DetermineUnmatchedChecks(IQueryable<DispositionRow> originalRows)
        {
            foreach (DispositionRow row in originalRows)
            {
                UpdateLBVD(row);
                UpdateTID(row);
                UpdateTDL(row);
                UpdateMBVD(row);
                UpdateSD(row);
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

        private static void AppendToLongUnmatched(List<Check> unmatchedChecks)
        {
            using (var dbCtx = new MSMEntities1())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (Check check in unmatchedChecks)
                {
                    LongUnmatched existing = (from c in longUnmatched
                                              where c.Num == check.Num
                                              select c).FirstOrDefault();

                    if (existing == null && !IsKnownDisposition(check.Num))
                    {
                        LongUnmatched unm = new LongUnmatched
                        {
                            RecordID = check.RecordID,
                            InterviewRecordID = check.InterviewRecordID,
                            Num = check.Num,
                            Name = check.Name,
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

        private static void PrepareImportHeader()
        {
            string pathToDispositionHeader = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Check Disposition Header.csv"));
            string pathToImportMeFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/importme.csv"));
            var retainedLines = File.ReadAllLines(pathToDispositionHeader);
            File.WriteAllLines(pathToImportMeFile, retainedLines);
        }

        private static void PrepareImportFile(List<DispositionRow> updatedRows)
        {
            // Create file importme.csv and write 2 header lines from Check Disposition Header.csv
            PrepareImportHeader();
            string pathToImportMeFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/importme.csv"));

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

        private static List<DispositionRow> GetLongUnmatchedDispositionRows()
        {
            List<DispositionRow> longUnmatchedDispositionRows = new List<DispositionRow>();
            List<Check> supplementalQuickbookChecks = new List<Check>();

            using (var dbCtx = new MSMEntities1())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (var unmatched in longUnmatched)
                {
                    if (unmatched.InterviewRecordID != 0 && unmatched.Type == AP)
                    {
                        DispositionRow d = (from r in longUnmatchedDispositionRows
                                            where r.InterviewRecordID == unmatched.InterviewRecordID
                                            select r).FirstOrDefault();

                        if (d == null)
                        {
                            d = new DispositionRow
                            {
                                RecordID = unmatched.RecordID,
                                Name = unmatched.Name,
                                InterviewRecordID = unmatched.InterviewRecordID,
                                Date = unmatched.Date
                            };
                        }

                        switch (unmatched.Service)
                        {
                            case "LBVD":
                                d.LBVDCheckNum = unmatched.Num;
                                break;
                            case "TID":
                                d.TIDCheckNum = unmatched.Num;
                                break;
                            case "TDL":
                                d.TDLCheckNum = unmatched.Num;
                                break;
                            case "MBVD":
                                d.MBVDCheckNum = unmatched.Num;
                                break;
                            case "SD":
                                d.SDCheckNum = unmatched.Num;
                                break;
                        }

                        longUnmatchedDispositionRows.Add(d);
                    }
                }
            }

            return longUnmatchedDispositionRows;
        }

        private static void ResolveLongUnmatched(string vcFileName, string vcFileType, string qbFileName, string qbFileType)
        {
            List<DispositionRow> longUnmatchedDispositionRows = GetLongUnmatchedDispositionRows();
            ProcessLongUnmatched(longUnmatchedDispositionRows, vcFileName, vcFileType, qbFileName, qbFileType);
        }

        private static void ProcessLongUnmatched(List<DispositionRow> longUnmatchedDispositionRows, string vcFileName, string vcFileType, string qbFileName, string qbFileType)
        {
            string sheetName = "Sheet1";
            string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", (vcFileName == "unknown" ? "VCEmpty" : vcFileName), vcFileType));
            string pathToQuickbooksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", (qbFileName == "unknown" ? "QBEmpty" : qbFileName), qbFileType));

            var voidedChecksFile = Linq2Excel.GetFactory(pathToVoidedChecksFile);
            var quickbooksFile = Linq2Excel.GetFactory(pathToQuickbooksFile);

            var qbChecks = from c in quickbooksFile.Worksheet<Check>(sheetName) select c;
            var voidedChecks = from vc in voidedChecksFile.Worksheet<Check>(sheetName) select vc;

            matchedChecks = new List<int>();
            usedChecks = new List<int>();
            knownDisposition = new List<int>();
            unmatchedChecks = new List<Check>();
            List<DispositionRow> updatedRows = new List<DispositionRow>();

            ProcessChecks(voidedChecks, longUnmatchedDispositionRows, updatedRows);
            ProcessChecks(qbChecks, longUnmatchedDispositionRows, updatedRows);

            DetermineUnmatchedChecks(longUnmatchedDispositionRows);

            AppendToLongUnmatched(unmatchedChecks);

            MarkAsMatched(updatedRows);

            ResolvedController.SetResolved(resolvedChecks);

            PrepareImportFile(updatedRows);
        }

        private static void DetermineUnmatchedChecks(List<DispositionRow> rows)
        {
            foreach (DispositionRow row in rows)
            {
                UpdateLBVD(row);
                UpdateTID(row);
                UpdateTDL(row);
                UpdateMBVD(row);
                UpdateSD(row);
            }
        }

        private static void MarkAsMatched(List<DispositionRow> updatedRows)
        {
            using (var dbCtx = new MSMEntities1())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (LongUnmatched lu in longUnmatched)
                {
                    DispositionRow d = (from row in updatedRows
                                        where row.InterviewRecordID == lu.InterviewRecordID
                                        select row).FirstOrDefault();

                    if (d != null)
                    {
                        Check resolvedCheck = new Check
                        {
                            RecordID = d.RecordID,
                            InterviewRecordID = d.InterviewRecordID,
                            Name = lu.Name, /* string.Format("{0}, {1}", d.Lname, d.Fname), */
                            Date = d.Date
                        };

                        switch (lu.Service)
                        {
                            case "LBVD":
                                if (d.LBVDCheckNum == lu.Num)
                                {
                                    lu.Matched = true;
                                    resolvedCheck.Num = lu.Num;
                                    resolvedCheck.Service = "LBVD";
                                    resolvedCheck.Clr = d.LBVDCheckDisposition;
                                }
                                break;
                            case "TID":
                                if (d.TIDCheckNum == lu.Num)
                                {
                                    lu.Matched = true;
                                    resolvedCheck.Num = lu.Num;
                                    resolvedCheck.Service = "TID";
                                    resolvedCheck.Clr = d.TIDCheckDisposition;
                                }
                                break;
                            case "TDL":
                                if (d.TDLCheckNum == lu.Num)
                                {
                                    lu.Matched = true;
                                    resolvedCheck.Num = lu.Num;
                                    resolvedCheck.Service = "TDL";
                                    resolvedCheck.Clr = d.TDLCheckDisposition;
                                }
                                break;
                            case "MBVD":
                                if (d.MBVDCheckNum == lu.Num)
                                {
                                    lu.Matched = true;
                                    resolvedCheck.Num = lu.Num;
                                    resolvedCheck.Service = "MBVD";
                                    resolvedCheck.Clr = d.MBVDCheckDisposition;
                                }
                                break;
                            case "SD":
                                if (d.SDCheckNum == lu.Num)
                                {
                                    lu.Matched = true;
                                    resolvedCheck.Num = lu.Num;
                                    resolvedCheck.Service = "SD";
                                    resolvedCheck.Clr = d.SDCheckDisposition;
                                }
                                break;
                        }

                        resolvedChecks.Add(resolvedCheck);
                    }
                }

                dbCtx.SaveChanges();
            }
        }

        private static void MarkAsMatched(List<int> knownDisposition)
        {
            using (var dbCtx = new MSMEntities1())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (LongUnmatched lu in longUnmatched)
                {
                    if (IsKnownDisposition(lu.Num))
                    {
                        lu.Matched = true;
                    }
                }

                dbCtx.SaveChanges();
            }
        }

        private static void UpdateLongUnmatched(string apFileName, string apFileType)
        {
            // Deliberately get the file VCEmpty.xlsx.
            string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", "VCEmpty", "xlsx"));
            string pathToApricotReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", apFileName, apFileType));

            var voidedChecksFile = Linq2Excel.GetFactory(pathToVoidedChecksFile);
            var apricotReportFile = Linq2Excel.GetFactory(pathToApricotReportFile);
            Linq2Excel.PrepareApricotMapping(apricotReportFile);

            var originalRows = from d in apricotReportFile.Worksheet<DispositionRow>("Sheet1") select d;
            var voidedChecks = from vc in voidedChecksFile.Worksheet<Check>("Sheet1") select vc;

            matchedChecks = new List<int>();
            usedChecks = new List<int>();
            knownDisposition = new List<int>();
            unmatchedChecks = new List<Check>();
            List<DispositionRow> updatedRows = new List<DispositionRow>();

            // This is tricky. We know that the set of voided checks is empty. So this call has
            // the effect of creating a set of unmatched checks from the Apricot Report File.
            // This set will then be appended to the long unmatched checks below.
            // This call also has the side effect of creating a list of check numbers of
            // known disposition. This set will be used to mark as matched any check number
            // of known disposition that occurs among the long unmatched checks.
            ProcessChecks(voidedChecks, originalRows, updatedRows);

            DetermineUnmatchedChecks(originalRows);
            AppendToLongUnmatched(unmatchedChecks);
            MarkAsMatched(knownDisposition);
        }


        [HttpGet]
        public void PerformMerge(string vcFileName, string vcFileType, string apFileName, string apFileType, string qbFileName, string qbFileType)
        {
            if (apFileName.Equals("unknown"))
            {
                // The user did not specify an Apricot Reprot File on the merge screen. The user is trying
                // to resolve some long unmatched checks.
                ResolveLongUnmatched(vcFileName, vcFileType, qbFileName, qbFileType);
                return;
            }

            if (vcFileName.Equals("unknown") && qbFileName.Equals("unknown"))
            {
                // The user has only specified an Apricot Report File. Use this file to update the 
                // long unmatched checks.
                UpdateLongUnmatched(apFileName, apFileType);
                return;
            }

            string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", (vcFileName == "unknown" ? "VCEmpty" : vcFileName), vcFileType));
            string pathToApricotReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", apFileName, apFileType));
            string pathToQuickbooksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", (qbFileName == "unknown" ? "QBEmpty" : qbFileName), qbFileType));

            string sheetName = "Sheet1";
            int z;

            var voidedChecksFile = Linq2Excel.GetFactory(pathToVoidedChecksFile);
            var quickbooksFile = Linq2Excel.GetFactory(pathToQuickbooksFile);
            var apricotReportFile = Linq2Excel.GetFactory(pathToApricotReportFile);
            Linq2Excel.PrepareApricotMapping(apricotReportFile);

            var qbChecks = from c in quickbooksFile.Worksheet<Check>(sheetName) select c;
            var originalRows = from d in apricotReportFile.Worksheet<DispositionRow>(sheetName) select d;
            var voidedChecks = from vc in voidedChecksFile.Worksheet<Check>(sheetName) select vc;

            matchedChecks = new List<int>();
            usedChecks = new List<int>();
            knownDisposition = new List<int>();
            unmatchedChecks = new List<Check>();
            List<DispositionRow> updatedRows = new List<DispositionRow>();

            ProcessChecks(voidedChecks, originalRows, updatedRows);
            ProcessChecks(qbChecks, originalRows, updatedRows);

            DetermineUnmatchedChecks(originalRows);

            AppendToLongUnmatched(unmatchedChecks);

            MarkAsMatched(updatedRows);

            ResolvedController.SetResolved(resolvedChecks);


            PrepareImportFile(updatedRows);
        }
    }
}