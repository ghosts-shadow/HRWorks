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
    
    public partial class iclock_terminaluploadlog
    {
        public int id { get; set; }
        public string @event { get; set; }
        public string content { get; set; }
        public int upload_count { get; set; }
        public int error_count { get; set; }
        public System.DateTime upload_time { get; set; }
        public int terminal_id { get; set; }
    
        public virtual iclock_terminal iclock_terminal { get; set; }
    }
}
