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
        /*
        private static void UpdateDisposition(bool lbvd, bool tid, bool tdl, bool mbvd, bool sd, DispositionRow d, Check check)
        {
            if (lbvd && string.IsNullOrEmpty(d.LBVDCheckDisposition))
            {
                DataManager.NewResolvedCheck(check, d, "LBVD");
            }

            if (tid && string.IsNullOrEmpty(d.TIDCheckDisposition))
            {
                DataManager.NewResolvedCheck(check, d, "TID");
            }

            if (tdl && string.IsNullOrEmpty(d.TDLCheckDisposition))
            {
                DataManager.NewResolvedCheck(check, d, "TDL");
            }

            if (mbvd && string.IsNullOrEmpty(d.MBVDCheckDisposition))
            {
                DataManager.NewResolvedCheck(check, d, "MBVD");
            }

            if (sd && string.IsNullOrEmpty(d.SDCheckDisposition))
            {
                DataManager.NewResolvedCheck(check, d, "SD");
            }
        }

        private static void ProcessChecks(List<Check> checks, List<DispositionRow> researchRows)
        {
            foreach (Check check in checks)
            {
                if (check.Num > 0)
                {
                    // There could be more than a single DispositionRow associated with a check number.
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
                        drows = (from r in researchRows
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
        
      // The user has supplied either a voided checks file or a Quickbooks file or both to resolve
      // unmatched checks occuring on the supplied Research File. This will not resolve any checks
      // in the Research Table except those which are incidentally resolved by their presence on
      // the Research File.
      private static void ResolveResearchChecks(string vcFileName, string vcFileType, string apFileName, string apFileType, string qbFileName, string qbFileType)
      {
          List<Check> qbChecks = DataManager.GetQuickbooksChecks(qbFileName, qbFileType);
          List<DispositionRow> researchRows = DataManager.GetResearchRows(apFileName, apFileType);
          List<Check> voidedChecks = DataManager.GetVoidedChecks(vcFileName, vcFileType);

          DataManager.Init();

          // Do this first.
          DataManager.HandleIncidentalChecks(researchRows);
       
          ProcessChecks(voidedChecks, researchRows);
          ProcessChecks(qbChecks, researchRows);

          DataManager.PersistUnmatchedChecks(researchRows);

          // As a side effect of processing checks, we will have determined a set of resolved
          // checks. Remove this set of resolved checks from the Research Table.
          DataManager.RemoveResolvedChecks();
      }
      */

        private static void UpdateDisposition(Check matchedCheck, string disposition, DispositionRow drow)
        {
            int checkNum = matchedCheck.Num;

            switch (matchedCheck.Service)
            {
                case "LBVD":
                    drow.LBVDCheckNum = checkNum;
                    drow.LBVDCheckDisposition = disposition;
                    break;
                case "TID":
                    drow.TIDCheckNum = checkNum;
                    drow.TIDCheckDisposition = disposition;
                    break;
                case "TDL":
                    drow.TDLCheckNum = checkNum;
                    drow.TDLCheckDisposition = disposition;
                    break;
                case "MBVD":
                    drow.MBVDCheckNum = checkNum;
                    drow.MBVDCheckDisposition = disposition;
                    break;
                case "SD":
                    drow.SDCheckNum = checkNum;
                    drow.SDCheckDisposition = disposition;
                    break;
                default:
                    break;
            }
        }

        private static void ProcessChecks(List<Check> checks, List<Check> researchChecks)
        {
            foreach (Check check in checks)
            {
                List<Check> matchedChecks = researchChecks.FindAll(c => c.Num == check.Num || c.Num == -check.Num);

                // Normally, matchedChecks.Count() == 0 or matchedChecks.Count == 1 
                // But in the case of a birth certificate, a single check number may cover
                // multiple children. In this case matchedChecks.Count() > 1.
                // The foreach loop below handles both the case where matchedChecks.Count() == 1
                // and the case where matchedChecks.Count() > 1.
                if (matchedChecks.Count() != 0)
                {
                    foreach (Check matchedCheck in matchedChecks)
                    {
                        string disposition = DataManager.GetDispositionFromCheck(check);

                        DataManager.NewResolvedCheck(matchedCheck, disposition);

                        List<DispositionRow> drows = (from r in DataManager.GetUpdatedRows()
                                                      where r.LBVDCheckNum == matchedCheck.Num
                                                            || r.TIDCheckNum == matchedCheck.Num
                                                            || r.TDLCheckNum == matchedCheck.Num
                                                            || r.MBVDCheckNum == matchedCheck.Num
                                                            || r.SDCheckNum == matchedCheck.Num
                                                            || r.InterviewRecordID == matchedCheck.InterviewRecordID
                                                      select r).ToList();
                        if (drows.Count() == 0)
                        {
                            // There is no disposition row representing this matched check.
                            // Create one.
                            DataManager.NewUpdatedRow(matchedCheck, disposition);
                        }
                        else
                        {
                            foreach (DispositionRow drow in drows)
                            {
                                if ((matchedCheck.InterviewRecordID != 0 && matchedCheck.InterviewRecordID != drow.InterviewRecordID)
                                    ||
                                    (matchedCheck.RecordID != 0 && matchedCheck.RecordID != drow.RecordID))
                                {
                                    // Case of same check number being used for multiple
                                    // birth certificates.
                                    DataManager.NewUpdatedRow(matchedCheck, disposition);
                                }
                                else
                                {
                                    // Found row among already updated rows. There is more than one check
                                    // number on this row. In other words, the client had more than
                                    // one check written for the visit this row corresponds to.
                                    UpdateDisposition(matchedCheck, disposition, drow);
                                }
                            }
                        }
                    }
                }
            }
        }
      
        // The user specified only a Research File. Use this file to update the 
        // research checks. 
        private static void UpdateResearchTableFromResearchFile(string apFileName, string apFileType)
        {
            List<DispositionRow> researchRows = DataManager.GetResearchRows(apFileName, apFileType);

            DataManager.Init();

            DataManager.PersistUnmatchedChecks(researchRows);

            DataManager.HandleIncidentalChecks(researchRows);
        }

        // The user specified only a Modifications File. Use this file to update the 
        // research checks. 
        private static void UpdateResearchTableFromModsFile(string mdFileName, string mdFileType)
        {
            List<ModificationRow> modificationRows = DataManager.GetModificationRows(mdFileName, mdFileType);

            DataManager.Init();

            DataManager.PersistUnmatchedChecks(modificationRows);

            DataManager.HandleIncidentalChecks(modificationRows);
        }

        // The user did not specify a Research File on the merge screen. The user is trying
        // to resolve some checks currently in research. Since no Research File has been supplied
        // there are no incidentally resolved checks to handle.
        private static void ResolveResearchChecks(string vcFileName, string vcFileType, string qbFileName, string qbFileType)
        {
            DataManager.Init();

            List<Check> researchChecks = DataManager.GetResearchChecks();
            List<Check> qbChecks = DataManager.GetQuickbooksChecks(qbFileName, qbFileType);
            List<Check> voidedChecks = DataManager.GetVoidedChecks(vcFileName, vcFileType);

            // Since no Research File was supplied, there are no incidentally resolved checks.
            // So we don't have to call DataManager.HandleIncidentalChecks(researchRows)

            ProcessChecks(qbChecks, researchChecks);
            ProcessChecks(voidedChecks, researchChecks);

            // As a side effect of processing checks, we will have determined a set of resolved
            // checks. Remove this set of resolved checks from the Research Table. 
            DataManager.RemoveResolvedChecks();
        }

        [HttpGet]
        public void PerformMerge(string vcFileName, string vcFileType, string apFileName, string apFileType, string mdFileName, string mdFileType, string qbFileName, string qbFileType)
        {
            if (apFileName.Equals("unknown") && mdFileName.Equals("unknown"))
            {
                // The user did not specify a Research File or a Mods File on the merge screen. 
                // The user is trying to resolve some research checks in the Research Table.
                ResolveResearchChecks(vcFileName, vcFileType, qbFileName, qbFileType);
            }
            else if (vcFileName.Equals("unknown") && qbFileName.Equals("unknown"))
            {
                if (!apFileName.Equals("unknown") && mdFileName.Equals("unknown"))
                {
                    // The user specified only a Research File. Use this file to update the 
                    // research table.
                    UpdateResearchTableFromResearchFile(apFileName, apFileType);
                }
                else if (apFileName.Equals("unknown") && !mdFileName.Equals("unknown"))
                {
                    // The user specified only a Modifications File. Use this file to update the 
                    // research table.
                    UpdateResearchTableFromModsFile(mdFileName, mdFileType);
                }
            }
            /*
            else
            {
                // The user has supplied either a voided checks file or a Quickbooks file or both to resolve
                // unmatched checks occuring on the supplied Research File.
                ResolveResearchChecks(vcFileName, vcFileType, apFileName, apFileType, qbFileName, qbFileType); 
            }
            */
        }
    }
}