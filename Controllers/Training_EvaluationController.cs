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
    public class Training_EvaluationController : Controller
    {
        private HREntities db = new HREntities();

        // GET: Training_Evaluation
        public ActionResult Index()
        {
            var training_Evaluation = db.Training_Evaluation.Include(t => t.master_file);
            return View(training_Evaluation.ToList());
        }

        // GET: Training_Evaluation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training_Evaluation training_Evaluation = db.Training_Evaluation.Find(id);
            if (training_Evaluation == null)
            {
                return HttpNotFound();
            }
            return View(training_Evaluation);
        }

        // GET: Training_Evaluation/Create
        public ActionResult Create()
        {
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: Training_Evaluation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,traning_prog,traning_date,TEV_1_1,TEV_1_2,TEV_1_3,TEV_1_4,TEV_1_5,TEV_2_1,TEV_2_2,TEV_2_3,TEV_2_4,TEV_2_5,TEV_3_1,TEV_3_2,TEV_3_3,TEV_3_4,TEV_3_5,TEV_4_1,TEV_4_2,TEV_4_3,TEV_4_4,TEV_4_5,TEV_5_1,TEV_5_2,enhancements")] Training_Evaluation training_Evaluation)
        {
            if (ModelState.IsValid)
            {
                db.Training_Evaluation.Add(training_Evaluation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", training_Evaluation.Employee_id);
            return View(training_Evaluation);
        }

        // GET: Training_Evaluation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training_Evaluation training_Evaluation = db.Training_Evaluation.Find(id);
            if (training_Evaluation == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", training_Evaluation.Employee_id);
            return View(training_Evaluation);
        }

        // POST: Training_Evaluation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,traning_prog,traning_date,TEV_1_1,TEV_1_2,TEV_1_3,TEV_1_4,TEV_1_5,TEV_2_1,TEV_2_2,TEV_2_3,TEV_2_4,TEV_2_5,TEV_3_1,TEV_3_2,TEV_3_3,TEV_3_4,TEV_3_5,TEV_4_1,TEV_4_2,TEV_4_3,TEV_4_4,TEV_4_5,TEV_5_1,TEV_5_2,enhancements")] Training_Evaluation training_Evaluation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(training_Evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", training_Evaluation.Employee_id);
            return View(training_Evaluation);
        }

        // GET: Training_Evaluation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training_Evaluation training_Evaluation = db.Training_Evaluation.Find(id);
            if (training_Evaluation == null)
            {
                return HttpNotFound();
            }
            return View(training_Evaluation);
        }

        // POST: Training_Evaluation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Training_Evaluation training_Evaluation = db.Training_Evaluation.Find(id);
            db.Training_Evaluation.Remove(training_Evaluation);
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
