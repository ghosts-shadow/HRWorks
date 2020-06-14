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
    public class Training_RequestController : Controller
    {
        private HREntities db = new HREntities();

        // GET: Training_Request
        public ActionResult Index()
        {
            var training_Request = db.Training_Request.Include(t => t.master_file);
            return View(training_Request.ToList());
        }

        // GET: Training_Request/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training_Request training_Request = db.Training_Request.Find(id);
            if (training_Request == null)
            {
                return HttpNotFound();
            }
            return View(training_Request);
        }

        // GET: Training_Request/Create
        public ActionResult Create()
        {
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: Training_Request/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,TR_date,line_manager,e_mail_address,contact__no,course_name,training_provider,location,program_type,duration_from,duration_to,C_S_description,TR_objactive,TR_competencies,comments,budget,TE_program_cost_desc,TE_program_cost_price,TE_transportation_desc,TE_transportation_price,TE_accom_desc,TE_accom_price,TE_diem_desc,TE_diem_price,TE_total_desc,TE_total_price,HR_comments")] Training_Request training_Request)
        {
            if (ModelState.IsValid)
            {
                db.Training_Request.Add(training_Request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", training_Request.Employee_id);
            return View(training_Request);
        }

        // GET: Training_Request/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training_Request training_Request = db.Training_Request.Find(id);
            if (training_Request == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", training_Request.Employee_id);
            return View(training_Request);
        }

        // POST: Training_Request/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,TR_date,line_manager,e_mail_address,contact__no,course_name,training_provider,location,program_type,duration_from,duration_to,C_S_description,TR_objactive,TR_competencies,comments,budget,TE_program_cost_desc,TE_program_cost_price,TE_transportation_desc,TE_transportation_price,TE_accom_desc,TE_accom_price,TE_diem_desc,TE_diem_price,TE_total_desc,TE_total_price,HR_comments")] Training_Request training_Request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(training_Request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", training_Request.Employee_id);
            return View(training_Request);
        }

        // GET: Training_Request/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training_Request training_Request = db.Training_Request.Find(id);
            if (training_Request == null)
            {
                return HttpNotFound();
            }
            return View(training_Request);
        }

        // POST: Training_Request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Training_Request training_Request = db.Training_Request.Find(id);
            db.Training_Request.Remove(training_Request);
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
