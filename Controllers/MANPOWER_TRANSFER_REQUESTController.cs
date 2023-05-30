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
    public class MANPOWER_TRANSFER_REQUESTController : Controller
    {
        private HREntities db = new HREntities();

        // GET: MANPOWER_TRANSFER_REQUEST
        public ActionResult Index()
        {
            var mANPOWER_TRANSFER_REQUEST = db.MANPOWER_TRANSFER_REQUEST.Include(m => m.master_file);
            return View(mANPOWER_TRANSFER_REQUEST.ToList());
        }

        // GET: MANPOWER_TRANSFER_REQUEST/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MANPOWER_TRANSFER_REQUEST mANPOWER_TRANSFER_REQUEST = db.MANPOWER_TRANSFER_REQUEST.Find(id);
            if (mANPOWER_TRANSFER_REQUEST == null)
            {
                return HttpNotFound();
            }
            return View(mANPOWER_TRANSFER_REQUEST);
        }

        // GET: MANPOWER_TRANSFER_REQUEST/Create
        public ActionResult Create()
        {
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: MANPOWER_TRANSFER_REQUEST/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,request_date,pro_from,por_to,effective_date,remarks")] MANPOWER_TRANSFER_REQUEST mANPOWER_TRANSFER_REQUEST)
        {
            if (ModelState.IsValid)
            {
                db.MANPOWER_TRANSFER_REQUEST.Add(mANPOWER_TRANSFER_REQUEST);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", mANPOWER_TRANSFER_REQUEST.Employee_id);
            return View(mANPOWER_TRANSFER_REQUEST);
        }

        // GET: MANPOWER_TRANSFER_REQUEST/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MANPOWER_TRANSFER_REQUEST mANPOWER_TRANSFER_REQUEST = db.MANPOWER_TRANSFER_REQUEST.Find(id);
            if (mANPOWER_TRANSFER_REQUEST == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", mANPOWER_TRANSFER_REQUEST.Employee_id);
            return View(mANPOWER_TRANSFER_REQUEST);
        }

        // POST: MANPOWER_TRANSFER_REQUEST/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,request_date,pro_from,por_to,effective_date,remarks")] MANPOWER_TRANSFER_REQUEST mANPOWER_TRANSFER_REQUEST)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mANPOWER_TRANSFER_REQUEST).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", mANPOWER_TRANSFER_REQUEST.Employee_id);
            return View(mANPOWER_TRANSFER_REQUEST);
        }

        // GET: MANPOWER_TRANSFER_REQUEST/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MANPOWER_TRANSFER_REQUEST mANPOWER_TRANSFER_REQUEST = db.MANPOWER_TRANSFER_REQUEST.Find(id);
            if (mANPOWER_TRANSFER_REQUEST == null)
            {
                return HttpNotFound();
            }
            return View(mANPOWER_TRANSFER_REQUEST);
        }

        // POST: MANPOWER_TRANSFER_REQUEST/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            MANPOWER_TRANSFER_REQUEST mANPOWER_TRANSFER_REQUEST = db.MANPOWER_TRANSFER_REQUEST.Find(id);
            db.MANPOWER_TRANSFER_REQUEST.Remove(mANPOWER_TRANSFER_REQUEST);
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
