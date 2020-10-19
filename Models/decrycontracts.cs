using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    using System.ComponentModel.DataAnnotations;

    public class decrycontracts
    {
        
            [Display(Name = "Contract ID")]
            public Nullable<int> con_id { get; set; }

            [Display(Name = "Employee ID")]
            public int employee_no { get; set; }

            [Display(Name = "Designation")]
            public string designation { get; set; }

            public string grade { get; set; }

            [Display(Name = "Departmant Project")]
            public string departmant_project { get; set; }

            [Display(Name = "Gross Salary")]
            public Nullable<int> salary_details { get; set; }

            [Display(Name = "Basic")]
            public Nullable<int> basic { get; set; }

            [Display(Name = "Housing Allowance")]
            public Nullable<int> housing_allowance { get; set; }

            [Display(Name = "Transportation Allowance")]
            public Nullable<int> transportation_allowance { get; set; }

            public Nullable<int> FOT { get; set; }

            [Display(Name = "Food Allowance")]
            public Nullable<int> food_allowance { get; set; }

            [Display(Name = "Living Allowance")]
            public Nullable<int> living_allowance { get; set; }

            [Display(Name = "Ticket Allowance")]
            public Nullable<int> ticket_allowance { get; set; }

            [Display(Name = "Other")]
            public Nullable<int> others { get; set; }

            [Display(Name = "Arrears")]
            public Nullable<int> arrears { get; set; }

            public int employee_id { get; set; }

            [Display(Name = "Photo")]
            public string imgpath { get; set; }

            [Display(Name = "Changed by")]
            public string changed_by { get; set; }

            [Display(Name = "Date changed")]
            public Nullable<System.DateTime> date_changed { get; set; }

            public virtual master_file master_file { get; set; }
        
    }
}