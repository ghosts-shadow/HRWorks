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
    public class companLeaveRsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: companLeaveRs
        public ActionResult Index()
        {
            var companLeaveRs = db.companLeaveRs.Include(c => c.master_file);
            return View(companLeaveRs.ToList());
        }

        // GET: companLeaveRs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            companLeaveR companLeaveR = db.companLeaveRs.Find(id);
            if (companLeaveR == null)
            {
                return HttpNotFound();
            }
            return View(companLeaveR);
        }

        // GET: companLeaveRs/Create
        public ActionResult Create()
        {
            var mancontrollar = new master_fileController();
            var afinallist = mancontrollar.emplist();
            ViewBag.EmployeeList = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "emiid");
            ViewBag.HowManyHours = new SelectList(Enumerable.Range(1, 12).Select(x => new { Value = x, Text = x + " Hr(s)" }), "Value", "Text");
            return View();
        }

        // POST: companLeaveRs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ForWhichDate,dateModified,addedBy,EmpNo,is_halfday_included")] companLeaveR companLeaveR)
        {
            if (ModelState.IsValid)
            {
                var compballist = db.companleaveBals.ToList();
                if (compballist.Exists(x=>x.EmpNo == companLeaveR.EmpNo))
                {
                    var combal = compballist.Find(x=>x.EmpNo == companLeaveR.EmpNo);
                    if (companLeaveR.is_halfday_included)
                    {
                        combal.balance += 0.5d;
                    }
                    else
                    {
                        combal.balance += 1;

                    }
                }
                else
                {
                    var combal = new companleaveBal();
                    if (companLeaveR.is_halfday_included)
                    {
                        combal.balance = 0.5d;
                    }
                    else
                    {
                        combal.balance = 1;

                    }
                    combal.EmpNo = companLeaveR.EmpNo;
                    combal.dateModified = DateTime.Now;
                    db.companleaveBals.Add(combal);
                    db.SaveChanges();
                }

                companLeaveR.addedBy = "";
                companLeaveR.dateModified = DateTime.Now;
                db.companLeaveRs.Add(companLeaveR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            ViewBag.EmpNo = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", companLeaveR.EmpNo);
            return View(companLeaveR);
        }*/


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(compensatorylist compenLeaveRlist)
        {
            if (ModelState.IsValid)
            {
                var errorlist = new List<string>();
                foreach (var compenLeaveR in compenLeaveRlist.companLeaveRlist)
                {
                    var comlerslist = db.companLeaveRs.ToList();
                    var comleblist = db.companleaveBals.ToList();
                    var compenbal = new companleaveBal();
                    if (!comlerslist.Exists(x =>
                            x.EmpNo == compenLeaveR.EmpNo && x.ForWhichDate == compenLeaveR.ForWhichDate))
                    {
                        if (comleblist.Exists(x => x.EmpNo == compenLeaveR.EmpNo))
                        {
                            compenbal = comleblist.Find(x => x.EmpNo == compenLeaveR.EmpNo);
                            if (compenbal.hrs == null)
                            {
                                compenbal.hrs = 0;
                            }

                            compenbal.hrs += compenLeaveR.how_many_hrs;
                            if (compenbal.hrs >= 4)
                            {
                                var newHrbal = (compenbal.hrs % 4);
                                compenbal.balance += (compenbal.hrs - newHrbal) / 4 / 2;
                                compenbal.hrs = newHrbal;
                            }

                            compenLeaveR.addedBy = User.Identity.Name;
                            compenbal.dateModified = DateTime.Now;
                            db.Entry(compenbal).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            compenbal.EmpNo = compenLeaveR.EmpNo;
                            compenbal.hrs = 0;
                            compenbal.balance = 0;
                            compenbal.hrs += compenLeaveR.how_many_hrs;
                            if (compenbal.hrs >= 4)
                            {
                                var newHrbal = (compenbal.hrs % 4);
                                compenbal.balance += (compenbal.hrs - newHrbal) / 4 / 2;
                                compenbal.hrs = newHrbal;
                            }

                            compenbal.dateModified = DateTime.Now;
                            db.companleaveBals.Add(compenbal);
                            db.SaveChanges();
                        }

                        compenLeaveR.addedBy = User.Identity.Name;
                        compenLeaveR.dateModified = DateTime.Now;
                        db.companLeaveRs.Add(compenLeaveR);
                        db.SaveChanges();
                    }
                    else
                    {
                        errorlist.Add(
                            compenLeaveR.EmpNo.ToString() + " " + compenLeaveR.ForWhichDate + " already exist");
                    }
                }

                return RedirectToAction("Index");
            }

            var mancontrollar = new master_fileController();
            var afinallist = mancontrollar.emplist(); 
            ViewBag.EmployeeList = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "emiid", compenLeaveRlist.companLeaveRlist.FirstOrDefault().EmpNo);
            ViewBag.HowManyHours = new SelectList(Enumerable.Range(1, 12).Select(x => new { Value = x, Text = x + " Hr(s)" }), "Value", "Text");
            return View(compenLeaveRlist);
        }


        /*
        // GET: companLeaveRs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            companLeaveR companLeaveR = db.companLeaveRs.Find(id);
            if (companLeaveR == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpNo = new SelectList(db.master_file, "employee_id", "employee_name", companLeaveR.EmpNo);
            return View(companLeaveR);
        }

        // POST: companLeaveRs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ForWhichDate,dateModified,addedBy,EmpNo")] companLeaveR companLeaveR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companLeaveR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpNo = new SelectList(db.master_file, "employee_id", "employee_name", companLeaveR.EmpNo);
            return View(companLeaveR);
        }
        */

        // GET: companLeaveRs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            companLeaveR companLeaveR = db.companLeaveRs.Find(id);
            if (companLeaveR == null)
            {
                return HttpNotFound();
            }
            return View(companLeaveR);
        }


        // POST: companLeaveRs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            companLeaveR companLeaveR = db.companLeaveRs.Find(id);
            var compballist = db.companleaveBals.ToList();
            if (compballist.Exists(x => x.EmpNo == companLeaveR.EmpNo))
            {
                var combal = compballist.Find(x => x.EmpNo == companLeaveR.EmpNo);
                if (!combal.hrs.HasValue)
                {
                    combal.hrs = 0;
                }

                if (combal.balance != 0)
                {
                    combal.hrs += combal.balance * 8;

                    if (combal.hrs < 0)
                    {
                        ModelState.AddModelError("ForWhichDate", "insufficient balance remaining to delete record");
                        return View(companLeaveR);
                    }
                    combal.hrs -= companLeaveR.how_many_hrs;

                    var newHrbal = (combal.hrs % 4);
                    combal.balance = (combal.hrs - newHrbal) / 4 / 2;
                    combal.hrs = newHrbal;

                    if (combal.hrs < 0)
                    {
                        ModelState.AddModelError("ForWhichDate", "insufficient balance remaining to delete record");
                        return View(companLeaveR);
                    }

                }
                else
                {
                    if (combal.hrs == 0)
                    {
                        ModelState.AddModelError("ForWhichDate", "no balance remaining to delete record");
                        return View(companLeaveR);
                    }
                    else
                    {
                        combal.hrs -= companLeaveR.how_many_hrs;
                        if (combal.hrs < 0)
                        {
                            ModelState.AddModelError("ForWhichDate", "insufficient balance remaining to delete record");
                            return View(companLeaveR);
                        }
                    }
                }

                db.Entry(combal).State = EntityState.Modified;
                db.companLeaveRs.Remove(companLeaveR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(companLeaveR);
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
