using Excel;
using MSM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace MSM.Utils
{
    public class ExcelDataReader
    {
        public static List<DispositionRow> GetApricotRows (string filePath)
        {
            List<DispositionRow> apricotRows = new ExcelData(filePath).GetData("Sheet1").Select(dataRow => new DispositionRow
                {
                    RecordID = Convert.ToInt32(dataRow["Record ID"].ToString()),
                    Lname = dataRow["Last Name"].ToString(),
                    Fname = dataRow["First Name"].ToString(),
                    InterviewRecordID = Convert.ToInt32(dataRow["Interview Record ID"].ToString()),
                    Date = Convert.ToDateTime(dataRow["OPID Interview Date"].ToString()),
                    LBVDCheckNum = Convert.ToInt32(dataRow["LBVD Check Number"].ToString()),
                    LBVDCheckDisposition = dataRow["LBVD Check Disposition"].ToString(),
                    TIDCheckNum = Convert.ToInt32(dataRow["TID Check Number"].ToString()),
                    TIDCheckDisposition = dataRow["TID Check Disposition"].ToString(),
                    TDLCheckNum = Convert.ToInt32(dataRow["TDL Check Number"].ToString()),
                    TDLCheckDisposition = dataRow["TDL Check Disposition"].ToString(),
                    MBVDCheckNum = Convert.ToInt32(dataRow["MBVD Check Number"].ToString()),
                    MBVDCheckDisposition = dataRow["MBVD Check Disposition"].ToString(),
                    SDCheckNum = Convert.ToInt32(dataRow["SD Check Number"].ToString()),
                    SDCheckDisposition = dataRow["SD Check Disposition"].ToString(),
                }).ToList();

            return apricotRows;
        }

        public static List<Check> GetQuickbooksChecks(string filePath)
        {
            List<Check> rowChecks = new ExcelData(filePath).GetData("Sheet1").Select(dataRow => 
                new Check
                {
                    Date = GetDateValue(dataRow),
                    Num = GetCheckNum(dataRow),  
                    Memo = GetMemo(dataRow),  
                    Clr = GetCheckStatus(dataRow)
                }).ToList();

            List<Check> quickbooksChecks = new List<Check>();

            // Remove checks corresponding to blank rows in Excel file.
            foreach(Check check in rowChecks)
            {
                if (!check.Memo.Equals("NoCheck"))
                {
                    quickbooksChecks.Add(check);
                }
            }

            return quickbooksChecks;
        }

        public static List<Check> GetVoidedChecks(string filePath)
        {
            List<Check> rowChecks = new ExcelData(filePath).GetData("Sheet1").Select(dataRow =>
                new Check
                {
                    Date = GetDateValue(dataRow),
                    Num = GetCheckNum(dataRow),
                    Memo = GetMemo(dataRow)
                }).ToList();

            List<Check> voidedChecks = new List<Check>();

            // Remove checks corresponding to blank rows in Excel file.
            foreach (Check check in rowChecks)
            {
                if (!check.Memo.Equals("NoCheck"))
                {
                    voidedChecks.Add(check);
                }
            }

            return voidedChecks;
        }

        public static List<EmptyCol> GetEmptyFile(string filePath)
        {
            List<EmptyCol> emptyCols = new ExcelData(filePath).GetData("Sheet1").Select(dataRow =>
                new EmptyCol
                {
                    Empty = GetEmpty(dataRow)
                    
                }).ToList();

            return emptyCols;
        }

        private static DateTime GetDateValue(DataRow row)
        {
            string dvalue;

            if (DBNull.Value.Equals(row["Date"]))
            {
                // This is a blank row. Provide a dummy value.
                dvalue = "12/12/1900";
            }
            else
            {
                dvalue = row["Date"].ToString();
            }

            DateTime dtime = Convert.ToDateTime(dvalue);
            return dtime;
        }

        private static int GetCheckNum(DataRow row)
        {
            string cvalue;

            if (DBNull.Value.Equals(row["Num"]))
            {
                // This is a blank row. Provide a dummy value.
                cvalue = "0";
            }
            else
            {
                cvalue = row["Num"].ToString();
            }

            return Convert.ToInt32(cvalue);
        }

        private static string GetMemo(DataRow row)
        {
            string mvalue;

            if (DBNull.Value.Equals(row["Memo"]))
            {
                // This is a blank row. Provide a dummy value.
                mvalue = "NoCheck";
            }
            else
            {
                mvalue = row["Memo"].ToString();
            }

            return mvalue;
        }

        private static string GetCheckStatus(DataRow row)
        {
            string svalue;

            if (DBNull.Value.Equals(row["Clr"]))
            {
                svalue = "Unknown";
            }
            else
            {
                svalue = row["Clr"].ToString();
            }

            return svalue;
        }

        private static string GetEmpty(DataRow row)
        {
            return "Empty";
        }
    }
}