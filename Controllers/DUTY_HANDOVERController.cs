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
    public class DUTY_HANDOVERController : Controller
    {
        private HREntities db = new HREntities();

        // GET: DUTY_HANDOVER
        public ActionResult Index()
        {
            var dUTY_HANDOVER = db.DUTY_HANDOVER.Include(d => d.master_file).Include(d => d.master_file1);
            return View(dUTY_HANDOVER.ToList());
        }

        // GET: DUTY_HANDOVER/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DUTY_HANDOVER dUTY_HANDOVER = db.DUTY_HANDOVER.Find(id);
            if (dUTY_HANDOVER == null)
            {
                return HttpNotFound();
            }
            return View(dUTY_HANDOVER);
        }

        // GET: DUTY_HANDOVER/Create
        public ActionResult Create()
        {
            ViewBag.Employee_NO1 = new SelectList(db.master_file, "employee_id", "employee_name");
            ViewBag.Employee_NO2 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: DUTY_HANDOVER/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_NO1,Employee_NO2,leave_start,leave_end,reason_for_handover,RNR_HO1,RNR_HO2,RNR_HO3,RNR_HO4,RNR_HO5")] DUTY_HANDOVER dUTY_HANDOVER)
        {
            if (ModelState.IsValid)
            {
                db.DUTY_HANDOVER.Add(dUTY_HANDOVER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_NO1 = new SelectList(db.master_file, "employee_id", "employee_name", dUTY_HANDOVER.Employee_NO1);
            ViewBag.Employee_NO2 = new SelectList(db.master_file, "employee_id", "employee_name", dUTY_HANDOVER.Employee_NO2);
            return View(dUTY_HANDOVER);
        }

        // GET: DUTY_HANDOVER/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DUTY_HANDOVER dUTY_HANDOVER = db.DUTY_HANDOVER.Find(id);
            if (dUTY_HANDOVER == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_NO1 = new SelectList(db.master_file, "employee_id", "employee_name", dUTY_HANDOVER.Employee_NO1);
            ViewBag.Employee_NO2 = new SelectList(db.master_file, "employee_id", "employee_name", dUTY_HANDOVER.Employee_NO2);
            return View(dUTY_HANDOVER);
        }

        // POST: DUTY_HANDOVER/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_NO1,Employee_NO2,leave_start,leave_end,reason_for_handover,RNR_HO1,RNR_HO2,RNR_HO3,RNR_HO4,RNR_HO5")] DUTY_HANDOVER dUTY_HANDOVER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dUTY_HANDOVER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_NO1 = new SelectList(db.master_file, "employee_id", "employee_name", dUTY_HANDOVER.Employee_NO1);
            ViewBag.Employee_NO2 = new SelectList(db.master_file, "employee_id", "employee_name", dUTY_HANDOVER.Employee_NO2);
            return View(dUTY_HANDOVER);
        }

        [Authorize(Roles = "super_admin")]
        // GET: DUTY_HANDOVER/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DUTY_HANDOVER dUTY_HANDOVER = db.DUTY_HANDOVER.Find(id);
            if (dUTY_HANDOVER == null)
            {
                return HttpNotFound();
            }
            return View(dUTY_HANDOVER);
        }

        // POST: DUTY_HANDOVER/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "super_admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DUTY_HANDOVER dUTY_HANDOVER = db.DUTY_HANDOVER.Find(id);
            db.DUTY_HANDOVER.Remove(dUTY_HANDOVER);
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
