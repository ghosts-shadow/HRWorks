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
    public class detailsinarabicsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: detailsinarabics
        public ActionResult Index()
        {
            var detailsinarabics = db.detailsinarabics.Include(d => d.master_file);
            return View(detailsinarabics.ToList());
        }

        // GET: detailsinarabics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detailsinarabic detailsinarabic = db.detailsinarabics.Find(id);
            if (detailsinarabic == null)
            {
                return HttpNotFound();
            }
            return View(detailsinarabic);
        }

        // GET: detailsinarabics/Create
        public ActionResult Create()
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_name");
            return View();
        }

        // POST: detailsinarabics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,employee_id,ARname,ARposition,ARnationality")] detailsinarabic detailsinarabic)
        {
            if (ModelState.IsValid)
            {
                db.detailsinarabics.Add(detailsinarabic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_name", detailsinarabic.employee_id);
            return View(detailsinarabic);
        }

        // GET: detailsinarabics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detailsinarabic detailsinarabic = db.detailsinarabics.Find(id);
            if (detailsinarabic == null)
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
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_name", detailsinarabic.employee_id);
            return View(detailsinarabic);
        }

        // POST: detailsinarabics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,employee_id,ARname,ARposition,ARnationality")] detailsinarabic detailsinarabic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detailsinarabic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_name", detailsinarabic.employee_id);
            return View(detailsinarabic);
        }

        // GET: detailsinarabics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detailsinarabic detailsinarabic = db.detailsinarabics.Find(id);
            if (detailsinarabic == null)
            {
                return HttpNotFound();
            }
            return View(detailsinarabic);
        }

        // POST: detailsinarabics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            detailsinarabic detailsinarabic = db.detailsinarabics.Find(id);
            db.detailsinarabics.Remove(detailsinarabic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult subcertificatereq(int? certificatetype, string destination)
        {
            ViewBag.submmites = "";
            if (certificatetype != null && !destination.IsNullOrWhiteSpace())
            {
                var userempnolists = db.usernames
                    .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
                var empusers = userempnolists.Find(x => x.AspNetUser.UserName == User.Identity.Name);
                var leafcon = new LeavesController();
                var empjd = empusers.master_file;
                var certificatereqsave = new certificatesavingtest_();
                certificatereqsave.employee_id = empjd.employee_id;
                certificatereqsave.certificate_type = certificatetype.Value;
                certificatereqsave.destination = destination;
                certificatereqsave.submition_date = DateTime.Today;
                db.certificatesavingtest_.Add(certificatereqsave);
                db.SaveChanges();
                ViewBag.submmites = "certificate request has been successfully submitted";
            }

            ViewBag.certificatetype = new SelectList(db.certificatetypes, "Id", "certificate_name_");
            return View();
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
                            foreach (DataRow dr in dt.Rows)
                            {
                                var pro = new detailsinarabic();
                                var dublicatecheck = this.db.detailsinarabics.ToList();
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (column.ColumnName == "employee no")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var epid = afinallist.Find(x => x.employee_no == idm);
                                            if (epid == null ) goto e;
                                            if (epid.employee_no == 0) goto e;
                                            pro.employee_id = epid.employee_id;
                                        }
                                    }
                                    
                                    if (column.ColumnName == "NameArabic")
                                    {
                                        pro.ARname = dr[column].ToString();
                                    } 

                                    if (column.ColumnName == "PositionArabic")
                                    {
                                        pro.ARposition = dr[column].ToString();
                                    }

                                    if (column.ColumnName == "NationalityArabic")
                                    {
                                        pro.ARnationality = dr[column].ToString();
                                    }
                                }

                                if (pro.employee_id == 0) goto e;
                                if (!dublicatecheck.Exists(x =>x.employee_id == pro.employee_id))
                                {
                                    this.db.detailsinarabics.Add(pro);
                                    this.db.SaveChanges();
                                }
                            e: ;
                            }
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
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("CONTRACT");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "NameArabic";
            Sheet.Cells["C1"].Value = "PositionArabic";
            Sheet.Cells["D1"].Value = "NationalityArabic";
            var row = 2;
            Sheet.Cells[string.Format("A{0}", row)].Value = 5386;
            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename = detailsinarabic_template.xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
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
