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
    public class leave_absenceController : Controller
    {
        private HREntities db = new HREntities();

        // GET: leave_absence
        public ActionResult Index()
        {
            var leave_absence = db.leave_absence.Include(l => l.master_file);
            return View(leave_absence.ToList());
        }

        // GET: leave_absence/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            leave_absence leave_absence = db.leave_absence.Find(id);
            if (leave_absence == null)
            {
                return HttpNotFound();
            }
            return View(leave_absence);
        }

        // GET: leave_absence/Create
        public ActionResult Create()
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            ViewBag.Employee_id = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(afinallist.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View();
        }

        // POST: leave_absence/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Employee_id,Id,absence,month,fromd,tod")] leave_absence leave_absence)
        {
            if (ModelState.IsValid)
            {
                db.leave_absence.Add(leave_absence);
                db.SaveChanges();
                return RedirectToAction("leave_absence_Index","Leaves");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_no", leave_absence.Employee_id);
            return View(leave_absence);
        }

        // GET: leave_absence/Edit/5
        public ActionResult Edit(int? id,DateTime? eddate)
        {
            ViewBag.eddate = eddate;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            leave_absence leave_absence = db.leave_absence.Find(id);
            ViewBag.eddate = eddate;
            if (leave_absence == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_no", leave_absence.Employee_id);
            return View(leave_absence);
        }

        // POST: leave_absence/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Employee_id,Id,absence,month,fromd,tod")] leave_absence leave_absence,DateTime? eddate)
        {
            ViewBag.eddate = eddate;
            if (ModelState.IsValid)
            {
                db.Entry(leave_absence).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("leave_absence_Index","Leaves",new{ eddate=eddate});
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_no", leave_absence.Employee_id);
            return View(leave_absence);
        }
        
        [Authorize(Roles = "super_admin")]
        // GET: leave_absence/Delete/5
        public ActionResult Delete(int? id,DateTime? eddate)
        {
            ViewBag.eddate = eddate;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            leave_absence leave_absence = db.leave_absence.Find(id);
            if (leave_absence == null)
            {
                return HttpNotFound();
            }
            return View(leave_absence);
        }
        
        [Authorize(Roles = "super_admin")]
        // POST: leave_absence/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id,DateTime? eddate)
        {
            ViewBag.eddate = eddate;
            leave_absence leave_absence = db.leave_absence.Find(id);
            db.leave_absence.Remove(leave_absence);
            db.SaveChanges();
            return RedirectToAction("leave_absence_Index","Leaves",new{ eddate=eddate});
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
