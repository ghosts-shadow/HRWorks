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
    
    public partial class iclock_terminal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public iclock_terminal()
        {
            this.acc_accterminal = new HashSet<acc_accterminal>();
            this.ep_eptransaction = new HashSet<ep_eptransaction>();
            this.iclock_errorcommandlog = new HashSet<iclock_errorcommandlog>();
            this.iclock_publicmessage = new HashSet<iclock_publicmessage>();
            this.iclock_terminalcommand = new HashSet<iclock_terminalcommand>();
            this.iclock_terminalcommandlog = new HashSet<iclock_terminalcommandlog>();
            this.iclock_terminallog = new HashSet<iclock_terminallog>();
            this.iclock_terminalparameter = new HashSet<iclock_terminalparameter>();
            this.iclock_terminaluploadlog = new HashSet<iclock_terminaluploadlog>();
            this.iclock_transaction = new HashSet<iclock_transaction>();
            this.iclock_transactionproofcmd = new HashSet<iclock_transactionproofcmd>();
            this.meeting_meetingroomdevice = new HashSet<meeting_meetingroomdevice>();
            this.meeting_meetingtransaction = new HashSet<meeting_meetingtransaction>();
            this.visitor_visitortransaction = new HashSet<visitor_visitortransaction>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public string create_user { get; set; }
        public Nullable<System.DateTime> change_time { get; set; }
        public string change_user { get; set; }
        public short status { get; set; }
        public string ip_address { get; set; }
        public short terminal_tz { get; set; }
        public int heartbeat { get; set; }
        public short transfer_mode { get; set; }
        public int transfer_interval { get; set; }
        public string transfer_time { get; set; }
        public string fw_ver { get; set; }
        public string push_protocol { get; set; }
        public string push_ver { get; set; }
        public Nullable<int> language { get; set; }
        public string terminal_name { get; set; }
        public string platform { get; set; }
        public string oem_vendor { get; set; }
        public Nullable<int> user_count { get; set; }
        public Nullable<int> transaction_count { get; set; }
        public Nullable<int> fp_count { get; set; }
        public string fp_alg_ver { get; set; }
        public Nullable<int> face_count { get; set; }
        public string face_alg_ver { get; set; }
        public Nullable<int> fv_count { get; set; }
        public string fv_alg_ver { get; set; }
        public Nullable<int> palm_count { get; set; }
        public string palm_alg_ver { get; set; }
        public short lock_func { get; set; }
        public string log_stamp { get; set; }
        public string op_log_stamp { get; set; }
        public string capture_stamp { get; set; }
        public string sn { get; set; }
        public string alias { get; set; }
        public string real_ip { get; set; }
        public int state { get; set; }
        public Nullable<short> product_type { get; set; }
        public short is_attendance { get; set; }
        public short is_registration { get; set; }
        public Nullable<short> purpose { get; set; }
        public Nullable<short> controller_type { get; set; }
        public short authentication { get; set; }
        public string style { get; set; }
        public string upload_flag { get; set; }
        public bool is_tft { get; set; }
        public Nullable<System.DateTime> last_activity { get; set; }
        public Nullable<System.DateTime> upload_time { get; set; }
        public Nullable<System.DateTime> push_time { get; set; }
        public short is_access { get; set; }
        public Nullable<int> area_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<acc_accterminal> acc_accterminal { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ep_eptransaction> ep_eptransaction { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<iclock_errorcommandlog> iclock_errorcommandlog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<iclock_publicmessage> iclock_publicmessage { get; set; }
        public virtual personnel_area personnel_area { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<iclock_terminalcommand> iclock_terminalcommand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<iclock_terminalcommandlog> iclock_terminalcommandlog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<iclock_terminallog> iclock_terminallog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<iclock_terminalparameter> iclock_terminalparameter { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<iclock_terminaluploadlog> iclock_terminaluploadlog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<iclock_transaction> iclock_transaction { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<iclock_transactionproofcmd> iclock_transactionproofcmd { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<meeting_meetingroomdevice> meeting_meetingroomdevice { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<meeting_meetingtransaction> meeting_meetingtransaction { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<visitor_visitortransaction> visitor_visitortransaction { get; set; }
    }
}