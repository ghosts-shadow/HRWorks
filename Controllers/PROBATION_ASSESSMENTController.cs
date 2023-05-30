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
    public class PROBATION_ASSESSMENTController : Controller
    {
        private HREntities db = new HREntities();

        // GET: PROBATION_ASSESSMENT
        public ActionResult Index()
        {
            var pROBATION_ASSESSMENT = db.PROBATION_ASSESSMENT.Include(p => p.master_file);
            return View(pROBATION_ASSESSMENT.ToList());
        }

        // GET: PROBATION_ASSESSMENT/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROBATION_ASSESSMENT pROBATION_ASSESSMENT = db.PROBATION_ASSESSMENT.Find(id);
            if (pROBATION_ASSESSMENT == null)
            {
                return HttpNotFound();
            }
            return View(pROBATION_ASSESSMENT);
        }

        // GET: PROBATION_ASSESSMENT/Create
        public ActionResult Create()
        {
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: PROBATION_ASSESSMENT/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,employee_id,Knowledge_of_job,Quality_of_work,Achievement_Oriented,Ability_To_Learn,Work_Attitude_and_Co_operation,Ability_To_work_Independently,Reliability,Initiative,employee_excels,improvement_,C_success_and_strengths,further_development,support,Comments,confirming,Line_Managers_Comments,Directors_Comments")] PROBATION_ASSESSMENT pROBATION_ASSESSMENT)
        {
            if (ModelState.IsValid)
            {
                db.PROBATION_ASSESSMENT.Add(pROBATION_ASSESSMENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", pROBATION_ASSESSMENT.employee_id);
            return View(pROBATION_ASSESSMENT);
        }

        // GET: PROBATION_ASSESSMENT/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROBATION_ASSESSMENT pROBATION_ASSESSMENT = db.PROBATION_ASSESSMENT.Find(id);
            if (pROBATION_ASSESSMENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", pROBATION_ASSESSMENT.employee_id);
            return View(pROBATION_ASSESSMENT);
        }

        // POST: PROBATION_ASSESSMENT/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,employee_id,Knowledge_of_job,Quality_of_work,Achievement_Oriented,Ability_To_Learn,Work_Attitude_and_Co_operation,Ability_To_work_Independently,Reliability,Initiative,employee_excels,improvement_,C_success_and_strengths,further_development,support,Comments,confirming,Line_Managers_Comments,Directors_Comments")] PROBATION_ASSESSMENT pROBATION_ASSESSMENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pROBATION_ASSESSMENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", pROBATION_ASSESSMENT.employee_id);
            return View(pROBATION_ASSESSMENT);
        }

        // GET: PROBATION_ASSESSMENT/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROBATION_ASSESSMENT pROBATION_ASSESSMENT = db.PROBATION_ASSESSMENT.Find(id);
            if (pROBATION_ASSESSMENT == null)
            {
                return HttpNotFound();
            }
            return View(pROBATION_ASSESSMENT);
        }

        // POST: PROBATION_ASSESSMENT/Delete/5
        [Authorize(Roles = "super_admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PROBATION_ASSESSMENT pROBATION_ASSESSMENT = db.PROBATION_ASSESSMENT.Find(id);
            db.PROBATION_ASSESSMENT.Remove(pROBATION_ASSESSMENT);
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
