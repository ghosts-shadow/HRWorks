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
    public class lbperyearsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: lbperyears
        public ActionResult Index()
        {
            var lbperyears = db.lbperyears.Include(l => l.master_file);
            return View(lbperyears.ToList());
        }

        // GET: lbperyears/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lbperyear lbperyear = db.lbperyears.Find(id);
            if (lbperyear == null)
            {
                return HttpNotFound();
            }
            return View(lbperyear);
        }

        // GET: lbperyears/Create
        public ActionResult Create()
        {
            ViewBag.Employee_id = new SelectList(db.master_file.OrderBy(x=>x.employee_no), "employee_id", "emiid");
            return View();
        }

        // POST: lbperyears/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,total_leave_balance,year,date_updated,modified_date")] lbperyear lbperyear)
        {
            if (ModelState.IsValid)
            {
                lbperyear.date_updated = DateTime.Now;
                lbperyear.modified_date = DateTime.Now;
                db.lbperyears.Add(lbperyear);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file.OrderBy(x => x.employee_no), "employee_id", "emiid", lbperyear.Employee_id);
            return View(lbperyear);
        }

        // GET: lbperyears/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lbperyear lbperyear = db.lbperyears.Find(id);
            if (lbperyear == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file.OrderBy(x => x.employee_no), "employee_id", "emiid", lbperyear.Employee_id);
            return View(lbperyear);
        }

        // POST: lbperyears/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,total_leave_balance,year,date_updated,modified_date")] lbperyear lbperyear)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lbperyear).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file.OrderBy(x => x.employee_no), "employee_id", "emiid", lbperyear.Employee_id);
            return View(lbperyear);
        }

        // GET: lbperyears/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lbperyear lbperyear = db.lbperyears.Find(id);
            if (lbperyear == null)
            {
                return HttpNotFound();
            }
            return View(lbperyear);
        }

        // POST: lbperyears/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            lbperyear lbperyear = db.lbperyears.Find(id);
            db.lbperyears.Remove(lbperyear);
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
