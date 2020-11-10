namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;
    using System.Data;
    using HRworks.Models;

    using Microsoft.Ajax.Utilities;

    using OfficeOpenXml;

    using PagedList;

    using TEST2.Controllers;

    public class LeavesController : Controller
    {
        private readonly HREntities db = new HREntities();

        public void cal_bal()
        {
            var emp_list = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var emp_listfinal = new List<master_file>();
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            emp_listfinal = afinallist;

            /*
              if (check.Count != 0)
                        {
                            this.db.Entry(at).State = EntityState.Modified;
                            this.db.SaveChanges();
                        }
                        else
                        {
                            this.db.Attendances.Add(at);
                            this.db.SaveChanges();
                        }
             */
            var eddate = DateTime.Now.Date;
            foreach (var empjd in emp_listfinal)
            {
                var leaveballist = this.db.leavecals.ToList();
                var leavebal = new leavecal();
                var Employee_id = empjd.employee_id;
                if (Employee_id != null && eddate != null)
                {
                    double unpaid = 0;
                    double netperiod = 0;
                    double accrued = 0;
                    double avalied = 0;
                    double lbal = 0;
                    var asf = empjd.date_joined;
                    var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                        x => x.Employee_id == Employee_id && x.Start_leave >= asf && x.End_leave <= eddate);
                    var times = eddate - asf;
                    if (times != null)
                    {
                        var period = times.Value.TotalDays + 1;
                        var ump = leaves.ToList();
                        foreach (var leaf in ump)
                        {
                            if (leaf.leave_type == "6")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) unpaid += leaf.days.Value - 0.5;
                                    else
                                        unpaid += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) unpaid += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) unpaid += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            double sick = 0;
                            if (leaf.leave_type == "2")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) sick += leaf.days.Value - 0.5;
                                    else
                                        sick += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) sick += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) sick += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            this.ViewBag.sick = sick;

                            double comp = 0;
                            if (leaf.leave_type == "3")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) comp += leaf.days.Value - 0.5;
                                    else comp += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) comp += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) comp += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            this.ViewBag.comp = comp;

                            double mate = 0;
                            if (leaf.leave_type == "4")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) mate += leaf.days.Value - 0.5;
                                    else mate += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) mate += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) mate += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            this.ViewBag.mate = mate;
                            double haj = 0;
                            if (leaf.leave_type == "5")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) haj += leaf.days.Value - 0.5;
                                    else haj += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) haj += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) haj += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            this.ViewBag.haj = haj;

                            if (leaf.leave_type == "1")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) avalied += leaf.days.Value - 0.5;
                                    else avalied += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) avalied += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) avalied += times.Value.TotalDays + 1;
                                    }
                                }
                            }
                        }

                        netperiod = period - unpaid;
                        accrued = Math.Round(netperiod * 30 / 360);
                        lbal = accrued - avalied;
                        leavebal.leave_balance = lbal;
                        leavebal.period = period;
                        leavebal.anual_leave_taken = avalied;
                        leavebal.net_period = netperiod;
                        leavebal.unpaid_leave = unpaid;
                        leavebal.accrued = accrued;
                        leavebal.Employee_id = empjd.employee_id;
                        if (leaveballist.Exists(x => x.Employee_id == leavebal.Employee_id))
                        {
                            var check = leaveballist.Find(x => x.Employee_id == empjd.employee_id);
                            leavebal = check;
                            lbal = accrued - avalied;
                            leavebal.leave_balance = lbal;
                            leavebal.period = period;
                            leavebal.anual_leave_taken = avalied;
                            leavebal.net_period = netperiod;
                            leavebal.unpaid_leave = unpaid;
                            leavebal.accrued = accrued;
                            if (check != leavebal)
                            {
                                this.db.Entry(leavebal).State = EntityState.Modified;
                                this.db.SaveChanges();
                            }
                        }
                        else
                        {
                            this.db.leavecals.Add(leavebal);
                            this.db.SaveChanges();
                        }
                    }
                }
            }
        }

        public void cal_bal(DateTime eddate)
        {
            var emp_listfinal = new List<master_file>();
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            emp_listfinal = afinallist;

            /*
              if (check.Count != 0)
                        {
                            this.db.Entry(at).State = EntityState.Modified;
                            this.db.SaveChanges();
                        }
                        else
                        {
                            this.db.Attendances.Add(at);
                            this.db.SaveChanges();
                        }
             */
            foreach (var empjd in emp_listfinal)
            {
                var leaveballist = this.db.leavecals.ToList();
                var leavebal = new leavecal();
                var Employee_id = empjd.employee_id;
                if (Employee_id != null && eddate != null)
                {
                    double unpaid = 0;
                    double netperiod = 0;
                    double accrued = 0;
                    double avalied = 0;
                    double lbal = 0;
                    var asf = empjd.date_joined;
                    var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                        x => x.Employee_id == Employee_id && x.Start_leave >= asf && x.End_leave <= eddate);
                    var times = eddate - asf;
                    if (times != null)
                    {
                        var period = times.Value.TotalDays + 1;
                        var ump = leaves.ToList();
                        foreach (var leaf in ump)
                        {
                            if (leaf.leave_type == "6")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) unpaid += leaf.days.Value - 0.5;
                                    else
                                        unpaid += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) unpaid += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) unpaid += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            double sick = 0;
                            if (leaf.leave_type == "2")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) sick += leaf.days.Value - 0.5;
                                    else
                                        sick += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) sick += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) sick += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            this.ViewBag.sick = sick;

                            double comp = 0;
                            if (leaf.leave_type == "3")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) comp += leaf.days.Value - 0.5;
                                    else comp += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) comp += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) comp += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            this.ViewBag.comp = comp;

                            double mate = 0;
                            if (leaf.leave_type == "4")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) mate += leaf.days.Value - 0.5;
                                    else mate += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) mate += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) mate += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            this.ViewBag.mate = mate;
                            double haj = 0;
                            if (leaf.leave_type == "5")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) haj += leaf.days.Value - 0.5;
                                    else haj += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) haj += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) haj += times.Value.TotalDays + 1;
                                    }
                                }
                            }

                            this.ViewBag.haj = haj;

                            if (leaf.leave_type == "1")
                            {
                                if (leaf.days != null)
                                {
                                    if (leaf.half) avalied += leaf.days.Value - 0.5;
                                    else avalied += leaf.days.Value;
                                }
                                else
                                {
                                    if (leaf.half)
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) avalied += times.Value.TotalDays + 1 - 0.5;
                                    }
                                    else
                                    {
                                        times = leaf.End_leave - leaf.Start_leave;
                                        if (times != null) avalied += times.Value.TotalDays + 1;
                                    }
                                }
                            }
                        }

                        netperiod = period - unpaid;
                        accrued = Math.Round(netperiod * 30 / 360);
                        lbal = accrued - avalied;
                        leavebal.leave_balance = lbal;
                        leavebal.period = period;
                        leavebal.anual_leave_taken = avalied;
                        leavebal.net_period = netperiod;
                        leavebal.unpaid_leave = unpaid;
                        leavebal.accrued = accrued;
                        leavebal.Employee_id = empjd.employee_id;
                        if (leaveballist.Exists(x => x.Employee_id == leavebal.Employee_id))
                        {
                            var check = leaveballist.Find(x => x.Employee_id == empjd.employee_id);
                            leavebal = check;
                            lbal = accrued - avalied;
                            leavebal.leave_balance = lbal;
                            leavebal.period = period;
                            leavebal.anual_leave_taken = avalied;
                            leavebal.net_period = netperiod;
                            leavebal.unpaid_leave = unpaid;
                            leavebal.accrued = accrued;
                            if (check != leavebal)
                            {
                                this.db.Entry(leavebal).State = EntityState.Modified;
                                this.db.SaveChanges();
                            }
                        }
                        else
                        {
                            this.db.leavecals.Add(leavebal);
                            this.db.SaveChanges();
                        }
                    }
                }
            }
        }

        // GET: Leaves/Create               
        public ActionResult Create()
        {
            var listItems = new List<ListItem>
                                {
                                    new ListItem { Text = "Annual", Value = "1" },
                                    new ListItem { Text = "Sick", Value = "2" },
                                    new ListItem { Text = "Compassionate", Value = "3" },
                                    new ListItem { Text = "Maternity", Value = "4" },
                                    new ListItem { Text = "Haj", Value = "5" },
                                    new ListItem { Text = "Unpaid", Value = "6" }
                                };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_id = new SelectList(
                afinallist.OrderBy(x => x.employee_no),
                "employee_id",
                "employee_no");
            ViewBag.employee_id1 = new SelectList(
                afinallist.OrderBy(e => e.employee_name),
                "employee_id",
                "employee_name");
            return this.View();
        }

        // POST: Leaves/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                Include =
                    "Employee_id,Id,Date,Reference,Start_leave,End_leave,Return_leave,days,leave_type,actual_return_date,half")]
            Leave leave,
            HttpPostedFileBase fileBase)
        {

            var listItems = new List<ListItem>
                                {
                                    new ListItem { Text = "Annual", Value = "1" },
                                    new ListItem { Text = "Sick", Value = "2" },
                                    new ListItem { Text = "Compassionate", Value = "3" },
                                    new ListItem { Text = "Maternity", Value = "4" },
                                    new ListItem { Text = "Haj", Value = "5" },
                                    new ListItem { Text = "Unpaid", Value = "6" }
                                };
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_id = new SelectList(
                afinallist.OrderBy(x => x.employee_no),
                "employee_id",
                "employee_no");
            ViewBag.employee_id1 = new SelectList(
                afinallist.OrderBy(e => e.employee_name),
                "employee_id",
                "employee_name");
            var leaveid = afinallist.Find(x => x.employee_id == leave.Employee_id);
            var jd = leaveid.date_joined;
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            string serverfile;
            var leavelistc = this.db.Leaves.ToList();/*
            if (leavelistc.Exists(x=>x.Start_leave<=leave.Start_leave && x.End_leave >= leave.End_leave && x.Employee_id == leave.Employee_id))
            {
                ModelState.AddModelError("Start_leave", "already exists");

                goto jderr;
            }
            if (leavelistc.Exists(x=>x.End_leave >= leave.Start_leave && x.Employee_id == leave.Employee_id))
            {
                ModelState.AddModelError("Start_leave", leave.Start_leave+" already exists");

                goto jderr;
            }
            if (leave.Start_leave < jd && leave.Start_leave != default)
            {
                ModelState.AddModelError("Start_leave", "selected date is before " +jd);
                goto jderr;
            }*/
            if (leave.End_leave < jd && leave.End_leave != default)
            {
                ModelState.AddModelError("Start_leave", "selected date is before " + jd);
                goto jderr;
            }
            if (leave.End_leave < leave.Start_leave)
            {
                ModelState.AddModelError("Start_leave", "cannot add date in reverse");
                goto jderr;
            }
            if (fileBase != null)
            {
                var i = 0;
                var imgname = Path.GetFileName(fileBase.FileName);
                var fileexe = Path.GetExtension(fileBase.FileName);
                var filepath = new DirectoryInfo("D:/HR/leave/");
                serverfile = "D:/HR/leave/" + leave.Employee_id; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/leave/" + leave.Employee_id + "/" + leave.Employee_id + "_" + i + fileexe;
                    i++;
                }
                while (System.IO.File.Exists(
                    serverfile = "D:/HR/leave/" + leave.Employee_id + "/" + leave.Employee_id + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }

            var leavelist = this.db.Leaves.ToList();
            if (leavelist.Exists(
                x => x.Employee_id == leave.Employee_id && x.Start_leave == leave.Start_leave
                                                        && x.End_leave == leave.End_leave))
            {
                ViewBag.exhist = "the entry already exists";
                return this.View(leave);

            }

            ViewBag.exhist = "";
            if (this.ModelState.IsValid)
            {
                var file1 = new Leave();
                file1.Employee_id = leave.Employee_id;
                var masterstatus = this.db.master_file.Find(leave.Employee_id);
                file1.Date = leave.Date;
                file1.Reference = serverfile;
                file1.half = leave.half;
                file1.Start_leave = leave.Start_leave;
                file1.End_leave = leave.End_leave;
                file1.Return_leave = leave.Return_leave;
                file1.leave_type = leave.leave_type;
                file1.days = leave.days;
                file1.data_o_n = "New";
                file1.actual_return_date = leave.actual_return_date;
                if (leave.actual_return_date == null) masterstatus.status = "on leave";
                else masterstatus.status = "active";
                this.db.Leaves.Add(file1);
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }
            jderr: ;
            return this.View(leave);
        }

        // GET: Leaves/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var leave = this.db.Leaves.Find(id);
            if (leave == null) return this.HttpNotFound();
            return this.View(leave);
        }

        // POST: Leaves/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var leave = this.db.Leaves.Find(id);
            var masterstatus = this.db.master_file.Find(leave.Employee_id);
            masterstatus.status = "active";
            this.db.Leaves.Remove(leave);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        // GET: Leaves/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var leave = this.db.Leaves.Find(id);
            if (leave == null) return this.HttpNotFound();
            return this.View(leave);
        }

        public void DownloadExcel1(int? days, DateTime? leave_bal_till)
        {
            List<Leave> passexel = new List<Leave>();
            var leaves = new List<Leave>();
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            if (days.HasValue)
            {
                this.cal_bal();
                var leaveballist = this.db.leavecals.Where(x => x.leave_balance >= days.Value).ToList();
                foreach (var leavecal in leaveballist)
                {
                    var leaveempid = this.db.Leaves.Where(x => x.Employee_id == leavecal.Employee_id)
                        .Include(l => l.master_file).OrderByDescending(x => x.Id).ToList();
                    foreach (var leaf in leaveempid)
                    {
                        leaf.leave_bal = leavecal.leave_balance;
                        if (!leaves.Exists(x => x.Employee_id == leaf.Employee_id)) leaves.Add(leaf);
                    }
                }

                foreach (var emp in afinallist)
                {
                    if (emp.date_joined != null)
                    {
                        var leaf = new Leave();
                        if (leaveballist.Exists(x => x.Employee_id == emp.employee_id))
                        {
                        var lb = leaveballist.Find(x => x.Employee_id == emp.employee_id).leave_balance;
                        leaf.leave_bal = lb;
                        leaf.Employee_id = emp.employee_id;
                        leaf.master_file = emp;
                        if (!leaves.Exists(x => x.Employee_id == leaf.Employee_id)) leaves.Add(leaf);
                            
                        }
                    }
                }
                passexel = leaves;
            }

            if (leave_bal_till.HasValue)
            {
                var leaveslist = new List<Leave>();
                var leaveballist = this.db.leavecals.ToList();
                leaves = this.db.Leaves.ToList();
                this.cal_bal(leave_bal_till.Value);
                foreach (var leaf in leaves)
                {
                    var lb = leaveballist.Find(x => x.Employee_id == leaf.Employee_id).leave_balance;
                    leaf.leave_bal = lb;
                    if (!leaveslist.Exists(x => x.Employee_id == leaf.Employee_id)) leaveslist.Add(leaf);
                }

                foreach (var emp in afinallist)
                {
                    if (emp.date_joined != null)
                    {
                        var leaf = new Leave();
                        var lb = leaveballist.Find(x => x.Employee_id == emp.employee_id).leave_balance;
                        leaf.leave_bal = lb;
                        leaf.Employee_id = emp.employee_id;
                        leaf.master_file = emp;
                        if (!leaveslist.Exists(x => x.Employee_id == leaf.Employee_id)) leaveslist.Add(leaf);
                    }
                }
                passexel = leaveslist;
            }
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("leaves_report".ToUpper());
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "leave bal ";
            var row = 2;
            foreach (var item in passexel.OrderBy(x => x.leave_bal))
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.leave_bal;
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename =leaves_report.xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
        }
        public void DownloadExcel(long? Employee_id, DateTime? eddate)
        {
            double unpaid = 0;
            double netperiod = 0;
            double accrued = 0;
            double avalied = 0;
            double lbal = 0;
            double period = 0;
            List<Leave> passexel = new List<Leave>();
            var emp_list = this.db.master_file.ToList();
            var emp_listfinal = new List<master_file>();
            foreach (var empid in emp_list)
            {
                var emp_unfilted = emp_list.FindAll(x => x.employee_no == empid.employee_no)
                    .OrderBy(x => x.employee_id);
                if (emp_unfilted != null) emp_listfinal.Add(emp_unfilted.First());
            }

            if (Employee_id == null && eddate == null)
            {
                passexel = this.db.Leaves.ToList();
                goto all;
            }

            if (Employee_id != null && eddate != null)
            {
                var empjd = emp_listfinal.Find(x => x.employee_no == Employee_id);
                Employee_id = empjd.employee_id;
                var asf = empjd.date_joined;
                var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                    x => x.Employee_id == Employee_id && x.Start_leave >= asf && x.End_leave <= eddate);
                var times = eddate - asf;
                double haj = 0;
                double mate = 0;
                double comp = 0;
                double sick = 0;
                if (times != null)
                {
                    period = times.Value.TotalDays + 1;
                    var ump = leaves.ToList();
                    foreach (var leaf in ump)
                    {
                        if (leaf.leave_type == "6")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) unpaid += leaf.days.Value - 0.5;
                                else
                                    unpaid += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) unpaid += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) unpaid += times.Value.TotalDays + 1;
                                }
                            }
                        }

                        if (leaf.leave_type == "2")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) sick += leaf.days.Value - 0.5;
                                else
                                    sick += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) sick += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) sick += times.Value.TotalDays + 1;
                                }
                            }
                        }

                        if (leaf.leave_type == "3")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) comp += leaf.days.Value - 0.5;
                                else comp += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) comp += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) comp += times.Value.TotalDays + 1;
                                }
                            }
                        }

                        if (leaf.leave_type == "4")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) mate += leaf.days.Value - 0.5;
                                else mate += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) mate += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) mate += times.Value.TotalDays + 1;
                                }
                            }
                        }

                        if (leaf.leave_type == "5")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) haj += leaf.days.Value - 0.5;
                                else haj += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) haj += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) haj += times.Value.TotalDays + 1;
                                }
                            }
                        }

                        if (leaf.leave_type == "1")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) avalied += leaf.days.Value - 0.5;
                                else avalied += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) avalied += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) avalied += times.Value.TotalDays + 1;
                                }
                            }
                        }
                    }

                    netperiod = period - unpaid;
                    accrued = Math.Round(netperiod * 30 / 360);
                    lbal = accrued - avalied;
                    passexel = ump;
                }
            }
            all: var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("leaves_report".ToUpper());
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "from ";
            Sheet.Cells["D1"].Value = "to";
            Sheet.Cells["E1"].Value = "actual return date ";
            Sheet.Cells["F1"].Value = "leave type ";
            Sheet.Cells["G1"].Value = "is half-day included";
            var row = 2;
            foreach (var item in passexel)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Start_leave;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.End_leave;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.actual_return_date;
                if(item.leave_type == "1")
                {
                    Sheet.Cells[string.Format("F{0}", row)].Value =" Annual";
                }
                if(item.leave_type == "2")
                {
                    Sheet.Cells[string.Format("F{0}", row)].Value =" Sick";
                }
                if(item.leave_type == "3")
                {
                    Sheet.Cells[string.Format("F{0}", row)].Value =" Compassionate";
                }
                if(item.leave_type == "4")
                {
                    Sheet.Cells[string.Format("F{0}", row)].Value =" Maternity";
                }
                if(item.leave_type == "5")
                {
                    Sheet.Cells[string.Format("F{0}", row)].Value =" Haj";
                }
                if(item.leave_type == "6")
                {
                    Sheet.Cells[string.Format("F{0}", row)].Value =" Unpaid";
                }
                if(item.leave_type == "7")
                {
                    Sheet.Cells[string.Format("F{0}", row)].Value =" others";
                }
                if(item.leave_type == "8")
                {
                    Sheet.Cells[string.Format("F{0}", row)].Value =" half - day";
                }
                if (item.half)
                {
                    Sheet.Cells[string.Format("G{0}", row)].Value = "yes";
                }
                else
                {
                    Sheet.Cells[string.Format("G{0}", row)].Value = "no";
                }
                row++;
            }

            row += 5; 
            Sheet.Cells[string.Format("A{0}", row)].Value = "Period ="+ period ;
            Sheet.Cells[string.Format("A{0}", row +1)].Value = "Netperiod ="+ netperiod ;
            Sheet.Cells[string.Format("A{0}", row +2)].Value = "Unpaid ="+ unpaid ;
            Sheet.Cells[string.Format("A{0}", row +3)].Value = "Accrued ="+ accrued ;
            Sheet.Cells[string.Format("A{0}", row +4)].Value = "annual leave taken =" + avalied ;
            Sheet.Cells[string.Format("A{0}", row +5)].Value = "leave balance =" + lbal ;
            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename = leaves_report_summery_till_" + eddate.ToString() + ".xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
        }

        // GET: Leaves/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var leave = this.db.Leaves.Find(id);
            if (leave == null) return this.HttpNotFound();
            var listItems = new List<ListItem>
                                {
                                    new ListItem { Text = "Annual", Value = "1" },
                                    new ListItem { Text = "Sick", Value = "2" },
                                    new ListItem { Text = "Compassionate", Value = "3" },
                                    new ListItem { Text = "Maternity", Value = "4" },
                                    new ListItem { Text = "Haj", Value = "5" },
                                    new ListItem { Text = "Unpaid", Value = "6" },
                                    new ListItem { Text = "Other", Value = "7" }
                                };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text",leave.leave_type);
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            ViewBag.leavety = leave.leave_type;
            return this.View(leave);
        }

        // POST: Leaves/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(
                Include =
                    "Employee_id,Id,Date,Reference,Start_leave,End_leave,Return_leave,leave_type,actual_return_date")]
            Leave leave,
            HttpPostedFileBase fileBase)
        {
            var listItems = new List<ListItem>
                                {
                                    new ListItem { Text = "Annual", Value = "1" },
                                    new ListItem { Text = "Sick", Value = "2" },
                                    new ListItem { Text = "Compassionate", Value = "3" },
                                    new ListItem { Text = "Maternity", Value = "4" },
                                    new ListItem { Text = "Haj", Value = "5" },
                                    new ListItem { Text = "Unpaid", Value = "6" },
                                    new ListItem { Text = "others", Value = "7" }
                                };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            string serverfile;
            if (fileBase != null)
            {
                var i = 0;
                var imgname = Path.GetFileName(fileBase.FileName);
                var fileexe = Path.GetExtension(fileBase.FileName);
                var filepath = new DirectoryInfo("D:/HR/leave/");
                serverfile = "D:/HR/leave/" + leave.Employee_id; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/leave/" + leave.Employee_id + "/" + leave.Employee_id + "_" + i + fileexe;
                    i++;
                }
                while (System.IO.File.Exists(
                    serverfile = "D:/HR/leave/" + leave.Employee_id + "/" + leave.Employee_id + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }

            if (this.ModelState.IsValid)
            {
                var file1 = new Leave();
                file1.Employee_id = leave.Employee_id;
                var masterstatus = this.db.master_file.Find(leave.Employee_id);
                file1.Date = leave.Date;
                file1.Reference = serverfile;
                file1.Start_leave = leave.Start_leave;
                file1.End_leave = leave.End_leave;
                file1.Return_leave = leave.Return_leave;
                file1.leave_type = leave.leave_type;
                file1.actual_return_date = leave.actual_return_date;
                if (leave.actual_return_date == null) masterstatus.status = "on leave";
                else masterstatus.status = "active";
                this.db.Entry(leave).State = EntityState.Modified;
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_no");
            return this.View(leave);
        }

        // GET: Leaves
        public ActionResult Index()
        {
               var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).ToList();
            return this.View(leaves);
        }

        public ActionResult getallorone(string search, int? page, int? pagesize, int? idsearch)
        {
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize = 100;
            List<Leave> leaves = new List<Leave>();
            leaves = this.db.Leaves.Include(l => l.master_file).OrderBy(x => x.master_file.employee_no)
                .ThenByDescending(x => x.Start_leave)
                .ThenByDescending(x => x.master_file.employee_no).ToList();
            if (pagesize != 0)
            {
                int a = 100;
                if (pagesize != null)
                {
                    if (pagesize != 0)
                    {
                        a = (int)pagesize;
                    }
                }

                defaSize = a;
            }
            ViewBag.search = search;
            ViewBag.pagesize = defaSize;
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out var idk))
                {
                    leaves = db.Leaves
                        .Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenByDescending(x => x.Start_leave).ToList();
                }
                else
                {
                    leaves = db.Leaves
                        .Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenByDescending(x => x.Start_leave).ToList();
                }
            }
            
            return this.View(leaves.OrderBy(x => x.master_file.employee_no).ThenByDescending(x => x.Start_leave).ToPagedList(page ?? 1, defaSize));
        }

        public ActionResult leave_absence_Index(DateTime? eddate)
        {
            var leaves = new List<leave_absence>();
            if (eddate != null)
            {
                leaves = this.db.leave_absence.Where(x => x.month.Value.Month == eddate.Value.Month && x.month.Value.Year == eddate.Value.Year).OrderByDescending(x => x.Id).ToList();
            }

            return this.View(leaves.OrderBy(x=>x.master_file.employee_no));
        }

        public string humanize(double days)
        {
                // The string we're working with to create the representation
                string str = "";
            // Map lengths of `diff` to different time periods
            var values = new List<Tuple<string, double>>();
            values.Add(new Tuple<string, double>("year",365));
            values.Add(new Tuple<string, double>("month",30));
            values.Add(new Tuple<string, double>("day",1));
            // Iterate over the values...
            for (var i = 0; i < values.Count; i++)
                {
                    var amount = Math.Floor(days / values[i].Item2);
                    // ... and find the largest time value that fits into the diff
                    if (amount >= 1)
                    {
                        // If we match, add to the string ('s' is for pluralization)
                        str += amount +" "+ values[i].Item1 + (amount > 1 ? "s" : " ") + " ";

                        // and subtract from the diff
                        days -= amount * values[i].Item2;
                    }
                }

            if (string.IsNullOrEmpty(str))
            {
                str = "0 days";
            }
            return str;
        }

        public ActionResult report(long? Employee_id, DateTime? eddate)
        {
            eddate = new DateTime(2020,12,31);
            ViewBag.Employee_id = Employee_id;
            ViewBag.eddate = eddate;
            var emp_listfinal = new List<master_file>();
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            emp_listfinal = afinallist;
            var empjd = afinallist.Find(x => x.employee_id == Employee_id);
            this.ViewBag.employee_id = new SelectList(
                afinallist,
                "employee_id",
                "employee_no");

            if (Employee_id != null && eddate != null)
            {
                double unpaid = 0;
                double netperiod = 0;
                double accrued = 0;
                double avalied = 0;
                double lbal = 0;
                double sick = 0;
                double comp = 0;
                double mate = 0;
                double haj = 0;
                var asf = empjd.date_joined;
                var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                    x => x.Employee_id == Employee_id && x.Start_leave >= asf && x.End_leave <= eddate);
                var times = eddate - asf;
                if (times != null)
                {
                    var period = times.Value.TotalDays + 1;
                    var ump = leaves.ToList();
                    foreach (var leaf in ump)
                    {
                        if (leaf.leave_type == "6")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) unpaid += leaf.days.Value - 0.5;
                                else
                                    unpaid += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) unpaid += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) unpaid += times.Value.TotalDays + 1;
                                }
                            }
                        }

                        if (leaf.leave_type == "2")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) sick += leaf.days.Value - 0.5;
                                else
                                    sick += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) sick += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) sick += times.Value.TotalDays + 1;
                                }
                            }
                        }

                        if (leaf.leave_type == "3")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) comp += leaf.days.Value - 0.5;
                                else comp += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) comp += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) comp += times.Value.TotalDays + 1;
                                }
                            }
                        }

                        if (leaf.leave_type == "4")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) mate += leaf.days.Value - 0.5;
                                else mate += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) mate += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) mate += times.Value.TotalDays + 1;
                                }
                            }
                        }

                        if (leaf.leave_type == "5")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) haj += leaf.days.Value - 0.5;
                                else haj += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) haj += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) haj += times.Value.TotalDays + 1;
                                }
                            }
                        }


                        if (leaf.leave_type == "1")
                        {
                            if (leaf.days != null)
                            {
                                if (leaf.half) avalied += leaf.days.Value - 0.5;
                                else avalied += leaf.days.Value;
                            }
                            else
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) avalied += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) avalied += times.Value.TotalDays + 1;
                                }
                            }
                        }
                    }

                    netperiod = period - unpaid;
                    accrued = Math.Round(netperiod * 30 / 360);
                    lbal = accrued - avalied;
                    this.ViewBag.mate = mate;
                    this.ViewBag.haj = haj;
                    this.ViewBag.sick = sick;
                    this.ViewBag.comp = comp;
                    this.ViewBag.lbal = lbal;
                    this.ViewBag.per = period;
                    this.ViewBag.aval = avalied;
                    this.ViewBag.netp = netperiod;
                    this.ViewBag.ump = unpaid;
                    this.ViewBag.accr = accrued;
                    this.ViewBag.name = empjd.employee_name;
                    this.ViewBag.no = empjd.employee_no;

                    return this.View(leaves.OrderBy(x=>x.Start_leave).ToList());
                }
            }

            // if (Employee_id == null  && eddate != null)
            // {
            // var leaves = db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(x => x.Start_leave >= empjd.date_joined && x.End_leave <= eddate);
            // return View(leaves.ToList());
            // }
            var leave = new List<Leave>();
            return this.View(leave);
        }

        public ActionResult reportserch(int? days, DateTime? leave_bal_till)
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.days = days;
            ViewBag.leave_bal_till = leave_bal_till; 
            var leaves = new List<Leave>();
            if (days.HasValue)
            {
                this.cal_bal();
                var leaveballist = this.db.leavecals.Where(x => x.leave_balance >= days.Value).ToList();
                foreach (var leavecal in leaveballist)
                {
                    var leaveempid = this.db.Leaves.Where(x => x.Employee_id == leavecal.Employee_id)
                        .Include(l => l.master_file).OrderByDescending(x => x.leave_bal).ToList();
                    foreach (var leaf in leaveempid)
                    {
                        leaf.leave_bal = leavecal.leave_balance;
                        if (!leaves.Exists(x => x.Employee_id == leaf.Employee_id)) leaves.Add(leaf);
                    }
                }


                foreach (var emp in afinallist)
                {
                    if (emp.date_joined != null)
                    {
                        var leaf = new Leave();
                        if (leaveballist.Exists(x => x.Employee_id == emp.employee_id))
                        {
                        var lb = leaveballist.Find(x => x.Employee_id == emp.employee_id).leave_balance;
                        leaf.leave_bal = lb;
                        leaf.Employee_id = emp.employee_id;
                        leaf.master_file = emp;
                        if (!leaves.Exists(x => x.Employee_id == leaf.Employee_id)) leaves.Add(leaf);
                        }
                    }
                }
                var leavesnew = new List<Leave>();
                foreach (var leaf in leaves)
                {
                    if (leaf.leave_bal < days.Value)
                    {
                        continue;
                    }
                    else
                    {
                        leavesnew.Add(leaf);
                    }
                }
                return this.View(leavesnew.OrderBy(x => x.leave_bal));
            }
            if (leave_bal_till.HasValue)
            {
                var leaveslist = new List<Leave>();
                var leaveballist = this.db.leavecals.ToList();
                leaves = this.db.Leaves.ToList();
                this.cal_bal(leave_bal_till.Value);
                foreach (var leaf in leaves)
                {
                    if (leaf.master_file.date_joined != null)
                    {
                        var lb = leaveballist.Find(x => x.Employee_id == leaf.Employee_id).leave_balance;
                        leaf.leave_bal = lb;
                        if (!leaveslist.Exists(x => x.Employee_id == leaf.Employee_id)) leaveslist.Add(leaf);
                }
            }

                foreach (var emp in afinallist)
                {
                    if (emp.date_joined !=null)
                    {
                        var leaf = new Leave();
                        var lb = leaveballist.Find(x => x.Employee_id == emp.employee_id).leave_balance;
                        leaf.leave_bal = lb;
                        leaf.Employee_id = emp.employee_id;
                        leaf.master_file = emp;
                        if (!leaveslist.Exists(x => x.Employee_id == leaf.Employee_id)) leaveslist.Add(leaf);
                    }
                }
                return this.View(leaveslist.OrderBy(x => x.leave_bal).ThenBy(x => x.Employee_id));
            }

            return this.View(leaves);
        }

        HREntities hrdb = new HREntities();

        public ActionResult ImportExcel()
        {
            return View();
        }

        [ActionName("Importexcel")]
        [HttpPost]
        public ActionResult Importexcel()
        {
            if (this.Request.Files["FileUpload1"].ContentLength > 0)
            {
                var extension = Path.GetExtension(this.Request.Files["FileUpload1"].FileName).ToLower();
                string query = null;
                var connString = string.Empty;

                string[] validFileTypes = { ".csv" };

                var path1 = string.Format(
                    "{0}/{1}",
                    this.Server.MapPath("~/Content/Uploads"),
                    this.Request.Files["FileUpload1"].FileName);
                if (!Directory.Exists(path1)) Directory.CreateDirectory(this.Server.MapPath("~/Content/Uploads"));

                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1)) System.IO.File.Delete(path1);

                    this.Request.Files["FileUpload1"].SaveAs(path1);
                    if (extension == ".csv")
                    {
                        var dt = Utility.ConvertCSVtoDataTable(path1);
                        this.ViewBag.Data = dt;
                        if (dt.Rows.Count > 0)
                        {
                            var leavecheck = this.db.leave_absence.ToList();
                            var pro = new leave_absence();
                            foreach (DataRow dr in dt.Rows)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (dr[column] == null || dr[column].ToString() == " ") goto e;

                                    if (column.ColumnName == "Month")
                                    {
                                        DateTime.TryParse(dr[column].ToString(), out var dtt);
                                        pro.month = dtt;
                                    }

                                    if (column.ColumnName == "from")
                                    {
                                        DateTime.TryParse(dr[column].ToString(), out var dtt);
                                        pro.fromd = dtt;
                                    }

                                    if (column.ColumnName == "to")
                                    {
                                        DateTime.TryParse(dr[column].ToString(), out var dtt);
                                        pro.tod = dtt;
                                    }

                                    if (column.ColumnName == "Absents")
                                    {
                                        float.TryParse(dr[column].ToString(), out var dtt);
                                        pro.absence = dtt;
                                    }

                                    if (column.ColumnName == "EMPNO")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var dbmf = this.hrdb.master_file.ToList();
                                            var epid = dbmf.Find(x => x.employee_no == idm);
                                            if (epid == null) goto e;
                                            pro.Employee_id = epid.employee_id;
                                        }
                                    }
                                }

                                if (leavecheck.Exists(x => x.Employee_id == pro.Employee_id && x.month.Value.Year == pro.month.Value.Year && x.month.Value.Month == pro.month.Value.Month))
                                {
                                    var leaveab = leavecheck.Find(
                                        x => x.Employee_id == pro.Employee_id
                                             && x.month.Value.Year == pro.month.Value.Year
                                             && x.month.Value.Month == pro.month.Value.Month);
                                    leaveab.absence = pro.absence;
                                    this.db.Entry(leaveab).State = EntityState.Modified;
                                    this.db.SaveChanges();
                                }
                                else
                                {
                                    this.hrdb.leave_absence.Add(pro);
                                    this.hrdb.SaveChanges();
                                }
                                e: ;
                            }
                        }
                    }

                }
            }
            else
            {
                this.ViewBag.Error = "Please Upload Files in .csv format";
            }

            return this.View();
        }

        public ActionResult mrv(DateTime? eddate,DateTime? eddate1)
        {
            var con_leave = from l in this.db.Leaves
                            join con in this.db.contracts on l.master_file.employee_no equals con.master_file.employee_no
                            select new
                                       {
                                           l.master_file.employee_no,
                                           l.master_file.employee_name,
                                           con.designation,
                                           con.departmant_project,
                                           l.Start_leave,
                                           l.End_leave
                                       };
            var leavelist = this.db.Leaves.ToList();
            var listconleave = con_leave.OrderBy(x=>x.departmant_project).ToList();
            var cllist = new List<con_leavemodel>();
            int i = 0;
            foreach (var cl in listconleave)
            {
                var clitem = new con_leavemodel();
                clitem.id = ++i;
                clitem.employee_no = cl.employee_no;
                clitem.employee_name = cl.employee_name;
                clitem.designation = cl.designation;
                clitem.departmant_project = cl.departmant_project;
                clitem.Start_leave = cl.Start_leave;
                clitem.End_leave = cl.End_leave;
                cllist.Add(clitem);
            }

            if (eddate.HasValue && !eddate1.HasValue)
            {
                return this.View(cllist.Where(x => x.Start_leave >= eddate).OrderBy(x => x.departmant_project).ThenBy(x => x.Start_leave));
            }
            if (eddate.HasValue && eddate1.HasValue)
            {
                return this.View(cllist.Where(x => x.Start_leave >= eddate && x.Start_leave <= eddate1).OrderBy(x => x.departmant_project).ThenBy(x => x.Start_leave));
            }
            
            return this.View(new List<con_leavemodel>());
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}