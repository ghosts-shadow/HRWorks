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
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            ViewBag.EmpNo = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            return View();
        }

        // POST: companLeaveRs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ForWhichDate,dateModified,addedBy,EmpNo")] companLeaveR companLeaveR)
        {
            if (ModelState.IsValid)
            {
                var compballist = db.companleaveBals.ToList();
                if (compballist.Exists(x=>x.EmpNo == companLeaveR.EmpNo))
                {
                    var combal = compballist.Find(x=>x.EmpNo == companLeaveR.EmpNo);
                    combal.balance += 1;
                }
                else
                {
                    var combal = new companleaveBal();
                    combal.balance = 1;
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
                combal.balance -= 1;
            }
            db.companLeaveRs.Remove(companLeaveR);
            db.SaveChanges();
            return RedirectToAction("Index");
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
