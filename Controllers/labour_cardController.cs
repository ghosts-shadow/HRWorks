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
    public class labour_cardController : Controller
    {
        private HREntities db = new HREntities();

        public void DownloadExcel(string search)
        {
            List<labour_card> passexel;
            if (search != null)
            {
                if (int.TryParse(search, out var idk))
                {
                    passexel = db.labour_card
                        .Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                }
                else
                {
                    passexel = db.labour_card
                        .Where(
                            x => x.master_file.employee_name
                                .Contains(search) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                }
            }
            else
            {
                passexel = db.labour_card.Include(p => p.master_file).ToList();
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("labour_card".ToUpper());
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "work permit no";
            Sheet.Cells["D1"].Value = "personal no";
            Sheet.Cells["E1"].Value = "class type";
            Sheet.Cells["F1"].Value = "establishment";
            Sheet.Cells["G1"].Value = "lc expiry";
            Sheet.Cells["H1"].Value = "changed by";
            Sheet.Cells["I1"].Value = "imgpath";
            Sheet.Cells["J1"].Value = "date_changed";
            int row = 2;
            foreach (var item in passexel)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.work_permit_no;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.personal_no;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.proffession;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.establishment;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.lc_expiry;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.imgpath;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.date_changed;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename =labour_card.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        // GET: labour_card
        public ActionResult Index(string search, int? page, int? pagesize, int? idsearch)
        {
            //            ViewBag.employee_name = new SelectList(db.master_file, "employee_no", "employee_name");
            //            var labour_card = db.labour_card.Include(v => v.master_file);
            //            return View(labour_card.ToList());
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
            IPagedList<labour_card> passlist = null;
            passlist = db.labour_card.OrderBy(x => x.date_changed).ToPagedList(pageIndex, defaSize);
            var lists = new List<labour_card>();
            int j = 0;
            int i;
            var ab = db.labour_card.OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
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
                    ab = db.labour_card
                        .Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed)
                        .ToList();
                }
                else
                {
                    ab = db.labour_card
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

        // GET: labour_card/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            labour_card labour_card = db.labour_card.Find(id);
            if (labour_card == null)
            {
                return HttpNotFound();
            }
            return View(labour_card);
        }

        // GET: labour_card/Create

        [Authorize(Roles = "super_admin,admin,employee_VLC")]
        public ActionResult Create()
        {
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file.OrderBy(e => e.employee_no), "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View();
        }

        // POST: labour_card/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,employee_VLC")]
        public ActionResult Create( labour_card labour_card, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(labour_card.emp_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/labour_card/" + fileexe);
                serverfile = "D:/HR/img/labour_card/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile); int i = 0;
                do
                {
                    serverfile = "D:/HR/img/labour_card/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/labour_card/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new labour_card();
                img.work_permit_no = labour_card.work_permit_no;
                img.personal_no = labour_card.personal_no;
                img.emp_no = labour_card.emp_no;
                img.proffession = labour_card.proffession;
                img.establishment = labour_card.establishment;
                img.lc_expiry = labour_card.lc_expiry;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.labour_card.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(labour_card);
        }

        // GET: labour_card/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            labour_card labour_card = db.labour_card.Find(id);
            if (labour_card == null)
            {
                return HttpNotFound();
            }
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_name", labour_card.emp_no);
            return View(labour_card);
        }

        // POST: labour_card/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,employee_VLC")]
        public ActionResult Edit( labour_card labour_card, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(labour_card.emp_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/labour_card/" + fileexe);
                serverfile = "D:/HR/img/labour_card/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile); int i = 0;
                do
                {
                    serverfile = "D:/HR/img/labour_card/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/labour_card/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new labour_card();
                img.work_permit_no = labour_card.work_permit_no;
                img.personal_no = labour_card.personal_no;
                img.emp_no = labour_card.emp_no;
                img.proffession = labour_card.proffession;
                img.establishment = labour_card.establishment;
                img.lc_expiry = labour_card.lc_expiry;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.labour_card.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(labour_card);
        }

        // GET: labour_card/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            labour_card labour_card = db.labour_card.Find(id);
            if (labour_card == null)
            {
                return HttpNotFound();
            }
            return View(labour_card);
        }

        // POST: labour_card/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,employee_VLC")]
        public ActionResult DeleteConfirmed(int id)
        {
            labour_card labour_card = db.labour_card.Find(id);
            db.labour_card.Remove(labour_card);
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
