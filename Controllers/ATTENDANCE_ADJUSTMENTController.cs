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
    public class ATTENDANCE_ADJUSTMENTController : Controller
    {
        private HREntities db = new HREntities();

        // GET: ATTENDANCE_ADJUSTMENT
        public ActionResult Index()
        {
            var aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Include(a => a.master_file);
            return View(aTTENDANCE_ADJUSTMENT.ToList());
        }

        // GET: ATTENDANCE_ADJUSTMENT/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Find(id);
            if (aTTENDANCE_ADJUSTMENT == null)
            {
                return HttpNotFound();
            }
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // GET: ATTENDANCE_ADJUSTMENT/Create
        public ActionResult Create()
        {
            var alist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x=>x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0)
                {
                    afinallist.Add(file);
                }

                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    afinallist.Add(file);
                }
            }
            var alistc = db.contracts.OrderBy(e => e.master_file.employee_no).ToList();
            var afinallistc = new List<contract>();
            foreach (var file in alistc)
            {
                if (afinallistc.Count == 0)
                {
                    afinallistc.Add(file);
                }

                if (!afinallistc.Exists(x => x.employee_no == file.employee_no))
                {
                    afinallistc.Add(file);
                }
            }
            ViewBag.Employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            ViewBag.Employee_name = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_name");
            ViewBag.dept = new SelectList(afinallistc.OrderBy(x => x.master_file.employee_no), "employee_no", "departmant_project");
            ViewBag.pos = new SelectList(afinallistc.OrderBy(x => x.master_file.employee_no), "employee_no", "designation");
            return View();
        }

        // POST: ATTENDANCE_ADJUSTMENT/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,date_prepared,DJ_date,late_in,early_out,reason")] ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT)
        {
            if (ModelState.IsValid)
            {
                db.ATTENDANCE_ADJUSTMENT.Add(aTTENDANCE_ADJUSTMENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", aTTENDANCE_ADJUSTMENT.Employee_id);
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // GET: ATTENDANCE_ADJUSTMENT/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Find(id);
            if (aTTENDANCE_ADJUSTMENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", aTTENDANCE_ADJUSTMENT.Employee_id);
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // POST: ATTENDANCE_ADJUSTMENT/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,date_prepared,DJ_date,late_in,early_out,reason")] ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aTTENDANCE_ADJUSTMENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", aTTENDANCE_ADJUSTMENT.Employee_id);
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // GET: ATTENDANCE_ADJUSTMENT/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Find(id);
            if (aTTENDANCE_ADJUSTMENT == null)
            {
                return HttpNotFound();
            }
            return View(aTTENDANCE_ADJUSTMENT);
        }

        // POST: ATTENDANCE_ADJUSTMENT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            ATTENDANCE_ADJUSTMENT aTTENDANCE_ADJUSTMENT = db.ATTENDANCE_ADJUSTMENT.Find(id);
            db.ATTENDANCE_ADJUSTMENT.Remove(aTTENDANCE_ADJUSTMENT);
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
