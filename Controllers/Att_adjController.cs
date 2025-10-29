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
    public class Att_adjController : Controller
    {
        private HREntities db = new HREntities();
        private biometrics_DBEntities db1 = new biometrics_DBEntities();

        // GET: Att_adj
        public ActionResult Index()
        {
            var att_adj = db.Att_adj.Include(a => a.master_file);
            return View(att_adj.ToList());
        }

        public ActionResult empattindex(DateTime? empatdatefrom, DateTime? empatdateto)
        {
            var empuser = db.usernames
                .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);
            var empint = 7770000;
            var finallist = new List<hik>();
            if (empatdatefrom.HasValue)
            {
            var empstring = empuser.master_file.employee_no.ToString();
            if (empstring.Contains("7770"))
            {
                var sub = empstring.Substring(4, 4);
                int.TryParse(sub, out int result);
                empint += result;

            }

            var HOatt = db.hiks
                .Where(x => x.ID == empuser.master_file.employee_no.ToString() || x.ID == empint.ToString()).ToList();
            var projectatt = db1.iclock_transaction.Where(x =>
                x.emp_code == empuser.master_file.employee_no.ToString() || x.emp_code == empint.ToString()).ToList();
            if (HOatt.Count>0)
            {
                finallist.AddRange(HOatt);
            }

            if (projectatt.Count >0)
            {
                foreach (var tratt in projectatt)
                {
                    var protoho = new hik();
                    protoho.ID = empuser.employee_no.ToString();
                    protoho.datetime = tratt.punch_time;
                    if (tratt.punch_state == "0")
                    {
                        protoho.Status = "check in";
                    }
                    else
                    {
                        protoho.Status = "check out";
                    }
                    
                }
            }
                
            }
            return View(finallist);
        }

        //[Authorize(Roles = "HOD,employee,Manager")]
        public ActionResult EmpIndex() 
        {

            var emprel = db.emprels.ToList();
            var empuser = db.usernames
                .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);

            var att_adj = new List<Att_adj>();
            if (empuser == null)
            {
                return View(att_adj);
            }
            att_adj = db.Att_adj.Include(a => a.master_file).Where(x=>x.emp_ID == empuser.employee_no).OrderByDescending(x=>x.date_added).ToList();
            return View(att_adj);
        }

        // GET: Att_adj/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Att_adj att_adj = db.Att_adj.Find(id);
            if (att_adj == null)
            {
                return HttpNotFound();
            }
            return View(att_adj);
        }

        // GET: Att_adj/Create
        public ActionResult Create()
        {
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: Att_adj/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,emp_ID,which_date,late_in,early_out,reason,ap1,ap2,date_added,date_modified")] Att_adj att_adj)
        {
            if (ModelState.IsValid)
            {
                db.Att_adj.Add(att_adj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name", att_adj.emp_ID);
            return View(att_adj);
        }

        // GET: Att_adj/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Att_adj att_adj = db.Att_adj.Find(id);
            if (att_adj == null)
            {
                return HttpNotFound();
            }
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name", att_adj.emp_ID);
            return View(att_adj);
        }

        // POST: Att_adj/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,emp_ID,which_date,late_in,early_out,reason,ap1,ap2,date_added,date_modified")] Att_adj att_adj)
        {
            if (ModelState.IsValid)
            {
                db.Entry(att_adj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name", att_adj.emp_ID);
            return View(att_adj);
        }

        // GET: Att_adj/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Att_adj att_adj = db.Att_adj.Find(id);
            if (att_adj == null)
            {
                return HttpNotFound();
            }
            return View(att_adj);
        }

        // POST: Att_adj/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Att_adj att_adj = db.Att_adj.Find(id);
            db.Att_adj.Remove(att_adj);
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
