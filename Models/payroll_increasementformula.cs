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
    
    public partial class payroll_increasementformula
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public payroll_increasementformula()
        {
            this.payroll_salarystructure_increasementformula = new HashSet<payroll_salarystructure_increasementformula>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string formula { get; set; }
        public string remark { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<payroll_salarystructure_increasementformula> payroll_salarystructure_increasementformula { get; set; }
    }
}
