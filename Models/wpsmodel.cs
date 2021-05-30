using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    public class Wpsmodel
    {
        public int srno { get; set; }
        public virtual payrollsaved Payrollsaved { get; set; }
        public virtual labour_card LabourCard { get; set; }
        public virtual bank_details BankDetails { get; set; }
        public virtual master_file MasterFile { get; set; }
        public int Payrollsavedid { get; set; }
        public int LabourCardid { get; set; }
        public int BankDetailsid { get; set; }
        public int MasterFileid { get; set; }
    }
    
}