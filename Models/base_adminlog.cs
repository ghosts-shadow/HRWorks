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
    
    public partial class base_adminlog
    {
        public int id { get; set; }
        public string action { get; set; }
        public string targets { get; set; }
        public string targets_repr { get; set; }
        public short action_status { get; set; }
        public string description { get; set; }
        public string ip_address { get; set; }
        public bool can_routable { get; set; }
        public System.DateTime op_time { get; set; }
        public Nullable<int> content_type_id { get; set; }
        public int user_id { get; set; }
    
        public virtual auth_user auth_user { get; set; }
        public virtual django_content_type django_content_type { get; set; }
    }
}
