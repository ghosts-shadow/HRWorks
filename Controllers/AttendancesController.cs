using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using HRworks.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.Office.Interop.Word;

namespace HRworks.Controllers
{
    [Authorize]
    public class AttendancesController : Controller
    {
        private LogisticsSoftEntities db = new LogisticsSoftEntities();
        private HREntities db1 = new HREntities();
        private biometrics_DBEntities db2 = new biometrics_DBEntities();

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
                    x => (x.ManPowerSupplier == 1 || x.ManPowerSupplier == 8 || x.ManPowerSupplier == 9  ) && x.TMonth.Month == month.Value.Month
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
                    x => (x.ManPowerSupplier == 1 || x.ManPowerSupplier == 8 || x.ManPowerSupplier == 9) && x.TMonth.Month == month.Value.Month
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

        public ActionResult approveabsn(DateTime? month)
        {
            var abslistt= new List<hik>();
            if (!month.HasValue)
            {
                goto end;
            }
            ViewBag.eddate = month;
            var monthstart = new DateTime(month.Value.Year, month.Value.Month, 1);
            var startdate = new DateTime(month.Value.Month == 1 ? month.Value.Year - 1 : month.Value.Year, month.Value.Month - 1, 21);
            var enddate = new DateTime(month.Value.Year, month.Value.Month, 20);
            var hikplist = db1.hiks.Where(x =>x.date >= startdate && x.date<=enddate).ToList();
            var leaveabslist = db1.leave_absence.Where(x=>x.month == monthstart).ToList();
            var alist = this.db1.master_file.Where(x=>x.contracts.Count()!=0).OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var inalist = this.db1.master_file.Where(x => x.last_working_day.HasValue == true).OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    if (!inalist.Exists(x=>x.employee_no == file.employee_no))
                    {
                        afinallist.Add(file);
                    }
                }
            }
            /*
            var temp = afinallist.Find(x=>x.employee_no == 5426);
            afinallist.RemoveAll(x =>x.employee_no != null );
            afinallist.Add(temp);
            */

            var hikplistin = hikplist.Where(x=>x.Status == "");

