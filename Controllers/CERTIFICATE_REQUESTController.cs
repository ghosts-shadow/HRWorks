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
    public class CERTIFICATE_REQUESTController : Controller
    {
        private HREntities db = new HREntities();

        // GET: CERTIFICATE_REQUEST
        public ActionResult Index()
        {
            var cERTIFICATE_REQUEST = db.CERTIFICATE_REQUEST.Include(c => c.master_file);
            return View(cERTIFICATE_REQUEST.ToList());
        }

        // GET: CERTIFICATE_REQUEST/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CERTIFICATE_REQUEST cERTIFICATE_REQUEST = db.CERTIFICATE_REQUEST.Find(id);
            if (cERTIFICATE_REQUEST == null)
            {
                return HttpNotFound();
            }
            return View(cERTIFICATE_REQUEST);
        }

        // GET: CERTIFICATE_REQUEST/Create
        public ActionResult Create()
        {
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: CERTIFICATE_REQUEST/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,employee_id,LETTER_TYPE,MENTION_SALARY,BL_bank_nane,BL_account_no,EL_name,EL_purpose,EL_DOV_from,EL_DOV_to,TD_NCARREG,TD_CARREGREN,TD_DLA,TDLREN,other,Etisalat,ADWEA,Mawaqif,Immigration")] CERTIFICATE_REQUEST cERTIFICATE_REQUEST)
        {
            if (ModelState.IsValid)
            {
                db.CERTIFICATE_REQUEST.Add(cERTIFICATE_REQUEST);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", cERTIFICATE_REQUEST.employee_id);
            return View(cERTIFICATE_REQUEST);
        }

        // GET: CERTIFICATE_REQUEST/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CERTIFICATE_REQUEST cERTIFICATE_REQUEST = db.CERTIFICATE_REQUEST.Find(id);
            if (cERTIFICATE_REQUEST == null)
            {
                return HttpNotFound();
            }
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", cERTIFICATE_REQUEST.employee_id);
            return View(cERTIFICATE_REQUEST);
        }

        // POST: CERTIFICATE_REQUEST/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,employee_id,LETTER_TYPE,MENTION_SALARY,BL_bank_nane,BL_account_no,EL_name,EL_purpose,EL_DOV_from,EL_DOV_to,TD_NCARREG,TD_CARREGREN,TD_DLA,TDLREN,other,Etisalat,ADWEA,Mawaqif,Immigration")] CERTIFICATE_REQUEST cERTIFICATE_REQUEST)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cERTIFICATE_REQUEST).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", cERTIFICATE_REQUEST.employee_id);
            return View(cERTIFICATE_REQUEST);
        }

        // GET: CERTIFICATE_REQUEST/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CERTIFICATE_REQUEST cERTIFICATE_REQUEST = db.CERTIFICATE_REQUEST.Find(id);
            if (cERTIFICATE_REQUEST == null)
            {
                return HttpNotFound();
            }
            return View(cERTIFICATE_REQUEST);
        }

        // POST: CERTIFICATE_REQUEST/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CERTIFICATE_REQUEST cERTIFICATE_REQUEST = db.CERTIFICATE_REQUEST.Find(id);
            db.CERTIFICATE_REQUEST.Remove(cERTIFICATE_REQUEST);
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
