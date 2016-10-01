using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSM.Models
{
    public class Check
    {
        public DateTime Date { get; set; }

        public int Num { get; set; }

        public string Memo { get; set; }

        public string Service { get; set; }

        public string Clr { get; set; }

        public string Type { get; set; }

        public int InterviewRecordID { get; set; }
    }
}