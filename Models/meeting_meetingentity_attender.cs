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
    
    public partial class meeting_meetingentity_attender
    {
        public int id { get; set; }
        public int meetingentity_id { get; set; }
        public int employee_id { get; set; }
    
        public virtual meeting_meetingentity meeting_meetingentity { get; set; }
        public virtual personnel_employee personnel_employee { get; set; }
    }
}