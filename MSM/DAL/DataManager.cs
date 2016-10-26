using MSM.Models;
using MSM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSM.DAL
{
    public class DataManager
    {
        private static List<int> matchedChecks;
        private static List<int> knownDisposition;

        private static List<Check> unmatchedChecks;
        private static List<Check> resolvedChecks;
        private static List<DispositionRow> updatedRows;

        public static void Init()
        {
            matchedChecks = new List<int>();
            knownDisposition = new List<int>();
            unmatchedChecks = new List<Check>();
            resolvedChecks = new List<Check>();
            updatedRows = new List<DispositionRow>();
        }

        public static List<DispositionRow> GetApricotRows(string apFileName, string apFileType)
        {
            List<DispositionRow> originalRows = new List<DispositionRow>();
            string pathToApricotReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/{0}.{1}", apFileName, apFileType));
            var apricotReportFile = Linq2Excel.GetFactory(pathToApricotReportFile);
            Linq2Excel.PrepareApricotMapping(apricotReportFile);

            var apricotRows = from d in apricotReportFile.Worksheet<DispositionRow>("Sheet1") select d;

            foreach (DispositionRow d in apricotRows)
            {
                originalRows.Add(d);
            }

            return originalRows;
        }

        public static List<Check> GetVoidedChecks(string vcFileName, string vcFileType)
        {
            if (vcFileName.Equals("unknown"))
            {
                return GetEmptyVoidedChecks();
            }

            List<Check> voidedChecks = new List<Check>();
            string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/{0}.{1}", vcFileName, vcFileType));
            var voidedChecksFile = Linq2Excel.GetFactory(pathToVoidedChecksFile);
            var vChecks = from vc in voidedChecksFile.Worksheet<Check>("Sheet1") select vc;

            foreach(Check check in vChecks)
            {
                voidedChecks.Add(check);
            }

            return voidedChecks;
        }

        public static List<Check> GetQuickbookChecks(string qbFileName, string qbFileType)
        {
            if (qbFileName.Equals("unknown"))
            {
                return GetEmptyQuickbookChecks();
            }

            List<Check> quickbookChecks = new List<Check>();
            string pathToQuickbooksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/{0}.{1}", qbFileName, qbFileType));
            var quickbooksFile = Linq2Excel.GetFactory(pathToQuickbooksFile);
            var vChecks = from vc in quickbooksFile.Worksheet<Check>("Sheet1") select vc;

            foreach (Check check in vChecks)
            {
                quickbookChecks.Add(check);
            }

            return quickbookChecks;
        }

        public static List<Check> GetEmptyQuickbookChecks()
        {
            List<Check> quickbookChecks = new List<Check>();
            string pathToQuickbooksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Private/{0}.{1}", "QBEmpty", "xlsx"));
            var quickbooksFile = Linq2Excel.GetFactory(pathToQuickbooksFile);
            var qbChecks = from vc in quickbooksFile.Worksheet<Check>("Sheet1") select vc;

            foreach (Check check in qbChecks)
            {
                quickbookChecks.Add(check);
            }

            return quickbookChecks;
        }

        public static List<Check> GetEmptyVoidedChecks()
        {
            List<Check> voidedChecks = new List<Check>();
            string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Private/{0}.{1}", "VCEmpty", "xlsx"));
            var voidedChecksFile = Linq2Excel.GetFactory(pathToVoidedChecksFile);
            var vChecks = from vc in voidedChecksFile.Worksheet<Check>("Sheet1") select vc;

            foreach(Check check in vChecks)
            {
                voidedChecks.Add(check);
            }

            return voidedChecks;
        }

        public static void SetKnownDisposition(int checkNum)
        {
            knownDisposition.Add(checkNum);
        }

        public static void SetMatchedCheck(int checkNum)
        {
            matchedChecks.Add(checkNum);
        }

        private static bool IsKnownDisposition(int checkNum)
        {
            bool has = knownDisposition.Any(cnum => cnum == checkNum);
            return has;
        }

        public static bool IsMatched(int checkNum)
        {
            bool has = matchedChecks.Any(cnum => cnum == checkNum);
            return has;
        }

        public static bool IsUnmatched(int checkNum)
        {
            bool has = unmatchedChecks.Any(c => c.Num == checkNum);
            return has;
        }

        public static void PersistUnmatchedChecks()
        {
            AppendToLongUnmatched(unmatchedChecks);
        }

        public static void UpdateMatchedChecks()
        {
            MarkAsMatched(knownDisposition);
        }

        public static void RemoveMatchedChecks()
        {
            DeleteMatchedChecks();
        }

        public static Check GetResolvedCheck(LongUnmatched lu, DispositionRow d)
        {
            bool addCheck = false;

            Check resolvedCheck = new Check
            {
                RecordID = d.RecordID,
                InterviewRecordID = d.InterviewRecordID,
                Name = lu.Name, 
                Date = d.Date
            };

            switch (lu.Service)
            {
                case "LBVD":
                    if (d.LBVDCheckNum == lu.Num && d.LBVDCheckDisposition != null)
                    {
                        addCheck = true;
                        lu.Matched = true;
                        resolvedCheck.Num = lu.Num;
                        resolvedCheck.Service = "LBVD";
                        resolvedCheck.Clr = d.LBVDCheckDisposition;
                    }
                    break;

                case "TID":
                    if (d.TIDCheckNum == lu.Num && d.TIDCheckDisposition != null)
                    {
                        addCheck = true;
                        lu.Matched = true;
                        resolvedCheck.Num = lu.Num;
                        resolvedCheck.Service = "TID";
                        resolvedCheck.Clr = d.TIDCheckDisposition;
                    }
                    break;

                case "TDL":
                    if (d.TDLCheckNum == lu.Num && d.TDLCheckDisposition != null)
                    {
                        addCheck = true;
                        lu.Matched = true;
                        resolvedCheck.Num = lu.Num;
                        resolvedCheck.Service = "TDL";
                        resolvedCheck.Clr = d.TDLCheckDisposition;
                    }
                    break;

                case "MBVD":
                    if (d.MBVDCheckNum == lu.Num && d.MBVDCheckDisposition != null)
                    {
                        addCheck = true;
                        lu.Matched = true;
                        resolvedCheck.Num = lu.Num;
                        resolvedCheck.Service = "MBVD";
                        resolvedCheck.Clr = d.MBVDCheckDisposition;
                    }
                    break;

                case "SD":
                    if (d.SDCheckNum == lu.Num && d.SDCheckDisposition != null)
                    {
                        addCheck = true;
                        lu.Matched = true;
                        resolvedCheck.Num = lu.Num;
                        resolvedCheck.Service = "SD";
                        resolvedCheck.Clr = d.SDCheckDisposition;
                    }
                    break;
            }

            if (addCheck)
            {
                return resolvedCheck;
            }

            return null;
        }

        private static void MarkAsMatched(List<int> knownDisposition)
        {
            using (var dbCtx = new MSMEntities())
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

        public static void NewUnmatchedCheck(DispositionRow row, string service)
        {
            int checkNum;

            switch (service)
            {
                case "LBVD":
                    checkNum = row.LBVDCheckNum;
                    break;
                case "TID":
                    checkNum = row.TIDCheckNum;
                    break;
                case "TDL":
                    checkNum = row.TDLCheckNum;
                    break;
                case "MBVD":
                    checkNum = row.MBVDCheckNum;
                    break;
                case "SD":
                    checkNum = row.SDCheckNum;
                    break;
                default:
                    checkNum = -1;
                    break;
            }

             unmatchedChecks.Add(new Check
                    {
                        RecordID = row.RecordID,
                        InterviewRecordID = row.InterviewRecordID,
                        Num = checkNum,    
                        Name = string.Format("{0}, {1}", row.Lname, row.Fname),
                        Date = row.Date,
                        Service = service
                    });
        }

        public static void NewResolvedCheck(Check check)
        {
            resolvedChecks.Add(check);
        }

        public static Check GetExistingResolvedCheck(int interviewRecordID)
        {
            return (from c in resolvedChecks
                              where c.InterviewRecordID == interviewRecordID
                              select c).FirstOrDefault();
        }

        public static List<Check> GetResolvedChecks()
        {
            if (resolvedChecks == null)
            {
                return new List<Check>();
            }

            return resolvedChecks;
        }

        public static List<DispositionRow> GetUpdatedRows()
        {
            return updatedRows;
        }

        public static void NewUpdatedRow(DispositionRow d)
        {
            updatedRows.Add(d);
        }

        public static List<DispositionRow> GetLongUnmatchedDispositionRows()
        {
            List<DispositionRow> longUnmatchedDispositionRows = new List<DispositionRow>();

            using (var dbCtx = new MSMEntities())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (var unmatched in longUnmatched)
                {
                    if (unmatched.InterviewRecordID != 0)
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

        public static void UpdateResolvedChecks()
        {
            using (var dbCtx = new MSMEntities())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (LongUnmatched lu in longUnmatched)
                {
                    DispositionRow d = (from row in GetUpdatedRows()
                                        where row.InterviewRecordID == lu.InterviewRecordID
                                        select row).FirstOrDefault();

                    if (d != null)
                    {
                        Check existing = GetExistingResolvedCheck(d.InterviewRecordID);

                        if (existing == null)
                        {
                            // Prevent addition of duplicates to list of resolvedChecks.
                            Check resolvedCheck = GetResolvedCheck(lu, d);

                            if (resolvedCheck != null)
                            {
                               NewResolvedCheck(resolvedCheck);
                            }
                        }
                    }
                }

                dbCtx.SaveChanges();
            }
        }

        private static void AppendToLongUnmatched(List<Check> unmatchedChecks)
        {
            using (var dbCtx = new MSMEntities())
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

        private static void DeleteMatchedChecks()
        {
            using (var dbCtx = new MSMEntities())
            {
                dbCtx.LongUnmatcheds.RemoveRange(dbCtx.LongUnmatcheds.Where(lu => lu.Matched == true));
                dbCtx.SaveChanges();
            }
        }

    }
}