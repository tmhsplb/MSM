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
              //  knownDisposition = new List<int>();
                unmatchedChecks = new List<Check>();
                resolvedChecks = new List<Check>();
            //    updatedRows = new List<DispositionRow>();
                firstCall = false;
            }

            knownDisposition = new List<int>();
            updatedRows = new List<DispositionRow>();
        }

        public static List<DispositionRow> GetResearchRows(string apFileName, string apFileType)
        {
           // List<DispositionRow> originalRows = new List<DispositionRow>();
          //  string pathToApricotReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/{0}.{1}", apFileName, apFileType));
            string pathToApricotReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}.{1}", apFileName, apFileType));

            List<DispositionRow> apricotRows = ExcelDataReader.GetResearchRows(pathToApricotReportFile);
            
            /*
            var apricotReportFile = Linq2Excel.GetFactory(pathToApricotReportFile);
            Linq2Excel.PrepareApricotMapping(apricotReportFile);

            var apricotRows = from d in apricotReportFile.Worksheet<DispositionRow>("Sheet1") select d;

            foreach (DispositionRow d in apricotRows)
            {
                originalRows.Add(d);
            }
            */

            return apricotRows;
        }

        public static List<Check> GetVoidedChecks(string vcFileName, string vcFileType)
        {
            if (vcFileName.Equals("unknown"))
            {
                return GetEmptyVoidedChecks();
            }

            //List<Check> voidedChecks = new List<Check>();
            string pathToVoidedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}.{1}", vcFileName, vcFileType));

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
            string pathToQuickbooksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}.{1}", qbFileName, qbFileType));

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

        private static List<Check> DetermineUnmatchedChecks(List<DispositionRow> researchRows)
        {
            foreach (DispositionRow row in researchRows)
            {
                if (row.LBVDCheckNum != 0 && string.IsNullOrEmpty(row.LBVDCheckDisposition))
                {
                    NewUnmatchedCheck(row, "LBVD");
                }

                if (row.TIDCheckNum != 0 && string.IsNullOrEmpty(row.TIDCheckDisposition))
                {
                    NewUnmatchedCheck(row, "TID");
                }

                if (row.TDLCheckNum != 0 && string.IsNullOrEmpty(row.TDLCheckDisposition))
                {
                    NewUnmatchedCheck(row, "TDL");
                }

                if (row.MBVDCheckNum != 0 && string.IsNullOrEmpty(row.MBVDCheckDisposition))
                {
                    NewUnmatchedCheck(row, "MBVD");
                }

                if (row.SDCheckNum != 0 && string.IsNullOrEmpty(row.SDCheckDisposition))
                {
                    NewUnmatchedCheck(row, "SD");
                }
            }

            return unmatchedChecks;
        }

        public static void PersistUnmatchedChecks(List<DispositionRow> researchRows)
        {
            List<Check> unmatchedChecks = DetermineUnmatchedChecks(researchRows);
            AppendToResearchChecks(unmatchedChecks);
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
            DeleteMarkedChecks();
        }


        public static void MarkKnownDispositionChecks()
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

        public static void RemoveKnownDispositionChecksFromResearchTable()
        {
            MarkKnownDispositionChecks();
            DeleteMarkedChecks();
        }

        private static void ResolveIncidentalLBVD(DispositionRow row, List<Check> researchChecks)
        {
            if (row.LBVDCheckNum != 0
                && !IsKnownDisposition(row.LBVDCheckNum)
                && !string.IsNullOrEmpty(row.LBVDCheckDisposition))
            {
                SetKnownDisposition(row.LBVDCheckNum);

                // Find all checks among the researchChecks which are incidentally resolved by
                // this check number.
                List<Check> incidentalChecks = researchChecks.FindAll(c => c.Num == row.LBVDCheckNum).ToList();

                foreach (Check incidentalCheck in incidentalChecks)
                {
                    incidentalCheck.Clr = row.LBVDCheckDisposition;
                }
            }
        }

        private static void ResolveIncidentalTID(DispositionRow row, List<Check> researchChecks)
        {
            if (row.TIDCheckNum != 0
                && !IsKnownDisposition(row.TIDCheckNum)
                && !string.IsNullOrEmpty(row.TIDCheckDisposition))
            {
                SetKnownDisposition(row.TIDCheckNum);

                // Find all checks among the researchChecks which are incidentally resolved by
                // this check number.
                List<Check> incidentalChecks = researchChecks.FindAll(c => c.Num == row.TIDCheckNum).ToList();

                foreach (Check incidentalCheck in incidentalChecks)
                {
                    incidentalCheck.Clr = row.TIDCheckDisposition;
                }
            }
        }

        private static void ResolveIncidentalTDL(DispositionRow row, List<Check> researchChecks)
        {
            if (row.TDLCheckNum != 0
                && !IsKnownDisposition(row.TDLCheckNum)
                && !string.IsNullOrEmpty(row.TDLCheckDisposition))
            {
                SetKnownDisposition(row.TDLCheckNum);

                // Find all checks among the researchChecks which are incidentally resolved by
                // this check number.
                List<Check> incidentalChecks = researchChecks.FindAll(c => c.Num == row.TDLCheckNum).ToList();

                foreach (Check incidentalCheck in incidentalChecks)
                {
                    incidentalCheck.Clr = row.TDLCheckDisposition;
                }
            }
        }

        private static void ResolveIncidentalMBVD(DispositionRow row, List<Check> researchChecks)
        {
            if (row.MBVDCheckNum != 0
                && !IsKnownDisposition(row.MBVDCheckNum)
                && !string.IsNullOrEmpty(row.MBVDCheckDisposition))
            {
                SetKnownDisposition(row.MBVDCheckNum);

                // Find all checks among the researchChecks which are incidentally resolved by
                // this check number.
                List<Check> incidentalChecks = researchChecks.FindAll(c => c.Num == row.MBVDCheckNum).ToList();

                foreach (Check incidentalCheck in incidentalChecks)
                {
                    incidentalCheck.Clr = row.MBVDCheckDisposition;
                }
            }
        }

        private static void ResolveIncidentalSD(DispositionRow row, List<Check> researchChecks)
        {
            if (row.SDCheckNum != 0
                && !IsKnownDisposition(row.SDCheckNum)
                && !string.IsNullOrEmpty(row.SDCheckDisposition))
            {
                SetKnownDisposition(row.SDCheckNum);

                // Find all checks among the researchChecks which are incidentally resolved by
                // this check number.
                List<Check> incidentalChecks = researchChecks.FindAll(c => c.Num == row.SDCheckNum).ToList();

                foreach (Check incidentalCheck in incidentalChecks)
                {
                    incidentalCheck.Clr = row.SDCheckDisposition;
                }
            }
        }

        private static void ResolveIncidentalChecks(List<DispositionRow> researchRows, List<Check> researchChecks)
        {
            foreach (DispositionRow row in researchRows)
            {
                ResolveIncidentalLBVD(row, researchChecks);
                ResolveIncidentalTID(row, researchChecks);
                ResolveIncidentalTDL(row, researchChecks);
                ResolveIncidentalMBVD(row, researchChecks);
                ResolveIncidentalSD(row, researchChecks);
            }
        }

        public static void HandleIncidentalChecks(List<DispositionRow> researchRows)
        {
            List<Check> researchChecks = GetResearchChecks();
            ResolveIncidentalChecks(researchRows, researchChecks);

            // When merging a Research File against the Research Table, a check of
            // known disposition may resolve a check in the Research Table. Find
            // any such check and use it to create a new "incidental" resolved check
            // The status of an incidental resolved check will have been set by the call
            // to method ResolveIncidentalChecks above. If it has not been set, then
            // use "Resolved" as its status. By construction, checks in the Research Table
            // are only those which had no disposition on a Research File that has been used
            // as an input. Hence, only these checks can become new resolved checks.
            CreateIncidentalResolvedChecks(researchChecks);

            // Remove from the Research Table any incidental resolved checks created 
            // by the previous call. 
            RemoveResolvedChecks();
        }

        public static bool IsIncidental(int checkNum, List<int>incidentals)
        {
            bool incidental = incidentals.Any(cnum => cnum == checkNum);
            return incidental;
        }

        private static void CreateIncidentalResolvedChecks(List <Check> researchChecks)
        {
            List<int> incidentals = new List<int>();

            foreach (int cnum in knownDisposition)
            {
                if (!IsIncidental(cnum, incidentals))
                {
                    // Find all the research checks which have the same number as a check of
                    // of known disposition.
                    List<Check> matchedChecks = researchChecks.FindAll(c => c.Num == cnum);

                    foreach (Check matchedCheck in matchedChecks)
                    {
                        NewResolvedCheck(matchedCheck, (string.IsNullOrEmpty(matchedCheck.Clr) ? "Resolved" : matchedCheck.Clr));
                    }

                    incidentals.Add(cnum);
                }
            }
        }

        public static string GetDispositionFromCheck(Check check)
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

        // This method has a side effect on argument row. It sets a componet of row that is
        // read by the caller.
        public static void NewResolvedCheck(Check check, DispositionRow row, string service)
        {
            int checkNum;
            string status;
            string disposition = GetDispositionFromCheck(check);

            switch (service)
            {
                case "LBVD":
                    row.LBVDCheckDisposition = disposition;
                    checkNum = row.LBVDCheckNum;
                    status = disposition;
                    break;
                case "TID":
                    row.TIDCheckDisposition = disposition;
                    checkNum = row.TIDCheckNum;
                    status = disposition;
                    break;
                case "TDL":
                    row.TIDCheckDisposition = disposition;
                    checkNum = row.TDLCheckNum;
                    status = disposition;
                    break;
                case "MBVD":
                    row.MBVDCheckDisposition = disposition;
                    checkNum = row.MBVDCheckNum;
                    status = disposition;
                    break;
                case "SD":
                    row.SDCheckDisposition = disposition;
                    checkNum = row.SDCheckNum;
                    status = disposition;
                    break;
                default:
                    checkNum = -1;
                    status = "unknown";
                    break;
            }

          //  if (!IsResolved(checkNum))
          //  {
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
          //  }
        }

        public static void NewResolvedCheck(Check check, string status)
        {
            check.Clr = status;
            resolvedChecks.Add(check);
            /*
            if (!IsResolved(check.Num))
            {
                resolvedChecks.Add(check);
            }
             */
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

        public static void NewUpdatedRow(Check matchedCheck, string disposition)
        {
            DispositionRow drow = new DispositionRow
            {         
                InterviewRecordID = matchedCheck.InterviewRecordID,
                LBVDCheckNum = (matchedCheck.Service.Equals("LBVD") ? matchedCheck.Num : 0),
                LBVDCheckDisposition = (matchedCheck.Service.Equals("LBVD") ? disposition : ""),
                TIDCheckNum = (matchedCheck.Service.Equals("TID") ? matchedCheck.Num : 0),
                TIDCheckDisposition = (matchedCheck.Service.Equals("TID") ? disposition : ""),
                TDLCheckNum = (matchedCheck.Service.Equals("TDL") ? matchedCheck.Num : 0),
                TDLCheckDisposition = (matchedCheck.Service.Equals("TDL") ? disposition : ""),
                MBVDCheckNum = (matchedCheck.Service.Equals("MBVD") ? matchedCheck.Num : 0),
                MBVDCheckDisposition = (matchedCheck.Service.Equals("MBVD") ? disposition : ""),
                SDCheckNum = (matchedCheck.Service.Equals("SD") ? matchedCheck.Num : 0),
                SDCheckDisposition = (matchedCheck.Service.Equals("SD") ? disposition : "")
            };

            updatedRows.Add(drow);
        }

        public static List<Check> GetResearchChecks()
        {
            List<Check> researchChecks = new List<Check>();

            using (var dbCtx = new MSMEntities())
            {
                var longUnmatched = dbCtx.Set<LongUnmatched>();

                foreach (var lu in longUnmatched)
                {
                    researchChecks.Add(new Check
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

            return researchChecks;
        }

        private static void AppendToResearchChecks(List<Check> checks)
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

        private static void DeleteMarkedChecks()
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