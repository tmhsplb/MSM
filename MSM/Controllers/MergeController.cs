using LinqToExcel;
using MSM.DAL;
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
            if (lbvd && string.IsNullOrEmpty(d.LBVDCheckDisposition))
            {
                d.LBVDCheckDisposition = GetDispositionFromCheck(check);
                DataManager.NewResolvedCheck(d, "LBVD");
            }

            if (tid && string.IsNullOrEmpty(d.TIDCheckDisposition))
            {
                d.TIDCheckDisposition = GetDispositionFromCheck(check);
                DataManager.NewResolvedCheck(d, "TID");
            }

            if (tdl && string.IsNullOrEmpty(d.TDLCheckDisposition))
            {
                d.TDLCheckDisposition = GetDispositionFromCheck(check);
                DataManager.NewResolvedCheck(d, "TDL");
            }

            if (mbvd && string.IsNullOrEmpty(d.MBVDCheckDisposition))
            {
                d.MBVDCheckDisposition = GetDispositionFromCheck(check);
                DataManager.NewResolvedCheck(d, "MBVD");
            }

            if (sd && string.IsNullOrEmpty(d.SDCheckDisposition))
            {
                d.SDCheckDisposition = GetDispositionFromCheck(check);
                DataManager.NewResolvedCheck(d, "SD");
            }
        }

        private static void UpdateLBVD(DispositionRow row)
        {
            if (row.LBVDCheckNum != 0)
            {
                if (!string.IsNullOrEmpty(row.LBVDCheckDisposition))
                {
                    DataManager.SetKnownDisposition(row.LBVDCheckNum);
                }
                else
                {
                    DataManager.NewUnmatchedCheck(row, "LBVD");
                }
            }
        }

        private static void UpdateTID(DispositionRow row)
        {
            if (row.TIDCheckNum != 0)
            {
                if (!string.IsNullOrEmpty(row.TIDCheckDisposition))
                {
                    DataManager.SetKnownDisposition(row.TIDCheckNum);
                }
                else
                {
                    DataManager.NewUnmatchedCheck(row, "TID");
                }
            }
        }

        private static void UpdateTDL(DispositionRow row)
        {
            if (row.TDLCheckNum != 0)
            {
                if (!string.IsNullOrEmpty(row.TDLCheckDisposition))
                {
                    DataManager.SetKnownDisposition(row.TDLCheckNum);
                }
                else
                {
                    DataManager.NewUnmatchedCheck(row, "TDL");
                }
            }
        }

        private static void UpdateMBVD(DispositionRow row)
        {
            if (row.MBVDCheckNum != 0)
            {
                if (!string.IsNullOrEmpty(row.MBVDCheckDisposition))
                {
                    DataManager.SetKnownDisposition(row.MBVDCheckNum);
                }
                else
                {
                    DataManager.NewUnmatchedCheck(row, "MBVD");
                }
            }
        }

        private static void UpdateSD(DispositionRow row)
        {
            if (row.SDCheckNum != 0)
            {
                if (row.SDCheckDisposition != null)
                {
                    DataManager.SetKnownDisposition(row.SDCheckNum);
                }
                else
                {
                    DataManager.NewUnmatchedCheck(row, "SD");
                }
            }
        }

        private static void ProcessChecks(List<Check> checks, List<DispositionRow> originalRows)
        {
            foreach (var check in checks)
            {
                if (check.Num > 0)
                {
                    // There could be more than a single DispostionRow associated with a check number.
                    // This will happen when the same check number is used for multiple birth certificates.
                    List<DispositionRow> drows = (from r in DataManager.GetUpdatedRows()
                                        where r.LBVDCheckNum == check.Num
                                              || r.TIDCheckNum == check.Num
                                              || r.TDLCheckNum == check.Num
                                              || r.MBVDCheckNum == check.Num
                                              || r.SDCheckNum == check.Num
                                        select r).ToList();

                    if (drows.Count() == 0)
                    {
                        drows = (from r in originalRows
                                 where r.LBVDCheckNum == check.Num
                                   || r.TIDCheckNum == check.Num
                                   || r.TDLCheckNum == check.Num
                                   || r.MBVDCheckNum == check.Num
                                   || r.SDCheckNum == check.Num
                                 select r).ToList();

                        if (drows.Count() > 0)
                        {
                            foreach (DispositionRow drow in drows)
                            {
                                UpdateDisposition(check.Num == drow.LBVDCheckNum, check.Num == drow.TIDCheckNum, check.Num == drow.TDLCheckNum, check.Num == drow.MBVDCheckNum, check.Num == drow.SDCheckNum, drow, check);
                                DataManager.NewUpdatedRow(drow);
                            }
                        }
                    }
                    else
                    {
                        foreach (DispositionRow drow in drows)
                        {
                            // Found row among already updated rows. There is more than one check
                            // number on this row. In other words, the client had more than
                            // one check written for the visit this row corresponds to.
                            UpdateDisposition(check.Num == drow.LBVDCheckNum, check.Num == drow.TIDCheckNum, check.Num == drow.TDLCheckNum, check.Num == drow.MBVDCheckNum, check.Num == drow.SDCheckNum, drow, check);
                        }
                    }
                }
            }
        }

        // The user did not specify an Apricot Reprot File on the merge screen. The user is trying
        // to resolve some long unmatched checks.
        private static void ResolveLongUnmatched(string vcFileName, string vcFileType, string qbFileName, string qbFileType)
        {
            DataManager.Init();

            List<Check> longUnmatchedChecks = DataManager.GetLongUnmatchedChecks();
            List<Check> qbChecks = DataManager.GetQuickbooksChecks(qbFileName, qbFileType);
            List<Check> voidedChecks = DataManager.GetVoidedChecks(vcFileName, vcFileType);

            ProcessChecks(qbChecks, longUnmatchedChecks);
            ProcessChecks(voidedChecks, longUnmatchedChecks);

            DataManager.RemoveResolvedChecks();
        }

        private static void ProcessChecks(List<Check> checks, List<Check> longUnmatchedChecks)
        {
            foreach (Check check in checks)
            {
                if (!DataManager.IsKnownDisposition(check.Num))
                {
                    List<Check> matchedChecks = (from c in longUnmatchedChecks
                                                 where c.Num == check.Num
                                                 select c).ToList();

                    // Normally, matchedChecks.Count() == 0 or matchedChecks.Count == 1 
                    // But in the case of a birth certificate, a single check number may cover
                    // multiple children. In this case matchedChecks.Count() > 1.
                    // The foreach loop below handles both the case where matchedChecks.Count() == 1
                    // and the case where matchedChecks.Count() > 1.
                    if (matchedChecks.Count() != 0)
                    {
                        foreach (Check matchedCheck in matchedChecks)
                        {
                            DataManager.NewResolvedCheck(matchedCheck, GetDispositionFromCheck(check));
                        }

                        DataManager.SetKnownDisposition(check.Num);
                    }
                }
            } 
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

        // The user has only specified an Apricot Report File. Use this file to update the 
        // long unmatched checks.
        private static void UpdateLongUnmatched(string apFileName, string apFileType)
        {
            List<DispositionRow> originalRows = DataManager.GetApricotRows(apFileName, apFileType);

            // Deliberately get the file VCEmpty.xlsx.   
            List<Check> voidedChecks = DataManager.GetEmptyVoidedChecks();
           
            DataManager.Init();
  
            // This is tricky. We know that the set of voided checks is empty. So this call has
            // the effect of creating a set of unmatched checks from the Apricot Report File.
            // This set will then be appended to the long unmatched checks below.
            ProcessChecks(voidedChecks, originalRows);

            DetermineUnmatchedChecks(originalRows);
            DataManager.PersistUnmatchedChecks();
        }

        // The user has supplied either a voided checks file or a Quickbooks file or both to resolve
        // unmatched checks occuring on the supplied Apricot Report File.
        private static void ResolveUnmatchedChecks(string vcFileName, string vcFileType, string apFileName, string apFileType, string qbFileName, string qbFileType)
        {
 
            List<Check> qbChecks = DataManager.GetQuickbooksChecks(qbFileName, qbFileType);
            List<DispositionRow> originalRows = DataManager.GetApricotRows(apFileName, apFileType);
            List<Check> voidedChecks = DataManager.GetVoidedChecks(vcFileName, vcFileType);

            DataManager.Init();
       
            ProcessChecks(voidedChecks, originalRows);
            ProcessChecks(qbChecks, originalRows);

            DetermineUnmatchedChecks(originalRows);

            DataManager.PersistUnmatchedChecks();
        }

        [HttpGet]
        public void PerformMerge(string vcFileName, string vcFileType, string apFileName, string apFileType, string qbFileName, string qbFileType)
        {
            if (apFileName.Equals("unknown"))
            {
                // The user did not specify an Apricot Reprot File on the merge screen. The user is trying
                // to resolve some long unmatched checks.
                ResolveLongUnmatched(vcFileName, vcFileType, qbFileName, qbFileType);
            }
            else if (vcFileName.Equals("unknown") && qbFileName.Equals("unknown"))
            {
                // The user has only specified an Apricot Report File. Use this file to update the 
                // long unmatched checks.
                UpdateLongUnmatched(apFileName, apFileType);
            }
            else
            {
                // The user has supplied either a voided checks file or a Quickbooks file or both to resolve
                // unmatched checks occuring on the supplied Apricot Report File.
                ResolveUnmatchedChecks(vcFileName, vcFileType, apFileName, apFileType, qbFileName, qbFileType); 
            }
        }
    }
}