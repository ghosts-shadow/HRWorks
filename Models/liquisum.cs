using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    using System.ComponentModel.DataAnnotations;

    public class liquisum
    {

        public int Id { get; set; }
        public float amount { get; set; }

        [Display(Name = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> ldate { get; set; }

        public string description { get; set; }

        public int quantity { get; set; }

    }
}