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
    
    public partial class authtoken_token
    {
        public string key { get; set; }
        public System.DateTime created { get; set; }
        public int user_id { get; set; }
    
        public virtual auth_user auth_user { get; set; }
    }
}
