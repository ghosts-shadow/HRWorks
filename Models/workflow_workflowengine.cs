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
    
    public partial class workflow_workflowengine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public workflow_workflowengine()
        {
            this.workflow_workflowengine_employee = new HashSet<workflow_workflowengine_employee>();
            this.workflow_workflowinstance = new HashSet<workflow_workflowinstance>();
            this.workflow_workflownode = new HashSet<workflow_workflownode>();
        }
    
        public int id { get; set; }
        public string workflow_code { get; set; }
        public string workflow_name { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public string description { get; set; }
        public short workflow_type { get; set; }
        public Nullable<int> applicant_position_id { get; set; }
        public Nullable<int> content_type_id { get; set; }
        public Nullable<int> departments_id { get; set; }
    
        public virtual django_content_type django_content_type { get; set; }
        public virtual personnel_department personnel_department { get; set; }
        public virtual personnel_position personnel_position { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workflow_workflowengine_employee> workflow_workflowengine_employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workflow_workflowinstance> workflow_workflowinstance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workflow_workflownode> workflow_workflownode { get; set; }
    }
}