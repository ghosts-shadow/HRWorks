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

            var alist = this.db1.master_file
                .Where(e => e.last_working_day == null)
                .OrderBy(e => e.employee_no)
                .ThenByDescending(x => x.date_changed)
                .ToList();
            var afinallist = alist
                .GroupBy(x => x.employee_no)
                .Select(g => g.First())
                .Where(x => x.employee_no != 0 && x.employee_no != 1 && x.employee_no != 100001)
                .ToList();


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
                        var firstin = empatt.First();
                        firstin.mobile = file.employee_name;
                        if (empatt.Count() > 1)
                        {
                            var lastout = empatt.Last();
                            lastout.punch_state = "1";
                            lastout.mobile = file.employee_name;
                            patt.Add(lastout);

                        }

                        patt.Add(firstin);
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

        public ActionResult absreport(DateTime? dayfrom, DateTime? dayto, string empno)
        {
            var gradelist = new List<string>
            {
                "4C","4B","4A","5B","5A","6B","6A","7B","7A","8B","8A","9"
            };
            
            var alist = this.db1.master_file
                .Where(e => e.last_working_day == null)
                .OrderBy(e => e.employee_no)
                .ThenByDescending(x => x.date_changed)
                .ToList().FindAll(x => x.contracts.Count > 0 && (x.contracts.First().departmant_project != "Procurement" && gradelist.Exists(y=>y == x.contracts.First().grade))); ;
            var afinallist = alist
                .GroupBy(x => x.employee_no)
                .Select(g => g.First())
                .Where(x => x.employee_no != 0 && x.employee_no != 1 && x.employee_no != 100001)
                .ToList();
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
                        weekends.Add(DayOfWeek.Saturday);

                    }
                    if (!tempHOattlist.Any() && !tempproattlist.Any() && !leaveList.Exists(x =>
                            x.End_leave >= dayfrom && x.Start_leave <= dayfrom &&
                            x.Employee_id == file.employee_id) && !weekends.Contains(dayfrom.Value.DayOfWeek))
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