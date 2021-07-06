using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using PagedList;
using HRworks.Models;
using Microsoft.Ajax.Utilities;

namespace HRworks.Controllers
{
    using System.Web.Routing;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null || filterContext.HttpContext.Request.Url.Host
                != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Home", action = "Logout", area = "Main" }));
            }
        }
    }
    public class master_fileController : Controller
    {
        private HREntities db = new HREntities();

        // GET: master_file
        public ActionResult statusupdate()
        {

            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            foreach (var file in afinallist)
            {
                if (file.labour_card.Count() != 0 && file.visas.Count() != 0)
                {
                    file.status = "active";
                    var leavelist = db.Leaves.ToList();
                    var leaveflist = leavelist.FindAll(x => x.actual_return_date == null && x.Employee_id == file.employee_id);
                    foreach (var leaf in leaveflist)
                    {
                        if (leaf.Start_leave <= DateTime.Now.Date && DateTime.Now.Date <= leaf.End_leave)
                        {
                            file.status = "on leave";
                        }
                    }
                }
                else
                {
                    file.status = "in process";
                }
                if(file.last_working_day != null)
                {
                    file.status = "inactive";
                }
                this.db.Entry(file).State = EntityState.Modified;
                this.db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public void DownloadExcel(string search)
        {
            List<master_file> passexel;
            if (search != null)
            {
                passexel = db.master_file.Where(x => x.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).ToList();
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
        public ActionResult Index(string search, int? page, int? pagesize,int? idsearch)
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
            var ab = db.master_file.OrderBy(p => p.employee_no).ThenBy(x => x.date_changed).ToList();
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
                lists.RemoveRange(0, lists.Count);
                j = 0;
                int idk;
                if (int.TryParse(search, out idk))
                {
                    ab = db.master_file.Where(x => x.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.employee_no).ThenBy(x => x.date_changed).ToList();
                }
                else
                {
                    ab = db.master_file.Where(x => x.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.employee_no).ThenBy(x => x.date_changed).ToList();
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
        [Authorize(Roles = "super_admin,admin,payrole")]
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
        [Authorize(Roles = "super_admin,admin")]
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
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/masterfile/" + fileexe);
                serverfile = "D:/HR/img/masterfile/" + master_file.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/masterfile/" + master_file.employee_no + "/" + master_file.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/masterfile/" + master_file.employee_no + "/" + master_file.employee_no + "_" + i + fileexe));

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
        [Authorize(Roles = "super_admin,admin,payrole")]
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
        [Authorize(Roles = "super_admin,admin,payrole")]
        public ActionResult Edit([Bind(Include = "employee_no,employee_name,nationality,dob,date_joined,last_working_day,gender,IBAN,account_no,bank_name,img,id,status")] master_file master_file,HttpPostedFileBase fileBase)
        {
            var imglist = this.db.master_file.ToList();
            string serverfile;
            var img = imglist.Find(x => x.employee_no == master_file.employee_no);
            if (fileBase != null)
            {
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/masterfile/" + fileexe);
                serverfile = "D:/HR/img/masterfile/" + master_file.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/masterfile/" + master_file.employee_no + "/" + master_file.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/masterfile/" + master_file.employee_no + "/" + master_file.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                if (img.img != null)
                {
                    serverfile = img.img;
                }
                else
                {
                    serverfile = null;
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    /*var img = new master_file();
    //                img.emirates_id = master_file.emirates_id;
    //                img.employee_no = master_file.employee_no;
    //                img.employee_name = master_file.employee_name;
    //                img.nationality = master_file.nationality;
    //                img.dob = master_file.dob;
    //                img.date_joined = master_file.date_joined;
    //                img.last_working_day = master_file.last_working_day;
    //                img.gender = master_file.gender;
                    master_file.img = serverfile;
                    master_file.img = serverfile;
                    master_file.changed_by = User.Identity.Name;
                    master_file.date_changed = DateTime.Now;
                    db.master_file.Add(master_file);*/
                    var current = DateTime.Now;
                    img.employee_no = master_file.employee_no;
                    img.employee_name = master_file.employee_name;
                    img.nationality = master_file.nationality;
                    img.dob = master_file.dob;
                    img.date_joined = master_file.date_joined;
                    img.last_working_day = master_file.last_working_day;
                    img.gender = master_file.gender;
                    img.changed_by = User.Identity.Name;
                    img.date_changed = current;
                    if (!master_file.status.IsNullOrWhiteSpace())
                    {
                        img.status = master_file.status;
                    }
                    else
                    {
                        if (img.labour_card.Count != 0 || img.labour_card.Count != 0)
                        {
                            img.status = "active";
                        }
                    }
                   
                    img.img = serverfile;

                    this.db.Entry(img).State = EntityState.Modified;
                    this.db.SaveChanges();
                    /*
                    db.master_file.Add(img);
                    db.SaveChanges();*/
                    return RedirectToAction("Index");
                }/*
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    return Content(e.ToString());
                    throw;
                }*/
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                ve.PropertyName,
                                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                ve.ErrorMessage);
                        }
                    }

                    return Content(e.ToString());
                    throw;
                }             

            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no = new SelectList(db.emirates_id, "id", "id", master_file.employee_no);
            return View(master_file);
        }

        // GET: master_file/Delete/5
        [Authorize(Roles = "super_admin")]
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
        [Authorize(Roles = "super_admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            master_file master_file = db.master_file.Find(id);
            db.master_file.Remove(master_file);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult report(DateTime? from, DateTime? to, string status)
        {
            ViewBag.from = from;
            ViewBag.to = to;
            var afinallist = new List<master_file>();
                var alist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
                foreach (var file in alist)
                {
                    if (afinallist.Count == 0) afinallist.Add(file);

                    if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                        afinallist.Add(file);
                }
            var afinallist2 = new List<master_file>();
            if (status == "inactive")
            {
                foreach (var file in afinallist)
                {
                    if (file.status == "inactive" && file.last_working_day > from && file.last_working_day < to)
                    {
                        afinallist2.Add(file);
                    }
                }
            }
            if (status == "joining")
            {
                foreach (var file in afinallist)
                {
                    if (file.date_joined > from && file.date_joined < to)
                    {
                        afinallist2.Add(file);
                    }
                }
            }
            return View(afinallist2);
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
