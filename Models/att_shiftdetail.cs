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
    
    public partial class att_shiftdetail
    {
        public int id { get; set; }
        public System.DateTime in_time { get; set; }
        public System.DateTime out_time { get; set; }
        public int day_index { get; set; }
        public int shift_id { get; set; }
        public int time_interval_id { get; set; }
    
        public virtual att_attshift att_attshift { get; set; }
        public virtual att_timeinterval att_timeinterval { get; set; }
    }
}