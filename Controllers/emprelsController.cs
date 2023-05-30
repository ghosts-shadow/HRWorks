using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml;

namespace HRworks.Controllers
{
    [Authorize(Roles = "employee_rel,super_admin")]
    public class emprelsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: emprels
        public ActionResult Index()
        {
            var emprels = db.emprels.Include(e => e.master_file).Include(e => e.master_file1).Include(e => e.master_file2).ToList();
            return View(emprels.OrderBy(x=>x.master_file1.employee_no));
        }

        // GET: emprels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emprel emprel = db.emprels.Find(id);
            if (emprel == null)
            {
                return HttpNotFound();
            }
            return View(emprel);
        }

        // GET: emprels/Create
        public ActionResult Create()
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.HOD = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            ViewBag.Employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            ViewBag.line_man = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            return View();
        }

        // POST: emprels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Employee_id,line_man,HOD")] emprel emprel)
        {
            if (ModelState.IsValid)
            {
                var dublicatecheck = this.db.emprels.ToList();
                if (!dublicatecheck.Exists(x =>
                    x.Employee_id == emprel.Employee_id && x.line_man == emprel.line_man &&
                    x.HOD == emprel.HOD))
                {
                    this.db.emprels.Add(emprel);
                    this.db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.HOD = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", emprel.HOD);
            ViewBag.Employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", emprel.Employee_id);
            ViewBag.line_man = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", emprel.line_man);
            return View(emprel);
        }

        // GET: emprels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emprel emprel = db.emprels.Find(id);
            if (emprel == null)
            {
                return HttpNotFound();
            }
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.HOD = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", emprel.HOD);
            ViewBag.Employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", emprel.Employee_id);
            ViewBag.line_man = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", emprel.line_man);
            return View(emprel);
        }

        // POST: emprels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Employee_id,line_man,HOD")] emprel emprel)
        {
            if (ModelState.IsValid)
            {/*
                var emp =this.db.master_file.Find(emprel.Employee_id);
                var empLM =this.db.master_file.Find(emprel.line_man);
                var empHOD = new master_file();
                empHOD = null;
                if (emprel.HOD != null)
                { 
                    empHOD = this.db.master_file.Find(emprel.HOD);
                }*/
                var dublicatecheck = this.db.emprels.ToList();
                if (!dublicatecheck.Exists(x =>
                    x.Employee_id == emprel.Employee_id && x.line_man == emprel.line_man &&
                    x.HOD == emprel.HOD))
                {
                    var attachedEntity = db.emprels.Find(emprel.Id);
                    if (attachedEntity != null && db.Entry(attachedEntity).State != EntityState.Detached)
                    {
                        db.Entry(attachedEntity).State = EntityState.Detached;
                    }
                    db.Entry(emprel).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.HOD = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", emprel.HOD);
            ViewBag.Employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", emprel.Employee_id);
            ViewBag.line_man = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no", emprel.line_man);
            return View(emprel);
        }

        // GET: emprels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emprel emprel = db.emprels.Find(id);
            if (emprel == null)
            {
                return HttpNotFound();
            }
            return View(emprel);
        }

        // POST: emprels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            emprel emprel = db.emprels.Find(id);
            db.emprels.Remove(emprel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [ActionName("Importexcel")]
        [HttpPost]
        public ActionResult Importexcel()
        {
            if (this.Request.Files["FileUpload1"].ContentLength > 0)
            {
                var extension = Path.GetExtension(this.Request.Files["FileUpload1"].FileName).ToLower();
                string query = null;
                var connString = string.Empty;

                string[] validFileTypes = { ".csv" };

                var path1 = string.Format(
                    "{0}/{1}",
                    this.Server.MapPath("~/Content/Uploads"),
                    this.Request.Files["FileUpload1"].FileName);
                if (!Directory.Exists(path1)) Directory.CreateDirectory(this.Server.MapPath("~/Content/Uploads"));
                var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed)
                    .ToList();
                var afinallist = new List<master_file>();
                var erlist = new List<emprel>();
                foreach (var file in alist)
                {
                    if (afinallist.Count == 0) afinallist.Add(file);

                    if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
                }

                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1)) System.IO.File.Delete(path1);

                    this.Request.Files["FileUpload1"].SaveAs(path1);
                    if (extension == ".csv")
                    {
                        var dt = Utility.ConvertCSVtoDataTable(path1);
                        this.ViewBag.Data = dt;
                        if (dt.Rows.Count > 0)
                        {
                            var br = 0;
                            var bebr = 0;
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (bebr >= 3)
                                {
                                    bebr = 0;
                                    br++;
                                }

                                if (br > 1)
                                {
                                    goto brgoto;
                                }
                                var pro = new emprel();
                                var dublicatecheck = this.db.emprels.ToList();
                                foreach (DataColumn column in dt.Columns)
                                {

                                    if (dr[column].ToString().IsNullOrWhiteSpace())
                                    {
                                        bebr++;
                                    }
                                    else
                                    {
                                        if (bebr != 0)
                                        {
                                            bebr--;
                                        }
                                    }
                                    if (column.ColumnName == "employee no")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var epid = afinallist.Find(x => x.employee_no == idm);
                                            if (epid == null) goto e;
                                            pro.Employee_id = epid.employee_id;
                                        }
                                    }
                                    if (column.ColumnName == "Line Manager")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var epid = afinallist.Find(x => x.employee_no == idm);
                                            if (epid == null) goto e;
                                            pro.line_man = epid.employee_id;
                                        }
                                    }
                                    if (column.ColumnName == "HOD/GM")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var epid = afinallist.Find(x => x.employee_no == idm);
                                            if (epid != null)
                                                pro.HOD = epid.employee_id;
                                        }
                                    }
                                }

                                if (!dublicatecheck.Exists(x =>
                                    x.Employee_id == pro.Employee_id && x.line_man == pro.line_man &&
                                    x.HOD == pro.HOD) && pro.Employee_id != 0)
                                {
                                    this.db.emprels.Add(pro);
                                    this.db.SaveChanges();
                                }
                                e:;
                            }
                            brgoto: ;
                        }
                    }
                }
            }
            else
            {
                this.ViewBag.Error = "Please Upload Files in .csv format";
            }

            return this.View();
        }

        public ActionResult ImportExcel()
        {
            return this.View();
        }

        public void DownloadExcel()
        {
            /*
             var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("CONTRACT");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "Line Manager";
            Sheet.Cells["C1"].Value = "HOD/GM";
            var row = 2;
            Sheet.Cells[string.Format("A{0}", row)].Value = 5386;
            Sheet.Cells[string.Format("B{0}", row)].Value = 5342;
            Sheet.Cells[string.Format("C{0}", row)].Value = 4997;
            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename = relation_template.xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
            */
            var csv = "employee no,Line Manager,HOD/GM\n";
            csv += "5386,5342,4997\n";
            this.Response.Clear();
            this.Response.ContentType = "text/csv";
            this.Response.AddHeader("content-disposition", "filename = relation_template.csv");
            this.Response.Write(csv);
            this.Response.End();
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
