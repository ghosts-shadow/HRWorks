using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;

namespace HRworks.Controllers
{
    public class AttendancesController : Controller
    {
        private LogisticsSoftEntities db = new LogisticsSoftEntities();
        private HREntities db1 = new HREntities();

        // GET: Attendances
        public ActionResult Index(string variable ,DateTime? month)
        {
            var maintime = this.db.MainTimeSheets.ToList();
            var attendance = new List<Attendance>();
            var attendance1 = new List<Attendance>();
            var attendanc = this.db.Attendances.ToList();
            if (variable == "")
            {
                variable = null;
            }
            ViewBag.variable = variable;
            ViewBag.month = month;
            if (month.HasValue)
            {
                var maintim = maintime.FindAll(
                x => x.ManPowerSupplier == 1 && x.TMonth.Month == month.Value.Month
                                             && x.TMonth.Year == month.Value.Year);
                foreach (var sheet in maintim)
                {
                    var atten = attendanc.FindAll(x => x.SubMain == sheet.ID);
                    attendance.AddRange(atten);
                }

                foreach (var at in attendance)
                {
                    if (at.C1 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C2 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C3 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C4 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C5 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C6 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C7 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C8 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C9 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C10 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C11 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C12 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C13 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C14 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C15 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C16 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C17 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C18 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C19 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C20 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C21 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C22 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C23 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C24 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C25 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C26 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C27 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C28 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C29 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C30 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    if (at.C31 == variable)
                    {
                        attendance1.Add(at);
                        goto newval;
                    }
                    newval: ;
                }
                foreach (var at in attendance1)
                {
                    var days = 31;
                    if (at.C1 != variable)
                    {
                        
                        at.C1 ="0"; days--;
                    }
                    if (at.C2 != variable)
                    {
                        
                        at.C2 ="0"; days--;
                    }
                    if (at.C3 != variable)
                    {
                        
                        at.C3 ="0"; days--;
                    }
                    if (at.C4 != variable)
                    {
                        
                        at.C4 ="0"; days--;
                    }
                    if (at.C5 != variable)
                    {
                        
                        at.C5 ="0"; days--;
                    }
                    if (at.C6 != variable)
                    {
                        
                        at.C6 ="0"; days--;
                    }
                    if (at.C7 != variable)
                    {
                        
                        at.C7="0"; days--;
                    }
                    if (at.C8 != variable)
                    {
                        
                        at.C8 ="0"; days--;
                    }
                    if (at.C9 != variable)
                    {
                        
                        at.C9 ="0"; days--;
                    }
                    if (at.C10 != variable)
                    {
                        
                        at.C10 ="0"; days--;
                    }
                    if (at.C11 != variable)
                    {
                        
                        at.C11 ="0"; days--;
                    }
                    if (at.C12 != variable)
                    {
                        
                        at.C12 ="0"; days--;
                    }
                    if (at.C13 != variable)
                    {
                        
                        at.C13 ="0"; days--;
                    }
                    if (at.C14 != variable)
                    {
                        
                        at.C14 ="0"; days--;
                    }
                    if (at.C15 != variable)
                    {
                        
                        at.C15 ="0"; days--;
                    }
                    if (at.C16 != variable)
                    {
                        
                        at.C16 ="0"; days--;
                    }
                    if (at.C17 != variable)
                    {
                        
                        at.C17 ="0"; days--;
                    }
                    if (at.C18 != variable)
                    {
                        
                        at.C18 ="0"; days--;
                    }
                    if (at.C19 != variable)
                    {
                        
                        at.C19 ="0"; days--;
                    }
                    if (at.C20 != variable)
                    {
                        
                        at.C20 ="0"; days--;
                    }
                    if (at.C21 != variable)
                    {
                        
                        at.C21 ="0"; days--;
                    }
                    if (at.C22 != variable)
                    {
                        
                        at.C22 ="0"; days--;
                    }
                    if (at.C23 != variable)
                    {
                        
                        at.C23 ="0"; days--;
                    }
                    if (at.C24 != variable)
                    {
                        
                        at.C24 ="0"; days--;
                    }
                    if (at.C25 != variable)
                    {
                        
                        at.C25 ="0"; days--;
                    }
                    if (at.C26 != variable)
                    {
                        
                        at.C26 ="0"; days--;
                    }
                    if (at.C27 != variable)
                    {
                        
                        at.C27 ="0"; days--;
                    }
                    if (at.C28 != variable)
                    {
                        
                        at.C28 ="0"; days--;
                    }
                    if (at.C29 != variable)
                    {
                        
                        at.C29 ="0"; days--;
                    }
                    if (at.C30 != variable)
                    {
                        
                        at.C30 ="0"; days--;
                    }
                    if (at.C31 != variable)
                    {
                        
                        at.C31 ="0"; days--;
                    }

                    if (variable == null)
                    {
                        at.Totalnull =days;
                    }
                    else
                    {
                        at.Totalnull = 0;
                    }
                }
            }
            return View(attendance1.OrderBy(x=>x.LabourMaster.EMPNO));
        }

        public ActionResult notintimesheet(DateTime? month)
        {
            var maintime = this.db.MainTimeSheets.ToList();
            var HRemployee = db1.master_file.ToList();
            var HRempfinal = new List<master_file>();
            var tsemployee = db.LabourMasters.ToList();
            var attendance = new List<Attendance>();
            var attendanc = this.db.Attendances.ToList();
            if (month.HasValue)
            {
                var maintim = maintime.FindAll(
                    x =>(x.ManPowerSupplier == 1 || x.ManPowerSupplier == 8)   && x.TMonth.Month == month.Value.Month
                                                 && x.TMonth.Year == month.Value.Year);
                foreach (var sheet in maintim)
                {
                    var atten = attendanc.FindAll(x => x.SubMain == sheet.ID);
                    attendance.AddRange(atten);
                }
            }

            foreach (var file in HRemployee)
            {
                var tsemp = tsemployee.Find(x => x.EMPNO == file.employee_no);
                if (tsemp != null)
                {
                    if (!attendance.Exists(x=>x.EmpID == tsemp.ID))
                    {
                        if (!HRempfinal.Exists(x=>x.employee_no == file.employee_no) && file.last_working_day == null && file.date_joined < month)
                        {
                            HRempfinal.Add(file);
                        }
                    }
                }
            }
            return View(HRempfinal.OrderBy(x=>x.employee_no));
        }

        public ActionResult Report()
        {
            return this.View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
