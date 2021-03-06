//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRworks.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class labour_card
    {
        public int employee_id { get; set; }
        public Nullable<long> work_permit_no { get; set; }
        public Nullable<long> personal_no { get; set; }
        public int emp_no { get; set; }
        public string proffession { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> lc_expiry { get; set; }
        public string establishment { get; set; }
        public string imgpath { get; set; }
        public string changed_by { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> date_changed { get; set; }
    
        public virtual master_file master_file { get; set; }
    }
}
