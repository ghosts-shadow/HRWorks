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
// date format fun [DisplayFormat(ApplyFormatInEditMode=true, DataFormatString = "{0:d}")]
namespace TEST2.Controllers
{
    public class visa_and_labour_cardController : Controller
    {
        private HREntities db = new HREntities();

        // GET: visa_and_labour_card
        public void DownloadExcel(string search)
        {
            List<visa_and_labour_card> passexel;
            if (search != null)
            {
                passexel = db.visa_and_labour_card.Where(x => x.master_file.employee_name.StartsWith(search)).ToList();
            }
            else
            {
                passexel = db.visa_and_labour_card.Include(p => p.master_file).ToList();
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("INSURANCE");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "lc_no";
            Sheet.Cells["D1"].Value = "uid_no";
            Sheet.Cells["E1"].Value = "class_type";
            Sheet.Cells["F1"].Value = "rv_expiry";
            Sheet.Cells["G1"].Value = "lc_expiry";
            Sheet.Cells["H1"].Value = "proff_as_per_visa";
            Sheet.Cells["I1"].Value = "changed by";
            Sheet.Cells["J1"].Value = "imgpath";
            Sheet.Cells["K1"].Value = "date_changed";
            int row = 2;
            foreach (var item in passexel)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.emp_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.lc_no;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.uid_no;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.class_type;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.rv_expiry;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.lc_expiry;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.proff_as_per_visa;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.imgpath;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.date_changed;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename =visa_and_labour_card.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult Index(string search, int? page, int? pagesize)
        {
            //            ViewBag.employee_name = new SelectList(db.master_file, "employee_no", "employee_name");
            //            var visa_and_labour_card = db.visa_and_labour_card.Include(v => v.master_file);
            //            return View(visa_and_labour_card.ToList());
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
            IPagedList<visa_and_labour_card> passlist = null;
            passlist = db.visa_and_labour_card.OrderBy(x => x.employee_id).ToPagedList(pageIndex, defaSize);
            var lists = new List<visa_and_labour_card>();
            int j = 0;
            int i;
            var ab = db.visa_and_labour_card.OrderBy(x => x.employee_id).ToList();
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
                j = 0;  
                ab = db.visa_and_labour_card.Where(x => x.master_file.employee_name.StartsWith(search)).ToList();
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

        // GET: visa_and_labour_card/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            visa_and_labour_card visa_and_labour_card = db.visa_and_labour_card.Find(id);
            if (visa_and_labour_card == null)
            {
                return HttpNotFound();
            }
            return View(visa_and_labour_card);
        }

        // GET: visa_and_labour_card/Create

        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Create()
        {
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: visa_and_labour_card/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Create([Bind(Include = "id,lc_no,uid_no,emp_no,passport_no,company_code,nationality,person_code,class_type,rv_expiry,lc_expiry,passport_expiry,proff_as_per_visa")] visa_and_labour_card visa_and_labour_card, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/visa_and_labour_card/" + fileexe);
                serverfile = "D:/HR/visa_and_labour_card/" + visa_and_labour_card.emp_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile); int i = 1;
                do
                {
                    serverfile = "D:/HR/visa_and_labour_card/" + visa_and_labour_card.emp_no + "/" + visa_and_labour_card.emp_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/visa_and_labour_card/" + visa_and_labour_card.emp_no + "/" + visa_and_labour_card.emp_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new visa_and_labour_card();
                img.lc_no=visa_and_labour_card.lc_no;
                img.uid_no = visa_and_labour_card.uid_no;
                img.emp_no = visa_and_labour_card.emp_no;
                img.class_type = visa_and_labour_card.class_type;
                img.rv_expiry = visa_and_labour_card.rv_expiry;
                img.lc_expiry = visa_and_labour_card.lc_expiry;
                img.proff_as_per_visa = visa_and_labour_card.proff_as_per_visa;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.visa_and_labour_card.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(visa_and_labour_card);
        }

        // GET: visa_and_labour_card/Edit/5
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            visa_and_labour_card visa_and_labour_card = db.visa_and_labour_card.Find(id);
            if (visa_and_labour_card == null)
            {
                return HttpNotFound();
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(visa_and_labour_card);
        }

        // POST: visa_and_labour_card/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Edit([Bind(Include = "id,lc_no,uid_no,emp_no,passport_no,company_code,nationality,person_code,class_type,rv_expiry,lc_expiry,passport_expiry,proff_as_per_visa")] visa_and_labour_card visa_and_labour_card, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/visa_and_labour_card/" + fileexe);
                serverfile = "D:/HR/visa_and_labour_card/" + visa_and_labour_card.emp_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 1;
                do
                {
                    serverfile = "D:/HR/visa_and_labour_card/" + visa_and_labour_card.emp_no + "/" + visa_and_labour_card.emp_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/visa_and_labour_card/" + visa_and_labour_card.emp_no + "/"+ visa_and_labour_card.emp_no+"_" + i + fileexe));
                
                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new visa_and_labour_card();
                img.lc_no = visa_and_labour_card.lc_no;
                img.uid_no = visa_and_labour_card.uid_no;
                img.emp_no = visa_and_labour_card.emp_no;
                img.class_type = visa_and_labour_card.class_type;
                img.rv_expiry = visa_and_labour_card.rv_expiry;
                img.lc_expiry = visa_and_labour_card.lc_expiry;
                img.proff_as_per_visa = visa_and_labour_card.proff_as_per_visa;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.emp_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.emp_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(visa_and_labour_card);
        }

        // GET: visa_and_labour_card/Delete/5
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            visa_and_labour_card visa_and_labour_card = db.visa_and_labour_card.Find(id);
            if (visa_and_labour_card == null)
            {
                return HttpNotFound();
            }
            return View(visa_and_labour_card);
        }

        // POST: visa_and_labour_card/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_VLC")]
        public ActionResult DeleteConfirmed(int id)
        {
            visa_and_labour_card visa_and_labour_card = db.visa_and_labour_card.Find(id);
            db.visa_and_labour_card.Remove(visa_and_labour_card);
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
