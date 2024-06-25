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
using Microsoft.Office.Interop.Word;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace HRworks.Controllers
{
    public class project_attendenceController : Controller
    {
        private HREntities db = new HREntities();

        // GET: project_attendence
        public ActionResult Index()
        {
            var project_attendence = db.project_attendence.Include(p => p.HRprojectlist).Include(p => p.master_file);
            return View(project_attendence.ToList());
        }



        public ActionResult Importpattendance()
        {
            ViewBag.project_id = new SelectList(db.HRprojectlists, "Id", "project_name");
            return this.View();
        }

        [ActionName("Importpattendance")]
        [HttpPost]
        public ActionResult ImportpAttendance(int? project_id)
        {
            ViewBag.project_id = new SelectList(db.HRprojectlists, "Id", "project_name");
            if (!project_id.HasValue)
            {
                this.ViewBag.Error = "Please select project";
                return this.View();
            }

            var errorlist = new List<Exception>();
            var wcterror = new List<project_attendence>();
            var errorcount = 0;
            var uploadedcount = 0;
            var proattlist = new List<project_attendence>();
            var totalfilecount = 0;
            if (this.Request.Files["FileUpload1"].ContentLength > 0)
            {
                var extension = Path.GetExtension(this.Request.Files["FileUpload1"].FileName).ToLower();
                string query = null;
                var connString = string.Empty;
                string[] validFileTypes = { ".csv",".dat" };
                var path1 = string.Format(
                    "{0}/{1}",
                    this.Server.MapPath("~/Content/Uploads"),
                    this.Request.Files["FileUpload1"].FileName);
                if (!Directory.Exists(path1)) Directory.CreateDirectory(this.Server.MapPath("~/Content/Uploads"));
                var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed)
                    .ToList();
                var afinallist = new List<master_file>();
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
                                var pro = new project_attendence();
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (dr[column] == null || dr[column].ToString().IsNullOrWhiteSpace()) goto e;


                                    if (column.ColumnName == "sJobNo")
                                    {
                                        //need to fix it to be character by character 
                                        var tempconverts = dr[column].ToString();
                                        var tempconvert = tempconverts.ToCharArray();
                                        var convresult = "";
                                        for (int i = 0; i < tempconvert.Length; i++)
                                        {
                                            if (int.TryParse(tempconvert[i].ToString(), out int result))
                                            {
                                                convresult += tempconvert[i].ToString();
                                            }

                                        }
                                        int.TryParse(convresult, out var idm);
                                        if (idm != 0)
                                        {
                                            var epid = afinallist.Find(x => x.employee_no == idm);
                                            if (epid == null)
                                            {
                                                errorlist.Add(new Exception("no emp"+convresult));
                                                goto e;
                                            }
                                            pro.employee_id = epid.employee_id;
                                        }
                                        else
                                        {

                                            errorlist.Add(new Exception("not number" + convresult));
                                        }
                                    }



                                    else if (column.ColumnName == "Date")
                                    {
                                        var dtt = dr[column].ToString();
                                        DateTime.TryParse(dtt, out var a);
                                        pro.at_date = a;
                                    }
                                    else if (column.ColumnName == "Time")
                                    {
                                        var dtt = dr[column].ToString();
                                        TimeSpan.TryParse(dtt, out var a);
                                        pro.at_time = a;
                                    }
                                    else if (column.ColumnName == "AttendanceStatus")
                                    {
                                        pro.at_status = dr[column].ToString(); ;
                                    }


                                }
                                    totalfilecount++;

                                if (project_id.HasValue)
                                {
                                    pro.project_id = project_id.Value;
                                }
                                if (pro.project_id == 0)
                                {
                                    errorlist.Add(new Exception("no project"));
                                    goto e;
                                }
                                pro.at_datetime = new DateTime(pro.at_date.Value.Year, pro.at_date.Value.Month,
                                    pro.at_date.Value.Day, pro.at_time.Value.Hours, pro.at_time.Value.Minutes,
                                    pro.at_time.Value.Seconds);
                                /**/
                                try
                                { proattlist.Add(pro);
                                }
                                catch (Exception exception)
                                {
                                    errorlist.Add(exception);
                                }

                            e:;
                            }
                        }
                    }
                    else if (extension == ".dat")
                    {
                        if (System.IO.File.Exists(path1)) System.IO.File.Delete(path1);
                        this.Request.Files["FileUpload1"].SaveAs(path1);
                        string[] lines = System.IO.File.ReadAllLines(path1);
                        totalfilecount = lines.Length;
                        for (int i = 0; i <= lines.Length - 1; i++)
                        {

                            if (!string.IsNullOrEmpty(lines[i].Trim()))
                            {
                                var empnotemp = 0;
                                var attdatetime = new DateTime();
                                string[] attendance_entries = lines[i].Trim().Split('\t');
                                var status = attendance_entries[3];
                                int.TryParse(attendance_entries[0], out empnotemp);
                                DateTime.TryParse(attendance_entries[1], out attdatetime);
                                var epid = afinallist.Find(x => x.employee_no == empnotemp);
                                if (epid == null)
                                {
                                    errorlist.Add(new Exception("no emp"));
                                    goto e;
                                }

                                if (!project_id.HasValue || project_id ==0)
                                {
                                    errorlist.Add(new Exception("no project"));
                                    goto e;
                                }

                                proattlist.Add(new project_attendence
                                {
                                    employee_id = epid.employee_id,
                                    project_id = project_id.Value,
                                    at_datetime = attdatetime,
                                    at_date = attdatetime.Date,
                                    at_time = attdatetime.TimeOfDay,
                                    at_status = status,
                                    uploaded_by = null,
                                });

                            }
                            e: ;
                        }

                    }
                }
                foreach (var pro in proattlist)
                {
                    var proatlist = db.project_attendence.ToList();
                    try
                    {
                        if (!proatlist.Exists(x=>x.employee_id == pro.employee_id  && x.project_id == pro.project_id  && x.at_datetime == pro.at_datetime ))
                        {
                            if (pro.employee_id == 0)
                            {
                                throw new Exception("employee not found");
                            }
                            this.db.project_attendence.Add(pro);
                            this.db.SaveChanges();
                            uploadedcount++;
                        }
                        else
                        {
                            throw new Exception("already exist");
                        }

                    }
                    catch (Exception exception)
                    {
                        errorcount += 1;
                        errorlist.Add(exception);
                        if (exception.InnerException !=null)
                        {
                            pro.at_status = exception.InnerException.Message;
                        }
                        else
                        {
                            pro.at_status = exception.Message;
                        }
                        wcterror.Add(pro);
                    }
                }
            }
            else
            {
                this.ViewBag.Error = "Please Upload Files in .csv or .dat format";
            }

            var totalvalidcount = proattlist.Count;
            if (errorcount >= 0)
            {
                this.ViewBag.Error = errorcount + " records were not uploaded out of " + totalvalidcount + "("+totalfilecount+")";
            }
            return this.View();
        }




        /*
        // GET: project_attendence/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project_attendence project_attendence = db.project_attendence.Find(id);
            if (project_attendence == null)
            {
                return HttpNotFound();
            }
            return View(project_attendence);
        }

        // GET: project_attendence/Create
        public ActionResult Create()
        {
            ViewBag.project_id = new SelectList(db.HRprojectlists, "Id", "project_name");
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: project_attendence/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,employee_id,project_id,at_datetime,at_date,at_time,at_status,uploaded_by,modified_date")] project_attendence project_attendence)
        {
            if (ModelState.IsValid)
            {
                db.project_attendence.Add(project_attendence);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.project_id = new SelectList(db.HRprojectlists, "Id", "project_name", project_attendence.project_id);
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", project_attendence.employee_id);
            return View(project_attendence);
        }

        // GET: project_attendence/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project_attendence project_attendence = db.project_attendence.Find(id);
            if (project_attendence == null)
            {
                return HttpNotFound();
            }
            ViewBag.project_id = new SelectList(db.HRprojectlists, "Id", "project_name", project_attendence.project_id);
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", project_attendence.employee_id);
            return View(project_attendence);
        }

        // POST: project_attendence/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,employee_id,project_id,at_datetime,at_date,at_time,at_status,uploaded_by,modified_date")] project_attendence project_attendence)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project_attendence).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.project_id = new SelectList(db.HRprojectlists, "Id", "project_name", project_attendence.project_id);
            ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", project_attendence.employee_id);
            return View(project_attendence);
        }

        // GET: project_attendence/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project_attendence project_attendence = db.project_attendence.Find(id);
            if (project_attendence == null)
            {
                return HttpNotFound();
            }
            return View(project_attendence);
        }

        // POST: project_attendence/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            project_attendence project_attendence = db.project_attendence.Find(id);
            db.project_attendence.Remove(project_attendence);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

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
