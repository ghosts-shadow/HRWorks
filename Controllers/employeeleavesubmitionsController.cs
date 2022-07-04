using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using HRworks.Models;
using Microsoft.Ajax.Utilities;
using MimeKit;
using MailKit.Net.Smtp;

namespace HRworks.Controllers
{
    [Authorize(Roles = "HOD,employee,Manager")]
    public class employeeleavesubmitionsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: employeeleavesubmitions
        public ActionResult Index()
        {
            var leafcon = new LeavesController();
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            if (empuser == null)
            {
                return RedirectToAction("LogOff", "Account");
            }

            var empjd = empuser.master_file;
            var employeeleavesubmitions =
                db.employeeleavesubmitions.ToList()
                    .FindAll(x => x.Employee_id == empjd.employee_id && (x.apstatus != "approved"));
            leafcon.forfitedbalence(empjd.employee_id);
            var leavecal2020list = db.leavecal2020.ToList();
            var leavebal2020 = new leavecal2020();
            leavebal2020 = leavecal2020list.Find(x =>
                x.Employee_id == empjd.employee_id);
            var asf = empjd.date_joined;
            var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                x => x.Employee_id == empjd.employee_id && x.Start_leave >= asf).ToList();
            var ump = leaves.ToList();
            var rdate = new DateTime();
            var times = new TimeSpan?();
            double sick = 0;
            double comp = 0;
            double mate = 0;
            double haj = 0;
            double udd = 0;
            double esco = 0;
            double pater = 0;
            double availed = 0;
            var favailed = 0d;
            foreach (var leaf in ump)
            {
                var leavetoemp = new employeeleavesubmition();
                leavetoemp.Date = leaf.Date;
                leavetoemp.Start_leave = leaf.Start_leave;
                leavetoemp.End_leave = leaf.End_leave;
                leavetoemp.Employee_id = leaf.Employee_id;
                leavetoemp.Return_leave = leaf.Return_leave;
                leavetoemp.leave_type = leaf.leave_type;
                leavetoemp.toltal_requested_days = leaf.toltal_requested_days;
                leavetoemp.half = leaf.half;
                leavetoemp.toltal_requested_days = leaf.days;
                leavetoemp.apstatus = "approved";
                employeeleavesubmitions.Add(leavetoemp);
                if (leaf.Reference == null)
                {
                    leaf.Reference = DateTime.Now.ToString("F");
                }

                rdate = Convert.ToDateTime(leaf.Reference);

                if (leaf.leave_type == "1")
                {
                    if (leaf.half)
                    {
                        if (DateTime.Today > leaf.Start_leave)
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) availed += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) favailed += times.Value.TotalDays + 1 - 0.5;
                        }
                    }
                    else
                    {
                        if (DateTime.Today > leaf.Start_leave)
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) availed += times.Value.TotalDays + 1;
                        }
                        else
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) favailed += times.Value.TotalDays + 1;
                        }
                    }
                }

                if (leaf.leave_type == "2" || leaf.leave_type == "7")
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

                if (leaf.leave_type == "3")
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

                if (leaf.leave_type == "4")
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

                if (leaf.leave_type == "5")
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

                if (leaf.leave_type == "8")
                {
                    if (leaf.half)
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (times != null) udd += times.Value.TotalDays + 1 - 0.5;
                    }
                    else
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (times != null) udd += times.Value.TotalDays + 1;
                    }
                }

                if (leaf.leave_type == "9")
                {
                    if (leaf.half)
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (times != null) esco += times.Value.TotalDays + 1 - 0.5;
                    }
                    else
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (times != null) esco += times.Value.TotalDays + 1;
                    }
                }

                if (leaf.leave_type == "10")
                {
                    if (leaf.half)
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (times != null) pater += times.Value.TotalDays + 1 - 0.5;
                    }
                    else
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (times != null) pater += times.Value.TotalDays + 1;
                    }
                }
            }

            this.ViewBag.udd = udd;
            this.ViewBag.esco = esco;
            this.ViewBag.pater = pater;
            this.ViewBag.mate = mate;
            this.ViewBag.haj = haj;
            this.ViewBag.sick = sick;
            this.ViewBag.comp = comp;
            if (leavebal2020.leave_balance < leavebal2020.ifslbal)
            {
                this.ViewBag.lbal = leavebal2020.leave_balance;
            }
            else
            {
                this.ViewBag.lbal = leavebal2020.ifslbal;
            }

            this.ViewBag.per = leavebal2020.periodtill2020 + leavebal2020.periodafter2020;
            this.ViewBag.aval = availed;
            this.ViewBag.faval = favailed;
            this.ViewBag.taval = availed + favailed;
            this.ViewBag.netp = leavebal2020.net_periodtill2020 + leavebal2020.net_periodafter2020;
            this.ViewBag.ump = leavebal2020.unpaid_leavetill2020 + leavebal2020.unpaid_leaveafter2020;
            this.ViewBag.accr = leavebal2020.accruedafter2020 + leavebal2020.accruedtill2020;
            this.ViewBag.name = empjd.employee_name;
            this.ViewBag.no = empjd.employee_no;
            this.ViewBag.forfited = leavebal2020.forfitedafter2020 + leavebal2020.forfitedtill2020;
            return View(employeeleavesubmitions);
        }

        // GET: employeeleavesubmitions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            employeeleavesubmition employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            if (employeeleavesubmition == null)
            {
                return HttpNotFound();
            }

            return View(employeeleavesubmition);
        }

        // GET: employeeleavesubmitions/Create
        public ActionResult Create()
        {
            var userempnolists = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empusers = userempnolists.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var leafcon = new LeavesController();
            var empjd = empusers.master_file;
            leafcon.forfitedbalence(empjd.employee_id);
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "Annual", Value = "1"},
                new ListItem {Text = "Sick(non industrial)", Value = "2"},
                new ListItem {Text = "Compassionate", Value = "3"},
                new ListItem {Text = "Maternity", Value = "4"},
                new ListItem {Text = "Haj", Value = "5"},
                new ListItem {Text = "Unpaid", Value = "6"},
                new ListItem {Text = "Sick(industrial)", Value = "7"},
                new ListItem {Text = "UDDAH", Value = "8"},
                new ListItem {Text = "ESCORT", Value = "9"},
                new ListItem {Text = "PATERNITY ", Value = "10"}
            };
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            this.ViewBag.employee_id = empuser.master_file.employee_id;
            return View();
        }

        // POST: employeeleavesubmitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include =
                "Id,Employee_id,Date,Start_leave,End_leave,Return_leave,leave_type,toltal_requested_days,submitted_by,apstatus,half,approved_byline,approved_byhod")]
            employeeleavesubmition employeeleavesubmition, HttpPostedFileBase fileBase)
        {
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "Annual", Value = "1"},
                new ListItem {Text = "Sick(non industrial)", Value = "2"},
                new ListItem {Text = "Compassionate", Value = "3"},
                new ListItem {Text = "Maternity", Value = "4"},
                new ListItem {Text = "Haj", Value = "5"},
                new ListItem {Text = "Unpaid", Value = "6"},
                new ListItem {Text = "Sick(industrial)", Value = "7"},
                new ListItem {Text = "UDDAH", Value = "8"},
                new ListItem {Text = "ESCORT", Value = "9"},
                new ListItem {Text = "PATERNITY ", Value = "10"}
            };
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            employeeleavesubmition.Employee_id = empuser.master_file.employee_id;
            this.ViewBag.employee_id = empuser.master_file.employee_id;
            var jd = empuser.master_file.date_joined;
            var leavelistc = this.db.Leaves.ToList();
            var empsubleave = db.employeeleavesubmitions.ToList();
            var leaveabal = db.leavecal2020.ToList().Find(x => x.Employee_id == empuser.master_file.employee_id);
            var leavebalsub = 0d;
            if (leaveabal.ifslbal < leaveabal.leave_balance)
            {
                if (leaveabal.ifslbal != null) leavebalsub = leaveabal.ifslbal.Value;
            }
            else
            {
                if (leaveabal.leave_balance != null) leavebalsub = leaveabal.leave_balance.Value;
            }

            if (employeeleavesubmition.toltal_requested_days != null &&
                leavebalsub - employeeleavesubmition.toltal_requested_days < 0)
            {
                ModelState.AddModelError("toltal_requested_days", "insufficient balance");
                goto jderr;
            }

            foreach (var leaf in empsubleave)
            {
                if (leaf.apstatus == "submitted" /*|| leaf.apstatus.Contains("rejected")*/ ||
                    leaf.apstatus == "approved by line manager")
                {
                    var templeave = new Leave();
                    templeave.Date = leaf.Date;
                    templeave.Start_leave = leaf.Start_leave;
                    templeave.End_leave = leaf.End_leave;
                    templeave.Return_leave = leaf.Return_leave;
                    templeave.half = leaf.half;
                    templeave.Employee_id = leaf.Employee_id;
                    templeave.leave_type = leaf.leave_type;
                    templeave.actualchangedby = User.Identity.Name;
                    leavelistc.Add(templeave);
                }
            }

            if (leavelistc.Exists(x =>
                ((x.Start_leave <= employeeleavesubmition.Start_leave &&
                  x.End_leave >= employeeleavesubmition.Start_leave) ||
                 (x.Start_leave <= employeeleavesubmition.End_leave &&
                  x.End_leave >= employeeleavesubmition.End_leave)) &&
                x.Employee_id == employeeleavesubmition.Employee_id ))
            {
                var testl = leavelistc.FindAll(x =>
                    ((x.Start_leave <= employeeleavesubmition.Start_leave &&
                      x.End_leave >= employeeleavesubmition.Start_leave) ||
                     (x.Start_leave <= employeeleavesubmition.End_leave &&
                      x.End_leave >= employeeleavesubmition.End_leave)) &&
                    x.Employee_id == employeeleavesubmition.Employee_id);
                ModelState.AddModelError("Start_leave", "already exists");

                goto jderr;
            }

            if (leavelistc.Exists(x =>
                (x.Start_leave >= employeeleavesubmition.Start_leave &&
                 x.End_leave <= employeeleavesubmition.End_leave) &&
                x.Employee_id == employeeleavesubmition.Employee_id ))
            {
                ModelState.AddModelError("Start_leave", "already exists");

                goto jderr;
            }


            //            if (leavelistc.Exists(x=>x.End_leave >= employeeleavesubmition.Start_leave && x.Employee_id == employeeleavesubmition.Employee_id))
            //            {
            //                ModelState.AddModelError("Start_leave", employeeleavesubmition.Start_leave+" already exists");
            //
            //                goto jderr;
            //            }
            if (employeeleavesubmition.Start_leave < jd && employeeleavesubmition.Start_leave != default)
            {
                ModelState.AddModelError("Start_leave", "selected date is before " + jd);
                goto jderr;
            }

            if (employeeleavesubmition.End_leave < jd && employeeleavesubmition.End_leave != default)
            {
                ModelState.AddModelError("Start_leave", "selected date is before " + jd);
                goto jderr;
            }

            if (employeeleavesubmition.End_leave < employeeleavesubmition.Start_leave)
            {
                ModelState.AddModelError("Start_leave", "cannot add date in reverse");
                goto jderr;
            }

            var leavelist = this.db.Leaves.ToList();
            if (leavelist.Exists(
                x => x.Employee_id == employeeleavesubmition.Employee_id && x.Start_leave ==
                                                                         employeeleavesubmition.Start_leave
                                                                         && x.End_leave == employeeleavesubmition
                                                                             .End_leave))
            {
                ModelState.AddModelError("Start_leave", "the entry already exists");
                goto jderr;
            }

            string serverfile;
            if (fileBase != null)
            {
                var i = 0;
                var imgname = Path.GetFileName(fileBase.FileName);
                var fileexe = Path.GetExtension(fileBase.FileName);
                var filepath = new DirectoryInfo("D:/HR/leave/");
                serverfile =
                    "D:/HR/leave/" + employeeleavesubmition.Employee_id; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/leave/" + employeeleavesubmition.Employee_id + "/" +
                                 employeeleavesubmition.Employee_id + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(
                    serverfile = "D:/HR/leave/" + employeeleavesubmition.Employee_id + "/" +
                                 employeeleavesubmition.Employee_id + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }

            if (ModelState.IsValid)
            {
                employeeleavesubmition.Employee_id = empuser.master_file.employee_id;
                employeeleavesubmition.submitted_by = User.Identity.Name;

                employeeleavesubmition.Date = DateTime.Today;
                if (!serverfile.IsNullOrWhiteSpace() && (employeeleavesubmition.leave_type == "2" ||
                                                         employeeleavesubmition.leave_type == "7"))
                {
                    employeeleavesubmition.apstatus = "approved";
                    var leaveentry = new Leave();
                    leaveentry.Date = employeeleavesubmition.Date;
                    leaveentry.Start_leave = employeeleavesubmition.Start_leave;
                    leaveentry.End_leave = employeeleavesubmition.End_leave;
                    leaveentry.Return_leave = employeeleavesubmition.Return_leave;
                    leaveentry.half = employeeleavesubmition.half;
                    leaveentry.Employee_id = employeeleavesubmition.Employee_id;
                    leaveentry.leave_type = employeeleavesubmition.leave_type;
                    leaveentry.actualchangedby = User.Identity.Name;
                    leaveentry.actualchangeddateby = DateTime.Now;
                    db.Leaves.Add(leaveentry);
                    db.SaveChanges();
                    var sendmailtridsick = db.employeeleavesubmitions.ToList().Last();
                    SendMail("", "approved", sendmailtridsick.Id);
                }
                else
                {
                    employeeleavesubmition.apstatus = "submitted";
                }

                employeeleavesubmition.imgpath = serverfile;
                db.employeeleavesubmitions.Add(employeeleavesubmition);
                db.SaveChanges();
                var sendmailtrid = db.employeeleavesubmitions.ToList().Last();
                SendMail("", "submitted", sendmailtrid.Id);
                return RedirectToAction("Index");
            }

            jderr: ;
            return View(employeeleavesubmition);
        }

        // GET: employeeleavesubmitions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            employeeleavesubmition employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            if (employeeleavesubmition == null)
            {
                return HttpNotFound();
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name",
                employeeleavesubmition.Employee_id);
            return View(employeeleavesubmition);
        }

        // POST: employeeleavesubmitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include =
                "Id,Employee_id,Date,Start_leave,End_leave,Return_leave,leave_type,toltal_requested_days,submitted_by,apstatus,half,approved_byline,approved_byhod")]
            employeeleavesubmition employeeleavesubmition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name",
                employeeleavesubmition.Employee_id);
            return View(employeeleavesubmition);
        }

        // GET: employeeleavesubmitions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            employeeleavesubmition employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            if (employeeleavesubmition == null)
            {
                return HttpNotFound();
            }

            return View(employeeleavesubmition);
        }

        // POST: employeeleavesubmitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            employeeleavesubmition employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            db.employeeleavesubmitions.Remove(employeeleavesubmition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "HOD,Manager,EXTHOD")]
        public ActionResult empleaveap()
        {
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var employeeleavesubmitions = new List<employeeleavesubmition>();
            if (empuser == null)
            {
                return RedirectToAction("LogOff", "Account");
            }

            var empjd = empuser.master_file;
            if (User.IsInRole("Manager"))
            {
                var relationlist = db.emprels.ToList().FindAll(x => x.line_man == empjd.employee_id);
                foreach (var emprel in relationlist)
                {
                    employeeleavesubmitions.AddRange(db.employeeleavesubmitions.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "submitted"));
                }
            }

            if (User.IsInRole("HOD") || User.IsInRole("EXTHOD"))
            {
                var relationlist = db.emprels.ToList().FindAll(x => x.HOD == empjd.employee_id);
                foreach (var emprel in relationlist)
                {
                    employeeleavesubmitions.AddRange(db.employeeleavesubmitions.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "approved by line manager"));
                }
            }

            return View(employeeleavesubmitions);
        }

        public ActionResult approve(int id)
        {
            if (User.IsInRole("Manager"))
            {
                var employeeleavesubmition = db.employeeleavesubmitions.Find(id);
                employeeleavesubmition.apstatus = "approved by line manager";
                var higherupchecklist = db.emprels.ToList();
                var hihigherupcheck = higherupchecklist.Find(x => x.Employee_id == employeeleavesubmition.Employee_id);
                if (!hihigherupcheck.HOD.HasValue)
                {
                    employeeleavesubmition.apstatus = "approved";
                    var leaveentry = new Leave();
                    leaveentry.Date = employeeleavesubmition.Date;
                    leaveentry.Start_leave = employeeleavesubmition.Start_leave;
                    leaveentry.End_leave = employeeleavesubmition.End_leave;
                    leaveentry.Return_leave = employeeleavesubmition.Return_leave;
                    leaveentry.half = employeeleavesubmition.half;
                    leaveentry.Employee_id = employeeleavesubmition.Employee_id;
                    leaveentry.leave_type = employeeleavesubmition.leave_type;
                    leaveentry.actualchangedby = User.Identity.Name;
                    leaveentry.actualchangeddateby = DateTime.Now;
                    db.Leaves.Add(leaveentry);
                    db.SaveChanges();
                    SendMail("", "approved", id);
                    return RedirectToAction("empleaveap");
                }

                employeeleavesubmition.approved_byline = User.Identity.Name;
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail("", "approved by line manager", id);
            }

            if (User.IsInRole("HOD") || User.IsInRole("EXTHOD"))
            {
                var employeeleavesubmition = db.employeeleavesubmitions.Find(id);
                employeeleavesubmition.apstatus = "approved";
                var leaveentry = new Leave();
                leaveentry.Date = employeeleavesubmition.Date;
                leaveentry.Start_leave = employeeleavesubmition.Start_leave;
                leaveentry.End_leave = employeeleavesubmition.End_leave;
                leaveentry.Return_leave = employeeleavesubmition.Return_leave;
                leaveentry.half = employeeleavesubmition.half;
                leaveentry.Employee_id = employeeleavesubmition.Employee_id;
                leaveentry.leave_type = employeeleavesubmition.leave_type;
                leaveentry.actualchangedby = User.Identity.Name;
                leaveentry.actualchangeddateby = DateTime.Now;
                db.Leaves.Add(leaveentry);
                db.SaveChanges();
                employeeleavesubmition.approved_byline = User.Identity.Name;
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail("", "approved", id);
            }

            return RedirectToAction("empleaveap");
        }

        public ActionResult reject(int id, string message)
        {
            if (User.IsInRole("Manager"))
            {
                var employeeleavesubmition = db.employeeleavesubmitions.Find(id);
                employeeleavesubmition.apstatus = "rejected by line manager for " + message;
                employeeleavesubmition.approved_byline = User.Identity.Name;
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail(message, "rejected by line manager for ", id);
            }

            if (User.IsInRole("HOD") || User.IsInRole("EXTHOD"))
            {
                var employeeleavesubmition = db.employeeleavesubmitions.Find(id);
                employeeleavesubmition.apstatus = "rejected by HOD for " + message;
                employeeleavesubmition.approved_byline = User.Identity.Name;
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail(message, "rejected by HOD for ", id);
            }

            return RedirectToAction("empleaveap");
        }


        public void SendMail(string msg, string action, int elsid)
        {
            var empleavelist = db.employeeleavesubmitions.ToList();
            var empleave = empleavelist.Find(x => x.Id == elsid);
            var message = new MimeMessage();
            var emprellist = db.emprels.ToList();
            var emprel = emprellist.Find(x => x.Employee_id == empleave.Employee_id);
            var userlist = db.AspNetUsers.ToList();
            var usernamelist = db.usernames.ToList();
            var emplusersname = usernamelist.Find(x => x.employee_no == emprel.Employee_id);
            message.From.Add(new MailboxAddress("Hrworks", "timekeeper@citiscapegroup.com"));

            if (action.Contains("submitted"))
            {
                var nextusersname = usernamelist.Find(x => x.employee_no == emprel.line_man);
                var nextuser = userlist.Find(x => x.Id == nextusersname.aspnet_uid);
                message.To.Add((new MailboxAddress(nextusersname.full_name, nextuser.Email)));
                message.Subject = "leave approval";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for  " +
                           emplusersname.full_name + " has been submitted for your approval" + "\n\n\n" +
                           "http://hrworks.ddns.net:6333/citiworks/employeeleavesubmitions" + "\n\n\n" + "Thanks Best Regards, "
                };
            }
            if (action.Contains("approved"))
            {
                if (action == "approved by line manager")
                {
                    var previoususersname = usernamelist.Find(x => x.employee_no == emprel.line_man);
                    var nextusersname = usernamelist.Find(x => x.employee_no == emprel.HOD);
                    var nextuser = userlist.Find(x => x.Id == nextusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(nextusersname.full_name, nextuser.Email)));
                    message.Subject = "leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for  " +
                               emplusersname.full_name + " has been approved by line manager "+ previoususersname.master_file.employee_name + " and forwarded for your approval" + "\n\n\n" +
                               "http://hrworks.ddns.net:6333/citiworks/employeeleavesubmitions" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }
                else if (action == "approved")
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for  " +
                               emplusersname.full_name + " has been approved" + "\n\n\n" +
                               "http://hrworks.ddns.net:6333/citiworks/employeeleavesubmitions" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }
            }
            if (action.Contains("rejected"))
                
            {
                if (action.Contains("rejected by line manager")) {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for  " +
                               emplusersname.master_file.employee_name + " has been rejected by line manager for "+message + "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }
                if (action.Contains("rejected by HOD"))
                {
                    var previoususersname = usernamelist.Find(x => x.employee_no == emprel.line_man);
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for  " +
                               emplusersname.master_file.employee_name + " has been rejected by HOD for " + message + "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }
            }

            if (message.To.Count != 0)
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("outlook.office365.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("timekeeper@citiscapegroup.com", "Vam15380");

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
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