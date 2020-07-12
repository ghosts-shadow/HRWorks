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
    public class emp_joi_sta_changeController : Controller
    {
        private HREntities db = new HREntities();

        // GET: emp_joi_sta_change
        public ActionResult Index()
        {
            var emp_joi_sta_change = db.emp_joi_sta_change.Include(e => e.master_file);
            return View(emp_joi_sta_change.ToList());
        }

        // GET: emp_joi_sta_change/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emp_joi_sta_change emp_joi_sta_change = db.emp_joi_sta_change.Find(id);
            if (emp_joi_sta_change == null)
            {
                return HttpNotFound();
            }
            return View(emp_joi_sta_change);
        }

        // GET: emp_joi_sta_change/Create
        public ActionResult Create()
        {
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: emp_joi_sta_change/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,action_active,New_Title,Action_Taken,Action_Taken2,Basic_Salary,House_Rent_Allowance,Telephone_Allowance,Living_Allowance,Transportation_Allowance,Other_Allowances,Total_Salary,Medical_Insurance_Policy,Annual_Leave,Bank_Name_Branch,Bank_Account_No,Remarks")] emp_joi_sta_change emp_joi_sta_change)
        {
            if (ModelState.IsValid)
            {
                db.emp_joi_sta_change.Add(emp_joi_sta_change);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", emp_joi_sta_change.Employee_id);
            return View(emp_joi_sta_change);
        }

        // GET: emp_joi_sta_change/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emp_joi_sta_change emp_joi_sta_change = db.emp_joi_sta_change.Find(id);
            if (emp_joi_sta_change == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", emp_joi_sta_change.Employee_id);
            return View(emp_joi_sta_change);
        }

        // POST: emp_joi_sta_change/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,action_active,New_Title,Action_Taken,Action_Taken2,Basic_Salary,House_Rent_Allowance,Telephone_Allowance,Living_Allowance,Transportation_Allowance,Other_Allowances,Total_Salary,Medical_Insurance_Policy,Annual_Leave,Bank_Name_Branch,Bank_Account_No,Remarks")] emp_joi_sta_change emp_joi_sta_change)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emp_joi_sta_change).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", emp_joi_sta_change.Employee_id);
            return View(emp_joi_sta_change);
        }

        // GET: emp_joi_sta_change/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emp_joi_sta_change emp_joi_sta_change = db.emp_joi_sta_change.Find(id);
            if (emp_joi_sta_change == null)
            {
                return HttpNotFound();
            }
            return View(emp_joi_sta_change);
        }

        // POST: emp_joi_sta_change/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            emp_joi_sta_change emp_joi_sta_change = db.emp_joi_sta_change.Find(id);
            db.emp_joi_sta_change.Remove(emp_joi_sta_change);
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
