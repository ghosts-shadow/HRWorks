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
    [Authorize]
    public class emirates_idController : Controller
    {
        private HREntities db = new HREntities();

        // GET: emirates_id
        public void DownloadExcel(string search)
        {
            List<emirates_id> passexel1;
            List<emirates_id> passexel = new List<emirates_id>();
            if (search != null)
            {
                passexel1 = db.emirates_id.Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).ToList();
            }
            else
            {
                passexel1 = db.emirates_id.Include(p => p.master_file).ToList();
            }

            foreach (var a in passexel1.OrderBy(x=>x.master_file.employee_no).ThenByDescending(y=>y.date_changed))
            {
                if (!passexel.Exists(x=>x.master_file.employee_no == a.master_file.employee_no))
                {
                    passexel.Add(a);
                }
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("passport");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "eid no";
            Sheet.Cells["D1"].Value = "eid expiry";
            Sheet.Cells["E1"].Value = "img path";
            Sheet.Cells["F1"].Value = "changed by";
            Sheet.Cells["G1"].Value = "date changed";
            int row = 2;
            foreach (var item in passexel)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.eid_no;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.eid_expiry;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.imgpath;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.date_changed;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename =Emirates_ID.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult Index(string search, int? page, int? pagesize/*, DateTime? pDate*/)
        {
            var emirates_id = db.emirates_id.Include(e => e.master_file);
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_name = new SelectList(afinallist.OrderBy(x => x.employee_name), "employee_no", "employee_name");
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
            }

            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem() { Value="10", Text= "10" },
                new SelectListItem() { Value="15", Text= "15" },
                new SelectListItem() { Value="25", Text= "25" },
                new SelectListItem() { Value="50", Text= "50" },
                new SelectListItem() { Value="100", Text= "100" }
            };
            ViewBag.pagesize = defaSize;
            IPagedList<emirates_id> passlist = null;
            passlist = db.emirates_id.OrderBy(p=>p.master_file.employee_no).ToPagedList(pageIndex, defaSize);
            //            if (search != null)
            //            {
            //                return View(db.emirates_id.Where(x => x.master_file.employee_name.Contains(search) /*.StartsWith(search)*/).ToList().ToPagedList(page ?? 1, defaSize));
            //            }
            var ab = db.emirates_id.OrderBy(p => p.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
            var lists = new List<emirates_id>();
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
                    if (ab[--j] != ab[i -= 2])
                    {
                        lists.Add(ab[j]);
                    }
                }
            }

            /*if (pDate.HasValue)
            {
                lists.RemoveRange(0, ab.Count);
                j = 0;
                ab = db.emirates_id.Where(x=>x.eid_expiry >= pDate.Value).OrderBy(p => p.employee_no).ToList();
                lists = new List<emirates_id>();
                if (ab.Count != 0)
                {
                    for ( i = 0; i < ab.Count; i++)
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
            else*/ if (!string.IsNullOrEmpty(search))
            {
                lists.RemoveRange(0, lists.Count);
                j = 0;
                int idk;
                if (int.TryParse(search, out idk))
                {
                    ab = db.emirates_id.Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                }
                else
                {
                    ab = db.emirates_id.Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                }
                lists = new List<emirates_id>(); ;
                if (ab.Count!=0)
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

                if (ab.Count!=1)
                {
                    if (ab[--j]!=ab[i=i-2])
                    {
                        lists.Add(ab[j]);
                    }
                }
                
                if (lists.Count==0)
                {
                    i -= 1;
                    lists.Add(ab[i]);
                }}
                return View(lists.ToPagedList(page ?? 1, defaSize));
            }
            
            return View(lists.ToPagedList(page ?? 1, defaSize));

        }

        // GET: emirates_id/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emirates_id emirates_id = db.emirates_id.Find(id);
            if (emirates_id == null)
            {
                return HttpNotFound();
            }
            return View(emirates_id);
        }

        // GET: emirates_id/Create
        [Authorize(Roles = "super_admin,admin,employee_EID")]
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

        // POST: emirates_id/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,employee_EID")]
        public ActionResult Create([Bind(Include = "employee_id,employee_no,eid_no,eid_expiry")] emirates_id emirates_id, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(emirates_id.employee_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/EMID/" + fileexe);
                serverfile = "D:/HR/img/EMID/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/EMID/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/EMID/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new emirates_id();
                img.employee_no=emirates_id.employee_no;
                img.eid_no = emirates_id.eid_no;
                img.eid_expiry = emirates_id.eid_expiry;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.emirates_id.Add(img);
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
            return View(emirates_id);
        }

        // GET: emirates_id/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emirates_id emirates_id = db.emirates_id.Find(id);
            if (emirates_id == null)
            {
                return HttpNotFound();
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no = new SelectList(db.master_file.OrderBy(e => e.employee_no), "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(db.master_file.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View(emirates_id);
        }

        // POST: emirates_id/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "employee_id,employee_no,eid_no,eid_expiry")] emirates_id emirates_id, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(emirates_id.employee_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/EMID/" + fileexe);
                serverfile = "D:/HR/img/EMID/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/EMID/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/EMID/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
                var imglist = db.emirates_id.ToList().FindAll(x => x.employee_no == emirates_id.employee_no)
                    .OrderByDescending(x => x.date_changed).ToList();
                var imgpath = "";
                imgpath = null;
                if (imglist.FindAll(c => c.imgpath != null).Count != 0)
                    imgpath=imglist.FindAll(c => c.imgpath != null).OrderByDescending(x => x.date_changed).First().imgpath;
                serverfile = imgpath;
            }
            if (ModelState.IsValid)
            {
                var img = new emirates_id();
                img.employee_no = emirates_id.employee_no;
                img.eid_no = emirates_id.eid_no;
                img.eid_expiry = emirates_id.eid_expiry;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now; 
                db.emirates_id.Add(img);
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
            return View(emirates_id);
        }

        // GET: emirates_id/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emirates_id emirates_id = db.emirates_id.Find(id);
            if (emirates_id == null)
            {
                return HttpNotFound();
            }
            return View(emirates_id);
        }

        // POST: emirates_id/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            emirates_id emirates_id = db.emirates_id.Find(id);
            db.emirates_id.Remove(emirates_id);
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
