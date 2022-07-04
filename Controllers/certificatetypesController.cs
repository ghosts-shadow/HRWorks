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
    public class certificatetypesController : Controller
    {
        private HREntities db = new HREntities();

        // GET: certificatetypes
        public ActionResult Index()
        {
            return View(db.certificatetypes.ToList());
        }

        // GET: certificatetypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            certificatetype certificatetype = db.certificatetypes.Find(id);
            if (certificatetype == null)
            {
                return HttpNotFound();
            }
            return View(certificatetype);
        }

        // GET: certificatetypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: certificatetypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,certificate_name_")] certificatetype certificatetype)
        {
            if (ModelState.IsValid)
            {
                db.certificatetypes.Add(certificatetype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(certificatetype);
        }

        // GET: certificatetypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            certificatetype certificatetype = db.certificatetypes.Find(id);
            if (certificatetype == null)
            {
                return HttpNotFound();
            }
            return View(certificatetype);
        }

        // POST: certificatetypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,certificate_name_")] certificatetype certificatetype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(certificatetype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(certificatetype);
        }

        // GET: certificatetypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            certificatetype certificatetype = db.certificatetypes.Find(id);
            if (certificatetype == null)
            {
                return HttpNotFound();
            }
            return View(certificatetype);
        }

        // POST: certificatetypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            certificatetype certificatetype = db.certificatetypes.Find(id);
            db.certificatetypes.Remove(certificatetype);
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
