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
    
    public partial class EmpPromise
    {
        public int ID { get; set; }
        public Nullable<int> EMpID { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Ext { get; set; }
        public string LandLine { get; set; }
        public bool Active { get; set; }
        public string Remark { get; set; }
        public Nullable<double> PhoneAllowance { get; set; }
        public Nullable<System.DateTime> RegDate { get; set; }
        public string RegPC { get; set; }
    }
}