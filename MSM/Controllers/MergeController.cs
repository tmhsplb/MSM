﻿using LinqToExcel;
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
        private static void DetermineResolvedChecks(List<Check> checks, List<Check> researchChecks)
        {
            foreach (Check check in checks)
            {
                List<Check> matchedChecks = researchChecks.FindAll(c => c.Num == check.Num || c.Num == -check.Num);

                // Normally, matchedChecks.Count() == 0 or matchedChecks.Count == 1 
                // But in the case of a birth certificate, a single check number may cover
                // multiple children. In this case matchedChecks.Count() > 1.
                // The foreach loop below creates a new resolved check for each matched check.
                // This means that if one check number is used by a parent and his/her children,
                // then there will be a resolved check for the parent and each child.
                if (matchedChecks.Count() != 0)
                {
                    foreach (Check matchedCheck in matchedChecks)
                    {
                        DataManager.NewResolvedCheck(matchedCheck, DataManager.GetDispositionFromCheck(check));
                    }
                }
            }
        }
      
        // The user specified only an Interview Research File. Use this file to update the 
        // research checks. 
        private static void UpdateResearchTableFromInterviewResearchFile(string apFileName, string apFileType)
        {
            List<DispositionRow> researchRows = DataManager.GetResearchRows(apFileName, apFileType);

            DataManager.Init();
            DataManager.PersistUnmatchedChecks(researchRows);
            DataManager.HandleIncidentalChecks(researchRows);
        }

        // The user specified only a Modifications Research File. Use this file to update the 
        // research checks. 
        private static void UpdateResearchTableFromModificationsResearchFile(string mdFileName, string mdFileType)
        {
            List<ModificationRow> modificationRows = DataManager.GetModificationRows(mdFileName, mdFileType);

            DataManager.Init();
            DataManager.PersistUnmatchedChecks(modificationRows);
            DataManager.HandleIncidentalChecks(modificationRows);
        }

        // The user did not specify a Research File on the merge screen. The user is trying
        // to resolve some checks currently in research.
        private static void ResolveResearchChecks(string vcFileName, string vcFileType, string qbFileName, string qbFileType)
        {
            DataManager.Init();

            List<Check> researchChecks = DataManager.GetResearchChecks();
            List<Check> qbChecks = DataManager.GetQuickbooksChecks(qbFileName, qbFileType);
            List<Check> voidedChecks = DataManager.GetVoidedChecks(vcFileName, vcFileType);
 
            DetermineResolvedChecks(qbChecks, researchChecks);
            DetermineResolvedChecks(voidedChecks, researchChecks);

            // Remove the set of resolved checks determined above from the Research Table. 
            DataManager.RemoveResolvedChecks();
        }

        [HttpGet]
        public void PerformMerge(string vcFileName, string vcFileType, string apFileName, string apFileType, string mdFileName, string mdFileType, string qbFileName, string qbFileType)
        {
            if (apFileName.Equals("unknown") && mdFileName.Equals("unknown"))
            {
                // The user did not specify an Interview Research File or a Modifications Research File 
                // on the merge screen. 
                // The user is trying to resolve some research checks in the Research Table.
                ResolveResearchChecks(vcFileName, vcFileType, qbFileName, qbFileType);
            }
            else if (vcFileName.Equals("unknown") && qbFileName.Equals("unknown"))
            {
                if (!apFileName.Equals("unknown") && mdFileName.Equals("unknown"))
                {
                    // The user specified only an Interview Research File. Use this file to update the 
                    // research table.
                    UpdateResearchTableFromInterviewResearchFile(apFileName, apFileType);
                }
                else if (apFileName.Equals("unknown") && !mdFileName.Equals("unknown"))
                {
                    // The user specified only a Modifications Research File. Use this file to update the 
                    // research table.
                    UpdateResearchTableFromModificationsResearchFile(mdFileName, mdFileType);
                }
            }
        }
    }
}