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
    
    public partial class base_sendemail
    {
        public int id { get; set; }
        public int purpose { get; set; }
        public string email_to { get; set; }
        public string email_cc { get; set; }
        public string email_bcc { get; set; }
        public string email_subject { get; set; }
        public string email_content { get; set; }
        public Nullable<System.DateTime> send_time { get; set; }
        public Nullable<short> send_status { get; set; }
    }
}