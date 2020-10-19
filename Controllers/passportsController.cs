using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using PagedList;
using HRworks.Models;

namespace HRworks.Controllers
{
    public class passportsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: passports
        public ActionResult Index(string search, int? page, int? pagesize /*, DateTime? pDate*/)
        {
            var passports = db.passports;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize=10;
            if (pagesize!=0)
            {
                defaSize = (pagesize??10);
            }
            ViewBag.pagesize = defaSize;
            ViewBag.search = search;/*
            ViewBag.pDate = pDate;*/
            IPagedList<passport> passlist = null;

           passlist= db.passports.OrderBy(x => x.master_file.employee_no).ToPagedList(pageIndex, defaSize);
           var ab = db.passports.OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
           var lists = new List<passport>();
           int i;
            int j = 0;
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
            /*if (pDate.HasValue)
            {
                lists.RemoveRange(0, ab.Count);
                j = 0;
                ab = db.passports.Where(x =>x.passport_issue_date.Value >= pDate.Value).ToList();
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
            }
            else*/ if (search != null)
            {
                lists.RemoveRange(0, lists.Count);
                j = 0;
                int idk;
                if (int.TryParse(search, out idk))
                {
                    ab = db.passports.Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/).ToList();
                }
                else
                {
                    ab = db.passports.Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).ToList();
                }
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
            }
            
            return View(lists.ToPagedList(page ?? 1, defaSize));
        }

        public void DownloadExcel(string search)
        {
            List<passport> passexel;
            if (search != null)
            {
                passexel = db.passports.Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).ToList();
            }
            else
            {
                passexel = db.passports.Include(p => p.master_file).ToList();
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("passport");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "company code";
            Sheet.Cells["C1"].Value = "passport no";
            Sheet.Cells["D1"].Value = "passport expiry";
            Sheet.Cells["E1"].Value = "passport issue_date";
            Sheet.Cells["F1"].Value = "passport return_date";
            Sheet.Cells["G1"].Value = "passport remarks";
            Sheet.Cells["H1"].Value = "status";
            Sheet.Cells["I1"].Value = "img path";
            Sheet.Cells["J1"].Value = "employee name";
            Sheet.Cells["K1"].Value = "changed_by";
            Sheet.Cells["L1"].Value = "date_changed";
            int row = 2;
            foreach (var item in passexel)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.company_code;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.passport_no;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.passport_expiry;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.passport_issue_date;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.passport_return_date;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.passport_remarks;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.status;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.imgpath;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.date_changed.ToString();
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename = passport.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        // GET: passports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            passport passport = db.passports.Find(id);
            if (passport == null)
            {
                return HttpNotFound();
            }
            return View(passport);
        }

        // GET: passports/Create
        [Authorize(Roles = "super_admin,admin,employee_PASS")]
        public ActionResult Create()
        {
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(afinallist.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View();
        }

        // POST: passports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,employee_PASS")]
        public ActionResult Create([Bind(Include = "employee_no,company_code,passport_no,passport_expiry,passport_issue_date,passport_return_date,passport_remarks,status,rv_expiry,vl_start,vl_end,imgpath")] passport passport, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(passport.employee_no);
                int i = 0;
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/passport/");
                serverfile = "D:/HR/img/passport/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/img/passport/" + a.employee_no + "/" + a.employee_no+"_"+i + fileexe;
                    i++;
                } while (System.IO.File.Exists(
                    serverfile = "D:/HR/img/passport/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));
                fileBase.SaveAs(serverfile);
                
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new passport();
                img.employee_no = passport.employee_no;
                img.company_code = passport.company_code;
                img.passport_no = passport.passport_no;
                img.passport_expiry = passport.passport_expiry;
                img.passport_issue_date = passport.passport_issue_date;
                img.passport_return_date = passport.passport_return_date;
                img.passport_remarks = passport.passport_remarks;
                img.status = passport.status;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.passports.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(afinallist.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View(passport);
        }

        // GET: passports/Edit/5
        [Authorize(Roles = "super_admin,admin,employee_PASS")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            passport passport = db.passports.Find(id);
            if (passport == null)
            {
                return HttpNotFound();
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(afinallist.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View(passport);
        }

        // POST: passports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,employee_PASS")]
        public ActionResult Edit([Bind(Include = "employee_id,employee_no,company_code,passport_no,passport_expiry,passport_issue_date,passport_return_date,passport_remarks,status,rv_expiry,vl_start,vl_end,imgpath")] passport passport, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(passport.employee_no);
                int i = 0;
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/passport/");
                serverfile = "D:/HR/img/passport/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/img/passport/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(
                    serverfile = "D:/HR/img/passport/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));
                fileBase.SaveAs(serverfile);

            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new passport();
                img = db.passports.Find(passport.employee_id);
                img.employee_id = passport.employee_id;
                img.employee_no = passport.employee_no;
                img.company_code = passport.company_code;
                img.passport_no = passport.passport_no;
                img.passport_expiry = passport.passport_expiry;
                img.passport_issue_date = passport.passport_issue_date;
                img.passport_return_date = passport.passport_return_date;
                img.passport_remarks = passport.passport_remarks;
                img.status = passport.status;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                passport = img;
                db.passports.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(afinallist.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View(passport);
        }

        // GET: passports/Delete/5
        [Authorize(Roles = "super_admin,admin,employee_PASS")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            passport passport = db.passports.Find(id);
            if (passport == null)
            {
                return HttpNotFound();
            }
            return View(passport);
        }

        // POST: passports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,employee_PASS")]
        public ActionResult DeleteConfirmed(int id)
        {
            passport passport = db.passports.Find(id);
            db.passports.Remove(passport);
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
