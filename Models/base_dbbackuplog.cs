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
    
    public partial class base_dbbackuplog
    {
        public int id { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public string create_user { get; set; }
        public Nullable<System.DateTime> change_time { get; set; }
        public string change_user { get; set; }
        public short status { get; set; }
        public string db_type { get; set; }
        public string db_name { get; set; }
        public string @operator { get; set; }
        public string backup_file { get; set; }
        public System.DateTime backup_time { get; set; }
        public short backup_status { get; set; }
        public string remark { get; set; }
    }
}
