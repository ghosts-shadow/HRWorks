using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    public class paysavedlist
    {
        public IEnumerable<HRworks.Models.payrole> Payroll { get; set; }
        public IEnumerable<HRworks.Models.payrollsaved> Payrollsaved { get; set; }
    }

    public class payslipmodel
    {
        public payrollsaved paysaved { get; set; }
        public contract contract { get; set; }
        public master_file master_file { get; set; }
    }

}