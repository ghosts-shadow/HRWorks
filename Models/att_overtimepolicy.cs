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
    
    public partial class att_overtimepolicy
    {
        public int id { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public string create_user { get; set; }
        public Nullable<System.DateTime> change_time { get; set; }
        public string change_user { get; set; }
        public short status { get; set; }
        public short mode { get; set; }
        public decimal hrs_from { get; set; }
        public decimal hrs_to { get; set; }
        public string master { get; set; }
        public Nullable<int> overnight_pay_code_id { get; set; }
        public Nullable<int> pay_code_id { get; set; }
    
        public virtual att_paycode att_paycode { get; set; }
        public virtual att_paycode att_paycode1 { get; set; }
    }
}