using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    public class employeeleavesub
    {
        public List<Leave> Leaves { get; set; }
        public List<emprel> Emprels { get; set; }
    }
}