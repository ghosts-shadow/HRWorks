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
    public class OVERTIME_APPROVALController : Controller
    {
        private HREntities db = new HREntities();

        // GET: OVERTIME_APPROVAL
        public ActionResult Index()
        {
            return View(db.OVERTIME_APPROVAL.ToList());
        }

        // GET: OVERTIME_APPROVAL/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OVERTIME_APPROVAL oVERTIME_APPROVAL = db.OVERTIME_APPROVAL.Find(id);
            if (oVERTIME_APPROVAL == null)
            {
                return HttpNotFound();
            }
            return View(oVERTIME_APPROVAL);
        }

        // GET: OVERTIME_APPROVAL/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OVERTIME_APPROVAL/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date_Prepared,project,Location,NO_EOTH,start_date,end_date")] OVERTIME_APPROVAL oVERTIME_APPROVAL)
        {
            if (ModelState.IsValid)
            {
                db.OVERTIME_APPROVAL.Add(oVERTIME_APPROVAL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(oVERTIME_APPROVAL);
        }

        // GET: OVERTIME_APPROVAL/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OVERTIME_APPROVAL oVERTIME_APPROVAL = db.OVERTIME_APPROVAL.Find(id);
            if (oVERTIME_APPROVAL == null)
            {
                return HttpNotFound();
            }
            return View(oVERTIME_APPROVAL);
        }

        // POST: OVERTIME_APPROVAL/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date_Prepared,project,Location,NO_EOTH,start_date,end_date")] OVERTIME_APPROVAL oVERTIME_APPROVAL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oVERTIME_APPROVAL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(oVERTIME_APPROVAL);
        }

        // GET: OVERTIME_APPROVAL/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OVERTIME_APPROVAL oVERTIME_APPROVAL = db.OVERTIME_APPROVAL.Find(id);
            if (oVERTIME_APPROVAL == null)
            {
                return HttpNotFound();
            }
            return View(oVERTIME_APPROVAL);
        }

        // POST: OVERTIME_APPROVAL/Delete/5
        [Authorize(Roles = "super_admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OVERTIME_APPROVAL oVERTIME_APPROVAL = db.OVERTIME_APPROVAL.Find(id);
            db.OVERTIME_APPROVAL.Remove(oVERTIME_APPROVAL);
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
