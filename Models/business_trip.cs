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
    
    public partial class business_trip
    {
        public int Id { get; set; }
        public Nullable<int> Employee_id { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string type_of_trip { get; set; }
        public string IO_specify { get; set; }
        public string destination { get; set; }
        public Nullable<System.DateTime> Departure_Date { get; set; }
        public Nullable<System.DateTime> Return_Date { get; set; }
        public string TRIP_Purpose { get; set; }
        public string tp_Mode_of_Travel { get; set; }
        public string tp_Accommodation { get; set; }
    
        public virtual master_file master_file { get; set; }
    }
}
