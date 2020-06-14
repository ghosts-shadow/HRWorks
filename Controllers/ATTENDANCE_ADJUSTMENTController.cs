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
    public class ATTENDANCE_ADJUSTMENTController : Controller
    {
        private HREntities db = new HREntities();

        // GET: ATTENDANCE_ADJUSTMENT
        public ActionResult Index()
        {
            var aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Include(a => a.master_file);
            return View(aTTENDANCE_ADJUSTMENT.ToList());
        }

        // GET: ATTENDANCE_ADJUSTMENT/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Find(id);
            if (aTTENDANCE_ADJUSTMENT == null)
            {
                return HttpNotFound();
            }
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // GET: ATTENDANCE_ADJUSTMENT/Create
        public ActionResult Create()
        {
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: ATTENDANCE_ADJUSTMENT/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,date_prepared,DJ_date,late_in,early_out,reason")] ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT)
        {
            if (ModelState.IsValid)
            {
                db.ATTENDANCE_ADJUSTMENT.Add(aTTENDANCE_ADJUSTMENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", aTTENDANCE_ADJUSTMENT.Employee_id);
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // GET: ATTENDANCE_ADJUSTMENT/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Find(id);
            if (aTTENDANCE_ADJUSTMENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", aTTENDANCE_ADJUSTMENT.Employee_id);
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // POST: ATTENDANCE_ADJUSTMENT/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,date_prepared,DJ_date,late_in,early_out,reason")] ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aTTENDANCE_ADJUSTMENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", aTTENDANCE_ADJUSTMENT.Employee_id);
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // GET: ATTENDANCE_ADJUSTMENT/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Find(id);
            if (aTTENDANCE_ADJUSTMENT == null)
            {
                return HttpNotFound();
            }
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // POST: ATTENDANCE_ADJUSTMENT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Find(id);
            db.ATTENDANCE_ADJUSTMENT.Remove(aTTENDANCE_ADJUSTMENT);
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
