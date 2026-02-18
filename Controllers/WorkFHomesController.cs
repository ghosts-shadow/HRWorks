using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using OfficeOpenXml;

namespace HRworks.Controllers
{
    [Authorize]
    public class WorkFHomesController : Controller
    {
        private HREntities db = new HREntities();

        // GET: WorkFHomes
        public ActionResult Index()
        {
            var workFHomes = db.WorkFHomes.Include(w => w.master_file);
            return View(workFHomes.ToList());
        }

        // GET: WorkFHomes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkFHome workFHome = db.WorkFHomes.Find(id);
            if (workFHome == null)
            {
                return HttpNotFound();
            }
            return View(workFHome);
        }

        // GET: WorkFHomes/Create
        public ActionResult Create()
        {
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: WorkFHomes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,emp_ID,date_off,date_added,date_modified,by_whom")] WorkFHome workFHome)
        {
            if (ModelState.IsValid)
            {
                workFHome.date_added = DateTime.Now;
                workFHome.by_whom = User.Identity.Name;
                workFHome.date_modified = DateTime.Now;
                db.WorkFHomes.Add(workFHome);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name", workFHome.emp_ID);
            return View(workFHome);
        }

        // GET: WorkFHomes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkFHome workFHome = db.WorkFHomes.Find(id);
            if (workFHome == null)
            {
                return HttpNotFound();
            }
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name", workFHome.emp_ID);
            return View(workFHome);
        }

        // POST: WorkFHomes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,emp_ID,date_off,date_added,date_modified,by_whom")] WorkFHome workFHome)
        {
            if (ModelState.IsValid)
            {
                workFHome.by_whom = User.Identity.Name;
                workFHome.date_modified = DateTime.Now;
                db.Entry(workFHome).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name", workFHome.emp_ID);
            return View(workFHome);
        }

        [Authorize(Roles = "super_admin")]
        // GET: WorkFHomes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkFHome workFHome = db.WorkFHomes.Find(id);
            if (workFHome == null)
            {
                return HttpNotFound();
            }
            return View(workFHome);
        }

        // POST: WorkFHomes/Delete/5
        [Authorize(Roles = "super_admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WorkFHome workFHome = db.WorkFHomes.Find(id);
            db.WorkFHomes.Remove(workFHome);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult ImportWFH()
        {
            return this.View();
        }

        [ActionName("ImportWFH")]
        [HttpPost]
        public ActionResult importWFH()
        {
            if (this.Request.Files["FileUpload1"].ContentLength > 0)
            {
                var extension = Path.GetExtension(this.Request.Files["FileUpload1"].FileName).ToLower();
                var connString = string.Empty;

                // ✅ allow Excel now
                string[] validFileTypes = { ".csv", ".xls", ".xlsx" };

                // ✅ correct upload folder creation
                var uploadDir = this.Server.MapPath("~/Content/Uploads");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var path1 = Path.Combine(uploadDir, this.Request.Files["FileUpload1"].FileName);

                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                        System.IO.File.Delete(path1);

                    this.Request.Files["FileUpload1"].SaveAs(path1);

                    DataTable dt = null;

                    if (extension == ".csv")
                    {
                        dt = Utility.ConvertCSVtoDataTable(path1);
                    }
                    else if (extension == ".xls" || extension == ".xlsx")
                    {
                        if (extension == ".xls")
                        {
                            connString =
                                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 +
                                ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\";";
                        }
                        else
                        {
                            connString =
                                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 +
                                ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                        }

                        dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                    }

                    this.ViewBag.Data = dt;

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var mancon = new master_fileController();
                        var afinallist = mancon.emplist();

                        foreach (DataRow dr in dt.Rows)
                        {
                            var pro = new WorkFHome();

                            foreach (DataColumn column in dt.Columns)
                            {
                                if (dr[column] == null || dr[column].ToString() == " ") goto e;

                                if (column.ColumnName == "WFH")
                                {
                                    var dtt = dr[column].ToString();
                                    DateTime.TryParse(dtt, out var a);
                                    pro.date_off = a;
                                }

                                if (column.ColumnName == "employee no")
                                {
                                    var dtt = dr[column].ToString();
                                    var epid = afinallist.Find(x => x.emiid == dtt);
                                    if (epid == null) goto e;
                                    pro.emp_ID = epid.employee_id;
                                }
                            }
                            pro.date_added = DateTime.Now;
                            pro.by_whom = User.Identity.Name;
                            pro.date_modified = DateTime.Now;
                            this.db.WorkFHomes.Add(pro);
                            this.db.SaveChanges();

                        e:;
                        }
                    }
                }
                else
                {
                    this.ViewBag.Error = "Please Upload Files in .csv / .xls / .xlsx format";
                }
            }
            else
            {
                this.ViewBag.Error = "Please Upload Files in .csv / .xls / .xlsx format";
            }

            return this.View();
        }

        [HttpGet]
        public FileResult DownloadEmpWFHTemplateCsv()
        {
            var headers = new[] { "employee no", "WFH" };

            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", headers));

            // Optional example row
            // sb.AppendLine("1001,2026-01-15");

            var data = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
                .ToArray();

            return File(data, "text/csv", "empWFH_template.csv");
        }
        [HttpGet]
        public FileResult DownloadEmpWFHTemplateXlsx()
        {
            // If using EPPlus v5+ you may need:
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Template");

                string[] headers = { "employee no", "WFH" };

                // Header row
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cells[1, i + 1].Value = headers[i];
                }

                ws.Cells[1, 1, 1, headers.Length].Style.Font.Bold = true;
                ws.Cells.AutoFitColumns();

                // Optional example row
                // ws.Cells[2, 1].Value = "1001";
                // ws.Cells[2, 2].Value = "2026-01-15";

                var bytes = package.GetAsByteArray();
                return File(
                    bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "empWFH_template.xlsx"
                );
            }
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
