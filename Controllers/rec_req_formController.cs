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
    public class rec_req_formController : Controller
    {
        private HREntities db = new HREntities();

        // GET: rec_req_form
        public ActionResult Index()
        {
            return View(db.rec_req_form.ToList());
        }

        // GET: rec_req_form/Details/5
        public ActionResult Details()
        {
          
            var rec_req_form = new rec_req_form();
            if (db.rec_req_form.Count() != 0)
            {
                var rrflist = db.rec_req_form.ToList();
                rec_req_form = rrflist.Last();
            }
            return View(rec_req_form);
        }

        // GET: rec_req_form/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: rec_req_form/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,projobtitle,department,reporting_to,type_of_hire,new_hire,justification_NH,proposed_salary,replacement_name,gross_salary,J_P_specification,age_range_from,edu_qua_and_ex_required,add_req_job_resp")] rec_req_form rec_req_form)
        {
            if (ModelState.IsValid)
            {
                db.rec_req_form.Add(rec_req_form);
                db.SaveChanges();
                return RedirectToAction("Details");
            }

            return View(rec_req_form);
        }

        // GET: rec_req_form/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rec_req_form rec_req_form = db.rec_req_form.Find(id);
            if (rec_req_form == null)
            {
                return HttpNotFound();
            }
            return View(rec_req_form);
        }

        // POST: rec_req_form/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,projobtitle,department,reporting_to,type_of_hire,new_hire,justification_NH,proposed_salary,replacement_name,gross_salary,J_P_specification,age_range_from,edu_qua_and_ex_required,add_req_job_resp")] rec_req_form rec_req_form)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rec_req_form).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rec_req_form);
        }

        [Authorize(Roles = "super_admin")]
        // GET: rec_req_form/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rec_req_form rec_req_form = db.rec_req_form.Find(id);
            if (rec_req_form == null)
            {
                return HttpNotFound();
            }
            return View(rec_req_form);
        }

        // POST: rec_req_form/Delete/5
        [Authorize(Roles = "super_admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            rec_req_form rec_req_form = db.rec_req_form.Find(id);
            db.rec_req_form.Remove(rec_req_form);
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
