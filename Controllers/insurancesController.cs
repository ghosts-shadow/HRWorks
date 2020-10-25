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
    public class insurancesController : Controller
    {
        private HREntities db = new HREntities();

        // GET: insurances
        public void DownloadExcel(string search)
        {
            List<insurance> passexel;
            if (search != null)
            {
                passexel = db.insurances.Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).ToList();
            }
            else
            {
                passexel = db.insurances.Include(p => p.master_file).ToList();
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("INSURANCE");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "card_no";
            Sheet.Cells["D1"].Value = "age";
            Sheet.Cells["E1"].Value = "dependency";
            Sheet.Cells["F1"].Value = "marital_status";
            Sheet.Cells["G1"].Value = "annual_primium";
            Sheet.Cells["H1"].Value = "deletion_date";
            Sheet.Cells["I1"].Value = "invoice_no";
            Sheet.Cells["J1"].Value = "credit_amt";
            Sheet.Cells["K1"].Value = "imgpath";
            Sheet.Cells["L1"].Value = "changed_by";
            Sheet.Cells["M1"].Value = "date_changed";
            int row = 2;
            foreach (var item in passexel)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.card_no;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.age;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.dependency;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.marital_status;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.annual_primium;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.deletion_date;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.invoice_no;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.credit_amt;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.imgpath;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.date_changed.ToString();
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename =insurance.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult Index(string search, int? page, int? pagesize)
        {/*
            var insurances = db.insurances.Include(i => i.master_file);
            return View(insurances.ToList());*/
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize = 10;
            if (pagesize != 0)
            {
                defaSize = (pagesize ?? 10);
            }

            
            ViewBag.pagesize = defaSize;
            IPagedList<insurance> passlist = null;
            passlist = db.insurances.OrderBy(x => x.master_file.employee_id).ThenBy(x => x.date_changed).ToPagedList(pageIndex, defaSize);
            var ab = db.insurances.OrderBy(p => p.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
            var lists = new List<insurance>();
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
            if (search != null)
            {
                lists.RemoveRange(0, lists.Count);
                j = 0;
                int idk;
                if (int.TryParse(search, out idk))
                {
                    ab = db.insurances.Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                }
                else
                {
                    ab = db.insurances.Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
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

        // GET: insurances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            insurance insurance = db.insurances.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // GET: insurances/Create
        [Authorize(Roles = "super_admin,admin,employee_INC")]
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

        // POST: insurances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,employee_INC")]
        public ActionResult Create([Bind(Include = "employee_id,employee_no,card_no,dob,age,gender,dependency,marital_status,nationality,eid_no,pasport_no,uid_no,emitae_visa_issue,annual_primium,deletion_date,invoice_no,credit_amt")] insurance insurance, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(insurance.employee_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/insurance/" + fileexe);
                serverfile = "D:/HR/img/insurance/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/insurance/" + a.employee_no + "/" + a.employee_no + "_" + i +fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/insurance/" + a.employee_no + "/" + a.employee_no + "_" + i +fileexe));
                    fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new insurance();
                img.employee_no=insurance.employee_no;
                img.card_no = insurance.card_no;
                img.age = insurance.age;
                img.dependency = insurance.dependency;
                img.marital_status = insurance.marital_status;
                img.annual_primium = insurance.annual_primium;
                img.deletion_date = insurance.deletion_date;
                img.invoice_no = insurance.invoice_no;
                img.credit_amt = insurance.credit_amt;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.insurances.Add(img);
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
            return View(insurance);
        }

        // GET: insurances/Edit/5
        [Authorize(Roles = "super_admin,admin,employee_INC")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            insurance insurance = db.insurances.Find(id);
            if (insurance == null)
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
            return View(insurance);
        }

        // POST: insurances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,employee_INC")]
        public ActionResult Edit([Bind(Include = "id,employee_no,card_no,dob,age,gender,dependency,marital_status,nationality,eid_no,pasport_no,uid_no,emitae_visa_issue,annual_primium,deletion_date,invoice_no,credit_amt")] insurance insurance, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(insurance.employee_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/insurance/" + fileexe);
                serverfile = "D:/HR/img/insurance/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/insurance/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/insurance/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));
                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new insurance();
                img.employee_no = insurance.employee_no;
                img.card_no = insurance.card_no;
                img.age = insurance.age;
                img.dependency = insurance.dependency;
                img.marital_status = insurance.marital_status;
                img.annual_primium = insurance.annual_primium;
                img.deletion_date = insurance.deletion_date;
                img.invoice_no = insurance.invoice_no;
                img.credit_amt = insurance.credit_amt;
                img.imgpath = serverfile;
                db.insurances.Add(img);
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
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
            return View(insurance);
        }

        // GET: insurances/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            insurance insurance = db.insurances.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // POST: insurances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            insurance insurance = db.insurances.Find(id);
            db.insurances.Remove(insurance);
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
