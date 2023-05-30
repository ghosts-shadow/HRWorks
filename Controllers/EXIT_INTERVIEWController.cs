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
    public class EXIT_INTERVIEWController : Controller
    {
        private HREntities db = new HREntities();

        // GET: EXIT_INTERVIEW
        public ActionResult Index()
        {
            return View(db.EXIT_INTERVIEW.ToList());
        }

        // GET: EXIT_INTERVIEW/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXIT_INTERVIEW eXIT_INTERVIEW = db.EXIT_INTERVIEW.Find(id);
            if (eXIT_INTERVIEW == null)
            {
                return HttpNotFound();
            }
            return View(eXIT_INTERVIEW);
        }

        // GET: EXIT_INTERVIEW/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EXIT_INTERVIEW/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,C1B,C2B,C3B,C4B,C5B,C6B,C7B,C8B")] EXIT_INTERVIEW eXIT_INTERVIEW)
        {
            if (ModelState.IsValid)
            {
                db.EXIT_INTERVIEW.Add(eXIT_INTERVIEW);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eXIT_INTERVIEW);
        }

        // GET: EXIT_INTERVIEW/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXIT_INTERVIEW eXIT_INTERVIEW = db.EXIT_INTERVIEW.Find(id);
            if (eXIT_INTERVIEW == null)
            {
                return HttpNotFound();
            }
            return View(eXIT_INTERVIEW);
        }

        // POST: EXIT_INTERVIEW/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,C1B,C2B,C3B,C4B,C5B,C6B,C7B,C8B")] EXIT_INTERVIEW eXIT_INTERVIEW)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eXIT_INTERVIEW).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eXIT_INTERVIEW);
        }

        // GET: EXIT_INTERVIEW/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXIT_INTERVIEW eXIT_INTERVIEW = db.EXIT_INTERVIEW.Find(id);
            if (eXIT_INTERVIEW == null)
            {
                return HttpNotFound();
            }
            return View(eXIT_INTERVIEW);
        }

        // POST: EXIT_INTERVIEW/Delete/5
        [Authorize(Roles = "super_admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EXIT_INTERVIEW eXIT_INTERVIEW = db.EXIT_INTERVIEW.Find(id);
            db.EXIT_INTERVIEW.Remove(eXIT_INTERVIEW);
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
