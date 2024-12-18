using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using Microsoft.Ajax.Utilities;

namespace HRworks.Controllers
{
    public class FormsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: Forms
        public ActionResult Index()
        {
            return View(db.Forms.ToList());
        }

        // GET: Forms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = db.Forms.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        // GET: Forms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Forms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,form_name,department")] Form form)
        {
            if (ModelState.IsValid)
            {
                db.Forms.Add(form);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(form);
        }

        // GET: Forms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = db.Forms.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        // POST: Forms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,form_name,department")] Form form)
        {
            if (ModelState.IsValid)
            {
                db.Entry(form).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(form);
        }

        // GET: Forms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = db.Forms.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        // POST: Forms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Form form = db.Forms.Find(id);
            db.Forms.Remove(form);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        

        // GET: Forms/Create
        public ActionResult NewFormsFlow(int? formId)
        {
            if (formId.HasValue)
            {
                var form = db.Forms.ToList().Find(x => x.Id == formId);
                if (form == null)
                {
                    return RedirectToAction("Index", "Forms");
                }

                ViewBag.formID = form.Id;
                var prealist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList()
                    .GroupBy(x => x.employee_no).Select(s => s.First());
                var alist = prealist
                    .Where(e => e.last_working_day == null)
                    .ToList();

                var afinallist = alist
                    .GroupBy(x => x.employee_no)
                    .Select(g => g.First())
                    .Where(file => file.employee_no != 0 && file.employee_no != 1 && file.employee_no != 100001)
                    .ToList();
                this.ViewBag.employee_no = new SelectList(afinallist, "employee_name", "employee_no", null);
                var authlist = new List<SelectListItem> {
                    new SelectListItem { Value = "line manager", Text = "line manager" },
                    new SelectListItem { Value = "reviewer1", Text = "reviewer1" },
                    new SelectListItem { Value = "reviewer2", Text = "reviewer2" },
                    new SelectListItem { Value = "approver1", Text = "approver1" },
                    new SelectListItem { Value = "approver2", Text = "approver2" }
                };

                this.ViewBag.authlist = new SelectList(authlist, "Value", "Text");
                return View();

            }

            return RedirectToAction("Index", "Forms");
        }

        // POST: Forms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewFormsFlow(FormsFlow formsFlow)
        {
                ViewBag.formID = formsFlow.form_ID;
                var prealist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList()
                    .GroupBy(x => x.employee_no).Select(s => s.First());
                var alist = prealist
                    .Where(e => e.last_working_day == null)
                    .ToList();

                var afinallist = alist
                    .GroupBy(x => x.employee_no)
                    .Select(g => g.First())
                    .Where(file => file.employee_no != 0 && file.employee_no != 1 && file.employee_no != 100001)
                    .ToList();
                this.ViewBag.employee_no = new SelectList(afinallist, "employee_name", "employee_no", null);
                var authlist = new List<SelectListItem> {
                    new SelectListItem { Value = "line manager", Text = "line manager" },
                    new SelectListItem { Value = "reviewer1", Text = "reviewer1" },
                    new SelectListItem { Value = "reviewer2", Text = "reviewer2" },
                    new SelectListItem { Value = "approver1", Text = "approver1" },
                    new SelectListItem { Value = "approver2", Text = "approver2" }
                };

                this.ViewBag.authlist = new SelectList(authlist, "Value", "Text");
            if (ModelState.IsValid)
            {

                var checkformflow = db.FormsFlows.ToList().Find(x => x.form_ID == formsFlow.form_ID);
                if (checkformflow != null)
                {
                    ModelState.AddModelError("originator", "The record already exists.");
                    return View(formsFlow);
                }
                db.FormsFlows.Add(formsFlow);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(formsFlow);
        }
        public ActionResult IndexFormsFlow()
        {

            return View();
        }


        //add code for new forms routes 
        public ActionResult newFormssubmission(int? formname,string department)
        {

            ViewBag.department = department;
            if (!department.IsNullOrWhiteSpace())
            {
                
            this.ViewBag.formname = new SelectList(db.Forms.Where(x=>x.department == department).ToList(), "Id", "form_name", null);
            if (formname.HasValue)
            {
                if (formname.Value == 1)
                {
                    return RedirectToAction("OSRForm");
                }
            }
            }

            return View();
        }



        public ActionResult Formstatus()
        {

            return View();
        }

        
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            // Get properties of the type T
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add columns to DataTable
            foreach (PropertyInfo property in properties)
            {
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            // Add rows to DataTable
            foreach (T item in items)
            {
                DataRow row = dataTable.NewRow();
                foreach (PropertyInfo property in properties)
                {
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
        

        public ActionResult OSRForm()
        {

            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var OSRFcheck = new List<OSRFormref>();
            if (db.OSRFormrefs.ToList().Any())
            {
                OSRFcheck = db.OSRFormrefs
                    .Where(x => x.Employee_id == empuser.master_file.employee_id && x.Fdate == DateTime.Today).ToList();

            }

            if (!OSRFcheck.Any())
            {
                var OSRFormvar = new OSRFormref();
                OSRFormvar.master_file = empuser.master_file;
                OSRFormvar.Fdate = DateTime.Now;
                OSRFormvar.Employee_id = empuser.employee_no;
                OSRFormvar.org = "citiscape";
                OSRFormvar.prep_by = empuser.master_file.employee_name;
                db.OSRFormrefs.Add(OSRFormvar);
                db.SaveChanges();
            }

            var OSRFcheck1 = db.OSRFormrefs
                .Where(x => x.Employee_id == empuser.master_file.employee_id && x.Fdate == DateTime.Today).ToList();

            return RedirectToAction("OSRitems", new { osrFormrefvar = OSRFcheck1.First().Id });
        }

        public ActionResult OSRitems(int osrFormrefvar,bool? save)
        {
            ViewBag.osrFormref = osrFormrefvar;
            if (save.HasValue && save.Value)
            {
                return RedirectToAction("");
            }

            var OSRFdetailes = db.OSRFormrefs
                .Where(x => x.Id == osrFormrefvar).ToList();
            if (OSRFdetailes.Any())
            {
                if (OSRFdetailes.First().OSRF_items.ToList().Any())
                {
                    var itemlist = OSRFdetailes.First().OSRF_items.ToList();
                    var dt = ToDataTable(itemlist);
                    ViewBag.Data = dt;
                }
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OSRitems(OSRF_items osrfItems)
        {

            ViewBag.osrFormref = osrfItems.OSRFref.Value;

            if (osrfItems.description.IsNullOrWhiteSpace())
            {
                ViewBag.ErrorMessage = "no item was added";
                return View();
            }
            if (osrfItems.quantity.IsNullOrWhiteSpace())
            {
                osrfItems.quantity = "1";
            }
            ViewBag.disablebutton = "";
            if (db.OSRF_items.Where(x=>x.OSRFref == osrfItems.OSRFref).Count() <= 14)
            {
                db.OSRF_items.Add(osrfItems);
                db.SaveChanges();
            }
            else
            {
                ViewBag.ErrorMessage = "max entries have been added";
                ViewBag.disablebutton = "disabled";
                return View();
            }
            return RedirectToAction("OSRitems",new{ osrFormrefvar = osrfItems.OSRFref.Value });
        }

        public ActionResult formsdashboard()
        {

            return View();
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
