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
    
    public partial class workflow_workflowengine_employee
    {
        public int id { get; set; }
        public int workflowengine_id { get; set; }
        public int employee_id { get; set; }
    
        public virtual personnel_employee personnel_employee { get; set; }
        public virtual workflow_workflowengine workflow_workflowengine { get; set; }
    }
}