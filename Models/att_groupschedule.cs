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
    
    public partial class att_groupschedule
    {
        public int id { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public string create_user { get; set; }
        public Nullable<System.DateTime> change_time { get; set; }
        public string change_user { get; set; }
        public short status { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public int group_id { get; set; }
        public Nullable<int> shift_id { get; set; }
    
        public virtual att_attgroup att_attgroup { get; set; }
        public virtual att_attshift att_attshift { get; set; }
    }
}