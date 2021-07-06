using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    public class Reconsilationmodel
    {
        public int id { get; set; }
        public int employee_id { get; set; }
        public string employee_name { get; set; }
        public double gross { get; set; }
        public double amount { get; set; }
        public string remarks { get; set; }
        public string ded_add { get; set; }
    }
}