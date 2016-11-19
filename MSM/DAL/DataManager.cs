using MSM.Models;
using MSM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Excel;
using System.Data;

namespace MSM.DAL
{
    public class DataManager
    {
        private static bool firstCall = true;
        private static List<int> knownDisposition;

        private static List<Check> unmatchedChecks;
        private static List<Check> resolvedChecks;
        private static List<DispositionRow> updatedRows;

        public static void Init()
        {
            if (firstCall)
            {
                knownDisposition = new List<int>();
                unmatchedChecks = new List<Check>();
                resolvedChecks = new List<Check>();
                updatedRows = new List<DispositionRow>();
                firstCall = false;
            }
        }

        public static List<DispositionRow> GetApricotRows(string apFileName, string apFileType)
        {
           // List<DispositionRow> originalRows = new List<DispositionRow>();
          //  string pathToApricotReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/{0}.{1}", apFileName, apFileType));
            string pathToApricotReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", apFileName, apFileType));

            List<DispositionRow> apricotRows = ExcelDataReader.GetApricotRows(pathToApricotReportFile);
            
         //   var apricotReportFile = Linq2Excel.GetFactory(pathToApricotReportFile);
        //    Linq2Excel.PrepareApricotMapping(apricotReportFile);

         //   var apricotRows = from d in apricotReportFile.Worksheet<DispositionRow>("Sheet1") select d;

        //    foreach (DispositionRow d in apricotRows)
       //     {
        //        originalRows.Add(d);
        //    }

            return apricotRows;
        }

        public static List<Check> GetVoidedChecks(string vcFileName, string vcFileType)
        {
            if (vcFileName.Equals("unknown"))
            {
                return GetEmptyVoidedChecks();
            }

            //List<Check> voidedChecks = new List<Check>();
            string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", vcFileName, vcFileType));

            List<Check> voidedChecks = ExcelDataReader.GetVoidedChecks(pathToVoidedChecksFile);

            /*
            var voidedChecksFile = Linq2Excel.GetFactory(pathToVoidedChecksFile);
            var vChecks = from vc in voidedChecksFile.Worksheet<Check>("Sheet1") select vc;

            foreach(Check check in vChecks)
            {
                voidedChecks.Add(check);
            }
            */

            return voidedChecks;
        }

        public static List<Check> GetQuickbooksChecks(string qbFileName, string qbFileType)
        {
            if (qbFileName.Equals("unknown"))
            {
                return GetEmptyQuickbookChecks();
            }

           // List<Check> quickbooksChecks = new List<Check>();
            string pathToQuickbooksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", qbFileName, qbFileType));

            List<Check> quickbooksChecks = ExcelDataReader.GetQuickbooksChecks(pathToQuickbooksFile);

            /*
            var quickbooksFile = Linq2Excel.GetFactory(pathToQuickbooksFile);
            var vChecks = from vc in quickbooksFile.Worksheet<Check>("Sheet1") select vc;

            foreach (Check check in vChecks)
            {
                quickbookChecks.Add(check);
            }
            */

            return quickbooksChecks;
        }

        public static List<Check> GetEmptyQuickbookChecks()
        {
           // List<Check> quickbooksChecks = new List<Check>();
            //string pathToQuickbooksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Private/{0}.{1}", "QBEmpty", "xlsx"));
            string pathToQuickbooksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", "QBEmpty", "xlsx"));

            List<Check> quickbooksChecks = ExcelDataReader.GetQuickbooksChecks(pathToQuickbooksFile);

            /*
            var quickbooksFile = Linq2Excel.GetFactory(pathToQuickbooksFile);
            
            var qbChecks = from vc in quickbooksFile.Worksheet<Check>("Sheet1") select vc;

            foreach (Check check in qbChecks)
            {
                quickbooksChecks.Add(check);
            }
            */
            return quickbooksChecks;
        }

        public static List<Check> GetEmptyVoidedChecks()
        {
           // List<Check> voidedChecks = new List<Check>();
           // string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Private/{0}.{1}", "VCEmpty", "xlsx"));
            string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/{0}.{1}", "VCEmpty", "xlsx"));

            List<Check> voidedChecks = ExcelDataReader.GetVoidedChecks(pathToVoidedChecksFile);

            /*
            var voidedChecksFile = Linq2Excel.GetFactory(pathToVoidedChecksFile);
            var vChecks = from vc in voidedChecksFile.Worksheet<Check>("Sheet1") select vc;

            foreach(Check check in vChecks)
            {
                voidedChecks.Add(check);
            }
            */

            return voidedChecks;
        }

