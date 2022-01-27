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
    using System.Web.WebSockets;
    
   [Authorize(Roles = "super_admin,payrole,employee_con")]
    public class end_of_serviceController : Controller
    {
        private const string Purpose = "equalizer";
        private HREntities db = new HREntities();
        private LogisticsSoftEntities db2 = new LogisticsSoftEntities();

        public static string Protect(string unprotectedText)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedText);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }

        public static bool IsBase64Encoded(string str)
        {
            if (str.Replace(" ", "").Length % 4 != 0)
            {
                return false;
            }

            try
            {
                var str1 = Convert.FromBase64String(str);
                MachineKey.Unprotect(str1, Purpose);
                return true;
            }
            catch (Exception exception)
            {
                // Handle the exception
            }

            return false;
        }

        public static string Unprotect(string protectedText)
        {
            var protectedBytes = Convert.FromBase64String(protectedText);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
            var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
            return unprotectedText;
        }
        // GET: end_of_service
        public ActionResult Index()
        {
            var end_of_service = db.end_of_service.Include(e => e.master_file);
            return View(end_of_service.ToList());
        }

        // GET: end_of_service/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            end_of_service end_of_service = db.end_of_service.Find(id);
            if (end_of_service == null)
            {
                return HttpNotFound();
            }
            return View(end_of_service);
        }

        public ActionResult EOSB(end_of_service endOfService )
        {
            var EOSBvar = new end_of_service();
            var emplist = this.db.master_file.ToList();
            var conlist = this.db.contracts.ToList();
            var leavcal = new LeavesController();
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x=>x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            ViewBag.hasdata = false;
            ViewBag.status = new SelectList(
                new List<SelectListItem>
                    {
                        new SelectListItem { Selected = false, Text = "resign", Value = 1.ToString() },
                        new SelectListItem { Selected = false, Text = "terminate", Value = 2.ToString() },
                        new SelectListItem { Selected = false, Text = "transfer", Value = 3.ToString() },
                    },
                "Value",
                "Text");
            afinallist = afinallist.FindAll(x => x.contracts.Count != 0);
            ViewBag.Employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            ViewBag.Employee_id1 = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_name");
            if (endOfService.Employee_id != null && endOfService.last_working_day.HasValue && endOfService.status != null)
            {
                ViewBag.hasdata = true;
                var emp = emplist.Find(x => x.employee_id == endOfService.Employee_id);
                var con = conlist.Find(x => x.employee_no == emp.employee_id);
                EOSBvar.Employee_id = emp.employee_id;
                var abs = emp.leave_absence.ToList();
                var q = 0d;
                foreach (var ab in abs)
                {
                    q += ab.absence.Value;
                }

                EOSBvar.master_file = emp;
                if (con != null)
                {
                    EOSBvar.con_id = con.employee_id;
                    EOSBvar.contract = con;
                }

                EOSBvar.abstotal = q;
                leavcal.forfitedbalence(emp.employee_id);
                var leavecallist = this.db.leavecals.ToList();
                var leavecal = leavecallist.Find(x => x.Employee_id == emp.employee_id);
                EOSBvar.leavecal = leavecal;
                EOSBvar.leavedetails = leavecal.Id;
                EOSBvar.last_working_day = endOfService.last_working_day.Value;
                EOSBvar.status = endOfService.status;
                var eoslist = this.db.end_of_service.ToList();
                if (endOfService.save)
                {
                    if (ModelState.IsValid)
                    {
                        db.end_of_service.Add(endOfService);
                        db.SaveChanges();
                        return RedirectToAction("EOSB");
                    }  
                }
                else
                {
                    EOSBvar.pendingSalary = 0d;
                    EOSBvar.noticePeriod = 0d;
                    EOSBvar.nagativeBalance = 0d;
                    EOSBvar.others = 0d;
                    EOSBvar.cashAdvance = 0d;
                    EOSBvar.pettyCash = 0d;
                    EOSBvar.HRAdvances = 0d;
                    EOSBvar.telecomDeductions = 0d;
                    EOSBvar.trafficeFines = 0d;
                    EOSBvar.save = true;
                }
            }
            return this.View(EOSBvar);
        }

        // GET: end_of_service/Create
        public ActionResult Create()
        {
            LeavesController an = new LeavesController();
            var alist = db.master_file.OrderBy(e => e.employee_no).ThenBy(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            var ab = db.master_file.OrderBy(p => p.employee_no).ThenBy(x => x.date_changed).ToList();
            var lists = new List<master_file>();
            int j = 0;
            int i;
            if (ab.Count != 0)
            {
                for (i = 0; i < ab.Count; i++)
                {
                    if (++j != ab.Count())
                    {
                        if (ab[i].employee_no == ab[j].employee_no)
                        {
                            continue;
                        }
                        else
                        {
                            lists.Add(ab[i]);
                        }
                    }
                }

                if (ab.Count != 1)
                {
                    if (ab[--j] != ab[i = i - 2])
                    {
                        lists.Add(ab[j]);
                    }
                }

                if (lists.Count == 0)
                {
                    i -= 1;
                    lists.Add(ab[i]);
                }
            }
            var absentlist = new List<leave_absence>();
            var absent = this.db.leave_absence.ToList();
            foreach (var absence in absent.OrderBy(x => x.Employee_id))
            {
                if (!absentlist.Exists(x=>x.Employee_id == absence.Employee_id))
                {
                    var b = absent.FindAll(x => x.Employee_id == absence.Employee_id);
                    var abs = 0d;
                    foreach (var c in b)
                    {
                        abs = abs + c.absence.Value;
                    }

                    absence.absence = abs;
                    absentlist.Add(absence);
                }
            }
            ViewBag.absencel = new SelectList(absentlist.OrderBy(x=>x.Employee_id), "Employee_id", "absence");
            
            var ab1 = db.contracts.OrderBy(p => p.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
            var lists1 = new List<contract>();
            foreach (var aq in ab1)
            {
                var asif = lists.Find(x => x.employee_id == aq.employee_no);
                if (asif == null)
                {
                    var x = 0;
                }
                aq.master_file = asif;
            }
            int j1 = 0;
            int i1;
            if (ab1.Count != 0)
            {
                for (i1 = 0; i1 < ab1.Count; i1++)
                {
                    if (++j1 != ab1.Count())
                    {
                        if (ab1[i1].master_file.employee_no == ab1[j1].master_file.employee_no)
                        {
                            continue;
                        }
                        else
                        {
                            lists1.Add(ab1[i1]);
                        }
                    }
                }

                if (ab1.Count != 1)
                {
                    if (ab1[--j1] != ab1[i1 = i1 - 2])
                    {
                        lists1.Add(ab1[j1]);
                    }
                }

                if (lists1.Count == 0)
                {
                    i1 -= 1;
                    lists1.Add(ab1[i1]);
                }
            }
            var dyccon = new contractsController();
            
            foreach (var contract in lists1)
            {
                if (contract.living_allowance != null)
                    contract.living_allowance = dyccon.Unprotect(contract.living_allowance);

                if (contract.others != null) contract.others = dyccon.Unprotect(contract.others);

                if (contract.food_allowance != null)
                    contract.food_allowance = dyccon.Unprotect(contract.food_allowance);

                if (contract.transportation_allowance != null)
                    contract.transportation_allowance = dyccon.Unprotect(contract.transportation_allowance);

                if (contract.ticket_allowance != null)
                    contract.ticket_allowance = dyccon.Unprotect(contract.ticket_allowance);

                if (contract.arrears != null) contract.arrears = dyccon.Unprotect(contract.arrears);

                if (contract.housing_allowance != null)
                    contract.housing_allowance = dyccon.Unprotect(contract.housing_allowance);

                if (contract.basic != null) contract.basic = dyccon.Unprotect(contract.basic);

                if (contract.salary_details != null)
                    contract.salary_details = dyccon.Unprotect(contract.salary_details);

                if (contract.FOT != null) contract.FOT = dyccon.Unprotect(contract.FOT);
            }


            foreach (var file in alist)
            {
                if (afinallist.Count == 0)
                {
                    {
                        afinallist.Add(file);
                        an.forfitedbalence(file.employee_id);
                    }
                }

                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    {
                        afinallist.Add(file);
                        an.forfitedbalence(file.employee_id);
                    }
                }
            }

            var alistc = db.contracts.OrderBy(e => e.master_file.employee_no).ThenByDescending(x=>x.date_changed).ToList();
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

            ViewBag.Employee_id = new SelectList(lists.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            ViewBag.Employee_name = new SelectList(
                lists.OrderBy(x => x.employee_no),
                "employee_id",
                "employee_name");
            ViewBag.joi = new SelectList(
                lists.OrderBy(x => x.employee_no),
                "employee_id",
                "date_joined");
            ViewBag.unp = new SelectList(
                this.db.leavecals.OrderBy(x => x.Employee_id),
                "employee_id",
                "unpaid_leave");
            ViewBag.dept = new SelectList(
                lists1.OrderBy(x => x.master_file.employee_no),
                "employee_no",
                "departmant_project");
            ViewBag.pos = new SelectList(
                lists1.OrderBy(x => x.master_file.employee_no),
                "employee_no",
                "designation");
            ViewBag.gra = new SelectList(
                lists1.OrderBy(x => x.master_file.employee_no),
                "employee_no",
                "grade");
            ViewBag.bac = new SelectList(
                lists1.OrderBy(x => x.master_file.employee_no),
                "employee_no",
                "basic");
            ViewBag.hou = new SelectList(
                lists1.OrderBy(x => x.master_file.employee_no),
                "employee_no",
                "housing_allowance");
            ViewBag.gro = new SelectList(
                lists1.OrderBy(x => x.master_file.employee_no),
                "employee_no",
                "salary_details");
            ViewBag.status = new SelectList(
                new List<SelectListItem>
                    {
                        new SelectListItem { Selected = false, Text = "resign", Value = 1.ToString() },
                        new SelectListItem { Selected = false, Text = "terminate", Value = 2.ToString() },
                    },
                "Value",
                "Text");
            return View();
        }

        // POST: end_of_service/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(end_of_service end_of_service)
        {
            if (ModelState.IsValid)
            {
                db.end_of_service.Add(end_of_service);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            var alist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
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

            var alistc = db.contracts.OrderBy(e => e.master_file.employee_no).ThenByDescending(x => x.date_changed)
                .ToList();
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

            ViewBag.status = new SelectList(
                new List<SelectListItem>
                    {
                        new SelectListItem { Selected = true, Text = string.Empty, Value = "-1" },
                        new SelectListItem
                            {
                                Selected = false, Text = "resign", Value = 1.ToString()
                            },
                        new SelectListItem
                            {
                                Selected = false, Text = "terminate", Value = 2.ToString()
                            },
                    },
                "Value",
                "Text");
            ViewBag.Employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            ViewBag.Employee_name = new SelectList(
                afinallist.OrderBy(x => x.employee_no),
                "employee_id",
                "employee_name");
            ViewBag.joi = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "joining_date");
            ViewBag.dept = new SelectList(
                afinallistc.OrderBy(x => x.master_file.employee_no),
                "employee_id",
                "departmant_project");
            ViewBag.pos = new SelectList(
                afinallistc.OrderBy(x => x.master_file.employee_no),
                "employee_id",
                "designation");
            ViewBag.gra = new SelectList(afinallistc.OrderBy(x => x.master_file.employee_no), "employee_id", "grade");
            ViewBag.bac = new SelectList(afinallistc.OrderBy(x => x.master_file.employee_no), "employee_id", "basic");
            ViewBag.hou = new SelectList(
                afinallistc.OrderBy(x => x.master_file.employee_no),
                "employee_id",
                "housing_allowance");
            ViewBag.gro = new SelectList(
                afinallistc.OrderBy(x => x.master_file.employee_no),
                "employee_id",
                "salary_details");
            return View(end_of_service);
        }

        // GET: end_of_service/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            end_of_service end_of_service = db.end_of_service.Find(id);
            if (end_of_service == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", end_of_service.Employee_id);
            return View(end_of_service);
        }

        // POST: end_of_service/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,last_working_day,status")] end_of_service end_of_service)
        {
            if (ModelState.IsValid)
            {
                db.Entry(end_of_service).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name", end_of_service.Employee_id);
            return View(end_of_service);
        }

        // GET: end_of_service/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            end_of_service end_of_service = db.end_of_service.Find(id);
            if (end_of_service == null)
            {
                return HttpNotFound();
            }
            return View(end_of_service);
        }

        // POST: end_of_service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            end_of_service end_of_service = db.end_of_service.Find(id);
            db.end_of_service.Remove(end_of_service);
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
