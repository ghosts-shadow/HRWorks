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
    
    public partial class companleaveBal
    {
        public int Id { get; set; }
        public int EmpNo { get; set; }
        public System.DateTime dateModified { get; set; }
        public Nullable<double> balance { get; set; }
        public Nullable<double> hrs { get; set; }
    
        public virtual master_file master_file { get; set; }
    }
}
