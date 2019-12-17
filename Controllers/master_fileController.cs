using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
    public class master_fileController : Controller
    {
        private HREntities db = new HREntities();

        // GET: master_file
        public void DownloadExcel(string search)
        {
            List<master_file> passexel;
            if (search != null)
            {
                passexel = db.master_file.Where(x => x.employee_name.StartsWith(search)).ToList();
            }
            else
            {
                passexel = db.master_file.ToList();
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("employee details");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "nationality";
            Sheet.Cells["D1"].Value = "dob";
            Sheet.Cells["E1"].Value = "date_joined";
            Sheet.Cells["F1"].Value = "last_working_day";
            Sheet.Cells["G1"].Value = "gender";
            Sheet.Cells["H1"].Value = "status";
            Sheet.Cells["I1"].Value = "changed by";
            Sheet.Cells["J1"].Value = "date changed";
            Sheet.Cells["K1"].Value = "img";
            int row = 2;
            foreach (var item in passexel)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.nationality;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.dob;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.date_joined;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.last_working_day;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.gender;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.status;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.date_changed.ToString();
                Sheet.Cells[string.Format("K{0}", row)].Value = item.img;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename =EMPLOYEE_DETAILS.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult Index(string search, int? page, int? pagesize)
        {
            //            return View(db.master_file.ToList());
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize = 10;
            
            if (pagesize != 0)
            {
                int a=10;
                if (pagesize!=null)
                {
                    if (pagesize!=0)
                    {
                        a  = (int)pagesize;
                    }
                }
                defaSize = a;
            }
            ViewBag.pagesize = defaSize;
            IPagedList<master_file> passlist = null;
            var ab = db.master_file.OrderBy(p => p.employee_no).ToList();
            var lists = new List<master_file>();
            int j = 0;
            passlist = db.master_file.OrderBy(x => x.employee_id).ToPagedList(pageIndex, defaSize);
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
                ab = db.master_file.Where(x => x.employee_name.StartsWith(search)).ToList();
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

        // GET: master_file/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            master_file master_file = db.master_file.Find(id);
            if (master_file == null)
            {
                return HttpNotFound();
            }
            return View(master_file);
        }

        // GET: master_file/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {

            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            return View();
        }

        // POST: master_file/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "employee_no,employee_name,nationality,dob,date_joined,last_working_day,gender,IBAN,account_no,bank_name,img,id")] master_file master_file, HttpPostedFileBase fileBase)
        {


            //            if (ModelState.IsValid)
            //            {
            //                db.master_file.Add(master_file);
            //                db.SaveChanges();
            //                return RedirectToAction("Index");
            //            }
            string serverfile;
            if (fileBase != null)
            {
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/masterfile/" + fileexe);
                serverfile = "D:/HR/masterfile/" + master_file.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile); 
                int i = 0;
                do
                {
                    serverfile = "D:/HR/masterfile/" + master_file.employee_no + "/" + master_file.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/masterfile/" + master_file.employee_no + "/" + master_file.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var current = DateTime.Now;
                var img = new master_file();
                img.employee_no = master_file.employee_no;
                img.employee_name = master_file.employee_name;
                img.nationality = master_file.nationality;
                img.dob = master_file.dob;
                img.date_joined = master_file.date_joined;
                img.last_working_day = master_file.last_working_day;
                img.gender = master_file.gender;
                img.changed_by= User.Identity.Name;
                img.date_changed = current;
                img.status = "in process";
                img.img = serverfile;
                db.master_file.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender"); 

            return View(master_file);
        }

        // GET: master_file/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            master_file master_file = db.master_file.Find(id);
            if (master_file == null)
            {
                return HttpNotFound();
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            return View(master_file);
        }

        // POST: master_file/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "employee_no,employee_name,nationality,dob,date_joined,last_working_day,gender,IBAN,account_no,bank_name,img,id")] master_file master_file,HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/masterfile/" + fileexe);
                serverfile = "D:/HR/masterfile/" + master_file.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/masterfile/" + master_file.employee_no + "/" + master_file.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/masterfile/" + master_file.employee_no + "/" + master_file.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new master_file();
                img.employee_no = master_file.employee_no;
                img.employee_name = master_file.employee_name;
                img.nationality = master_file.nationality;
                img.dob = master_file.dob;
                img.date_joined = master_file.date_joined;
                img.last_working_day = master_file.last_working_day;
                img.gender = master_file.gender;
                img.img = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.master_file.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
               
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no = new SelectList(db.emirates_id, "id", "id", master_file.employee_no);
            return View(master_file);
        }

        // GET: master_file/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            master_file master_file = db.master_file.Find(id);
            if (master_file == null)
            {
                return HttpNotFound();
            }
            return View(master_file);
        }

        // POST: master_file/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            master_file master_file = db.master_file.Find(id);
            db.master_file.Remove(master_file);
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
