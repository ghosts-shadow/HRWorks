//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace HRworks.Models
{
    using System;
    using System.Collections.Generic;

    public partial class employeeleavesubmition
    {
        public int Id { get; set; }
        [Display(Name = "Employee ID")]
        public int Employee_id { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public System.DateTime Date { get; set; }
        [Display(Name = "from")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> Start_leave { get; set; }

        [Display(Name = "to")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> End_leave { get; set; }

        [Display(Name = "Return leave")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> Return_leave { get; set; }

        [Display(Name = "leave type")]
        public string leave_type { get; set; }
        [Display(Name = "days")]
        public Nullable<double> toltal_requested_days { get; set; }
        public string submitted_by { get; set; }
        public string apstatus { get; set; }
        [Display(Name = "is half-day included")]
        public bool half { get; set; }
        public string approved_byline { get; set; }
        public string approved_byhod { get; set; }

        public virtual master_file master_file { get; set; }
    }
}