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
    
    public partial class mobile_gpsforemployee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public mobile_gpsforemployee()
        {
            this.mobile_gpsforemployee_location = new HashSet<mobile_gpsforemployee_location>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public string create_user { get; set; }
        public Nullable<System.DateTime> change_time { get; set; }
        public string change_user { get; set; }
        public short status { get; set; }
        public int distance { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public int employee_id { get; set; }
    
        public virtual personnel_employee personnel_employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mobile_gpsforemployee_location> mobile_gpsforemployee_location { get; set; }
    }
}
