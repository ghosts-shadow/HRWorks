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
    
    public partial class guardian_userobjectpermission
    {
        public int id { get; set; }
        public string object_pk { get; set; }
        public int content_type_id { get; set; }
        public int permission_id { get; set; }
        public int user_id { get; set; }
    
        public virtual auth_permission auth_permission { get; set; }
        public virtual auth_user auth_user { get; set; }
        public virtual django_content_type django_content_type { get; set; }
    }
}