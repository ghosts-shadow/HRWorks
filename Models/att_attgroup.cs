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
    
    public partial class att_attgroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public att_attgroup()
        {
            this.att_attemployee = new HashSet<att_attemployee>();
            this.att_grouppolicy = new HashSet<att_grouppolicy>();
            this.att_groupschedule = new HashSet<att_groupschedule>();
            this.att_holiday = new HashSet<att_holiday>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public string create_user { get; set; }
        public Nullable<System.DateTime> change_time { get; set; }
        public string change_user { get; set; }
        public short status { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<att_attemployee> att_attemployee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<att_grouppolicy> att_grouppolicy { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<att_groupschedule> att_groupschedule { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<att_holiday> att_holiday { get; set; }
    }
}
