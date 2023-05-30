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
    public class liquiexpsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: liquiexps
        public ActionResult Index()
        {
            return View(db.liquiexps.ToList());
        }

        // GET: liquiexps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            liquiexp liquiexp = db.liquiexps.Find(id);
            if (liquiexp == null)
            {
                return HttpNotFound();
            }
            return View(liquiexp);
        }

        // GET: liquiexps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: liquiexps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,expence,issuer")] liquiexp liquiexp)
        {
            if (ModelState.IsValid)
            {
                liquiexp.expence = liquiexp.expence.ToUpper();
                liquiexp.issuer = liquiexp.issuer.ToUpper();
                db.liquiexps.Add(liquiexp);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(liquiexp);
        }

        // GET: liquiexps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            liquiexp liquiexp = db.liquiexps.Find(id);
            if (liquiexp == null)
            {
                return HttpNotFound();
            }
            return View(liquiexp);
        }

        // POST: liquiexps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,expence,issuer")] liquiexp liquiexp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liquiexp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(liquiexp);
        }

        // GET: liquiexps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            liquiexp liquiexp = db.liquiexps.Find(id);
            if (liquiexp == null)
            {
                return HttpNotFound();
            }
            return View(liquiexp);
        }

        // POST: liquiexps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            liquiexp liquiexp = db.liquiexps.Find(id);
            db.liquiexps.Remove(liquiexp);
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
