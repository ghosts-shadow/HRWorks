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
    
    public partial class att_payloadbreak
    {
        public string uuid { get; set; }
        public Nullable<System.DateTime> break_out { get; set; }
        public Nullable<System.DateTime> break_in { get; set; }
        public Nullable<int> duration { get; set; }
        public Nullable<int> taken { get; set; }
        public Nullable<int> actual_duration { get; set; }
        public Nullable<int> early_in { get; set; }
        public Nullable<int> late_in { get; set; }
        public Nullable<int> late { get; set; }
        public Nullable<int> early_leave { get; set; }
        public Nullable<int> absent { get; set; }
        public Nullable<int> work_time { get; set; }
        public Nullable<int> overtime { get; set; }
        public Nullable<int> weekend_ot { get; set; }
        public Nullable<int> holiday_ot { get; set; }
    }
}