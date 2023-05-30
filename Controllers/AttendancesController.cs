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
    [Authorize]
    public class AttendancesController : Controller
    {
        private LogisticsSoftEntities db = new LogisticsSoftEntities();
        private HREntities db1 = new HREntities();

        // GET: Attendances
        public ActionResult Index(string variable, DateTime? month, string idstr)
        {
            var maintime = this.db.MainTimeSheets.ToList();
            var attendance = new List<Attendance>();
            var attendance1 = new List<Attendance>();
            var attendanc = this.db.Attendances.ToList();
            if (variable == "")
            {
                variable = null;
            }

            if (idstr == null)
            {
                idstr = " ";
                ViewBag.idstr = idstr;
            }
            else
            {
                ViewBag.idstr = idstr;
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
                    }

                    newval: ;
                }

                foreach (var at in attendance1)
                {
                    var days = 31;
                    if (idstr.Contains(at.ID.ToString()))
                    {
                        at.absapproved_ = true;
                    }

                    if (at.C1 != variable)
                    {
                        at.C1 = "0";
                        days--;
                    }

                    if (at.C2 != variable)
                    {
                        at.C2 = "0";
                        days--;
                    }

                    if (at.C3 != variable)
                    {
                        at.C3 = "0";
                        days--;
                    }

                    if (at.C4 != variable)
                    {
                        at.C4 = "0";
                        days--;
                    }

                    if (at.C5 != variable)
                    {
                        at.C5 = "0";
                        days--;
                    }

                    if (at.C6 != variable)
                    {
                        at.C6 = "0";
                        days--;
                    }

                    if (at.C7 != variable)
                    {
                        at.C7 = "0";
                        days--;
                    }

                    if (at.C8 != variable)
                    {
                        at.C8 = "0";
                        days--;
                    }

                    if (at.C9 != variable)
                    {
                        at.C9 = "0";
                        days--;
                    }

                    if (at.C10 != variable)
                    {
                        at.C10 = "0";
                        days--;
                    }

                    if (at.C11 != variable)
                    {
                        at.C11 = "0";
                        days--;
                    }

                    if (at.C12 != variable)
                    {
                        at.C12 = "0";
                        days--;
                    }

                    if (at.C13 != variable)
                    {
                        at.C13 = "0";
                        days--;
                    }

                    if (at.C14 != variable)
                    {
                        at.C14 = "0";
                        days--;
                    }

                    if (at.C15 != variable)
                    {
                        at.C15 = "0";
                        days--;
                    }

                    if (at.C16 != variable)
                    {
                        at.C16 = "0";
                        days--;
                    }

                    if (at.C17 != variable)
                    {
                        at.C17 = "0";
                        days--;
                    }

                    if (at.C18 != variable)
                    {
                        at.C18 = "0";
                        days--;
                    }

                    if (at.C19 != variable)
                    {
                        at.C19 = "0";
                        days--;
                    }

                    if (at.C20 != variable)
                    {
                        at.C20 = "0";
                        days--;
                    }

                    if (at.C21 != variable)
                    {
                        at.C21 = "0";
                        days--;
                    }

                    if (at.C22 != variable)
                    {
                        at.C22 = "0";
                        days--;
                    }

                    if (at.C23 != variable)
                    {
                        at.C23 = "0";
                        days--;
                    }

                    if (at.C24 != variable)
                    {
                        at.C24 = "0";
                        days--;
                    }

                    if (at.C25 != variable)
                    {
                        at.C25 = "0";
                        days--;
                    }

                    if (at.C26 != variable)
                    {
                        at.C26 = "0";
                        days--;
                    }

                    if (at.C27 != variable)
                    {
                        at.C27 = "0";
                        days--;
                    }

                    if (at.C28 != variable)
                    {
                        at.C28 = "0";
                        days--;
                    }

                    if (at.C29 != variable)
                    {
                        at.C29 = "0";
                        days--;
                    }

                    if (at.C30 != variable)
                    {
                        at.C30 = "0";
                        days--;
                    }

                    if (at.C31 != variable)
                    {
                        at.C31 = "0";
                        days--;
                    }

                    if (variable == null)
                    {
                        at.Totalnull = days;
                    }
                    else
                    {
                        at.Totalnull = 0;
                    }
                }
            }

            return View(attendance1.OrderBy(x => x.LabourMaster.EMPNO));
        }

        public ActionResult notintimesheet(DateTime? month)
        {
            ViewBag.month = month;
            var maintime = this.db.MainTimeSheets.ToList();
            var HRemployee = db1.master_file.ToList();
            var hrcontractslist = db1.contracts.ToList();
            var HRempfinal = new List<master_file>();
            var tsemployee = db.LabourMasters.ToList();
            var attendance = new List<Attendance>();
            var attendanc = this.db.Attendances.ToList();
            if (month.HasValue)
            {
                var maintim = maintime.FindAll(
                    x => (x.ManPowerSupplier == 1 || x.ManPowerSupplier == 8) && x.TMonth.Month == month.Value.Month
                                                                              && x.TMonth.Year == month.Value.Year);
                foreach (var sheet in maintim)
                {
                    var atten = attendanc.FindAll(x => x.SubMain == sheet.ID);
                    attendance.AddRange(atten);
                }

                foreach (var file in HRemployee)
                {
                    if (hrcontractslist.Exists(x => x.employee_no == file.employee_id))
                    {
                        var tsemp = tsemployee.Find(x => x.EMPNO == file.employee_no);
                        if (tsemp != null)
                        {
                            if (!attendance.Exists(x => x.EmpID == tsemp.ID))
                            {
                                if (!HRempfinal.Exists(x => x.employee_no == file.employee_no) &&
                                    (file.last_working_day == null ||
                                     (file.last_working_day.Value.Month > month.Value.Month &&
                                      file.last_working_day.Value.Year == month.Value.Year)) &&
                                    file.date_joined < month)
                                {
                                    HRempfinal.Add(file);
                                }
                            }
                        }
                    }
                }
            }


            return View(HRempfinal.OrderBy(x => x.employee_no));
        }

        public ActionResult Report()
        {
            return this.View();
        }

        public ActionResult approveabs(string variable, DateTime? month, long? id, string idstr)
        {
            var var1 = variable;
            var var2 = month;
            var var3 = idstr;
            var var4 = id.ToString();
            var3 += var4 + " , ";
            var absvar = new leave_absence();
            var at = db.Attendances.Find(id);
            var abslist = db1.leave_absence.ToList();
            var mastervar = db1.master_file.OrderByDescending(x => x.date_changed).ToList();
            if (at != null)
            {
                var emp = mastervar.Find(x => x.employee_no == at.LabourMaster.EMPNO);
                if (at.C1 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 1);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C2 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 2);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C3 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 3);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C4 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 4);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C5 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 5);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C6 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 6);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C7 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 7);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C8 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 8);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C9 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 9);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C10 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 10);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C11 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 11);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C12 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 12);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C13 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 13);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C14 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 14);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C15 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 15);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C16 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 16);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C17 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 17);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C18 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 18);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C19 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 19);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C20 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 20);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C21 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 21);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C22 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 22);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C23 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 23);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C24 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 24);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C25 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 25);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C26 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 26);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C27 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 27);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C28 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 28);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C29 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 29);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C30 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 30);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }

                if (at.C31 == variable)
                {
                    var date1 = new DateTime(month.Value.Year, month.Value.Month, 31);
                    var date2 = date1.AddDays(-1);
                    abslist = db1.leave_absence.ToList();
                    if (!abslist.Exists(x => x.master_file.employee_no == emp.employee_no && x.fromd <= date1 && x.tod >= date1))
                        if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                        {
                            var abs1 = abslist.Find(x =>
                                x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                            abs1.tod = date1;
                            abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                            this.db1.Entry(abs1).State = EntityState.Modified;
                            this.db1.SaveChanges();
                        }
                        else
                        {
                            absvar.Employee_id = emp.employee_id;
                            absvar.fromd = date1;
                            absvar.tod = date1;
                            absvar.absence = 1;
                            absvar.month = date1;
                            this.db1.leave_absence.Add(absvar);
                            this.db1.SaveChanges();
                        }
                }
            }

            return RedirectToAction("Index", new {variable = var1, month = var2, idstr = var3});
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