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

    using HRworks.Models;

    public class LeavesController : Controller
    {
        private readonly HREntities db = new HREntities();

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
                                    new ListItem { Text = "Unpaid", Value = "6" },
                                    new ListItem { Text = "others", Value = "7" }
                                };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
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
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            this.ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_no");

            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
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

        // GET: Leaves/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var leave = this.db.Leaves.Find(id);
            if (leave == null) return this.HttpNotFound();
            var listItems = new List<ListItem>
                                {
                                    new ListItem { Text = "Annual", Value = "Annual" },
                                    new ListItem { Text = "Sick", Value = "Sick" },
                                    new ListItem { Text = "Compassionate", Value = "Compassionate" },
                                    new ListItem { Text = "Maternity", Value = "Maternity" },
                                    new ListItem { Text = "Haj", Value = "Haj" },
                                    new ListItem { Text = "Unpaid", Value = "Unpaid" },
                                    new ListItem { Text = "Other", Value = "Other" }
                                };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
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

        public ActionResult report(long? Employee_id, DateTime? eddate)
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            var empjd = afinallist.Find(x => x.employee_id == Employee_id);
            this.ViewBag.employee_id = new SelectList(
                afinallist.OrderBy(x => x.employee_no),
                "employee_id",
                "employee_no");

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
                                comp += leaf.days.Value;
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
                                mate += leaf.days.Value;
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
                                haj += leaf.days.Value;
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
                                avalied += leaf.days.Value;
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
                    this.ViewBag.lbal = lbal;
                    this.ViewBag.per = period;
                    this.ViewBag.aval = avalied;
                    this.ViewBag.netp = netperiod;
                    this.ViewBag.ump = unpaid;
                    this.ViewBag.accr = accrued;
                    this.ViewBag.name = empjd.employee_name;

                    return this.View(leaves.ToList());
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

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}