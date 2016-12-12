using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSM.Models
{
    public class ModificationRow : DataRow
    {
        
        public string ModificationType { get; set; }

        public string LBVDModificationReason { get; set; }
    //    public int LBVDModifiedCheckNum { get; set; }
     //   public string LBVDModifiedCheckDisposition { get; set; }

        public string TIDModificationReason { get; set; }
   //     public int TIDModifiedCheckNum { get; set; }
    //    public string TIDModifiedCheckDisposition { get; set; }

        public string TDLModificationReason { get; set; }
    //    public int TDLModifiedCheckNum { get; set; }
     //   public string TDLModifiedCheckDisposition { get; set; }


        public string MBVDModificationReason { get; set; }
      //  public int MBVDModifiedCheckNum { get; set; }
      //  public string MBVDModifiedCheckDisposition { get; set; }

        public string SDMReason { get; set; }
     //   public int SDMCheckNum { get; set; }
     //   public string SDMCheckDisposition { get; set; }

    }
}