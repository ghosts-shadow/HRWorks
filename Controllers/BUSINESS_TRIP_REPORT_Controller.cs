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
    public class BUSINESS_TRIP_REPORT_Controller : Controller
    {
        private HREntities db = new HREntities();

        // GET: BUSINESS_TRIP_REPORT_
        public ActionResult Index()
        {
            var bUSINESS_TRIP_REPORT_ = db.BUSINESS_TRIP_REPORT_.Include(b => b.master_file);
            return View(bUSINESS_TRIP_REPORT_.ToList());
        }

        // GET: BUSINESS_TRIP_REPORT_/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BUSINESS_TRIP_REPORT_ bUSINESS_TRIP_REPORT_ = db.BUSINESS_TRIP_REPORT_.Find(id);
            if (bUSINESS_TRIP_REPORT_ == null)
            {
                return HttpNotFound();
            }
            return View(bUSINESS_TRIP_REPORT_);
        }

        // GET: BUSINESS_TRIP_REPORT_/Create
        public ActionResult Create()
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.Employee_id = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.Employee_id1 = new SelectList(afinallist.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View();
        }

        // POST: BUSINESS_TRIP_REPORT_/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,date,type_of_trip,IO_specify,destination,Departure_Date,Return_Date,TRIP_OBJECTIVE,Objectives")] BUSINESS_TRIP_REPORT_ bUSINESS_TRIP_REPORT_)
        {
            if (ModelState.IsValid)
            {
                db.BUSINESS_TRIP_REPORT_.Add(bUSINESS_TRIP_REPORT_);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", bUSINESS_TRIP_REPORT_.Employee_id);
            return View(bUSINESS_TRIP_REPORT_);
        }

        // GET: BUSINESS_TRIP_REPORT_/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BUSINESS_TRIP_REPORT_ bUSINESS_TRIP_REPORT_ = db.BUSINESS_TRIP_REPORT_.Find(id);
            if (bUSINESS_TRIP_REPORT_ == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", bUSINESS_TRIP_REPORT_.Employee_id);
            return View(bUSINESS_TRIP_REPORT_);
        }

        // POST: BUSINESS_TRIP_REPORT_/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,date,type_of_trip,IO_specify,destination,Departure_Date,Return_Date,TRIP_OBJECTIVE,Objectives")] BUSINESS_TRIP_REPORT_ bUSINESS_TRIP_REPORT_)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bUSINESS_TRIP_REPORT_).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", bUSINESS_TRIP_REPORT_.Employee_id);
            return View(bUSINESS_TRIP_REPORT_);
        }

        // GET: BUSINESS_TRIP_REPORT_/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BUSINESS_TRIP_REPORT_ bUSINESS_TRIP_REPORT_ = db.BUSINESS_TRIP_REPORT_.Find(id);
            if (bUSINESS_TRIP_REPORT_ == null)
            {
                return HttpNotFound();
            }
            return View(bUSINESS_TRIP_REPORT_);
        }

        // POST: BUSINESS_TRIP_REPORT_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            BUSINESS_TRIP_REPORT_ bUSINESS_TRIP_REPORT_ = db.BUSINESS_TRIP_REPORT_.Find(id);
            db.BUSINESS_TRIP_REPORT_.Remove(bUSINESS_TRIP_REPORT_);
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
