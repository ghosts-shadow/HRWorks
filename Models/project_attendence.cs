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
    
    public partial class project_attendence
    {
        public int Id { get; set; }
        public int employee_id { get; set; }
        public int project_id { get; set; }
        public Nullable<System.DateTime> at_datetime { get; set; }
        public Nullable<System.DateTime> at_date { get; set; }
        public Nullable<System.TimeSpan> at_time { get; set; }
        public string at_status { get; set; }
        public string uploaded_by { get; set; }
        public Nullable<System.DateTime> modified_date { get; set; }
    
        public virtual HRprojectlist HRprojectlist { get; set; }
        public virtual master_file master_file { get; set; }
    }
}