            var timesheetlist = db.access_date.Where(x =>x.entrydate > startdate && x.entrydate <= enddate).OrderBy(x=>x.Id).ToList();
            var leavelist = db1.Leaves.ToList();
            foreach (var file in afinallist)
            {
                var tempdate = startdate;
                do
                {
                    if (tempdate.DayOfWeek == DayOfWeek.Saturday || tempdate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        goto weekend;
                    }
                    var temphik = new hik();
                    if (hikplist.FindAll(x => x.ID == file.employee_no.ToString() && x.date == tempdate).Count() < 2)
                    {
                        temphik.date = tempdate;
                        temphik.ID = file.employee_no.ToString();
                        temphik.EMPID = file.employee_no;
                        temphik.Person = file.employee_name;
                        temphik.Status = "Absent";
                        abslistt.Add(temphik);
                        goto skip;
                    }
                    if (!hikplist.Exists(x=>x.ID == file.employee_no.ToString() && x.date == tempdate))
                    {
                        temphik.date= tempdate;
                        temphik.ID = file.employee_no.ToString();
                        temphik.EMPID = file.employee_no;
                        temphik.Person = file.employee_name;
                        temphik.Status = "Absent";
                        abslistt.Add(temphik);
                    }
                    skip: ;
                    if (timesheetlist.Exists(x => x.emp_no == file.employee_no && x.entrydate == tempdate ))
                    {
                        abslistt.Remove(temphik);
                        goto weekend;
                    }
                    if (leavelist.Exists(x => x.Employee_id == file.employee_id && x.Start_leave <= tempdate && x.End_leave >= tempdate))
                    {
                        abslistt.Remove(temphik);
                        goto weekend;
                    }

                    if (leaveabslist.Exists(x=>x.Employee_id == file.employee_id && x.fromd <= tempdate && x.tod >= tempdate))
                    {
                        abslistt.Remove(temphik);
                        goto weekend;
                    }
                    weekend: ;
                    tempdate = tempdate.AddDays(1);
                    if (tempdate > DateTime.Now)
                    {
                        goto fend;
                    }
                } while (tempdate <= enddate);
                fend: ;
            }
            end: ;
            return View(abslistt.OrderBy(x=>x.EMPID).ToList());
        }

        public ActionResult Abstransfer(int? id, DateTime? date1,DateTime? month)
        {
            var newlistemo = new List<int>();
            var neweos = "";
            var monthstart = new DateTime(month.Value.Year, month.Value.Month, 1);
            var abslist = db1.leave_absence.ToList();
            var mastervar = db1.master_file.OrderByDescending(x => x.date_changed).ToList();
            if (!abslist.Exists(x => x.master_file.employee_no == id && x.fromd <= date1 && x.tod >= date1))
            {
                var absvar = new leave_absence();
                var emp = mastervar.Find(x => x.employee_no == id);
                if (emp != null)
                {
                    var date2 = date1.Value.AddDays(-1);
                    if (abslist.Exists(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                    {
                        var abs1 = abslist.Find(x => x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
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
                        absvar.month = monthstart;
                        this.db1.leave_absence.Add(absvar);
                        this.db1.SaveChanges();
                    }
                    
                }
            }

            return RedirectToAction("approveabsn", new { month = monthstart});
        }

        public ActionResult ProjectAtt(DateTime? dayfrom, DateTime? dayto, string empno)
        {
            var patt = new List<iclock_transaction>();
            if (dayfrom == null)
            {
                dayfrom = DateTime.Today;
            }

            if (dayto == null)
            {
                dayto = dayfrom.Value.AddDays(1);
            }
            else
            {
                dayto = dayto.Value.AddDays(1);
            }
            var workcode = db2.iclock_terminalworkcode.ToList();

            var mancon = new master_fileController();
            var afinallist = mancon.emplist(true);


            do
            {
                var startDate = dayfrom.Value.Date;
                var endDate = startDate.AddDays(1);


                var iclocklist = db2.iclock_transaction
                    .Where(x => x.punch_time >= startDate && x.punch_time < endDate)
                    .ToList();
                if (iclocklist.Count() < 0)
                {
                    goto end;
                }



                foreach (var file in afinallist)
                {
                    if (iclocklist.Exists(x => x.emp_code == file.employee_no.ToString()))
                    {
                        var empatt = iclocklist.FindAll(x => x.emp_code == file.employee_no.ToString())
                            .OrderBy(x => x.punch_time).ToList();
                        foreach (var transaction in empatt)
                        {
                            transaction.mobile = file.employee_name;
                            if (transaction.area_alias.IsNullOrWhiteSpace() && !transaction.work_code.IsNullOrWhiteSpace())
                            {
                                transaction.area_alias = workcode.Find(x => x.code == transaction.work_code).alias;
                            }

                        }

                       
                        patt.AddRange(empatt);
                    }
                }

                end: ;
                dayfrom = dayfrom.Value.AddDays(1);
            } while (dayfrom != dayto);

            if (!empno.IsNullOrWhiteSpace())
            {
                patt = patt.FindAll(x => x.emp_code == empno);
            }

            return View(patt.OrderBy(x => x.area_alias).ThenBy(x => x.emp_code).ThenBy(x => x.punch_time).ToList());
        }

        public ActionResult newprojectatt(DateTime? dayfrom, DateTime? dayto, string empno)
        {

            var patt = new List<iclock_transaction>();
            if (dayfrom == null)
            {
                dayfrom = DateTime.Today;
            }

            if (dayto == null)
            {
                dayto = dayfrom.Value.AddDays(1);
            }
            else
            {
                dayto = dayto.Value.AddDays(1);
            }
            var workcode = db2.iclock_terminalworkcode.ToList();

            var mancon = new master_fileController();
            var afinallist = mancon.emplist(true).FindAll(x => x.emiid == "4710");

            foreach (var file in afinallist)
            {
                do
                {
                    var startDate = dayfrom.Value.Date;
                    var endDate = startDate.AddDays(1);
                    var iclocklist = db2.iclock_transaction
                        .Where(x => x.punch_time >= startDate && x.punch_time < endDate && x.emp_code == file.emiid)
                        .ToList();
                    if (iclocklist.Count() < 0)
                    {
                        goto end;
                    }
                    


                    end:;
                    dayfrom.Value.AddDays(1);
                } while (dayfrom != dayto);
            }

            return View();
        }
        /*
        public ActionResult absreport(DateTime? dayfrom, DateTime? dayto, string empno)
        {
            if (dayfrom == null)
            {
                dayfrom = DateTime.Today;
            }

            if (dayto == null)
            {
                dayto = dayfrom.Value.AddDays(1);
            }
            else
            {
                dayto = dayto.Value.AddDays(1);
            }
            var mancon = new master_fileController();
            var afinallist = mancon.emplistatt(dayto).FindAll(x=>x.employee_no != 0 );
            if (!empno.IsNullOrWhiteSpace())
            {
                afinallist = afinallist.FindAll(x => x.emiid == empno);
            }
            var HObioatt = db1.hiks
                .Where(x => x.date >= dayfrom && x.date <= dayto)
                .ToList();
            var leaveList = db1.Leaves
                .Where(x => x.Start_leave <= dayto && x.End_leave >= dayfrom)
                .ToList();
            var holidayList = db.Holidays.Where(x => x.Date >= dayfrom && x.Date <= dayto).ToList();

            var proatt = db2.iclock_transaction
                .Where(x => x.punch_time >= dayfrom && x.punch_time <= dayto)
                .ToList();
            var abslist = new List<hik>();
                var datereset = dayfrom;

                if (!empno.IsNullOrWhiteSpace())
                {
                    int.TryParse(empno, out var tempResult);
                    afinallist = afinallist.FindAll(x => x.employee_no == tempResult);
                }
            foreach (var file in afinallist)
            {
                dayfrom = datereset;
                do
                {
                    var tempHOattlist = HObioatt
                        .Where(x => x.ID == file.employee_no.ToString() && x.date == dayfrom)
                        .ToList();
                    var tempholiday = holidayList.Where(x => x.Date == dayfrom).ToList();

                    var tempproattlist = proatt
                        .Where(x => x.emp_code == file.employee_no.ToString() && x.punch_time.Date == dayfrom)
                        .ToList();

                    var tranferlist = db1.transferlists
                        .Where(x => x.Employee_id == file.employee_id && x.reason == "approved")
                        .OrderByDescending(x => x.datemodifief).ToList();
                    var weekends = new List<DayOfWeek>();
                    if (tranferlist.Any())
                    {

                        var proweekend = new List<HRweekend>();
                        foreach (var trvar in tranferlist)
                        {
                            proweekend.AddRange(db1.HRweekends.Where(x => x.pro_id == trvar.npro_id).ToList());
                        }

                        foreach (var weekend in proweekend)
                        {
                            if (!weekends.Contains(weekend.dweek.DayOfWeek))
                            {
                                weekends.Add(weekend.dweek.DayOfWeek);
                            }
                        }
                    }
                    else
                    {
                        weekends.Add(DayOfWeek.Sunday);
                       // weekends.Add(DayOfWeek.Saturday);

                    }
                    if (!tempHOattlist.Any() && !tempproattlist.Any() && !tempholiday.Any() && !leaveList.Exists(x =>
                            x.End_leave >= dayfrom && x.Start_leave <= dayfrom &&
                            x.Employee_id == file.employee_id)  && !weekends.Contains(dayfrom.Value.DayOfWeek))
                    {
                        var absvar = new hik();
                        absvar.ID = file.employee_no.ToString();
                        absvar.Person = file.employee_name;
                        absvar.date = dayfrom;
                        abslist.Add(absvar);
                    }

                    end: ;
                    dayfrom = dayfrom.Value.AddDays(1);
                } while (dayto > dayfrom);
            }
            
            return View(abslist);
        }
        */
        public ActionResult absreport(DateTime? dayfrom, DateTime? dayto, string empno)
        {
            // ----- CONFIG -----
            // Define weekends per site
            var hoWeekend = new HashSet<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var projectWeekend = new HashSet<DayOfWeek> { DayOfWeek.Sunday };
            var defaultWeekend = hoWeekend; // fallback if we cannot infer site
            var siteLookbackDays = 14;      // how many days to look back to infer site on no-punch days

            // ----- DATE NORMALIZATION -----
            if (dayfrom == null) dayfrom = DateTime.Today;
            if (dayto == null) dayto = dayfrom.Value; // inclusive range [dayfrom..dayto]
                                                      // we iterate with a day cursor; to keep your original semantics, use endExclusive = dayto+1
            var endExclusive = dayto.Value.Date.AddDays(1);
            var startDate = dayfrom.Value.Date;

            // ----- EMPLOYEE LIST -----
            var mancon = new master_fileController();
            var afinallist = mancon.emplistatt(endExclusive).FindAll(x => x.employee_no != 0);
            var proremove = afinallist.FindAll(x =>
                x.contracts.Any() && !x.contracts.FirstOrDefault().departmant_project.IsNullOrWhiteSpace() && x.contracts.FirstOrDefault().departmant_project.ToLower() == "procurement");

            // Filter by empno, allowing either emiid or numeric employee_no
            if (!empno.IsNullOrWhiteSpace())
            {
                int empNoParsed;
                if (int.TryParse(empno, out empNoParsed))
                    afinallist = afinallist.FindAll(x => x.employee_no == empNoParsed);
                else
                    afinallist = afinallist.FindAll(x => x.emiid == empno);
            }

            // ----- DATA PULLS (limit to date window) -----
            var HObioatt = db1.hiks
                .Where(x => x.date >= startDate && x.date < endExclusive)
                .ToList();

            var leaveList = db1.Leaves
                .Where(x => x.Start_leave <= endExclusive && x.End_leave >= startDate)
                .ToList();

            var holidayList = db.Holidays
                .Where(x => x.Date >= startDate && x.Date < endExclusive)
                .ToList();

            var proatt = db2.iclock_transaction
                .Where(x => x.punch_time >= startDate && x.punch_time < endExclusive)
                .ToList();

            // ----- INDEX FOR FAST LOOKUPS -----
            // Normalize keys: use employee_no string for both sources; dates as Date (no time)
            Func<DateTime, DateTime> asDate = d => d.Date;

            var hoByEmpDate = HObioatt
                .GroupBy(x => new { Emp = x.ID, Day = asDate(x.date.Value) })
                .ToDictionary(g => g.Key, g => true);

            var projByEmpDate = proatt
                .GroupBy(x => new { Emp = x.emp_code, Day = asDate(x.punch_time) })
                .ToDictionary(g => g.Key, g => true);

            // Also keep per-employee recent site history (for lookback inference)
            // Build a map: employee -> list of (date, site)
            var recentSiteByEmp = new Dictionary<string, SortedSet<DateTime>>();

            // Optional: precompute last seen site per day for each employee
            // We will not store the site type here; we’ll check both dicts when needed.

            // Helper to check presence quickly
            bool HasHOPunch(string empKey, DateTime day)
            {
                var testval = false;
                var emktem = empKey;

                if (empKey.Contains("777") && empKey.Length > 3)
                {
                    var tempemp = empKey.Substring(0, 3);
                    var tempemp2 = empKey.Remove(0, 3);
                    if (tempemp2.Length == 5)
                    {
                        tempemp2 = tempemp2.Remove(0, 1);
                    }

                    emktem = tempemp + tempemp2;
                    if (hoByEmpDate.ContainsKey(new { Emp = emktem, Day = day }) ||
                        hoByEmpDate.ContainsKey(new { Emp = empKey, Day = day }))
                    {
                        testval = true;
                    }
                }
                else
                {
                    testval = hoByEmpDate.ContainsKey(new { Emp = empKey, Day = day });
                }

                return testval;
            }

            bool HasProjPunch(string empKey, DateTime day)
            {
                var testval = false;
                var emktem = empKey;

                if (empKey.Contains("777") && empKey.Length > 3)
                {
                    var tempemp = empKey.Substring(0, 3);
                    var tempemp2 = empKey.Remove(0, 3);
                    if (tempemp2.Length == 5)
                    {
                        tempemp2 = tempemp2.Remove(0, 1);
                    }

                    emktem = tempemp + tempemp2;
                    if (projByEmpDate.ContainsKey(new { Emp = emktem, Day = day }) ||
                        projByEmpDate.ContainsKey(new { Emp = empKey, Day = day }))
                    {
                        testval = true;
                    }

                }
                else
                {
                    testval = projByEmpDate.ContainsKey(new { Emp = empKey, Day = day });
                }
                return testval;

            }

            // Infer which weekend applies for (employee, day)
            HashSet<DayOfWeek> WeekendFor(string empKey, DateTime day)
            {
                // 1) Direct inference from same-day punches
                bool ho = HasHOPunch(empKey, day);
                bool pr = HasProjPunch(empKey, day);
                if (ho && !pr) return hoWeekend;
                if (pr && !ho) return projectWeekend;
                if (ho && pr)
                {
                    // If both exist the same day, prefer "working" as true day (rare). Choose any; or treat as working day (no weekend).
                    // Here, pick the stricter weekend (union) would mark fewer working days as weekends. We’ll prefer none (i.e., not weekend).
                    return new HashSet<DayOfWeek>(); // no weekend -> treat as working day
                }

                // 2) Look back up to N days for last seen site
                for (int i = 1; i <= siteLookbackDays; i++)
                {
                    var d = day.AddDays(-i);
                    bool hoBack = HasHOPunch(empKey, d);
                    bool prBack = HasProjPunch(empKey, d);
                    if (hoBack && !prBack) return hoWeekend;
                    if (prBack && !hoBack) return projectWeekend;
                    if (hoBack && prBack) return new HashSet<DayOfWeek>(); // ambiguous, treat as working day
                }

                // 3) Fallback
                return defaultWeekend;
            }

            // ----- MAIN -----
            var abslist = new List<hik>();

            foreach (var file in afinallist)
            {
                var empKey = file.employee_no.ToString();
                var cursor = startDate;
                if (proremove.Exists(x=>x.employee_id == file.employee_id))
                {
                    continue;
                }
                while (cursor < endExclusive)
                {
                    // Skip holidays (common list). If you have site-specific holidays, split lists and choose by site here.
                    bool isHoliday = holidayList.Any(h => h.Date.Value.Date == cursor);

                    // Determine weekend for this employee on this day (based on inferred site)
                    var weekend = WeekendFor(empKey, cursor);
                    bool isWeekend = weekend.Contains(cursor.DayOfWeek);

                    if (!isHoliday && !isWeekend)
                    {
                        // Get punches for that day
                        bool hasHO = HasHOPunch(empKey, cursor);
                        bool hasProj = HasProjPunch(empKey, cursor);

                        // On-leave test (inclusive)
                        bool onLeave = leaveList.Exists(x =>
                            x.Employee_id == file.employee_id &&
                            x.Start_leave.Value.Date <= cursor &&
                            x.End_leave.Value.Date >= cursor);

                        if (!hasHO && !hasProj && !onLeave)
                        {
                            // Mark absent
                            var absvar = new hik
                            {
                                ID = file.emiid,
                                Person = file.employee_name,
                                date = cursor
                            };
                            abslist.Add(absvar);
                        }
                    }

                    cursor = cursor.AddDays(1);
                }
            }
            
            return View(abslist.OrderBy(x=>{
                // Try to parse numeric part
                if (int.TryParse(x.ID, out var num))
                    return (0, num); // group 0 = plain numbers
                else if (x.ID.StartsWith("G-") && int.TryParse(x.ID.Substring(2), out var gnum))
                    return (1, gnum); // group 1 = G-numbers
                else
                    return (2, int.MaxValue); // group 2 = anything else
            }).ToList());
        }

        public ActionResult mobileappatt()
        {
            var attlist = db2.iclock_transaction.Where(x => x.terminal_sn.ToUpper() == "APP").ToList();
            var workcode = db2.iclock_terminalworkcode.ToList();
            ViewBag.worktopro = workcode;
            return View(attlist);
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