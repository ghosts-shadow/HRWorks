using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRworks.Models
{
    public class SickLeaveR
    {
        
        public int EmpID { get; set; }
        public double SLTaken { get; set; }
        public double HalfPaid { get; set; }
        public double unpaid { get; set; }
        public virtual master_file master_file { get; set; }
    }
    
}