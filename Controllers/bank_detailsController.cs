﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using PagedList;

namespace HRworks.Controllers
{
    public class bank_detailsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: bank_details
        public ActionResult Index(string search, int? page, int? pagesize)
        {
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize = 10;

            if (pagesize != 0)
            {
                int a = 10;
                if (pagesize != null)
                {
                    if (pagesize != 0)
                    {
                        a = (int)pagesize;
                    }
                }
                defaSize = a;
            } ViewBag.pagesize = defaSize;
            IPagedList<bank_details> passlist = null;
            var ab = db.bank_details.OrderBy(p => p.employee_no).ToList();
            var lists = new List<bank_details>();
            int j = 0;
            passlist = db.bank_details.OrderBy(x => x.Employee_Id).ToPagedList(pageIndex, defaSize);
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
            if (!string.IsNullOrEmpty(search))
            {
                lists.RemoveRange(0,ab.Count);
                j = 0;
                ab = db.bank_details.Where(x => x.master_file.employee_name.StartsWith(search)).ToList();
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
                return View(lists.ToPagedList(page ?? 1, defaSize));
//
//                return View(.ToPagedList(page ?? 1, defaSize));
            }
            return View(lists.ToPagedList(page ?? 1, defaSize));
        }

        // GET: bank_details/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bank_details bank_details = db.bank_details.Find(id);
            if (bank_details == null)
            {
                return HttpNotFound();
            }
            return View(bank_details);
        }

        // GET: bank_details/Create
        public ActionResult Create()
        {
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: bank_details/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Employee_Id,employee_no,IBAN,Account_no,bank_name")] bank_details bank_details)
        {
            if (ModelState.IsValid)
            {
                db.bank_details.Add(bank_details);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_name", bank_details.employee_no);
            return View(bank_details);
        }

        // GET: bank_details/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bank_details bank_details = db.bank_details.Find(id);
            if (bank_details == null)
            {
                return HttpNotFound();
            }
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_name", bank_details.employee_no);
            return View(bank_details);
        }

        // POST: bank_details/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Employee_Id,employee_no,IBAN,Account_no,bank_name")] bank_details bank_details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bank_details).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_name", bank_details.employee_no);
            return View(bank_details);
        }

        // GET: bank_details/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bank_details bank_details = db.bank_details.Find(id);
            if (bank_details == null)
            {
                return HttpNotFound();
            }
            return View(bank_details);
        }

        // POST: bank_details/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bank_details bank_details = db.bank_details.Find(id);
            db.bank_details.Remove(bank_details);
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