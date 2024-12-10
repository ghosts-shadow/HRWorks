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
    public class transferlistsController : Controller
    {
        private HREntities db = new HREntities();
        private biometrics_DBEntities db1 = new biometrics_DBEntities();

        // GET: transferlists
        public ActionResult Index()
        {
            var transferlists = db.transferlists.Include(t => t.HRprojectlist).Include(t => t.HRprojectlist1).Include(t => t.master_file);
            return View(transferlists.ToList());
        }

        // GET: transferlists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            transferlist transferlist = db.transferlists.Find(id);
            if (transferlist == null)
            {
                return HttpNotFound();
            }
            return View(transferlist);
        }

        // GET: transferlists/Create
        public ActionResult Create()
        {
            ViewBag.cpro_id = new SelectList(db.HRprojectlists, "Id", "project_name");
            ViewBag.npro_id = new SelectList(db.HRprojectlists, "Id", "project_name");
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: transferlists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,cpro_id,npro_id,requested_by,approved_by,effectivedate,dateadded,datemodifief,reason")] transferlist transferlist)
        {
            if (ModelState.IsValid)
            {
                db.transferlists.Add(transferlist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cpro_id = new SelectList(db.HRprojectlists, "Id", "project_name", transferlist.cpro_id);
            ViewBag.npro_id = new SelectList(db.HRprojectlists, "Id", "project_name", transferlist.npro_id);
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", transferlist.Employee_id);
            return View(transferlist);
        }

        // GET: transferlists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            transferlist transferlist = db.transferlists.Find(id);
            if (transferlist == null)
            {
                return HttpNotFound();
            }
            ViewBag.cpro_id = new SelectList(db.HRprojectlists, "Id", "project_name", transferlist.cpro_id);
            ViewBag.npro_id = new SelectList(db.HRprojectlists, "Id", "project_name", transferlist.npro_id);
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", transferlist.Employee_id);
            return View(transferlist);
        }

        // POST: transferlists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,cpro_id,npro_id,requested_by,approved_by,effectivedate,dateadded,datemodifief,reason")] transferlist transferlist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transferlist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cpro_id = new SelectList(db.HRprojectlists, "Id", "project_name", transferlist.cpro_id);
            ViewBag.npro_id = new SelectList(db.HRprojectlists, "Id", "project_name", transferlist.npro_id);
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", transferlist.Employee_id);
            return View(transferlist);
        }

        /*// GET: transferlists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            transferlist transferlist = db.transferlists.Find(id);
            if (transferlist == null)
            {
                return HttpNotFound();
            }
            return View(transferlist);
        }

        // POST: transferlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            transferlist transferlist = db.transferlists.Find(id);
            db.transferlists.Remove(transferlist);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        public ActionResult weekends()
        {
            var hrprojectlist= db.HRprojectlists.ToList();
            var biopeojectlisticlock = db1.iclock_terminal.ToList();
            var biopeojectlist = db1.personnel_area.ToList();
            if (hrprojectlist.Count() != biopeojectlist.Count())
            {
                
            }
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