        public static void SetKnownDisposition(int checkNum)
        {
            knownDisposition.Add(checkNum);
        }

        public static bool IsKnownDisposition(int checkNum)
        {
            bool has = knownDisposition.Any(cnum => cnum == checkNum);
            return has;
        }

        public static void PersistUnmatchedChecks()
        {
            AppendToLongUnmatched(unmatchedChecks);
        }
    
        private static bool IsResolved(int checkNum)
        {
            var rc = (from c in resolvedChecks
                      where c.Num == checkNum
                      select c).FirstOrDefault();

            return rc != null;
        }

        public static void MarkResolvedChecks()
        {
            using (var dbCtx = new MSMEntities())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (LongUnmatched lu in longUnmatched)
                {
                    if (IsResolved(lu.Num))
                    {
                        lu.Matched = true;
                    }
                }

                dbCtx.SaveChanges();
            }
        }

        public static void RemoveResolvedChecks()
        {
            MarkResolvedChecks();
            DeleteMatchedChecks();
        }

        public static void NewResolvedCheck(DispositionRow row, string service)
        {
            int checkNum;
            string status;

            switch (service)
            {
                case "LBVD":
                    checkNum = row.LBVDCheckNum;
                    status = row.LBVDCheckDisposition;
                    break;
                case "TID":
                    checkNum = row.TIDCheckNum;
                    status = row.TIDCheckDisposition;
                    break;
                case "TDL":
                    checkNum = row.TDLCheckNum;
                    status = row.TDLCheckDisposition;
                    break;
                case "MBVD":
                    checkNum = row.MBVDCheckNum;
                    status = row.MBVDCheckDisposition;
                    break;
                case "SD":
                    checkNum = row.SDCheckNum;
                    status = row.SDCheckDisposition;
                    break;
                default:
                    checkNum = -1;
                    status = "unknown";
                    break;
            }

            if (!IsResolved(checkNum))
            {
                resolvedChecks.Add(new Check
                {
                    RecordID = row.RecordID,
                    InterviewRecordID = row.InterviewRecordID,
                    Num = checkNum,
                    Name = (row.Name != null ? row.Name : string.Format("{0}, {1}", row.Lname, row.Fname)),
                    Date = row.Date,
                    Clr = status,
                    Service = service
                });
            }
        }

        public static void NewResolvedCheck(Check check, string status)
        {
            check.Clr = status;
            if (!IsResolved(check.Num))
            {
                resolvedChecks.Add(check);
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

        public static List<Check> GetLongUnmatchedChecks()
        {
            List<Check> longUnmatchedChecks = new List<Check>();

            using (var dbCtx = new MSMEntities())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (var lu in longUnmatched)
                {
                    longUnmatchedChecks.Add(new Check
                    {
                        RecordID = lu.RecordID,
                        InterviewRecordID = lu.InterviewRecordID,
                        Num = lu.Num,
                        Name = lu.Name,
                        Date = lu.Date,
                        Service = lu.Service
                    });
                }
            }

            return longUnmatchedChecks;
        }

        private static void AppendToLongUnmatched(List<Check> checks)
        {
            using (var dbCtx = new MSMEntities())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (Check check in checks)
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

        public static string ResolveCheck(int checkNum)
        {
            string status;
             
            using (var dbCtx = new MSMEntities())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                var check = (from lu in longUnmatched
                             where lu.Num == checkNum
                             select lu).FirstOrDefault();

                if (check == null)
                {
                    status = string.Format("<p>Could not find check with number {0} in research table.<p>", checkNum);
                }
                else
                {
                    longUnmatched.Remove(check);
                    dbCtx.SaveChanges();
                    status = string.Format("<p>Removed from research table:<br/>&nbsp;&nbsp;&nbsp;Date: {0}<br/>&nbsp;&nbsp;&nbsp;Record ID: {1}<br/>&nbsp;&nbsp;&nbsp;Interview Record ID: {2}<br/>&nbsp;&nbsp;&nbsp;Name: {3}<br/>&nbsp;&nbsp;&nbsp;Check number: {4}<br/>&nbsp;&nbsp;&nbsp;Service: {5}</p>", check.Date.ToString("d"), check.RecordID, check.InterviewRecordID, check.Name, check.Num, check.Service);
                }

                return status;
            }
        }
    }
}