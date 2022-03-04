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
    public class employeeleavesubmitionsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: employeeleavesubmitions
        public ActionResult Index()
        {
            var employeeleavesubmitions = db.employeeleavesubmitions.Include(e => e.master_file).ToList();
            var userempnolist = db.usernames.Where(x=>x.employee_no != null).ToList();
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
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: employeeleavesubmitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include =
                "Id,Employee_id,Date,Start_leave,End_leave,Return_leave,leave_type,toltal_requested_days,submitted_by,apstatus,half,approved_byline,approved_byhod")]
            employeeleavesubmition employeeleavesubmition)
        {
            if (ModelState.IsValid)
            {
                db.employeeleavesubmitions.Add(employeeleavesubmition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name",
                employeeleavesubmition.Employee_id);
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