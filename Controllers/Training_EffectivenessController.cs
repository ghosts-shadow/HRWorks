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
    public class Training_EffectivenessController : Controller
    {
        private HREntities db = new HREntities();

        // GET: Training_Effectiveness
        public ActionResult Index()
        {
            var training_Effectiveness = db.Training_Effectiveness.Include(t => t.master_file);
            return View(training_Effectiveness.ToList());
        }

        // GET: Training_Effectiveness/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training_Effectiveness training_Effectiveness = db.Training_Effectiveness.Find(id);
            if (training_Effectiveness == null)
            {
                return HttpNotFound();
            }
            return View(training_Effectiveness);
        }

        // GET: Training_Effectiveness/Create
        public ActionResult Create()
        {
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: Training_Effectiveness/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,traning_prog,traning_date,line_manager,prog_objective,emp_work_per,emp_work_att,emp_work_team,RC_otheremp,RC_FDporg,comments")] Training_Effectiveness training_Effectiveness)
        {
            if (ModelState.IsValid)
            {
                db.Training_Effectiveness.Add(training_Effectiveness);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", training_Effectiveness.Employee_id);
            return View(training_Effectiveness);
        }

        // GET: Training_Effectiveness/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training_Effectiveness training_Effectiveness = db.Training_Effectiveness.Find(id);
            if (training_Effectiveness == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", training_Effectiveness.Employee_id);
            return View(training_Effectiveness);
        }

        // POST: Training_Effectiveness/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,traning_prog,traning_date,line_manager,prog_objective,emp_work_per,emp_work_att,emp_work_team,RC_otheremp,RC_FDporg,comments")] Training_Effectiveness training_Effectiveness)
        {
            if (ModelState.IsValid)
            {
                db.Entry(training_Effectiveness).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", training_Effectiveness.Employee_id);
            return View(training_Effectiveness);
        }

        // GET: Training_Effectiveness/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training_Effectiveness training_Effectiveness = db.Training_Effectiveness.Find(id);
            if (training_Effectiveness == null)
            {
                return HttpNotFound();
            }
            return View(training_Effectiveness);
        }

        // POST: Training_Effectiveness/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Training_Effectiveness training_Effectiveness = db.Training_Effectiveness.Find(id);
            db.Training_Effectiveness.Remove(training_Effectiveness);
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
