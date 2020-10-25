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
    public class HRA_requsetController : Controller
    {
        private HREntities db = new HREntities();

        // GET: HRA_requset
        public ActionResult Index()
        {
            return View(db.HRA_requset.ToList());
        }

        // GET: HRA_requset/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HRA_requset hRA_requset = db.HRA_requset.Find(id);
            if (hRA_requset == null)
            {
                return HttpNotFound();
            }
            return View(hRA_requset);
        }

        // GET: HRA_requset/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HRA_requset/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,Amonths,OTdate,remarks,BS,house_RA,HRA_advance_com")] HRA_requset hRA_requset)
        {
            if (ModelState.IsValid)
            {
                db.HRA_requset.Add(hRA_requset);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hRA_requset);
        }

        // GET: HRA_requset/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HRA_requset hRA_requset = db.HRA_requset.Find(id);
            if (hRA_requset == null)
            {
                return HttpNotFound();
            }
            return View(hRA_requset);
        }

        // POST: HRA_requset/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,Amonths,OTdate,remarks,BS,house_RA,HRA_advance_com")] HRA_requset hRA_requset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hRA_requset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hRA_requset);
        }

        // GET: HRA_requset/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HRA_requset hRA_requset = db.HRA_requset.Find(id);
            if (hRA_requset == null)
            {
                return HttpNotFound();
            }
            return View(hRA_requset);
        }

        // POST: HRA_requset/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "super_admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HRA_requset hRA_requset = db.HRA_requset.Find(id);
            db.HRA_requset.Remove(hRA_requset);
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
