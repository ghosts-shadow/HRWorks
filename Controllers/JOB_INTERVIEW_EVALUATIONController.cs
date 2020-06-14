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
    public class JOB_INTERVIEW_EVALUATIONController : Controller
    {
        private HREntities db = new HREntities();

        // GET: JOB_INTERVIEW_EVALUATION
        public ActionResult Index()
        {
            return View(db.JOB_INTERVIEW_EVALUATION.ToList());
        }

        // GET: JOB_INTERVIEW_EVALUATION/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JOB_INTERVIEW_EVALUATION jOB_INTERVIEW_EVALUATION = db.JOB_INTERVIEW_EVALUATION.Find(id);
            if (jOB_INTERVIEW_EVALUATION == null)
            {
                return HttpNotFound();
            }
            return View(jOB_INTERVIEW_EVALUATION);
        }

        // GET: JOB_INTERVIEW_EVALUATION/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: JOB_INTERVIEW_EVALUATION/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,interviewee_name,interview_date,job_applied_for,department,education,experience,job_knoledge,conceptual_clarity,appearance,communication,confidence_level,attitude,current_salary,expected_salary,reason_F_L_P_C,overall_evaluation,recommented_to_be_employed,in_position_of,employment_in_the_future")] JOB_INTERVIEW_EVALUATION jOB_INTERVIEW_EVALUATION)
        {
            if (ModelState.IsValid)
            {
                db.JOB_INTERVIEW_EVALUATION.Add(jOB_INTERVIEW_EVALUATION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(jOB_INTERVIEW_EVALUATION);
        }

        // GET: JOB_INTERVIEW_EVALUATION/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JOB_INTERVIEW_EVALUATION jOB_INTERVIEW_EVALUATION = db.JOB_INTERVIEW_EVALUATION.Find(id);
            if (jOB_INTERVIEW_EVALUATION == null)
            {
                return HttpNotFound();
            }
            return View(jOB_INTERVIEW_EVALUATION);
        }

        // POST: JOB_INTERVIEW_EVALUATION/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,interviewee_name,interview_date,job_applied_for,department,education,experience,job_knoledge,conceptual_clarity,appearance,communication,confidence_level,attitude,current_salary,expected_salary,reason_F_L_P_C,overall_evaluation,recommented_to_be_employed,in_position_of,employment_in_the_future")] JOB_INTERVIEW_EVALUATION jOB_INTERVIEW_EVALUATION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jOB_INTERVIEW_EVALUATION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jOB_INTERVIEW_EVALUATION);
        }

        // GET: JOB_INTERVIEW_EVALUATION/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JOB_INTERVIEW_EVALUATION jOB_INTERVIEW_EVALUATION = db.JOB_INTERVIEW_EVALUATION.Find(id);
            if (jOB_INTERVIEW_EVALUATION == null)
            {
                return HttpNotFound();
            }
            return View(jOB_INTERVIEW_EVALUATION);
        }

        // POST: JOB_INTERVIEW_EVALUATION/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JOB_INTERVIEW_EVALUATION jOB_INTERVIEW_EVALUATION = db.JOB_INTERVIEW_EVALUATION.Find(id);
            db.JOB_INTERVIEW_EVALUATION.Remove(jOB_INTERVIEW_EVALUATION);
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
