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
    
    public partial class Attendance
    {
        public long ID { get; set; }
        public long EmpID { get; set; }
        public long SubMain { get; set; }
        public string C1 { get; set; }
        public string C2 { get; set; }
        public string C3 { get; set; }
        public string C4 { get; set; }
        public string C5 { get; set; }
        public string C6 { get; set; }
        public string C7 { get; set; }
        public string C8 { get; set; }
        public string C9 { get; set; }
        public string C10 { get; set; }
        public string C11 { get; set; }
        public string C12 { get; set; }
        public string C13 { get; set; }
        public string C14 { get; set; }
        public string C15 { get; set; }
        public string C16 { get; set; }
        public string C17 { get; set; }
        public string C18 { get; set; }
        public string C19 { get; set; }
        public string C20 { get; set; }
        public string C21 { get; set; }
        public string C22 { get; set; }
        public string C23 { get; set; }
        public string C24 { get; set; }
        public string C25 { get; set; }
        public string C26 { get; set; }
        public string C27 { get; set; }
        public string C28 { get; set; }
        public string C29 { get; set; }
        public string C30 { get; set; }
        public string C31 { get; set; }
        public Nullable<long> TotalHours { get; set; }
        public Nullable<long> TotalOverTime { get; set; }
        public Nullable<long> TotalAbsent { get; set; }
        public Nullable<long> AccommodationDeduction { get; set; }
        public Nullable<long> FoodDeduction { get; set; }
        public Nullable<long> TotalWorkingDays { get; set; }
        public Nullable<long> TotalVL { get; set; }
        public Nullable<long> TotalTransefer { get; set; }
        public Nullable<long> TotalSickLeave { get; set; }
        public Nullable<long> FridayHours { get; set; }
        public Nullable<long> Holidays { get; set; }
        public string ManPowerSupply { get; set; }
        public Nullable<long> CompID { get; set; }
        public string Encoded_Absolute_URL { get; set; }
        public string Item_Type { get; set; }
        public string Path { get; set; }
        public string URL_Path { get; set; }
        public string Workflow_Instance_ID { get; set; }
        public string File_Type { get; set; }
        public Nullable<long> xABST { get; set; }
        public Nullable<long> nABST { get; set; }
        public Nullable<long> xOT { get; set; }
        public Nullable<long> nnOT { get; set; }
        public string status { get; set; }
    
        public virtual MainTimeSheet MainTimeSheet { get; set; }
        public virtual LabourMaster LabourMaster { get; set; }
    }
}