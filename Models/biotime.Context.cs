﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class biometrics_DBEntities : DbContext
    {
        public biometrics_DBEntities()
            : base("name=biometrics_DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<acc_acccombination> acc_acccombination { get; set; }
        public virtual DbSet<acc_accgroups> acc_accgroups { get; set; }
        public virtual DbSet<acc_accholiday> acc_accholiday { get; set; }
        public virtual DbSet<acc_accprivilege> acc_accprivilege { get; set; }
        public virtual DbSet<acc_accterminal> acc_accterminal { get; set; }
        public virtual DbSet<acc_acctimezone> acc_acctimezone { get; set; }
        public virtual DbSet<accounts_adminbiodata> accounts_adminbiodata { get; set; }
        public virtual DbSet<accounts_usernotification> accounts_usernotification { get; set; }
        public virtual DbSet<att_attcalclog> att_attcalclog { get; set; }
        public virtual DbSet<att_attcode> att_attcode { get; set; }
        public virtual DbSet<att_attemployee> att_attemployee { get; set; }
        public virtual DbSet<att_attgroup> att_attgroup { get; set; }
        public virtual DbSet<att_attpolicy> att_attpolicy { get; set; }
        public virtual DbSet<att_attreportsetting> att_attreportsetting { get; set; }
        public virtual DbSet<att_attrule> att_attrule { get; set; }
        public virtual DbSet<att_attschedule> att_attschedule { get; set; }
        public virtual DbSet<att_attshift> att_attshift { get; set; }
        public virtual DbSet<att_breaktime> att_breaktime { get; set; }
        public virtual DbSet<att_calculatelastdate> att_calculatelastdate { get; set; }
        public virtual DbSet<att_calculatetask> att_calculatetask { get; set; }
        public virtual DbSet<att_changeschedule> att_changeschedule { get; set; }
        public virtual DbSet<att_departmentpolicy> att_departmentpolicy { get; set; }
        public virtual DbSet<att_departmentschedule> att_departmentschedule { get; set; }
        public virtual DbSet<att_deptattrule> att_deptattrule { get; set; }
        public virtual DbSet<att_grouppolicy> att_grouppolicy { get; set; }
        public virtual DbSet<att_groupschedule> att_groupschedule { get; set; }
        public virtual DbSet<att_holiday> att_holiday { get; set; }
        public virtual DbSet<att_leave> att_leave { get; set; }
        public virtual DbSet<att_leavegroup> att_leavegroup { get; set; }
        public virtual DbSet<att_leavegroupdetail> att_leavegroupdetail { get; set; }
        public virtual DbSet<att_leaveyearbalance> att_leaveyearbalance { get; set; }
        public virtual DbSet<att_manuallog> att_manuallog { get; set; }
        public virtual DbSet<att_overtime> att_overtime { get; set; }
        public virtual DbSet<att_overtimepolicy> att_overtimepolicy { get; set; }
        public virtual DbSet<att_paycode> att_paycode { get; set; }
        public virtual DbSet<att_payloadattcode> att_payloadattcode { get; set; }
        public virtual DbSet<att_payloadbase> att_payloadbase { get; set; }
        public virtual DbSet<att_payloadbreak> att_payloadbreak { get; set; }
        public virtual DbSet<att_payloadeffectpunch> att_payloadeffectpunch { get; set; }
        public virtual DbSet<att_payloadexception> att_payloadexception { get; set; }
        public virtual DbSet<att_payloadmulpunchset> att_payloadmulpunchset { get; set; }
        public virtual DbSet<att_payloadovertime> att_payloadovertime { get; set; }
        public virtual DbSet<att_payloadparing> att_payloadparing { get; set; }
        public virtual DbSet<att_payloadpaycode> att_payloadpaycode { get; set; }
        public virtual DbSet<att_payloadpunch> att_payloadpunch { get; set; }
        public virtual DbSet<att_payloadtimecard> att_payloadtimecard { get; set; }
        public virtual DbSet<att_reportparam> att_reportparam { get; set; }
        public virtual DbSet<att_reporttemplate> att_reporttemplate { get; set; }
        public virtual DbSet<att_shiftdetail> att_shiftdetail { get; set; }
        public virtual DbSet<att_temporaryschedule> att_temporaryschedule { get; set; }
        public virtual DbSet<att_tempschedule> att_tempschedule { get; set; }
        public virtual DbSet<att_timeinterval> att_timeinterval { get; set; }
        public virtual DbSet<att_timeinterval_break_time> att_timeinterval_break_time { get; set; }
        public virtual DbSet<att_training> att_training { get; set; }
        public virtual DbSet<att_webpunch> att_webpunch { get; set; }
        public virtual DbSet<attparam> attparams { get; set; }
        public virtual DbSet<auth_group> auth_group { get; set; }
        public virtual DbSet<auth_group_permissions> auth_group_permissions { get; set; }
        public virtual DbSet<auth_permission> auth_permission { get; set; }
        public virtual DbSet<auth_user> auth_user { get; set; }
        public virtual DbSet<auth_user_auth_area> auth_user_auth_area { get; set; }
        public virtual DbSet<auth_user_auth_dept> auth_user_auth_dept { get; set; }
        public virtual DbSet<auth_user_groups> auth_user_groups { get; set; }
        public virtual DbSet<auth_user_profile> auth_user_profile { get; set; }
        public virtual DbSet<auth_user_user_permissions> auth_user_user_permissions { get; set; }
        public virtual DbSet<authtoken_token> authtoken_token { get; set; }
        public virtual DbSet<base_adminlog> base_adminlog { get; set; }
        public virtual DbSet<base_attparamdepts> base_attparamdepts { get; set; }
        public virtual DbSet<base_autoattexporttask> base_autoattexporttask { get; set; }
        public virtual DbSet<base_autoexporttask> base_autoexporttask { get; set; }
        public virtual DbSet<base_autoimporttask> base_autoimporttask { get; set; }
        public virtual DbSet<base_bookmark> base_bookmark { get; set; }
        public virtual DbSet<base_dbbackuplog> base_dbbackuplog { get; set; }
        public virtual DbSet<base_emailtemplate> base_emailtemplate { get; set; }
        public virtual DbSet<base_eventalertsetting> base_eventalertsetting { get; set; }
        public virtual DbSet<base_fixedexporttask> base_fixedexporttask { get; set; }
        public virtual DbSet<base_linenotifysetting> base_linenotifysetting { get; set; }
        public virtual DbSet<base_securitypolicy> base_securitypolicy { get; set; }
        public virtual DbSet<base_sendemail> base_sendemail { get; set; }
        public virtual DbSet<base_sftpsetting> base_sftpsetting { get; set; }
        public virtual DbSet<base_sysparam> base_sysparam { get; set; }
        public virtual DbSet<base_sysparamdept> base_sysparamdept { get; set; }
        public virtual DbSet<base_systemlog> base_systemlog { get; set; }
        public virtual DbSet<base_systemsetting> base_systemsetting { get; set; }
        public virtual DbSet<base_whatsapplog> base_whatsapplog { get; set; }
        public virtual DbSet<django_admin_log> django_admin_log { get; set; }
        public virtual DbSet<django_celery_beat_clockedschedule> django_celery_beat_clockedschedule { get; set; }
        public virtual DbSet<django_celery_beat_crontabschedule> django_celery_beat_crontabschedule { get; set; }
        public virtual DbSet<django_celery_beat_intervalschedule> django_celery_beat_intervalschedule { get; set; }
        public virtual DbSet<django_celery_beat_periodictask> django_celery_beat_periodictask { get; set; }
        public virtual DbSet<django_celery_beat_periodictasks> django_celery_beat_periodictasks { get; set; }
        public virtual DbSet<django_celery_beat_solarschedule> django_celery_beat_solarschedule { get; set; }
        public virtual DbSet<django_content_type> django_content_type { get; set; }
        public virtual DbSet<django_migrations> django_migrations { get; set; }
        public virtual DbSet<django_session> django_session { get; set; }
        public virtual DbSet<ep_epsetup> ep_epsetup { get; set; }
        public virtual DbSet<ep_eptransaction> ep_eptransaction { get; set; }
        public virtual DbSet<guardian_groupobjectpermission> guardian_groupobjectpermission { get; set; }
        public virtual DbSet<guardian_userobjectpermission> guardian_userobjectpermission { get; set; }
        public virtual DbSet<iclock_biodata> iclock_biodata { get; set; }
        public virtual DbSet<iclock_biophoto> iclock_biophoto { get; set; }
        public virtual DbSet<iclock_devicemoduleconfig> iclock_devicemoduleconfig { get; set; }
        public virtual DbSet<iclock_errorcommandlog> iclock_errorcommandlog { get; set; }
        public virtual DbSet<iclock_privatemessage> iclock_privatemessage { get; set; }
        public virtual DbSet<iclock_publicmessage> iclock_publicmessage { get; set; }
        public virtual DbSet<iclock_shortmessage> iclock_shortmessage { get; set; }
        public virtual DbSet<iclock_terminal> iclock_terminal { get; set; }
        public virtual DbSet<iclock_terminalcommand> iclock_terminalcommand { get; set; }
        public virtual DbSet<iclock_terminalcommandlog> iclock_terminalcommandlog { get; set; }
        public virtual DbSet<iclock_terminalemployee> iclock_terminalemployee { get; set; }
        public virtual DbSet<iclock_terminallog> iclock_terminallog { get; set; }
        public virtual DbSet<iclock_terminalparameter> iclock_terminalparameter { get; set; }
        public virtual DbSet<iclock_terminaluploadlog> iclock_terminaluploadlog { get; set; }
        public virtual DbSet<iclock_terminalworkcode> iclock_terminalworkcode { get; set; }
        public virtual DbSet<iclock_transaction> iclock_transaction { get; set; }
        public virtual DbSet<iclock_transactionproofcmd> iclock_transactionproofcmd { get; set; }
        public virtual DbSet<meeting_meetingentity> meeting_meetingentity { get; set; }
        public virtual DbSet<meeting_meetingentity_attender> meeting_meetingentity_attender { get; set; }
        public virtual DbSet<meeting_meetingmanuallog> meeting_meetingmanuallog { get; set; }
        public virtual DbSet<meeting_meetingpayloadbase> meeting_meetingpayloadbase { get; set; }
        public virtual DbSet<meeting_meetingroom> meeting_meetingroom { get; set; }
        public virtual DbSet<meeting_meetingroomdevice> meeting_meetingroomdevice { get; set; }
        public virtual DbSet<meeting_meetingtransaction> meeting_meetingtransaction { get; set; }
        public virtual DbSet<mobile_announcement> mobile_announcement { get; set; }
        public virtual DbSet<mobile_appactionlog> mobile_appactionlog { get; set; }
        public virtual DbSet<mobile_applist> mobile_applist { get; set; }
        public virtual DbSet<mobile_appnotification> mobile_appnotification { get; set; }
        public virtual DbSet<mobile_gpsfordepartment> mobile_gpsfordepartment { get; set; }
        public virtual DbSet<mobile_gpsfordepartment_location> mobile_gpsfordepartment_location { get; set; }
        public virtual DbSet<mobile_gpsforemployee> mobile_gpsforemployee { get; set; }
        public virtual DbSet<mobile_gpsforemployee_location> mobile_gpsforemployee_location { get; set; }
        public virtual DbSet<mobile_gpslocation> mobile_gpslocation { get; set; }
        public virtual DbSet<mobile_mobileapirequestlog> mobile_mobileapirequestlog { get; set; }
        public virtual DbSet<payroll_deductionformula> payroll_deductionformula { get; set; }
        public virtual DbSet<payroll_emploan> payroll_emploan { get; set; }
        public virtual DbSet<payroll_emppayrollprofile> payroll_emppayrollprofile { get; set; }
        public virtual DbSet<payroll_exceptionformula> payroll_exceptionformula { get; set; }
        public virtual DbSet<payroll_extradeduction> payroll_extradeduction { get; set; }
        public virtual DbSet<payroll_extraincrease> payroll_extraincrease { get; set; }
        public virtual DbSet<payroll_increasementformula> payroll_increasementformula { get; set; }
        public virtual DbSet<payroll_leaveformula> payroll_leaveformula { get; set; }
        public virtual DbSet<payroll_overtimeformula> payroll_overtimeformula { get; set; }
        public virtual DbSet<payroll_payrollpayload> payroll_payrollpayload { get; set; }
        public virtual DbSet<payroll_payrollpayloadpaycode> payroll_payrollpayloadpaycode { get; set; }
        public virtual DbSet<payroll_reimbursement> payroll_reimbursement { get; set; }
        public virtual DbSet<payroll_salaryadvance> payroll_salaryadvance { get; set; }
        public virtual DbSet<payroll_salarystructure> payroll_salarystructure { get; set; }
        public virtual DbSet<payroll_salarystructure_deductionformula> payroll_salarystructure_deductionformula { get; set; }
        public virtual DbSet<payroll_salarystructure_exceptionformula> payroll_salarystructure_exceptionformula { get; set; }
        public virtual DbSet<payroll_salarystructure_increasementformula> payroll_salarystructure_increasementformula { get; set; }
        public virtual DbSet<payroll_salarystructure_leaveformula> payroll_salarystructure_leaveformula { get; set; }
        public virtual DbSet<payroll_salarystructure_overtimeformula> payroll_salarystructure_overtimeformula { get; set; }
        public virtual DbSet<personnel_area> personnel_area { get; set; }
        public virtual DbSet<personnel_assignareaemployee> personnel_assignareaemployee { get; set; }
        public virtual DbSet<personnel_certification> personnel_certification { get; set; }
        public virtual DbSet<personnel_company> personnel_company { get; set; }
        public virtual DbSet<personnel_department> personnel_department { get; set; }
        public virtual DbSet<personnel_employee> personnel_employee { get; set; }
        public virtual DbSet<personnel_employee_area> personnel_employee_area { get; set; }
        public virtual DbSet<personnel_employee_flow_role> personnel_employee_flow_role { get; set; }
        public virtual DbSet<personnel_employeecalendar> personnel_employeecalendar { get; set; }
        public virtual DbSet<personnel_employeecertification> personnel_employeecertification { get; set; }
        public virtual DbSet<personnel_employeecustomattribute> personnel_employeecustomattribute { get; set; }
        public virtual DbSet<personnel_employeeextrainfo> personnel_employeeextrainfo { get; set; }
        public virtual DbSet<personnel_employeeprofile> personnel_employeeprofile { get; set; }
        public virtual DbSet<personnel_employment> personnel_employment { get; set; }
        public virtual DbSet<personnel_position> personnel_position { get; set; }
        public virtual DbSet<personnel_resign> personnel_resign { get; set; }
        public virtual DbSet<rest_framework_tracking_apirequestlog> rest_framework_tracking_apirequestlog { get; set; }
        public virtual DbSet<staff_stafftoken> staff_stafftoken { get; set; }
        public virtual DbSet<sync_area> sync_area { get; set; }
        public virtual DbSet<sync_department> sync_department { get; set; }
        public virtual DbSet<sync_employee> sync_employee { get; set; }
        public virtual DbSet<sync_job> sync_job { get; set; }
        public virtual DbSet<visitor_reason> visitor_reason { get; set; }
        public virtual DbSet<visitor_reservation> visitor_reservation { get; set; }
        public virtual DbSet<visitor_visitor> visitor_visitor { get; set; }
        public virtual DbSet<visitor_visitor_acc_groups> visitor_visitor_acc_groups { get; set; }
        public virtual DbSet<visitor_visitor_area> visitor_visitor_area { get; set; }
        public virtual DbSet<visitor_visitorbiodata> visitor_visitorbiodata { get; set; }
        public virtual DbSet<visitor_visitorbiophoto> visitor_visitorbiophoto { get; set; }
        public virtual DbSet<visitor_visitorconfig> visitor_visitorconfig { get; set; }
        public virtual DbSet<visitor_visitorlog> visitor_visitorlog { get; set; }
        public virtual DbSet<visitor_visitortransaction> visitor_visitortransaction { get; set; }
        public virtual DbSet<workflow_nodeinstance> workflow_nodeinstance { get; set; }
        public virtual DbSet<workflow_workflowengine> workflow_workflowengine { get; set; }
        public virtual DbSet<workflow_workflowengine_employee> workflow_workflowengine_employee { get; set; }
        public virtual DbSet<workflow_workflowinstance> workflow_workflowinstance { get; set; }
        public virtual DbSet<workflow_workflownode> workflow_workflownode { get; set; }
        public virtual DbSet<workflow_workflownode_approver> workflow_workflownode_approver { get; set; }
        public virtual DbSet<workflow_workflownode_notifier> workflow_workflownode_notifier { get; set; }
        public virtual DbSet<workflow_workflowrole> workflow_workflowrole { get; set; }

        public System.Data.Entity.DbSet<HRworks.Models.hik> hiks { get; set; }
    }
}
