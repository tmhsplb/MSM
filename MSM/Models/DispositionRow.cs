using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSM.Models
{
    public class DispositionRow
    {
        public int RecordID { get; set; }
        public DateTime Date { get; set; }

        public int LBVDCheckNum { get; set; }
        public string LBVDCheckDisposition { get; set; }

        public int TIDCheckNum { get; set; }
        public string TIDCheckDisposition { get; set; }

        public int TDLCheckNum { get; set; }
        public string TDLCheckDisposition { get; set; }

        public int MBVDCheckNum { get; set; }
        public string MBVDCheckDisposition { get; set; }

        public int SDCheckNum { get; set; }
        public string SDCheckDisposition { get; set; }
    }
}