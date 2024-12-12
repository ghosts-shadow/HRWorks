using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using Microsoft.Ajax.Utilities;

namespace HRworks.Controllers
{
    public class FormsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: Forms
        public ActionResult Index()
        {
            return View(db.Forms.ToList());
        }

        // GET: Forms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = db.Forms.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        // GET: Forms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Forms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,form_name,department")] Form form)
        {
            if (ModelState.IsValid)
            {
                db.Forms.Add(form);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(form);
        }

        // GET: Forms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = db.Forms.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        // POST: Forms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,form_name,department")] Form form)
        {
            if (ModelState.IsValid)
            {
                db.Entry(form).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(form);
        }

        // GET: Forms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = db.Forms.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        // POST: Forms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Form form = db.Forms.Find(id);
            db.Forms.Remove(form);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        

        // GET: Forms/Create
        public ActionResult newFormsFlow(int? formId)
        {
            if (formId.HasValue)
            {
                var form = db.Forms.ToList().Find(x => x.Id == formId);
                if (form == null)
                {
                    return RedirectToAction("Index", "Forms");
                }

                ViewBag.formID = form.Id;
                var prealist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList()
                    .GroupBy(x => x.employee_no).Select(s => s.First());
                var alist = prealist
                    .Where(e => e.last_working_day == null)
                    .ToList();

                var afinallist = alist
                    .GroupBy(x => x.employee_no)
                    .Select(g => g.First())
                    .Where(file => file.employee_no != 0 && file.employee_no != 1 && file.employee_no != 100001)
                    .ToList();
                this.ViewBag.employee_no = new SelectList(afinallist, "employee_name", "employee_no", null);
                var authlist = new List<SelectListItem> {
                    new SelectListItem { Value = "line manager", Text = "line manager" },
                    new SelectListItem { Value = "reviewer1", Text = "reviewer1" },
                    new SelectListItem { Value = "reviewer2", Text = "reviewer2" },
                    new SelectListItem { Value = "approver1", Text = "approver1" },
                    new SelectListItem { Value = "approver2", Text = "approver2" }
                };

                this.ViewBag.authlist = new SelectList(authlist, "Value", "Text");
                return View();

            }

            return RedirectToAction("Index", "Forms");
        }

        // POST: Forms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewFormsFlow(FormsFlow formsFlow)
        {
                ViewBag.formID = formsFlow.form_ID;
                var prealist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList()
                    .GroupBy(x => x.employee_no).Select(s => s.First());
                var alist = prealist
                    .Where(e => e.last_working_day == null)
                    .ToList();

                var afinallist = alist
                    .GroupBy(x => x.employee_no)
                    .Select(g => g.First())
                    .Where(file => file.employee_no != 0 && file.employee_no != 1 && file.employee_no != 100001)
                    .ToList();
                this.ViewBag.employee_no = new SelectList(afinallist, "employee_name", "employee_no", null);
                var authlist = new List<SelectListItem> {
                    new SelectListItem { Value = "line manager", Text = "line manager" },
                    new SelectListItem { Value = "reviewer1", Text = "reviewer1" },
                    new SelectListItem { Value = "reviewer2", Text = "reviewer2" },
                    new SelectListItem { Value = "approver1", Text = "approver1" },
                    new SelectListItem { Value = "approver2", Text = "approver2" }
                };

                this.ViewBag.authlist = new SelectList(authlist, "Value", "Text");
            if (ModelState.IsValid)
            {

                var checkformflow = db.FormsFlows.ToList().Find(x => x.form_ID == formsFlow.form_ID);
                if (checkformflow != null)
                {
                    ModelState.AddModelError("originator", "The record already exists.");
                    return View(formsFlow);
                }
                db.FormsFlows.Add(formsFlow);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(formsFlow);
        }
        public ActionResult IndexFormsFlow()
        {

            return View();
        }

        public ActionResult newFormssubmission()
        {

            return View();
        }


        public ActionResult Formstatus()
        {

            return View();
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
