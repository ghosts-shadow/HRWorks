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
    
    public partial class workflow_workflowrole
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public workflow_workflowrole()
        {
            this.personnel_employee_flow_role = new HashSet<personnel_employee_flow_role>();
            this.workflow_workflownode_approver = new HashSet<workflow_workflownode_approver>();
            this.workflow_workflownode_notifier = new HashSet<workflow_workflownode_notifier>();
        }
    
        public int id { get; set; }
        public string role_code { get; set; }
        public string role_name { get; set; }
        public string description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<personnel_employee_flow_role> personnel_employee_flow_role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workflow_workflownode_approver> workflow_workflownode_approver { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workflow_workflownode_notifier> workflow_workflownode_notifier { get; set; }
    }
}
