using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    public class CustomModels
    {
    }
    public enum AdjustmentType { None, LateIn, EarlyOut }
    public enum AdjustmentStatus { None, Pending, Approved }

    public class AttCalendarPunch
    {
        public string Id { get; set; }
        public string PersonName { get; set; }
        public System.DateTime Date { get; set; }
        public System.TimeSpan Time { get; set; }
        public System.DateTime DateTime { get; set; }
        public bool IsCheckIn { get; set; }
        public AdjustmentType AdjustmentType { get; set; }
        public AdjustmentStatus AdjustmentStatus { get; set; }

        public bool HasAdjustment => AdjustmentType != AdjustmentType.None;
        public string PunchLabel => IsCheckIn ? "check in" : "check out";
        public string AdjustmentLabel =>
            AdjustmentType == AdjustmentType.LateIn ? "Late In" :
            AdjustmentType == AdjustmentType.EarlyOut ? "Early Out" : "";
    }
}