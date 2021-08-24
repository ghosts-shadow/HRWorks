using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    using System.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    public class con_leavemodel
    {
        public int id { get; set; }
        [Display(Name = "leave type")]
        public string leave_type { get; set; }
        [Display(Name = "is half-day included")]
        public bool half { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "Departmant Project")]
        public string departmant_project { get; set; }

        [Display(Name = "from")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> Start_leave { get; set; }

        [Display(Name = "to")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> End_leave { get; set; }

        [Display(Name = "actual return date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> Areturn_leave { get; set; }

        [Display(Name = "Employee NO")]
        public int employee_no { get; set; }

        [Display(Name = "Employee Name")]
        public string employee_name { get; set; }
    }
}