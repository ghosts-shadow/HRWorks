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
    
    public partial class att_changeschedule
    {
        public int workflowinstance_ptr_id { get; set; }
        public System.DateTime att_date { get; set; }
        public string previous_timeinterval { get; set; }
        public System.DateTime apply_time { get; set; }
        public string apply_reason { get; set; }
        public string attachment { get; set; }
        public int timeinterval_id { get; set; }
    
        public virtual att_timeinterval att_timeinterval { get; set; }
        public virtual workflow_workflowinstance workflow_workflowinstance { get; set; }
    }
}