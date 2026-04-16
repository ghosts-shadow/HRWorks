using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using DataTable = System.Data.DataTable;

namespace HRworks.Controllers
{
    [Authorize]
    public class empdayoffsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: empdayoffs
        public ActionResult Index()
        {
            var empdayoffs = db.empdayoffs.Include(e => e.master_file);
            return View(empdayoffs.ToList());
        }

        // GET: empdayoffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empdayoff empdayoff = db.empdayoffs.Find(id);
            if (empdayoff == null)
            {
                return HttpNotFound();
            }
            return View(empdayoff);
        }

        // GET: empdayoffs/Create
        public ActionResult Create()
        {
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "emiid");
            return View();
        }

        // POST: empdayoffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,emp_ID,date_off,date_added,date_modified,by_whom")] empdayoff empdayoff)
        {
            if (ModelState.IsValid)
            {
                empdayoff.date_added = DateTime.Now;
                empdayoff.by_whom = User.Identity.Name;
                empdayoff.date_modified = DateTime.Now;
                db.empdayoffs.Add(empdayoff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "emiid", empdayoff.emp_ID);
            return View(empdayoff);
        }

        // GET: empdayoffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empdayoff empdayoff = db.empdayoffs.Find(id);
            if (empdayoff == null)
            {
                return HttpNotFound();
            }
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "emiid", empdayoff.emp_ID);
            return View(empdayoff);
        }

        // POST: empdayoffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,emp_ID,date_off")] empdayoff model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "emiid", model.emp_ID);
                return View(model);
            }

            var entity = db.empdayoffs.Find(model.Id);
            if (entity == null) return HttpNotFound();

            entity.emp_ID = model.emp_ID;
            entity.date_off = model.date_off;
            entity.by_whom = User?.Identity?.Name ?? "";
            entity.date_modified = DateTime.Now;

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList();

                // Visual Studio Output window
                Debug.WriteLine(string.Join(" | ", errors));

                // Show on the page too
                foreach (var err in errors)
                    ModelState.AddModelError("", err);

                ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "emiid", model.emp_ID);
                return View(model);
            }
        }

        // GET: empdayoffs/Delete/5

        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empdayoff empdayoff = db.empdayoffs.Find(id);
            if (empdayoff == null)
            {
                return HttpNotFound();
            }
            return View(empdayoff);
        }

        // POST: empdayoffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "super_admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            empdayoff empdayoff = db.empdayoffs.Find(id);
            db.empdayoffs.Remove(empdayoff);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Importempdayoff()
        {
            return this.View();
        }

        [ActionName("Importempdayoff")]
        [HttpPost]
        public ActionResult importempdayoff()
        {
            var file = this.Request.Files["FileUpload1"];

            if (file == null || file.ContentLength <= 0)
            {
                this.ViewBag.Error = "Please Upload Files in .csv / .xls / .xlsx format";
                return this.View();
            }

            var extension = (Path.GetExtension(file.FileName) ?? string.Empty).ToLower();
            string[] validFileTypes = { ".csv", ".xls", ".xlsx" };

            if (!validFileTypes.Contains(extension))
            {
                this.ViewBag.Error = "Please Upload Files in .csv / .xls / .xlsx format";
                return this.View();
            }

            var uploadDir = this.Server.MapPath("~/Content/Uploads");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            // Strip any directory segments from the client filename to prevent path traversal
            var safeName = Path.GetFileName(file.FileName);
            var path1 = Path.Combine(uploadDir, safeName);

            try
            {
                if (System.IO.File.Exists(path1))
                    System.IO.File.Delete(path1);

                file.SaveAs(path1);

                DataTable dt;
                if (extension == ".csv")
                {
                    dt = Utility.ConvertCSVtoDataTable(path1);
                }
                else
                {
                    var connString = extension == ".xls"
                        ? "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 +
                          ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\";"
                        : "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 +
                          ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";

                    dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                }

                this.ViewBag.Data = dt;

                if (dt == null || dt.Rows.Count == 0)
                    return this.View();

                // Build an O(1) lookup once, instead of List.Find per row
                var mancon = new master_fileController();
                var empLookup = mancon.emplist()
                    .Where(x => !string.IsNullOrWhiteSpace(x.emiid))
                    .GroupBy(x => x.emiid.Trim())
                    .ToDictionary(g => g.Key, g => g.First().employee_id,
                                  StringComparer.OrdinalIgnoreCase);

                // Resolve columns once, outside the row loop
                var dateCol = dt.Columns.Contains("Date off") ? dt.Columns["Date off"] : null;
                var empCol = dt.Columns.Contains("employee no") ? dt.Columns["employee no"] : null;

                if (dateCol == null || empCol == null)
                {
                    this.ViewBag.Error = "Required columns 'Date off' and 'employee no' not found.";
                    return this.View();
                }

                var now = DateTime.Now;
                var user = User.Identity.Name;
                var toInsert = new List<empdayoff>();
                var skipped = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    var rawDate = dr[dateCol] == null ? null : dr[dateCol].ToString().Trim();
                    var rawEmp = dr[empCol] == null ? null : dr[empCol].ToString().Trim();

                    if (string.IsNullOrWhiteSpace(rawDate) || string.IsNullOrWhiteSpace(rawEmp))
                    {
                        skipped++;
                        continue;
                    }

                    DateTime parsedDate;
                    if (!TryParseAnyDate(rawDate, out parsedDate))
                    {
                        skipped++;
                        continue;
                    }

                    int empId;
                    if (!empLookup.TryGetValue(rawEmp, out empId))
                    {
                        skipped++;
                        continue;
                    }

                    toInsert.Add(new empdayoff
                    {
                        date_off = parsedDate,
                        emp_ID = empId,
                        date_added = now,
                        by_whom = user,
                        date_modified = now
                    });
                }

                if (toInsert.Count > 0)
                {
                    this.db.empdayoffs.AddRange(toInsert);
                    this.db.SaveChanges(); // single round-trip instead of one per row
                }

                this.ViewBag.Message = "Imported: " + toInsert.Count + ", Skipped: " + skipped;
            }
            catch (Exception ex)
            {
                // Swap for your logger (NLog / Serilog / log4net) if you have one
                System.Diagnostics.Trace.TraceError("Importempdayoff failed: " + ex);
                this.ViewBag.Error = "Import failed. Please check the file and try again.";
            }

            return this.View();
        }

        private static bool TryParseAnyDate(string input, out DateTime result)
        {
            result = default(DateTime);
            if (string.IsNullOrWhiteSpace(input)) return false;

            input = input.Trim();

            // Excel sometimes exports dates as numeric serials (strings like "45678")
            double oa;
            if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out oa)
                && oa > 0 && oa < 200000) // sanity window: ~1900-01-01 to ~2447
            {
                try
                {
                    var d = DateTime.FromOADate(oa);
                    if (d.Year >= 1900 && d.Year <= 2999)
                    {
                        result = d;
                        return true;
                    }
                }
                catch { /* fall through to string parsing */ }
            }

            string[] formats =
            {
        // Day-first (most of the world, incl. UAE)
        "d/M/yyyy",  "dd/MM/yyyy",  "d/M/yy",  "dd/MM/yy",
        "d-M-yyyy",  "dd-MM-yyyy",  "d-M-yy",  "dd-MM-yy",
        "d.M.yyyy",  "dd.MM.yyyy",
        // Month-first (US)
        "M/d/yyyy",  "MM/dd/yyyy",  "M/d/yy",  "MM/dd/yy",
        "M-d-yyyy",  "MM-dd-yyyy",
        // ISO / year-first
        "yyyy-MM-dd", "yyyy/MM/dd", "yyyy.MM.dd", "yyyyMMdd",
        // Month names
        "d MMM yyyy",  "dd MMM yyyy",  "d MMMM yyyy",  "dd MMMM yyyy",
        "d-MMM-yyyy",  "dd-MMM-yyyy",
        "MMM d, yyyy", "MMMM d, yyyy",
        "MMM d yyyy",  "MMMM d yyyy",
    };

            if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces, out result))
                return true;

            // Loose fallbacks
            if (DateTime.TryParse(input, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal, out result))
                return true;

            if (DateTime.TryParse(input, CultureInfo.CurrentCulture,
                    DateTimeStyles.AssumeLocal, out result))
                return true;

            return false;
        }

        [HttpGet]
        public FileResult DownloadEmpDayOffTemplateCsv()
        {
            var headers = new[] { "employee no", "Date off" };

            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", headers));

            // Optional example row
            // sb.AppendLine("1001,2026-01-15");

            var data = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
                .ToArray();

            return File(data, "text/csv", "empdayoff_template.csv");
        }
        [HttpGet]
        public FileResult DownloadEmpDayOffTemplateXlsx()
        {
            // If using EPPlus v5+ you may need:
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Template");

                string[] headers = { "employee no", "Date off" };

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
                    "empdayoff_template.xlsx"
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
