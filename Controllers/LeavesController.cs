using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using HRworks.Models;

namespace HRworks.Controllers
{
    public class LeavesController : Controller
    {
        private HREntities db = new HREntities();

        // GET: Leaves
        public ActionResult Index()
        {
            var leaves = db.Leaves.Include(l => l.master_file).OrderByDescending(x=>x.Id);
            return View(leaves.ToList());
        }
        public ActionResult report(long? Employee_id, DateTime? stdate,DateTime? eddate)
        {
            var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
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

            ViewBag.employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");

            if (Employee_id != null && stdate != null && eddate != null)
            {
                var leaves = db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(x => x.Employee_id == Employee_id && x.Start_leave >= stdate && x.End_leave <= eddate);
                return View(leaves.ToList());
            }
            if (Employee_id == null && stdate != null && eddate != null)
            {
                var leaves = db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(x => x.Start_leave >= stdate && x.End_leave <= eddate);
                return View(leaves.ToList());
            }
            var leave = new List<Leave>();
            return this.View(leave);
        }

        // GET: Leaves/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leave leave = db.Leaves.Find(id);
            if (leave == null)
            {
                return HttpNotFound();
            }
            return View(leave);
        }

        // GET: Leaves/Create               
        public ActionResult Create()
        {
            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Annual", Value="1" },
                new ListItem { Text = "Sick", Value="2" },
                new ListItem { Text = "Compassionate", Value="3" },
                new ListItem { Text = "Maternity", Value="4" },
                new ListItem { Text = "Haj", Value="5" },
                new ListItem { Text = "Unpaid", Value="6" },
                new ListItem { Text = "others", Value="7" },
                new ListItem { Text = "half-day", Value="8" }
            };
            ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
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

            ViewBag.employee_id = new SelectList(afinallist.OrderBy(x=>x.employee_no), "employee_id", "employee_no");
            return View();
        }

        // POST: Leaves/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Employee_id,Id,Date,Reference,Start_leave,End_leave,Return_leave,days,leave_type,actual_return_date")] Leave leave, HttpPostedFileBase fileBase)
        {
            
            string serverfile;
            if (fileBase != null)
            {
                int i = 0;
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/leave/");
                serverfile = "D:/HR/leave/" + leave.Employee_id;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/leave/" + leave.Employee_id + "/" + leave.Employee_id + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(
                    serverfile = "D:/HR/leave/" + leave.Employee_id + "/" + leave.Employee_id + "_" + i + fileexe));
                fileBase.SaveAs(serverfile);

            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var file1=new Leave();
                file1.Employee_id= leave.Employee_id;
                var masterstatus = db.master_file.Find(leave.Employee_id);
                file1.Date =leave.Date;
                file1.Reference= serverfile;
                file1.Start_leave= leave.Start_leave;
                file1.End_leave= leave.End_leave;
                file1.Return_leave= leave.Return_leave;
                file1.leave_type= leave.leave_type;
                file1.days= leave.days;
                file1.data_o_n= "New";
                file1.actual_return_date= leave.actual_return_date;
                if (leave.actual_return_date==null)
                {
                    masterstatus.status = "on leave";
                }
                else
                {
                    masterstatus.status = "active";
                }
                db.Leaves.Add(file1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Annual", Value="Annual" },
                new ListItem { Text = "Sick", Value="Sick" },
                new ListItem { Text = "Compassionate", Value="Compassionate" },
                new ListItem { Text = "Maternity", Value="Maternity" },
                new ListItem { Text = "Haj", Value="Haj" },
                new ListItem { Text = "Unpaid", Value="Unpaid" },
                new ListItem { Text = "Other", Value="Other" }
            };
            var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
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
            ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_no");
            return View(leave);
        }

        // GET: Leaves/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leave leave = db.Leaves.Find(id);
            if (leave == null)
            {
                return HttpNotFound();
            }
            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Annual", Value="Annual" },
                new ListItem { Text = "Sick", Value="Sick" },
                new ListItem { Text = "Compassionate", Value="Compassionate" },
                new ListItem { Text = "Maternity", Value="Maternity" },
                new ListItem { Text = "Haj", Value="Haj" },
                new ListItem { Text = "Unpaid", Value="Unpaid" },
                new ListItem { Text = "Other", Value="Other" }
            };
            ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
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

            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_no");
            return View(leave);
        }

        // POST: Leaves/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Employee_id,Id,Date,Reference,Start_leave,End_leave,Return_leave,leave_type,actual_return_date")] Leave leave, HttpPostedFileBase fileBase)
        {
            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Annual", Value="Annual" },
                new ListItem { Text = "Sick", Value="Sick" },
                new ListItem { Text = "Compassionate", Value="Compassionate" },
                new ListItem { Text = "Maternity", Value="Maternity" },
                new ListItem { Text = "Haj", Value="Haj" },
                new ListItem { Text = "Unpaid", Value="Unpaid" },
                new ListItem { Text = "Other", Value="Other" }
            };
            ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            string serverfile;
            if (fileBase != null)
            {
                int i = 0;
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/leave/");
                serverfile = "D:/HR/leave/" + leave.Employee_id;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/leave/" + leave.Employee_id + "/" + leave.Employee_id + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(
                    serverfile = "D:/HR/leave/" + leave.Employee_id + "/" + leave.Employee_id + "_" + i + fileexe));
                fileBase.SaveAs(serverfile);

            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var file1 = new Leave();
                file1.Employee_id = leave.Employee_id;
                var masterstatus = db.master_file.Find(leave.Employee_id);
                file1.Date = leave.Date;
                file1.Reference = serverfile;
                file1.Start_leave = leave.Start_leave;
                file1.End_leave = leave.End_leave;
                file1.Return_leave = leave.Return_leave;
                file1.leave_type = leave.leave_type;
                file1.actual_return_date = leave.actual_return_date;
                if (leave.actual_return_date == null)
                {
                    masterstatus.status = "on leave";
                }
                else
                {
                    masterstatus.status = "active";
                }
                db.Entry(leave).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
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

            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_no");
            return View(leave);
        }

        // GET: Leaves/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leave leave = db.Leaves.Find(id);
            if (leave == null)
            {
                return HttpNotFound();
            }
            return View(leave);
        }

        // POST: Leaves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Leave leave = db.Leaves.Find(id);
            var masterstatus = db.master_file.Find(leave.Employee_id);
            masterstatus.status = "active";
            db.Leaves.Remove(leave);
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
