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
    
    public partial class FormsFlow
    {
        public int Id { get; set; }
        public Nullable<int> originator { get; set; }
        public string Auth1 { get; set; }
        public string Auth2 { get; set; }
        public string Auth3 { get; set; }
        public string Auth4 { get; set; }
        public string Auth5 { get; set; }
        public string Auth6 { get; set; }
        public string Auth7 { get; set; }
        public string Auth8 { get; set; }
        public string Auth9 { get; set; }
        public string Auth10 { get; set; }
        public string Auth11 { get; set; }
        public string Auth12 { get; set; }
        public string Auth13 { get; set; }
        public string Auth14 { get; set; }
        public string Auth15 { get; set; }
        public Nullable<int> form_ID { get; set; }
    
        public virtual Form Form { get; set; }
    }
}
