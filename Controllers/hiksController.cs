﻿namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;

    using HRworks.Models;

    using OfficeOpenXml;

    public class hiksController : Controller
    {
        private readonly HREntities db = new HREntities();

        // GET: hiks/Create
        /*public ActionResult Create()
        {
            return this.View();
        }

        // POST: hiks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "ID,datetime,date,time,Status,Device,DeviceNo,Person,SID")]
            hik hik)
        {
            if (this.ModelState.IsValid)
            {
                this.db.hiks.Add(hik);
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            return this.View(hik);
        }*/
        //
        // // GET: hiks/Delete/5
        // public ActionResult Delete(int? id)
        // {
        //     if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //     var hik = this.db.hiks.Find(id);
        //     if (hik == null) return this.HttpNotFound();
        //     return this.View(hik);
        // }

        /*
        // POST: hiks/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var hik = this.db.hiks.Find(id);
            this.db.hiks.Remove(hik);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        // GET: hiks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var hik = this.db.hiks.Find(id);
            if (hik == null) return this.HttpNotFound();
            return this.View(hik);
        }
        */

        /*
        // GET: hiks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var hik = this.db.hiks.Find(id);
            if (hik == null) return this.HttpNotFound();
            return this.View(hik);
        }

        // POST: hiks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "ID,datetime,date,time,Status,Device,DeviceNo,Person,SID")]
            hik hik)
        {
            if (this.ModelState.IsValid)
            {
                this.db.Entry(hik).State = EntityState.Modified;
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            return this.View(hik);
        }
        */

        // GET: hiks
        public ActionResult Index(DateTime? getdate)
        {
            
            ViewBag.eddate = getdate;
            var tmlist = this.db.hiks.OrderBy(x => x.ID).ThenBy(x => x.datetime).ToList();
            var hiklist = new List<hikshrs>();
            var chin = new List<hik>();
            var chout = new List<hik>();
            var schin = new List<hik>();
            var schout = new List<hik>();
            if (getdate != null)
            {
                foreach (var hik in tmlist)
                {
                    if (hik.Status == "CheckOut" && hik.date == getdate) chout.Add(hik);
                    if (hik.Status == "CheckIN" && hik.date == getdate) chin.Add(hik);
                }

                var msname = this.db.master_file.ToList();
                foreach (var w1 in chin.OrderBy(x => x.ID).ThenBy(x => x.datetime))
                    if (!schin.Exists(x => x.date == w1.date && x.ID == w1.ID))
                        schin.Add(w1);
                foreach (var w1 in chout.OrderBy(x => x.ID).ThenByDescending(x => x.datetime))
                    if (!schout.Exists(x => x.date == w1.date && x.ID == w1.ID))
                        schout.Add(w1);

                foreach (var hik in schout)
                {
                    var choutt1 = schin.Find(x => x.ID == hik.ID && x.date == hik.date);
                    if (choutt1 != null)
                    {
                        var choutt = choutt1.datetime;
                        var qw = new hikshrs();
                        var qwEmployeeId = 0;
                        int.TryParse(hik.ID, out qwEmployeeId);
                        qw.Employee_id = qwEmployeeId;
                        if (msname.Exists(x => x.employee_no == qwEmployeeId))
                            qw.Employee_name = msname.Find(x => x.employee_no == qwEmployeeId).employee_name;
                        else qw.Employee_name = hik.Person;
                        qw.datetime1 = hik.datetime;
                        qw.datelowerlimit = choutt.Value;
                        qw.dateupperlimit = hik.datetime.Value;
                        qw.hours = hik.datetime.Value - choutt.Value;
                        hiklist.Add(qw);
                    }
                }

                return this.View(hiklist.OrderBy(x => x.Employee_id).ThenBy(x => x.datetime1));
            }

            return this.View(hiklist.OrderBy(x => x.Employee_id).ThenBy(x => x.datetime1));
        }

        public ActionResult index1(DateTime? getdate)
        {

            ViewBag.eddate = getdate;
            List<hik> passexel = new List<hik>();
            var msname = this.db.master_file.ToList();
            var tmlist = this.db.hiks.OrderBy(x => x.ID).ThenBy(x => x.datetime).ToList();
            foreach (var hik in tmlist.FindAll(x => x.date == getdate))
            {
                var qwEmployeeId = 0;
                int.TryParse(hik.ID, out qwEmployeeId);
                if (msname.Exists(x => x.employee_no == qwEmployeeId))
                    hik.Person = msname.Find(x => x.employee_no == qwEmployeeId).employee_name;
                passexel.Add(hik);

            }
            return this.View(passexel);
        }

        public ActionResult DownloadExcel(DateTime? getdate)
        {
            List<hikshrs> passexel = new List<hikshrs>();
            var tmlist = this.db.hiks.OrderBy(x => x.ID).ThenBy(x => x.datetime).ToList();
            var hiklist = new List<hikshrs>();
            var chin = new List<hik>();
            var chout = new List<hik>();
            var schin = new List<hik>();
            var schout = new List<hik>();
            if (getdate != null)
            {
                foreach (var hik in tmlist)
                {
                    if (hik.Status == "CheckOut" && hik.date == getdate) chout.Add(hik);
                    if (hik.Status == "CheckIN" && hik.date == getdate) chin.Add(hik);
                }

                var msname = this.db.master_file.ToList();
                foreach (var w1 in chin.OrderBy(x => x.ID).ThenBy(x => x.datetime))
                    if (!schin.Exists(x => x.date == w1.date && x.ID == w1.ID))
                        schin.Add(w1);
                foreach (var w1 in chout.OrderBy(x => x.ID).ThenByDescending(x => x.datetime))
                    if (!schout.Exists(x => x.date == w1.date && x.ID == w1.ID))
                        schout.Add(w1);

                foreach (var hik in schout)
                {
                    var choutt1 = schin.Find(x => x.ID == hik.ID && x.date == hik.date);
                    if (choutt1 != null)
                    {
                        var choutt = choutt1.datetime;
                        var qw = new hikshrs();
                        var qwEmployeeId = 0;
                        int.TryParse(hik.ID, out qwEmployeeId);
                        qw.Employee_id = qwEmployeeId;
                        if (msname.Exists(x => x.employee_no == qwEmployeeId))
                            qw.Employee_name = msname.Find(x => x.employee_no == qwEmployeeId).employee_name;
                        else qw.Employee_name = hik.Person;
                        qw.datetime1 = hik.datetime;
                        qw.datelowerlimit = choutt.Value;
                        qw.dateupperlimit = hik.datetime.Value;
                        qw.hours = hik.datetime.Value - choutt.Value;
                        hiklist.Add(qw);
                        passexel.Add(qw);
                    }
                }

                var Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add( getdate.ToString());
                Sheet.Cells["A1"].Value = "employee no";
                Sheet.Cells["B1"].Value = "employee name";
                Sheet.Cells["C1"].Value = "date ";
                Sheet.Cells["D1"].Value = "hours ";
                var row = 2;
                foreach (var item in passexel.OrderBy(x=>x.Employee_id))
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Employee_id;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Employee_name;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.datetime1;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.hours.Hours;
                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                this.Response.Clear();
                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("content-disposition", "filename =attendance_for_"+getdate+".xlsx");
                this.Response.BinaryWrite(Ep.GetAsByteArray());
                this.Response.End();
            }
            return this.RedirectToAction("Index");
        }

        public ActionResult DownloadExcel1(DateTime? getdate)
        {
            List<hik> passexel = new List<hik>();
            var msname = this.db.master_file.ToList();
            var tmlist = this.db.hiks.OrderBy(x => x.ID).ThenBy(x => x.datetime).ToList();
            foreach (var hik in tmlist.FindAll(x=>x.date == getdate))
            {
                var qwEmployeeId = 0;
                int.TryParse(hik.ID, out qwEmployeeId);
                if (msname.Exists(x => x.employee_no == qwEmployeeId))
                    hik.Person = msname.Find(x => x.employee_no == qwEmployeeId).employee_name;
                passexel.Add(hik);
                
            }
            
            var Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add( getdate.ToString());
                Sheet.Cells["A1"].Value = "employee no";
                Sheet.Cells["B1"].Value = "employee name";
                Sheet.Cells["C1"].Value = "datetime ";
                Sheet.Cells["D1"].Value = "date ";
                Sheet.Cells["E1"].Value = "time";
                Sheet.Cells["F1"].Value = "status";
                Sheet.Cells["G1"].Value = "device ";
                Sheet.Cells["H1"].Value = "device no. ";
                var row = 2;
                foreach (var item in passexel.OrderBy(x=>x.ID))
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Person;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.datetime;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.date;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.time;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Status;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Device;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.DeviceNo;
                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                this.Response.Clear();
                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("content-disposition", "filename =attendance_for_"+getdate+".xlsx");
                this.Response.BinaryWrite(Ep.GetAsByteArray());
                this.Response.End();
            return this.RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}