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
            if (lbvd)
            {
                d.LBVDCheckDisposition = GetDispositionFromCheck(check);
                DataManager.SetMatchedCheck(d.LBVDCheckNum);
            }

            if (tid)
            {
                d.TIDCheckDisposition = GetDispositionFromCheck(check);
                DataManager.SetMatchedCheck(d.TIDCheckNum);
            }

            if (tdl)
            {
                d.TDLCheckDisposition = GetDispositionFromCheck(check);
                DataManager.SetMatchedCheck(d.TDLCheckNum);
            }

            if (mbvd)
            {
                d.MBVDCheckDisposition = GetDispositionFromCheck(check);
                DataManager.SetMatchedCheck(d.MBVDCheckNum);
            }

            if (sd)
            {
                d.SDCheckDisposition = GetDispositionFromCheck(check);
                DataManager.SetMatchedCheck(d.SDCheckNum);
            }
        }

        private static void UpdateLBVD(DispositionRow row)
        {
            if (row.LBVDCheckNum != 0 && !DataManager.IsMatched(row.LBVDCheckNum) && !DataManager.IsUnmatched(row.LBVDCheckNum))
            {
                if (row.LBVDCheckDisposition != null)
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
            if (row.TIDCheckNum != 0 && !DataManager.IsMatched(row.TIDCheckNum) && !DataManager.IsUnmatched(row.TIDCheckNum))
            {
                if (row.TIDCheckDisposition != null)
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
            if (row.TDLCheckNum != 0 && !DataManager.IsMatched(row.TDLCheckNum) && !DataManager.IsUnmatched(row.TDLCheckNum))
            {
                if (row.TDLCheckDisposition != null)
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
            if (row.MBVDCheckNum != 0 && !DataManager.IsMatched(row.MBVDCheckNum) && !DataManager.IsUnmatched(row.MBVDCheckNum))
            {
                if (row.MBVDCheckDisposition != null)
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
            if (row.SDCheckNum != 0 && !DataManager.IsMatched(row.SDCheckNum) && !DataManager.IsUnmatched(row.SDCheckNum))
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

        private static void ProcessChecks(List<Check> checks, IEnumerable<DispositionRow> originalRows)
        {
            foreach (var check in checks)
            {
                if (check.Num > 0)
                {
                    DispositionRow d = (from r in DataManager.GetUpdatedRows()
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
                            DataManager.NewUpdatedRow(d);
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

        private static void ResolveLongUnmatched(string vcFileName, string vcFileType, string qbFileName, string qbFileType)
        {
            List<DispositionRow> longUnmatchedDispositionRows = DataManager.GetLongUnmatchedDispositionRows();
            ProcessLongUnmatched(longUnmatchedDispositionRows, vcFileName, vcFileType, qbFileName, qbFileType);
        }

        private static void ProcessLongUnmatched(List<DispositionRow> longUnmatchedDispositionRows, string vcFileName, string vcFileType, string qbFileName, string qbFileType)
        {
            List<Check> qbChecks = DataManager.GetQuickbookChecks(qbFileName, qbFileType);
            List<Check> voidedChecks = DataManager.GetVoidedChecks(vcFileName, vcFileType);

            DataManager.Init();
      
            ProcessChecks(voidedChecks, longUnmatchedDispositionRows);
            ProcessChecks(qbChecks, longUnmatchedDispositionRows);

            DetermineUnmatchedChecks(longUnmatchedDispositionRows);

            DataManager.PersistUnmatchedChecks();

            DataManager.UpdateResolvedChecks();
            DataManager.RemoveMatchedChecks();
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

        private static void UpdateLongUnmatched(string apFileName, string apFileType)
        {
            // Deliberately get the file VCEmpty.xlsx.
            List<Check> voidedChecks = DataManager.GetEmptyVoidedChecks();
            List<DispositionRow> originalRows = DataManager.GetApricotRows(apFileName, apFileType);
            DataManager.Init();
  
            // This is tricky. We know that the set of voided checks is empty. So this call has
            // the effect of creating a set of unmatched checks from the Apricot Report File.
            // This set will then be appended to the long unmatched checks below.
            // This call also has the side effect of creating a list of check numbers of
            // known disposition. This set will be used to mark as matched any check number
            // of known disposition that occurs among the long unmatched checks.
            ProcessChecks(voidedChecks, originalRows);

            DetermineUnmatchedChecks(originalRows);
            DataManager.PersistUnmatchedChecks();
            DataManager.UpdateMatchedChecks();
            DataManager.RemoveMatchedChecks();
        }

        private static void ResolveUnmatchedChecks(string vcFileName, string vcFileType, string apFileName, string apFileType, string qbFileName, string qbFileType)
        {
            int z;

            List<Check> qbChecks = DataManager.GetQuickbookChecks(qbFileName, qbFileType);
            List<DispositionRow> originalRows = DataManager.GetApricotRows(apFileName, apFileType);
            List<Check> voidedChecks = DataManager.GetVoidedChecks(vcFileName, vcFileType);

            DataManager.Init();
       
            ProcessChecks(voidedChecks, originalRows);
            ProcessChecks(qbChecks, originalRows);

            DetermineUnmatchedChecks(originalRows);

            DataManager.PersistUnmatchedChecks();

            DataManager.UpdateResolvedChecks();
            DataManager.RemoveMatchedChecks();
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