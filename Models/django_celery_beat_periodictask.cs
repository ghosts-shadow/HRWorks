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
    
    public partial class django_celery_beat_periodictask
    {
        public int id { get; set; }
        public string name { get; set; }
        public string task { get; set; }
        public string args { get; set; }
        public string kwargs { get; set; }
        public string queue { get; set; }
        public string exchange { get; set; }
        public string routing_key { get; set; }
        public Nullable<System.DateTime> expires { get; set; }
        public bool enabled { get; set; }
        public Nullable<System.DateTime> last_run_at { get; set; }
        public int total_run_count { get; set; }
        public System.DateTime date_changed { get; set; }
        public string description { get; set; }
        public Nullable<int> crontab_id { get; set; }
        public Nullable<int> interval_id { get; set; }
        public Nullable<int> solar_id { get; set; }
        public bool one_off { get; set; }
        public Nullable<System.DateTime> start_time { get; set; }
        public Nullable<int> priority { get; set; }
        public string headers { get; set; }
        public Nullable<int> clocked_id { get; set; }
    
        public virtual django_celery_beat_clockedschedule django_celery_beat_clockedschedule { get; set; }
        public virtual django_celery_beat_crontabschedule django_celery_beat_crontabschedule { get; set; }
        public virtual django_celery_beat_intervalschedule django_celery_beat_intervalschedule { get; set; }
        public virtual django_celery_beat_solarschedule django_celery_beat_solarschedule { get; set; }
    }
}
