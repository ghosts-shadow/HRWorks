using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using MailKit.Net.Smtp;
using Microsoft.Ajax.Utilities;
using MimeKit;

namespace HRworks.Controllers
{
    [Authorize]
    public class empreturnfromleavesubsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: empreturnfromleavesubs
        public ActionResult Index()
        {
            var empreturnfromleavesubs = db.empreturnfromleavesubs.OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.dateadded).ToList();
            return View(empreturnfromleavesubs);
        }

        // GET: empreturnfromleavesubs/Details/5
        /* public ActionResult Details(int? id)
         {
             if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }
             empreturnfromleavesub empreturnfromleavesub = db.empreturnfromleavesubs.Find(id);
             if (empreturnfromleavesub == null)
             {
                 return HttpNotFound();
             }
             return View(empreturnfromleavesub);
         }
         */

        // GET: empreturnfromleavesubs/Create
        public ActionResult Create(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "employeeleavesubmitions");
            }

            var leavesub = db.Leaves.ToList().Find(x => x.Id == id);
            ViewBag.leaveid = leavesub.Id;
            ViewBag.Start_leave = leavesub.Start_leave;
            ViewBag.End_leave = leavesub.End_leave;
            ViewBag.Return_leave = leavesub.Return_leave;
            return View();
        }

        // POST: empreturnfromleavesubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "Id,Employee_id,Date,leaveid,actualreturnleave,submitted_by,approved_byline,approved_byhod,apstatus,dateadded")]
            empreturnfromleavesub empreturnfromleavesub)
        {
            if (ModelState.IsValid)
            {
                var els = db.Leaves.ToList().Find(x => x.Id == empreturnfromleavesub.leaveid);
                var exreturnleave = db.empreturnfromleavesubs.Where(x => x.Employee_id == els.Employee_id).ToList();
                empreturnfromleavesub.Date = DateTime.Today;
                empreturnfromleavesub.Employee_id = els.Employee_id;
                empreturnfromleavesub.apstatus = "submitted";
                empreturnfromleavesub.dateadded = DateTime.Now;
                empreturnfromleavesub.submitted_by = User.Identity.Name;
                if ((exreturnleave.Exists(x =>
                        x.leaveid == els.Id && (x.apstatus == "submitted" || x.apstatus == "approved"))) ||
                    !empreturnfromleavesub.actualreturnleave.HasValue)
                {
                    goto ex;
                }

                db.empreturnfromleavesubs.Add(empreturnfromleavesub);
                db.SaveChanges();
                var sendmailtrid = db.empreturnfromleavesubs.ToList().Last();
                SendMail("", "submitted", sendmailtrid.Id);
                ex: ;
                return RedirectToAction("Index", "employeeleavesubmitions");
            }

            ViewBag.empsubleaveid = new SelectList(db.employeeleavesubmitions, "Id", "leave_type",
                empreturnfromleavesub.leaveid);
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name",
                empreturnfromleavesub.Employee_id);
            return View(empreturnfromleavesub);
        }

        // GET: empreturnfromleavesubs/Edit/5
        /*public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empreturnfromleavesub empreturnfromleavesub = db.empreturnfromleavesubs.Find(id);
            if (empreturnfromleavesub == null)
            {
                return HttpNotFound();
            }
            ViewBag.empsubleaveid = new SelectList(db.employeeleavesubmitions, "Id", "leave_type", empreturnfromleavesub.empsubleaveid);
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", empreturnfromleavesub.Employee_id);
            return View(empreturnfromleavesub);
        }

        // POST: empreturnfromleavesubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,Date,empsubleaveid,actualreturnleave,submitted_by,approved_byline,approved_byhod,apstatus,dateadded")] empreturnfromleavesub empreturnfromleavesub)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empreturnfromleavesub).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.empsubleaveid = new SelectList(db.employeeleavesubmitions, "Id", "leave_type", empreturnfromleavesub.empsubleaveid);
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", empreturnfromleavesub.Employee_id);
            return View(empreturnfromleavesub);
        }

        // GET: empreturnfromleavesubs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empreturnfromleavesub empreturnfromleavesub = db.empreturnfromleavesubs.Find(id);
            if (empreturnfromleavesub == null)
            {
                return HttpNotFound();
            }
            return View(empreturnfromleavesub);
        }

        // POST: empreturnfromleavesubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            empreturnfromleavesub empreturnfromleavesub = db.empreturnfromleavesubs.Find(id);
            db.empreturnfromleavesubs.Remove(empreturnfromleavesub);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        */

        /*public ActionResult empreturnap()
        {
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var empreturnsub = new List<empreturnfromleavesub>();
            if (User.IsInRole("super_admin"))
            {
                goto superlogin;
            }
            if (empuser == null)
            {
                goto logend;
            }
            var empjd = empuser.master_file;
            if (User.IsInRole("Manager"))
            {
                var relationlist = db.emprels.ToList().FindAll(x => x.line_man == empjd.employee_id);
                foreach (var emprel in relationlist)
                {
                    empreturnsub.AddRange(db.empreturnfromleavesubs.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "submitted"));
                }
            }
            if (User.IsInRole("HOD") || User.IsInRole("EXTHOD"))
            {
                var relationlist = db.emprels.ToList().FindAll(x => x.HOD == empjd.employee_id);
                foreach (var emprel in relationlist)
                {
                    empreturnsub.AddRange(db.empreturnfromleavesubs.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "approved by line manager"));
                }
            }

            return View(empreturnsub);
            superlogin:;
            empreturnsub.AddRange(db.empreturnfromleavesubs.ToList().FindAll(x=>x.apstatus == "submitted for HR"));
            return View(empreturnsub);
            logend:;
            ViewBag.logend = "forcelogout";
            return View(empreturnsub);
        }*/
        public ActionResult empreturnap()
        {
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var empreturnsub = new List<empreturnfromleavesub>();
            if (User.IsInRole("super_admin"))
            {
                goto superlogin;
            }

            if (empuser == null)
            {
                goto logend;
            }

            var empjd = empuser.master_file;
            var relationlist = db.emprels.ToList();
            var logedinsrels = relationlist.FindAll(x => x.line_man == empjd.employee_id || x.HOD == empjd.employee_id);
            foreach (var emprel in logedinsrels)
            {
                if (emprel.HOD.HasValue && emprel.HOD == empjd.employee_id)
                {
                    empreturnsub.AddRange(db.empreturnfromleavesubs.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "approved by line manager"));
                }

                if (emprel.line_man == empjd.employee_id)
                {
                    empreturnsub.AddRange(db.empreturnfromleavesubs.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "submitted"));
                }
            }

            return View(empreturnsub);
            superlogin: ;
            empreturnsub.AddRange(db.empreturnfromleavesubs.ToList().FindAll(x => x.apstatus == "submitted for HR"));
            return View(empreturnsub);
            logend: ;
            ViewBag.logend = "forcelogout";
            return View(empreturnsub);
        }


        /*public ActionResult approve(int id)
        {
            if (ModelState.IsValid)
            {
                var empreturnapplist = db.empreturnfromleavesubs.ToList();
                var empreturnapp = empreturnapplist.Find(x => x.Id == id);
                empreturnapp.dateadded = DateTime.Now;
                if (User.IsInRole("Manager"))
                {
                    if (empreturnapp.apstatus == "approved by line manager")
                    {
                        goto back;
                    }

                    var higherupchecklist = db.emprels.ToList();
                    var hihigherupcheck = higherupchecklist.Find(x => x.Employee_id == empreturnapp.Employee_id);
                    if (!hihigherupcheck.HOD.HasValue)
                    {
                        if (empreturnapp.actualreturnleave == empreturnapp.Leave.Return_leave)
                        {
                            empreturnapp.apstatus = "approved";
                            empreturnapp.approved_byline = "N/A";
                            empreturnapp.approved_byhod = User.Identity.Name;
                            empreturnapp.Leave.actual_return_date = empreturnapp.actualreturnleave;
                            empreturnapp.Leave.actualchangedby =
                                "actual return date added by system after approval from " + User.Identity.Name;
                            db.Entry(empreturnapp).State = EntityState.Modified;
                            db.SaveChanges();
                            SendMail("", "approved", id);
                        }else if (empreturnapp.actualreturnleave > empreturnapp.Leave.Return_leave)
                        {
                            var todate = empreturnapp.actualreturnleave;
                            empreturnapp.apstatus = "approved";
                            empreturnapp.approved_byline = "N/A";
                            empreturnapp.approved_byhod = User.Identity.Name;
                            empreturnapp.Leave.actual_return_date = empreturnapp.Leave.Return_leave;
                            empreturnapp.actualreturnleave = empreturnapp.Leave.Return_leave;
                            empreturnapp.Leave.actualchangedby = "system";
                            empreturnapp.Leave.actualchangeddateby = DateTime.Now;
                            db.Entry(empreturnapp).State = EntityState.Modified;
                            db.SaveChanges();
                            SendMail("", "approved", id);
                            var unpaidauto = new Leave();
                            unpaidauto.leave_type = "6";
                            unpaidauto.Date = DateTime.Now;
                            unpaidauto.Employee_id = empreturnapp.Employee_id;
                            unpaidauto.master_file = empreturnapp.master_file;
                            unpaidauto.Start_leave = empreturnapp.Leave.End_leave.Value.AddDays(1);
                            unpaidauto.End_leave = todate.Value.AddDays(-1);
                            unpaidauto.Return_leave = todate;
                            unpaidauto.actual_return_date = todate;
                            unpaidauto.actualchangedby = "actual return date added by system after approval from " + User.Identity.Name;
                            unpaidauto.actualchangeddateby = DateTime.Now;
                            this.db.Leaves.Add(unpaidauto);
                            this.db.SaveChanges();
                        }
                    }
                    else
                    {
                        empreturnapp.apstatus = "approved by line manager";
                        db.Entry(empreturnapp).State = EntityState.Modified;
                        db.SaveChanges();
                        SendMail("", "approved by line manager", id);
                    }

                    return RedirectToAction("empreturnap");
                }

                back: ;
                if (User.IsInRole("HOD") || User.IsInRole("EXTHOD") || User.IsInRole("super_admin"))
                {
                    if (empreturnapp.actualreturnleave == empreturnapp.Leave.Return_leave)
                    {
                        empreturnapp.apstatus = "approved";
                        empreturnapp.approved_byhod = User.Identity.Name;
                        empreturnapp.Leave.actual_return_date = empreturnapp.actualreturnleave;
                        empreturnapp.Leave.actualchangedby =
                            "actual return date added by system after approval from " + User.Identity.Name;
                        db.Entry(empreturnapp).State = EntityState.Modified;
                        db.SaveChanges();
                        SendMail("", "approved", id);
                    }
                    else if (empreturnapp.actualreturnleave > empreturnapp.Leave.Return_leave)
                    {
                        empreturnapp.apstatus = "approved";
                        empreturnapp.approved_byhod = User.Identity.Name;
                        empreturnapp.Leave.actual_return_date = empreturnapp.Leave.Return_leave;
                        empreturnapp.Leave.actualchangedby = "system";
                        empreturnapp.Leave.actualchangeddateby = DateTime.Now;
                        db.Entry(empreturnapp).State = EntityState.Modified;
                        db.SaveChanges();
                        SendMail("", "approved", id);
                        var todate = empreturnapp.actualreturnleave;
                        var unpaidauto = new Leave();
                        unpaidauto.leave_type = "6";
                        unpaidauto.Date = DateTime.Now;
                        unpaidauto.Employee_id = empreturnapp.Employee_id;
                        unpaidauto.master_file = empreturnapp.master_file;
                        unpaidauto.Start_leave = empreturnapp.Leave.End_leave.Value.AddDays(1);
                        unpaidauto.End_leave = todate.Value.AddDays(-1);
                        unpaidauto.Return_leave = todate;
                        unpaidauto.actual_return_date = todate;
                        unpaidauto.actualchangedby = "actual return date added by system after approval from " +
                                                     User.Identity.Name;
                        unpaidauto.actualchangeddateby = DateTime.Now;
                        this.db.Leaves.Add(unpaidauto);
                        this.db.SaveChanges();
                    }
                    return RedirectToAction("empreturnap");
                }
            }
            return RedirectToAction("empreturnap");
        }*/
        public ActionResult approve(int id)
        {
            var empreturnapplist = db.empreturnfromleavesubs.ToList();
            var empreturnapp = empreturnapplist.Find(x => x.Id == id);
            empreturnapp.dateadded = DateTime.Now;
            var leavelist = db.Leaves.Where(x => x.Employee_id == empreturnapp.Employee_id).ToList();
            if (User.IsInRole("super_admin"))
            {
                if (empreturnapp.actualreturnleave == empreturnapp.Leave.Return_leave)
                {
                    empreturnapp.apstatus = "approved";
                    empreturnapp.approved_byline = "N/A";
                    empreturnapp.approved_byhod = User.Identity.Name;
                    empreturnapp.Leave.actual_return_date = empreturnapp.actualreturnleave;
                    empreturnapp.Leave.actualchangedby =
                        "actual return date added after approval from " + User.Identity.Name;
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }
                else if (empreturnapp.actualreturnleave > empreturnapp.Leave.Return_leave)
                {
                    var todate = empreturnapp.actualreturnleave;
                    var unpaidauto = new Leave();
                    unpaidauto.leave_type = "6";
                    unpaidauto.Date = DateTime.Now;
                    unpaidauto.Employee_id = empreturnapp.Employee_id;
                    unpaidauto.master_file = empreturnapp.master_file;
                    unpaidauto.Start_leave = empreturnapp.Leave.End_leave.Value.AddDays(1);
                    unpaidauto.End_leave = todate.Value.AddDays(-1);
                    unpaidauto.Return_leave = todate;
                    unpaidauto.actual_return_date = todate;
                    unpaidauto.actualchangedby =
                        "actual return date added by system after approval from " + User.Identity.Name;
                    unpaidauto.actualchangeddateby = DateTime.Now;
                    empreturnapp.Leave.actualchangedby =
                        "actual return date added after approval from " + User.Identity.Name;
                    if (!leavelist.Exists(x =>
                            x.Start_leave == unpaidauto.Start_leave && x.Return_leave == unpaidauto.Return_leave))
                    {
                        this.db.Leaves.Add(unpaidauto);
                        this.db.SaveChanges();
                        empreturnapp.Leave.actualchangedby = "system";
                    }

                    empreturnapp.apstatus = "approved";
                    empreturnapp.approved_byline = "N/A";
                    empreturnapp.approved_byhod = User.Identity.Name;
                    empreturnapp.Leave.actual_return_date = empreturnapp.Leave.Return_leave;
                    //empreturnapp.actualreturnleave = empreturnapp.Leave.Return_leave;
                    empreturnapp.Leave.actualchangeddateby = DateTime.Now;
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }

                return RedirectToAction("empreturnap");
            }

            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var relationlist = db.emprels.ToList();
            var empjd = empuser.master_file;
            var logedinsrels = relationlist.FindAll(x => x.line_man == empjd.employee_id || x.HOD == empjd.employee_id);
            var currentrel = logedinsrels.Find(x => x.Employee_id == empreturnapp.Employee_id);
            if (currentrel.line_man == empjd.employee_id)
            {
                if (empreturnapp.apstatus == "approved by line manager")
                {
                    goto back;
                }

                if (currentrel.HOD.HasValue)
                {
                    empreturnapp.apstatus = "approved by line manager";
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved by line manager", id);
                }
                else
                {
                    if (empreturnapp.actualreturnleave == empreturnapp.Leave.Return_leave)
                    {
                        empreturnapp.apstatus = "approved";
                        empreturnapp.approved_byline = "N/A";
                        empreturnapp.approved_byhod = User.Identity.Name;
                        empreturnapp.Leave.actual_return_date = empreturnapp.actualreturnleave;
                        empreturnapp.Leave.actualchangedby =
                            "actual return date added after approval from " + User.Identity.Name;
                        db.Entry(empreturnapp).State = EntityState.Modified;
                        db.SaveChanges();
                        SendMail("", "approved", id);
                    }
                    else if (empreturnapp.actualreturnleave > empreturnapp.Leave.Return_leave)
                    {
                        var todate = empreturnapp.actualreturnleave;
                        var unpaidauto = new Leave();
                        unpaidauto.leave_type = "6";
                        unpaidauto.Date = DateTime.Now;
                        unpaidauto.Employee_id = empreturnapp.Employee_id;
                        unpaidauto.master_file = empreturnapp.master_file;
                        unpaidauto.Start_leave = empreturnapp.Leave.End_leave.Value.AddDays(1);
                        unpaidauto.End_leave = todate.Value.AddDays(-1);
                        unpaidauto.Return_leave = todate;
                        unpaidauto.actual_return_date = todate;
                        unpaidauto.actualchangedby = "actual return date added by system after approval from " +
                                                     User.Identity.Name;
                        unpaidauto.actualchangeddateby = DateTime.Now;
                        empreturnapp.Leave.actualchangedby =
                            "actual return date added after approval from " + User.Identity.Name;
                        if (!leavelist.Exists(x =>
                                x.Start_leave == unpaidauto.Start_leave && x.Return_leave == unpaidauto.Return_leave))
                        {
                            this.db.Leaves.Add(unpaidauto);
                            this.db.SaveChanges();
                            empreturnapp.Leave.actualchangedby = "system";
                        }

                        empreturnapp.apstatus = "approved";
                        empreturnapp.approved_byline = "N/A";
                        empreturnapp.approved_byhod = User.Identity.Name;
                        empreturnapp.Leave.actual_return_date = empreturnapp.Leave.Return_leave;
                        empreturnapp.actualreturnleave = empreturnapp.Leave.Return_leave;
                        empreturnapp.Leave.actualchangeddateby = DateTime.Now;
                        db.Entry(empreturnapp).State = EntityState.Modified;
                        db.SaveChanges();
                        SendMail("", "approved", id);
                    }
                }
            }

            back: ;
            if (currentrel.HOD == empjd.employee_id)
            {
                if (empreturnapp.actualreturnleave == empreturnapp.Leave.Return_leave)
                {
                    empreturnapp.apstatus = "approved";
                    empreturnapp.approved_byline = "N/A";
                    empreturnapp.approved_byhod = User.Identity.Name;
                    empreturnapp.Leave.actual_return_date = empreturnapp.actualreturnleave;
                    empreturnapp.Leave.actualchangedby =
                        "actual return date added after approval from " + User.Identity.Name;
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }
                else if (empreturnapp.actualreturnleave > empreturnapp.Leave.Return_leave)
                {
                    var todate = empreturnapp.actualreturnleave;
                    var unpaidauto = new Leave();
                    unpaidauto.leave_type = "6";
                    unpaidauto.Date = DateTime.Now;
                    unpaidauto.Employee_id = empreturnapp.Employee_id;
                    unpaidauto.master_file = empreturnapp.master_file;
                    unpaidauto.Start_leave = empreturnapp.Leave.End_leave.Value.AddDays(1);
                    unpaidauto.End_leave = todate.Value.AddDays(-1);
                    unpaidauto.Return_leave = todate;
                    unpaidauto.actual_return_date = todate;
                    unpaidauto.actualchangedby =
                        "actual return date added by system after approval from " + User.Identity.Name;
                    unpaidauto.actualchangeddateby = DateTime.Now;
                    empreturnapp.Leave.actualchangedby =
                        "actual return date added after approval from " + User.Identity.Name;
                    if (!leavelist.Exists(x =>
                            x.Start_leave == unpaidauto.Start_leave && x.Return_leave == unpaidauto.Return_leave))
                    {
                        this.db.Leaves.Add(unpaidauto);
                        this.db.SaveChanges();
                        empreturnapp.Leave.actualchangedby = "system";
                    }

                    empreturnapp.apstatus = "approved";
                    empreturnapp.approved_byline = "N/A";
                    empreturnapp.approved_byhod = User.Identity.Name;
                    empreturnapp.Leave.actual_return_date = empreturnapp.Leave.Return_leave;
                    empreturnapp.actualreturnleave = empreturnapp.Leave.Return_leave;
                    empreturnapp.Leave.actualchangeddateby = DateTime.Now;
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }

                return RedirectToAction("empreturnap");
            }

            return RedirectToAction("empreturnap");
        }

        /*public ActionResult reject(int id, string message)
        {
            var empreturnapplist = db.empreturnfromleavesubs.ToList();
            var empreturnapp = empreturnapplist.Find(x => x.Id == id);
            if (User.IsInRole("Manager"))
            {
                if (User.IsInRole("HOD"))
                {
                    goto hodtra;
                }
                empreturnapp.apstatus = "rejected by line manager for " + message;
                empreturnapp.approved_byline = User.Identity.Name;
                db.Entry(empreturnapp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("empreturnap");

            }
            hodtra:;
            if (User.IsInRole("HOD") || User.IsInRole("EXTHOD"))
            {

                empreturnapp.apstatus = "rejected by HOD for " + message;
                empreturnapp.approved_byhod = User.Identity.Name;
                db.Entry(empreturnapp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("empreturnap");
            }

            if (User.IsInRole("super_admin"))
            {

                empreturnapp.apstatus = "rejected by HR for " + message;
                empreturnapp.approved_byhod = User.Identity.Name;
                empreturnapp.approved_byline = "N/A";
                db.Entry(empreturnapp).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("empreturnap");
        }*/
        public ActionResult reject(int id, string message)
        {
            var empreturnapplist = db.empreturnfromleavesubs.ToList();
            var empreturnapp = empreturnapplist.Find(x => x.Id == id);
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var relationlist = db.emprels.ToList();
            var empjd = empuser.master_file;
            var logedinsrels = relationlist.FindAll(x => x.line_man == empjd.employee_id || x.HOD == empjd.employee_id);
            var currentrel = logedinsrels.Find(x => x.Employee_id == empreturnapp.Employee_id);
            if (User.IsInRole("super_admin"))
            {
                empreturnapp.apstatus = "rejected by HR for " + message;
                empreturnapp.approved_byhod = User.Identity.Name;
                empreturnapp.approved_byline = "N/A";
                db.Entry(empreturnapp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("empreturnap");
            }

            if (currentrel.line_man == empjd.employee_id)
            {
                if (currentrel.HOD == empjd.employee_id)
                {
                    goto hodtra;
                }

                empreturnapp.apstatus = "rejected by line manager for " + message;
                empreturnapp.approved_byline = User.Identity.Name;
                db.Entry(empreturnapp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("empreturnap");
            }

            hodtra: ;
            if (currentrel.HOD == empjd.employee_id)
            {
                empreturnapp.apstatus = "rejected by HOD for " + message;
                empreturnapp.approved_byhod = User.Identity.Name;
                db.Entry(empreturnapp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("empreturnap");
            }

            return RedirectToAction("empreturnap");
        }

        public ActionResult allpendingapprovals()
        {
            var pendingempapp = db.empreturnfromleavesubs
                .Where(x => x.apstatus == "submitted" || x.apstatus == "approved by line manager" ||
                            x.apstatus == "submitted for HR").ToList();
            var empid = db.master_file.ToList();
            foreach (var pea in pendingempapp)
            {
                var emprellist = db.emprels.ToList();
                var emprel = emprellist.Find(x => x.Employee_id == pea.Employee_id);
                var conlist = db.contracts.ToList();
                var empcon = conlist.Find(x => x.employee_no == pea.Employee_id);
                if (emprel != null)
                {
                    if (emprel.HOD != null)
                    {
                        pea.rel = "line_man";
                    }
                    else
                    {
                        pea.rel = "";
                    }

                    if (pea.rel == "line_man")
                    {
                        if (pea.apstatus == "submitted")
                        {
                            pea.relwho = empid.Find(x => x.employee_id == emprel.line_man).employee_name;
                        }

                        if (pea.apstatus == "approved by line manager")
                        {
                            pea.relwho = empid.Find(x => x.employee_id == emprel.HOD).employee_name;
                        }
                    }
                    else
                    {
                        pea.relwho = empid.Find(x => x.employee_id == emprel.line_man).employee_name;
                    }
                }
                else
                {
                    pea.rel = "";
                    pea.relwho = "no approval flow";
                }

                if (empcon != null)
                {
                    pea.dep = empcon.designation;
                }
                else
                {
                    pea.dep = "";
                }
            }

            return View(pendingempapp);
        }

        public void SendMail(string msg, string action, int returnid)
        {
            var empreturn = db.empreturnfromleavesubs.ToList().Find(x => x.Id == returnid);
            var message = new MimeMessage();
            var emprellist = db.emprels.ToList();
            var emprel = emprellist.Find(x => x.Employee_id == empreturn.Employee_id);
            var userlist = db.AspNetUsers.ToList();
            var usernamelist = db.usernames.ToList();
            var contractlist = db.contracts.OrderByDescending(x => x.date_changed).ToList();
            var desig = "";
            if (contractlist.Exists(x => x.employee_no == empreturn.Employee_id))
            {
                var temp = contractlist.Find(x => x.employee_no == empreturn.Employee_id);
                if (!temp.designation.IsNullOrWhiteSpace())
                {
                    desig = temp.designation;
                }
            }

            var emplusersname = usernamelist.Find(x => x.employee_no == emprel.Employee_id);
            message.From.Add(new MailboxAddress("Hrworks", "leave@citiscapegroup.com"));

            if (action.Equals("submitted"))
            {
                var nextusersname = usernamelist.Find(x => x.employee_no == emprel.line_man);
                if (nextusersname == null)
                {
                    message.To.Add((new MailboxAddress("Yahya Rashid", "yrashid@citiscapegroup.com")));
                    message.Subject = "actual return leave approva(no higher up assigned/no user)";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" +
                               "Please note that actual return date for the leave application of employee (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.master_file.employee_name + "-" + desig +
                               " has been submitted for your approval" + "\n\n\n" +
                               "http://csmain.ddns.net:6333/citiworks" + "\n\n\n" +
                               "Thanks Best Regards, "
                    };
                    goto skip;
                }

                var nextuser = userlist.Find(x => x.Id == nextusersname.aspnet_uid);
                message.To.Add((new MailboxAddress(nextusersname.full_name, nextuser.Email)));
                message.Subject = "actual return leave approval";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" +
                           "Please note that actual return date for the leave application of employee (" +
                           emplusersname.master_file.employee_no + ") " +
                           emplusersname.master_file.employee_name + "-" + desig +
                           " has been submitted for your approval" + "\n\n\n" +
                           "http://csmain.ddns.net:6333/citiworks" + "\n\n\n" +
                           "Thanks Best Regards, "
                };
                skip: ;
            }

            if (action.Equals("submitted for HR"))
            {
                message.To.Add((new MailboxAddress("Yahya Rashid", "yrashid@citiscapegroup.com")));
                message.Subject = "actual return leave approval";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" +
                           "Please note that actual return date for the leave application of employee (" +
                           emplusersname.master_file.employee_no + ") " +
                           emplusersname.master_file.employee_name + "-" + desig +
                           " has been submitted for your approval" + "\n\n\n" +
                           "http://csmain.ddns.net:6333/citiworks" + "\n\n\n" +
                           "Thanks Best Regards, "
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
                    message.Subject = "actual return leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" +
                               "Please note that actual return date for the leave application of employee (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.master_file.employee_name + "-" + desig +
                               " has been approved by line manager " +
                               previoususersname.master_file.employee_name + " and forwarded for your approval" +
                               "\n\n\n" +
                               "http://csmain.ddns.net:6333/citiworks" + "\n\n\n" +
                               "Thanks Best Regards, "
                    };
                }
                else if (action == "approved")
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Cc.Add((new MailboxAddress("Yahya Rashid", "yrashid@citiscapegroup.com")));
                    message.Subject = "actual return leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" +
                               "Please note that actual return date for the leave application of employee (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.master_file.employee_name + "-" + desig + " has been approved" + "\n\n\n" +
                               "http://csmain.ddns.net:6333/citiworks" + "\n\n\n" +
                               "Thanks Best Regards, "
                    };
                }
            }

            if (action.Contains("rejected"))

            {
                if (action.Contains("rejected by line manager"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "actual return leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" +
                               "Please note that actual return date for the leave application of employee (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.master_file.employee_name + "-" + desig +
                               " has been rejected by line manager for " +
                               message + "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }

                if (action.Contains("rejected by HOD"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "actual return leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" +
                               "Please note that actual return date for the leave application of employee (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.master_file.employee_name + "-" + desig +
                               " has been rejected by HOD for " + message +
                               "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }

                if (action.Contains("rejected by HR"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "actual return leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" +
                               "Please note that actual return date for the leave application of employee (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.master_file.employee_name + "-" + desig + " has been rejected by HR for " +
                               message +
                               "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }
            }

            if (message.To.Count != 0)
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("outlook.office365.com", 587, false);
                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("leave@citiscapegroup.com", "Tak98020");
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
        }

        public ActionResult approveall()
        {
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var empreturnsub = new List<empreturnfromleavesub>();
            if (User.IsInRole("super_admin"))
            {
                goto superlogin;
            }

            if (empuser == null)
            {
                goto logend;
            }

            var empjd = empuser.master_file;
            var logedinsrels = db.emprels.ToList()
                .FindAll(x => x.line_man == empjd.employee_id || x.HOD == empjd.employee_id);
            foreach (var emprel in logedinsrels)
            {
                if (emprel.HOD.HasValue && emprel.HOD == empjd.employee_id)
                {
                    empreturnsub.AddRange(db.empreturnfromleavesubs.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "approved by line manager"));
                }

                if (emprel.line_man == empjd.employee_id)
                {
                    empreturnsub.AddRange(db.empreturnfromleavesubs.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "submitted"));
                }
            }

            foreach (var sub in empreturnsub)
            {
                approvepart(sub.Id);
            }

            return RedirectToAction("empreturnap");
            superlogin: ;
            empreturnsub.AddRange(db.empreturnfromleavesubs.ToList().FindAll(x => x.apstatus == "submitted for HR"));
            foreach (var sub in empreturnsub)
            {
                approvepart(sub.Id);
            }

            return RedirectToAction("empreturnap");
            logend: ;
            ViewBag.logend = "forcelogout";
            return RedirectToAction("Index", "Home");
        }


        public void approvepart(int id)
        {
            var empreturnapplist = db.empreturnfromleavesubs.ToList();
            var empreturnapp = empreturnapplist.Find(x => x.Id == id);
            empreturnapp.dateadded = DateTime.Now;
            var leavelist = db.Leaves.Where(x => x.Employee_id == empreturnapp.Employee_id).ToList();
            if (User.IsInRole("super_admin"))
            {
                if (empreturnapp.actualreturnleave == empreturnapp.Leave.Return_leave)
                {
                    empreturnapp.apstatus = "approved";
                    empreturnapp.approved_byline = "N/A";
                    empreturnapp.approved_byhod = User.Identity.Name;
                    empreturnapp.Leave.actual_return_date = empreturnapp.actualreturnleave;
                    empreturnapp.Leave.actualchangedby =
                        "actual return date added after approval from " + User.Identity.Name;
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }
                else if (empreturnapp.actualreturnleave > empreturnapp.Leave.Return_leave)
                {
                    var todate = empreturnapp.actualreturnleave;
                    var unpaidauto = new Leave();
                    unpaidauto.leave_type = "6";
                    unpaidauto.Date = DateTime.Now;
                    unpaidauto.Employee_id = empreturnapp.Employee_id;
                    unpaidauto.master_file = empreturnapp.master_file;
                    unpaidauto.Start_leave = empreturnapp.Leave.End_leave.Value.AddDays(1);
                    unpaidauto.End_leave = todate.Value.AddDays(-1);
                    unpaidauto.Return_leave = todate;
                    unpaidauto.actual_return_date = todate;
                    unpaidauto.actualchangedby =
                        "actual return date added by system after approval from " + User.Identity.Name;
                    unpaidauto.actualchangeddateby = DateTime.Now;
                    empreturnapp.Leave.actualchangedby =
                        "actual return date added after approval from " + User.Identity.Name;
                    if (!leavelist.Exists(x =>
                            x.Start_leave == unpaidauto.Start_leave && x.Return_leave == unpaidauto.Return_leave))
                    {
                        this.db.Leaves.Add(unpaidauto);
                        this.db.SaveChanges();
                        empreturnapp.Leave.actualchangedby = "system";
                    }

                    empreturnapp.apstatus = "approved";
                    empreturnapp.approved_byline = "N/A";
                    empreturnapp.approved_byhod = User.Identity.Name;
                    empreturnapp.Leave.actual_return_date = empreturnapp.Leave.Return_leave;
                    //empreturnapp.actualreturnleave = empreturnapp.Leave.Return_leave;
                    empreturnapp.Leave.actualchangeddateby = DateTime.Now;
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }

                goto end;
            }

            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var relationlist = db.emprels.ToList();
            var empjd = empuser.master_file;
            var logedinsrels = relationlist.FindAll(x => x.line_man == empjd.employee_id || x.HOD == empjd.employee_id);
            var currentrel = logedinsrels.Find(x => x.Employee_id == empreturnapp.Employee_id);
            if (currentrel.line_man == empjd.employee_id)
            {
                if (empreturnapp.apstatus == "approved by line manager")
                {
                    goto back;
                }

                if (currentrel.HOD.HasValue)
                {
                    empreturnapp.apstatus = "approved by line manager";
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved by line manager", id);
                }
                else
                {
                    if (empreturnapp.actualreturnleave == empreturnapp.Leave.Return_leave)
                    {
                        empreturnapp.apstatus = "approved";
                        empreturnapp.approved_byline = "N/A";
                        empreturnapp.approved_byhod = User.Identity.Name;
                        empreturnapp.Leave.actual_return_date = empreturnapp.actualreturnleave;
                        empreturnapp.Leave.actualchangedby =
                            "actual return date added after approval from " + User.Identity.Name;
                        db.Entry(empreturnapp).State = EntityState.Modified;
                        db.SaveChanges();
                        SendMail("", "approved", id);
                        goto end;
                    }
                    else if (empreturnapp.actualreturnleave > empreturnapp.Leave.Return_leave)
                    {
                        var todate = empreturnapp.actualreturnleave;
                        var unpaidauto = new Leave();
                        unpaidauto.leave_type = "6";
                        unpaidauto.Date = DateTime.Now;
                        unpaidauto.Employee_id = empreturnapp.Employee_id;
                        unpaidauto.master_file = empreturnapp.master_file;
                        unpaidauto.Start_leave = empreturnapp.Leave.End_leave.Value.AddDays(1);
                        unpaidauto.End_leave = todate.Value.AddDays(-1);
                        unpaidauto.Return_leave = todate;
                        unpaidauto.actual_return_date = todate;
                        unpaidauto.actualchangedby = "actual return date added by system after approval from " +
                                                     User.Identity.Name;
                        unpaidauto.actualchangeddateby = DateTime.Now;
                        empreturnapp.Leave.actualchangedby =
                            "actual return date added after approval from " + User.Identity.Name;
                        if (!leavelist.Exists(x =>
                                x.Start_leave == unpaidauto.Start_leave && x.Return_leave == unpaidauto.Return_leave))
                        {
                            this.db.Leaves.Add(unpaidauto);
                            this.db.SaveChanges();
                            empreturnapp.Leave.actualchangedby = "system";
                        }

                        empreturnapp.apstatus = "approved";
                        empreturnapp.approved_byline = "N/A";
                        empreturnapp.approved_byhod = User.Identity.Name;
                        empreturnapp.Leave.actual_return_date = empreturnapp.Leave.Return_leave;
                        empreturnapp.actualreturnleave = empreturnapp.Leave.Return_leave;
                        empreturnapp.Leave.actualchangeddateby = DateTime.Now;
                        db.Entry(empreturnapp).State = EntityState.Modified;
                        db.SaveChanges();
                        SendMail("", "approved", id);
                        goto end;
                    }
                }
            }

            back: ;
            if (currentrel.HOD == empjd.employee_id)
            {
                if (empreturnapp.actualreturnleave == empreturnapp.Leave.Return_leave)
                {
                    empreturnapp.apstatus = "approved";
                    empreturnapp.approved_byline = "N/A";
                    empreturnapp.approved_byhod = User.Identity.Name;
                    empreturnapp.Leave.actual_return_date = empreturnapp.actualreturnleave;
                    empreturnapp.Leave.actualchangedby =
                        "actual return date added after approval from " + User.Identity.Name;
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }
                else if (empreturnapp.actualreturnleave > empreturnapp.Leave.Return_leave)
                {
                    var todate = empreturnapp.actualreturnleave;
                    var unpaidauto = new Leave();
                    unpaidauto.leave_type = "6";
                    unpaidauto.Date = DateTime.Now;
                    unpaidauto.Employee_id = empreturnapp.Employee_id;
                    unpaidauto.master_file = empreturnapp.master_file;
                    unpaidauto.Start_leave = empreturnapp.Leave.End_leave.Value.AddDays(1);
                    unpaidauto.End_leave = todate.Value.AddDays(-1);
                    unpaidauto.Return_leave = todate;
                    unpaidauto.actual_return_date = todate;
                    unpaidauto.actualchangedby =
                        "actual return date added by system after approval from " + User.Identity.Name;
                    unpaidauto.actualchangeddateby = DateTime.Now;
                    empreturnapp.Leave.actualchangedby =
                        "actual return date added after approval from " + User.Identity.Name;
                    if (!leavelist.Exists(x =>
                            x.Start_leave == unpaidauto.Start_leave && x.Return_leave == unpaidauto.Return_leave))
                    {
                        this.db.Leaves.Add(unpaidauto);
                        this.db.SaveChanges();
                        empreturnapp.Leave.actualchangedby = "system";
                    }

                    empreturnapp.apstatus = "approved";
                    empreturnapp.approved_byline = "N/A";
                    empreturnapp.approved_byhod = User.Identity.Name;
                    empreturnapp.Leave.actual_return_date = empreturnapp.Leave.Return_leave;
                    empreturnapp.actualreturnleave = empreturnapp.Leave.Return_leave;
                    empreturnapp.Leave.actualchangeddateby = DateTime.Now;
                    db.Entry(empreturnapp).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }
            }

            end: ;
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