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
    
    public partial class ProjectList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProjectList()
        {
            this.MainTimeSheets = new HashSet<MainTimeSheet>();
        }
    
        public long ID { get; set; }
        public string ProjectCode { get; set; }
        public string STAMP_CODE { get; set; }
        public string INQUIRY { get; set; }
        public string PROJECT_NAME { get; set; }
        public string CLIENT_MC { get; set; }
        public string Notice_01 { get; set; }
        public string Notice_02 { get; set; }
        public string SCOPE_OF_WORK { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> INSURANCE_STATUS { get; set; }
        public string LOCATION { get; set; }
        public string PM { get; set; }
        public string PM_CONTACT { get; set; }
        public string Completion_Certificate { get; set; }
        public string REMARKS { get; set; }
        public Nullable<double> Notice { get; set; }
        public string Source { get; set; }
        public bool Closed { get; set; }
        public string Encoded_Absolute_URL { get; set; }
        public string Item_Type { get; set; }
        public string Path { get; set; }
        public string URL_Path { get; set; }
        public string Workflow_Instance_ID { get; set; }
        public string File_Type { get; set; }
        public string excute_by { get; set; }
        public string project_period { get; set; }
        public Nullable<decimal> equipment_budget { get; set; }
        public Nullable<decimal> man_power_budget { get; set; }
        public Nullable<bool> rate_w_wo_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MainTimeSheet> MainTimeSheets { get; set; }
    }
}