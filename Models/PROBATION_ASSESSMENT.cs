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
    
    public partial class PROBATION_ASSESSMENT
    {
        public int Id { get; set; }
        public Nullable<int> employee_id { get; set; }
        public string Knowledge_of_job { get; set; }
        public string Quality_of_work { get; set; }
        public string Achievement_Oriented { get; set; }
        public string Ability_To_Learn { get; set; }
        public string Work_Attitude_and_Co_operation { get; set; }
        public string Ability_To_work_Independently { get; set; }
        public string Reliability { get; set; }
        public string Initiative { get; set; }
        public string employee_excels { get; set; }
        public string improvement_ { get; set; }
        public string C_success_and_strengths { get; set; }
        public string further_development { get; set; }
        public string support { get; set; }
        public string Comments { get; set; }
        public string confirming { get; set; }
        public string Line_Managers_Comments { get; set; }
        public string Directors_Comments { get; set; }
    
        public virtual master_file master_file { get; set; }
    }
}
