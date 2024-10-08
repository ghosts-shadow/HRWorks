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
    
    public partial class visitor_reservation
    {
        public int workflowinstance_ptr_id { get; set; }
        public string vis_first_name { get; set; }
        public string vis_last_name { get; set; }
        public string cert_no { get; set; }
        public string gender { get; set; }
        public string company { get; set; }
        public Nullable<System.DateTime> update_time { get; set; }
        public int visit_quantity { get; set; }
        public System.DateTime visit_date { get; set; }
        public System.DateTime apply_time { get; set; }
        public string apply_reason { get; set; }
        public string email { get; set; }
        public int cert_type_id { get; set; }
        public Nullable<int> visit_department_id { get; set; }
        public Nullable<int> visit_reason_id { get; set; }
    
        public virtual personnel_certification personnel_certification { get; set; }
        public virtual personnel_department personnel_department { get; set; }
        public virtual visitor_reason visitor_reason { get; set; }
        public virtual workflow_workflowinstance workflow_workflowinstance { get; set; }
    }
}
