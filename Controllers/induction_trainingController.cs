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
    public class induction_trainingController : Controller
    {
        private HREntities db = new HREntities();

        // GET: induction_training
        public ActionResult Index()
        {
            var induction_training = db.induction_training.Include(i => i.master_file);
            return View(induction_training.ToList());
        }

        // GET: induction_training/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            induction_training induction_training = db.induction_training.Find(id);
            if (induction_training == null)
            {
                return HttpNotFound();
            }
            return View(induction_training);
        }

        // GET: induction_training/Create
        public ActionResult Create()
        {
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: induction_training/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,employee_id,C1a,C2a,C3a,C4a,C5a,C6a,C7a,C8a,C9a")] induction_training induction_training)
        {
            if (ModelState.IsValid)
            {
                db.induction_training.Add(induction_training);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", induction_training.employee_id);
            return View(induction_training);
        }

        // GET: induction_training/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            induction_training induction_training = db.induction_training.Find(id);
            if (induction_training == null)
            {
                return HttpNotFound();
            }
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", induction_training.employee_id);
            return View(induction_training);
        }

        // POST: induction_training/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,employee_id,C1a,C2a,C3a,C4a,C5a,C6a,C7a,C8a,C9a")] induction_training induction_training)
        {
            if (ModelState.IsValid)
            {
                db.Entry(induction_training).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", induction_training.employee_id);
            return View(induction_training);
        }

        // GET: induction_training/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            induction_training induction_training = db.induction_training.Find(id);
            if (induction_training == null)
            {
                return HttpNotFound();
            }
            return View(induction_training);
        }

        // POST: induction_training/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "super_admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            induction_training induction_training = db.induction_training.Find(id);
            db.induction_training.Remove(induction_training);
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
