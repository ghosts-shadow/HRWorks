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
    using System.Text;
    using System.Web.Security;
    public class payrolesController : Controller
    {
        private HREntities db = new HREntities();
        private LogisticsSoftEntities db1 = new LogisticsSoftEntities();

        // GET: payroles
        public ActionResult Index(DateTime? month)
        {
            var payroles = db.payroles.Include(p => p.contract).Include(p => p.Leave).Include(p => p.master_file);
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            var paylist = new List<payrole>();
            var lab = this.db1.LabourMasters.ToList();
            var mts= new List<MainTimeSheet>();
            var att= new List<Attendance>();
            if (month.HasValue)
            {
                mts = this.db1.MainTimeSheets.Where(x => x.TMonth.Month == month.Value.Month && x.TMonth.Year == month.Value.Year && x.ManPowerSupplier == 1).ToList();
                foreach (var mt in mts)
                {
                    if (mt.Attendances.Count != 0)
                    {
                        att.AddRange(mt.Attendances);
                    }
                }
            }
            else
            {

                mts = this.db1.MainTimeSheets.Where(
                    x => x.TMonth.Month == DateTime.Now.Month && x.TMonth.Year == DateTime.Now.Year
                                                             && x.ManPowerSupplier == 0).ToList();
                foreach (var mt in mts)
                {
                    if (mt.Attendances.Count != 0)
                    {
                        att.AddRange(mt.Attendances);
                    }
                }
            }
            foreach (var masterFile in afinallist)
            {
                var payr = new payrole();
                payr.master_file = masterFile;
                if (masterFile.contracts.Count !=0 )
                {
                    var conlist = this.db.contracts.ToList();
                    payr.contract = conlist.Find(c=>c.employee_no == masterFile.employee_id);
                    var leave1 =this.db.Leaves.Where(x => x.Employee_id == masterFile.employee_id && x.leave_type == "6").ToList();
                    var lowp = 0;
                    foreach (var leaf in leave1)
                    {
                        var dif = leaf.End_leave - leaf.Start_leave;
                        lowp = dif.Value.Days + 1;
                    }

                    var lab1 = lab.Find(x => x.EMPNO == masterFile.employee_no);
                    if (lab1 == null)
                    {
                        goto tos;
                    }
                    var attd = att.FindAll(x => x.EmpID == lab1.ID).ToList();
                    var aqt = 0l;
                    var aqf = 0l;
                    var aqh = 0l;
                    foreach (var aq in attd)
                    {
                        if (aq.TotalOverTime.HasValue)
                        {
                            aqt += aq.TotalOverTime.Value;
                        }
                        if (aq.FridayHours.HasValue)
                        {
                            aqf += aq.FridayHours.Value;
                        }
                        if (aq.Holidays.HasValue)
                        {
                            aqh += aq.Holidays.Value;
                        }
                    }

                    payr.OTRegular = (aqt).ToString();
                    payr.OTFriday = (aqf).ToString();
                    payr.HolidayOT = (aqh).ToString();
                    payr.LWOP = lowp; 
                    payr.contract.basic = Unprotect(payr.contract.basic);
                    payr.contract.salary_details = Unprotect(payr.contract.salary_details);
                    payr.contract.ticket_allowance = Unprotect(payr.contract.ticket_allowance);
                    payr.contract.arrears = Unprotect(payr.contract.arrears);
                    payr.totalpayable = Unprotect(payr.contract.basic) + Unprotect(
                                            payr.contract.salary_details)+ Unprotect(
                                            payr.contract.ticket_allowance) + Unprotect(
                                            payr.contract.arrears);
                paylist.Add(payr);
                tos: ;
                }
            }
            return View(paylist);
        }

        private const string Purpose = "equalizer";

        public static string Protect(string unprotectedText)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedText);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }

        public static string Unprotect(string protectedText)
        {
            var protectedBytes = Convert.FromBase64String(protectedText);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
            var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
            return unprotectedText;
        }
        // GET: payroles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            payrole payrole = db.payroles.Find(id);
            if (payrole == null)
            {
                return HttpNotFound();
            }
            return View(payrole);
        }

        // GET: payroles/Create
        public ActionResult Create()
        {
            ViewBag.con_id = new SelectList(db.contracts, "employee_id", "designation");
            ViewBag.LWOP = new SelectList(db.Leaves, "Id", "Reference");
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: payroles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,employee_no,con_id,totalpayable,OTRegular,OTFriday,OTNight,HolidayOT,Fot,TotalOT,cashAdvances,HouseAllow,FoodAllow,Timekeeping,Communication,TrafficFines,LWOP,TotalDedution,NetPay,remarks")] payrole payrole)
        {
            if (ModelState.IsValid)
            {
                db.payroles.Add(payrole);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.con_id = new SelectList(db.contracts, "employee_id", "designation", payrole.con_id);
            ViewBag.LWOP = new SelectList(db.Leaves, "Id", "Reference", payrole.LWOP);
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_name", payrole.employee_no);
            return View(payrole);
        }

        // GET: payroles/Edit/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            payrole payrole = db.payroles.Find(id);
            if (payrole == null)
            {
                return HttpNotFound();
            }
            ViewBag.con_id = new SelectList(db.contracts, "employee_id", "designation", payrole.con_id);
            ViewBag.LWOP = new SelectList(db.Leaves, "Id", "Reference", payrole.LWOP);
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_name", payrole.employee_no);
            return View(payrole);
        }

        // POST: payroles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin")]
        public ActionResult Edit([Bind(Include = "Id,employee_no,con_id,totalpayable,OTRegular,OTFriday,OTNight,HolidayOT,Fot,TotalOT,cashAdvances,HouseAllow,FoodAllow,Timekeeping,Communication,TrafficFines,LWOP,TotalDedution,NetPay,remarks")] payrole payrole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payrole).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.con_id = new SelectList(db.contracts, "employee_id", "designation", payrole.con_id);
            ViewBag.LWOP = new SelectList(db.Leaves, "Id", "Reference", payrole.LWOP);
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_name", payrole.employee_no);
            return View(payrole);
        }

        // GET: payroles/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            payrole payrole = db.payroles.Find(id);
            if (payrole == null)
            {
                return HttpNotFound();
            }
            return View(payrole);
        }

        // POST: payroles/Delete/5
        [Authorize(Roles = "super_admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            payrole payrole = db.payroles.Find(id);
            db.payroles.Remove(payrole);
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
