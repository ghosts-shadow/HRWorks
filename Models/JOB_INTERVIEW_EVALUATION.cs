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
    
    public partial class JOB_INTERVIEW_EVALUATION
    {
        public int Id { get; set; }
        public string interviewee_name { get; set; }
        public Nullable<System.DateTime> interview_date { get; set; }
        public string job_applied_for { get; set; }
        public string department { get; set; }
        public string education { get; set; }
        public string experience { get; set; }
        public string job_knoledge { get; set; }
        public string conceptual_clarity { get; set; }
        public string appearance { get; set; }
        public string communication { get; set; }
        public string confidence_level { get; set; }
        public string attitude { get; set; }
        public Nullable<long> current_salary { get; set; }
        public Nullable<long> expected_salary { get; set; }
        public string reason_F_L_P_C { get; set; }
        public string overall_evaluation { get; set; }
        public bool recommented_to_be_employed { get; set; }
        public string in_position_of { get; set; }
        public Nullable<bool> employment_in_the_future { get; set; }
    }
}
