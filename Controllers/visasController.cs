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
    public class visasController : Controller
    {
        private HREntities db = new HREntities();

        public void DownloadExcel(string search)
        {
            List<visa> passexel;
            if (search != null)
            {
                passexel = db.visas.Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).ToList();
            }
            else
            {
                passexel = db.visas.Include(p => p.master_file).ToList();
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("visa".ToUpper());
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "file no";
            Sheet.Cells["D1"].Value = "uid no";
            Sheet.Cells["E1"].Value = "place of issue";
            Sheet.Cells["F1"].Value = "rv expiry";
            Sheet.Cells["G1"].Value = "lc expiry";
            Sheet.Cells["H1"].Value = "proff as per visa";
            Sheet.Cells["I1"].Value = "imgpath";
            Sheet.Cells["J1"].Value = "accompanied by";
            Sheet.Cells["K1"].Value = "sponsor";
            Sheet.Cells["L1"].Value = "changed by";
            Sheet.Cells["M1"].Value = "date_changed";
            int row = 2;
            foreach (var item in passexel)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.file_no;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.uid_no;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.place_of_issue;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.rv_expiry;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.rv_issue;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.proff_as_per_visa;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.imgpath;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.accompanied_by;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.sponsor;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.date_changed;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename =visa.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        // GET: visas
        public ActionResult Index(string search, int? page, int? pagesize, int? idsearch)
        {
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize = 10;
            if (pagesize != 0)
            {
                defaSize = (pagesize ?? 10);
            }

            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem() { Value="10", Text= "10" },
                new SelectListItem() { Value="15", Text= "15" },
                new SelectListItem() { Value="25", Text= "25" },
                new SelectListItem() { Value="50", Text= "50" },
                new SelectListItem() { Value="100", Text= "100" },
            };
            ViewBag.pagesize = defaSize;
            IPagedList<visa> passlist = null;
            passlist = db.visas.OrderBy(x => x.date_changed).ToPagedList(pageIndex, defaSize);
            var lists = new List<visa>();
            int j = 0;
            int i;
            var ab = db.visas.OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
            if (ab.Count != 0)
            {
                for (i = 0; i < ab.Count; i++)
                {
                    if (++j != ab.Count())
                    {
                        if (ab[i].emp_no == ab[j].emp_no)
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
                    ab = db.visas
                        .Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed)
                        .ToList();
                }
                else
                {
                    ab = db.visas
                        .Where(
                            x => x.master_file.employee_name
                                .Contains(search) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                }

                if (ab.Count != 0)
                {
                    for (i = 0; i < ab.Count; i++)
                    {
                        if (++j != ab.Count())
                        {
                            if (ab[i].emp_no == ab[j].emp_no)
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

        // GET: visas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            visa visa = db.visas.Find(id);
            if (visa == null)
            {
                return HttpNotFound();
            }
            return View(visa);
        }

        // GET: visas/Create
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Create()
        {
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file.OrderBy(e => e.employee_no), "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View();
        }

        // POST: visas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Create( visa visa, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(visa.emp_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/visa/" + fileexe);
                serverfile = "D:/HR/img/visa/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile); int i = 0;
                do
                {
                    serverfile = "D:/HR/img/visa/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/visa/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new visa();
                img.file_no = visa.file_no;
                img.place_of_issue = visa.place_of_issue;
                img.accompanied_by = visa.accompanied_by;
                img.rv_issue = visa.rv_issue;
                img.sponsor = visa.sponsor;
                img.uid_no = visa.uid_no;
                img.emp_no = visa.emp_no;
                img.rv_expiry = visa.rv_expiry;
                img.proff_as_per_visa = visa.proff_as_per_visa;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.visas.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(visa);
        }

        // GET: visas/Edit/5
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            visa visa = db.visas.Find(id);
            if (visa == null)
            {
                return HttpNotFound();
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(visa);
        }

        // POST: visas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Edit( visa visa, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(visa.emp_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/visa/" + fileexe);
                serverfile = "D:/HR/img/visa/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile); int i = 0;
                do
                {
                    serverfile = "D:/HR/img/visa/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/visa/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new visa(); 
                img.uid_no = visa.uid_no;
                img.emp_no = visa.emp_no;
                img.rv_expiry = visa.rv_expiry;
                img.proff_as_per_visa = visa.proff_as_per_visa;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.visas.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(visa);
        }

        // GET: visas/Delete/5
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            visa visa = db.visas.Find(id);
            if (visa == null)
            {
                return HttpNotFound();
            }
            return View(visa);
        }

        // POST: visas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult DeleteConfirmed(int id)
        {
            visa visa = db.visas.Find(id);
            db.visas.Remove(visa);
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
