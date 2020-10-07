using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    using System.ComponentModel.DataAnnotations;

    public class hikshrs
    {
        [Display(Name = "Employee id")]
        public int Employee_id { get; set; }

        [Display(Name = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> datetime1 { get; set; }
        public TimeSpan hours { get; set; }

        [Display(Name = "Employee name")]
        public string Employee_name { get; set; }

        public DateTime dateupperlimit { get; set; }
        public DateTime datelowerlimit { get; set; }
    }
}