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
    public class end_of_serviceController : Controller
    {
        private HREntities db = new HREntities();

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

        // GET: end_of_service/Create
        public ActionResult Create()
        {
            LeavesController an = new LeavesController();
            an.cal_bal();
            
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
            var ab1 = db.contracts.OrderBy(p => p.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
            var lists1 = new List<contract>();
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
        public ActionResult Create([Bind(Include = "Id,Employee_id,last_working_day,status")] end_of_service end_of_service)
        {
            if (ModelState.IsValid)
            {
                db.end_of_service.Add(end_of_service);
                db.SaveChanges();
                return RedirectToAction("Index");
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
