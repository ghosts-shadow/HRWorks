using System.Data.Entity;
using Microsoft.Ajax.Utilities;

namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using HRworks.Models;

    using OfficeOpenXml;

    public class hiksController : Controller
    {
        private readonly HREntities db = new HREntities();
        private readonly LogisticsSoftEntities db1 = new LogisticsSoftEntities();

        public ActionResult Absent(DateTime? getdate,string empnos)
         {
            var month = getdate;
            this.ViewBag.eddate = getdate;
            var tmlist = this.db.hiks.OrderBy(x => x.datetime).ToList();
            var msname = this.db.master_file.ToList();
            var emplist = new List<hik>();
            var emplist1 = new List<hik>();
            var leavelist= db.Leaves.ToList();
            foreach (var hik in tmlist)
            {
                if (!emplist1.Exists(x => x.ID == hik.ID))
                {
                    if (hik.ID != string.Empty)
                    {
                        emplist1.Add(hik);
                        var qwEmployeeId = 0;
                        int.TryParse(hik.ID, out qwEmployeeId);
                        if (msname.Exists(x => x.employee_no == qwEmployeeId))
                            hik.Person = msname.Find(x => x.employee_no == qwEmployeeId).employee_name;
                        hik.EMPID = qwEmployeeId;
                    }
                }
            }

            foreach (var hik in emplist1)
            {

                var activeuser = msname.FindAll(x => x.status == "active"|| x.status == "in process");
                if (activeuser.Exists(x => x.employee_no.ToString() == hik.ID))
                {
                    emplist.Add(hik);
                }
            }
            var abslist = new List<hik>();
            if (month.HasValue)
            {
                var startdate = new DateTime(month.Value.Year, month.Value.Month, 1);
                var enddate = new DateTime(
                    month.Value.Year,
                    month.Value.Month,
                    DateTime.DaysInMonth(month.Value.Year, month.Value.Month));
                var holidays = db1.Holidays.Where(x =>
                    x.Date.Value.Month == month.Value.Month && x.Date.Value.Year == month.Value.Year).ToList();
                while (startdate <= enddate)
                {
                    if (startdate >= DateTime.Now.Date)
                    {
                        break;
                    }
                    foreach (var hik in emplist)
                    {
                        if (!tmlist.Exists(x => x.ID == hik.ID && x.date == startdate))
                        {
                            if (startdate.DayOfWeek != DayOfWeek.Saturday)
                            {
                                if (startdate.DayOfWeek != DayOfWeek.Sunday)
                                {
                                    if (!holidays.Exists(x=>x.Date == startdate))
                                    {
                                        var emp = new hik();
                                        var newdate = new DateTime(startdate.Year, startdate.Month, startdate.Day);
                                        emp.ID = hik.ID;
                                        emp.EMPID = hik.EMPID;
                                        emp.Person = hik.Person;
                                        emp.date = newdate;
                                        var qwEmployeeId = 0;
                                        int.TryParse(hik.ID, out qwEmployeeId);
                                        ViewBag.absapp = empnos;
                                        if (!empnos.IsNullOrWhiteSpace() && empnos.Contains(hik.ID + "("+newdate.ToString()+")"))
                                        {
                                            emp.absence_approved = true;
                                        }
                                        if (!leavelist.Exists(x =>
                                            x.Start_leave <= startdate && x.End_leave >= startdate &&
                                            x.master_file.employee_no == qwEmployeeId))
                                        {
                                            abslist.Add(emp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    startdate = startdate.AddDays(1);
                }
            }
            
            return this.View(abslist.OrderBy(x => x.date).ThenBy(x=>x.EMPID));
        }

        public ActionResult Abstransfer(int? id,DateTime? date1,string empnos)
        {
            var newlistemo = new List<int>();
            var neweos = "";
            var abslist = db.leave_absence.ToList();
            var mastervar = db.master_file.OrderByDescending(x => x.date_changed).ToList();
            if (!abslist.Exists(x=>x.master_file.employee_no == id && x.fromd <= date1 && x.tod >= date1))
            {
                var absvar = new leave_absence();
                var emp = mastervar.Find(x => x.employee_no == id);
                if (emp != null)
                {
                    var date2 = date1.Value.AddDays(-1);
                    if (abslist.Exists(x=>x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                    {
                        var abs1 = abslist.Find(x=>x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                        abs1.tod = date1;
                        abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                        this.db.Entry(abs1).State = EntityState.Modified;
                        this.db.SaveChanges();
                    }
                    else
                    {
                        absvar.Employee_id = emp.employee_id;
                        absvar.fromd = date1;
                        absvar.tod = date1;
                        absvar.absence = 1;
                        absvar.month = date1;
                        this.db.leave_absence.Add(absvar);
                        this.db.SaveChanges();
                    }

                    if (!empnos.IsNullOrWhiteSpace())
                    {
                        if (!empnos.Contains(emp.employee_no.ToString() + "("+date1.ToString()+")"))
                        {
                            empnos += " , " + emp.employee_no +"(" +date1.ToString()+ ")";
                            neweos = empnos;
                        }
                    }
                    else
                    {
                        empnos = emp.employee_no.ToString() +"(" +date1.ToString()+ ")";
                        neweos = empnos;
                    }
                }
            }

            return RedirectToAction("Absent", new {getdate = date1,empnos = neweos});
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
                var Sheet = Ep.Workbook.Worksheets.Add(getdate.ToString());
                Sheet.Cells["A1"].Value = "employee no";
                Sheet.Cells["B1"].Value = "employee name";
                Sheet.Cells["C1"].Value = "date ";
                Sheet.Cells["D1"].Value = "hours ";
                var row = 2;
                foreach (var item in passexel.OrderBy(x => x.Employee_id))
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
                this.Response.AddHeader("content-disposition", "filename =attendance_for_" + getdate + ".xlsx");
                this.Response.BinaryWrite(Ep.GetAsByteArray());
                this.Response.End();
            }

            return this.RedirectToAction("Index");
        }

        public ActionResult DownloadExcel1(DateTime? getdate, DateTime? todate)
        {

            if (!todate.HasValue && getdate.HasValue)
            {
                getdate = new DateTime(getdate.Value.Year, getdate.Value.Month, 1);
                todate = new DateTime(
                    getdate.Value.Year,
                    getdate.Value.Month,
                    DateTime.DaysInMonth(getdate.Value.Year, getdate.Value.Month));
            }
            var chin = new List<hik>();
            var chout = new List<hik>();
            var schin = new List<hik>();
            var schout = new List<hik>();
            List<hik> passexel = new List<hik>();
            var msname = this.db.master_file.ToList();
            var tmlist = this.db.hiks.OrderBy(x => x.datetime).ToList();
            foreach (var hik in tmlist.FindAll(x => x.date >= getdate && x.date <= todate))
            {
                int.TryParse(hik.ID, out var qwEmployeeId);
                hik.EMPID = qwEmployeeId;
                if (msname.Exists(x => x.employee_no == qwEmployeeId))
                    hik.Person = msname.Find(x => x.employee_no == qwEmployeeId).employee_name;
                if (hik.Status == "CheckOut" && hik.date >= getdate && hik.date <= todate) chout.Add(hik);
                if (hik.Status == "CheckIN" && hik.date >= getdate && hik.date <= todate) chin.Add(hik);
            }

            foreach (var w1 in chin.OrderBy(x => x.EMPID).ThenBy(x => x.datetime))
                if (!schin.Exists(x => x.date == w1.date && x.EMPID == w1.EMPID))
                    schin.Add(w1);

            foreach (var w1 in chout.OrderBy(x => x.EMPID).ThenByDescending(x => x.datetime))
                if (!schout.Exists(x => x.date == w1.date && x.EMPID == w1.EMPID))
                    schout.Add(w1);
            var chdecin = new List<hik>();
            chdecin = chin.OrderBy(x => x.EMPID).ThenByDescending(x => x.datetime).ToList();
            var schinout = new List<hik>();
            foreach (var hik in schin.OrderBy(x => x.EMPID).ThenBy(x => x.datetime))
            {
                if (!schinout.Exists(x => x.EMPID == hik.EMPID && x.date == hik.date) && !schout.Exists(x2 => x2.EMPID == hik.EMPID && x2.date == hik.date))
                {
                    var tempchin = new hik();
                    tempchin = chdecin.Find(c => c.EMPID == hik.EMPID);
                    if (!schin.Exists(x1 => x1.EMPID == tempchin.EMPID && x1.datetime == tempchin.datetime))
                    {
                        schinout.Add(tempchin);
                    }
                }
            }

            foreach (var hik1 in schinout)
            {
                if (hik1.Status == "CheckIN")
                {
                    hik1.Status = "CheckOut";
                }
            }
            passexel.AddRange(schinout);

            passexel.AddRange(schin);
            passexel.AddRange(schout);
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add(getdate.ToString());
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "datetime ";
            Sheet.Cells["D1"].Value = "date ";
            Sheet.Cells["E1"].Value = "time";
            Sheet.Cells["F1"].Value = "status";
            var row = 2;
            foreach (var item in passexel.OrderBy(x => x.date).ThenBy(x => x.EMPID))
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ID;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Person;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.datetime;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.date;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.time;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Status;
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename =attendance_for_" + getdate + ".xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
            return this.RedirectToAction("Index");
        }

        public ActionResult DownloadExcel2(DateTime? getdate)
        {
            List<hik> passexel = new List<hik>();
            var month = getdate;
            var tmlist = this.db.hiks.OrderBy(x => x.datetime).ToList();
            var msname = this.db.master_file.ToList();
            var emplist = new List<hik>();
            var leavelist = db.Leaves.ToList();
            foreach (var hik in tmlist)
            {
                if (!emplist.Exists(x => x.ID == hik.ID))
                {
                    if (hik.ID != string.Empty)
                    {
                        emplist.Add(hik);
                        var qwEmployeeId = 0;
                        int.TryParse(hik.ID, out qwEmployeeId);
                        if (msname.Exists(x => x.employee_no == qwEmployeeId))
                            hik.Person = msname.Find(x => x.employee_no == qwEmployeeId).employee_name;
                    }
                }
            }

            var abslist = new List<hik>();
            if (month.HasValue)
            {
                var startdate = new DateTime(month.Value.Year, month.Value.Month, 1);
                var enddate = new DateTime(
                    month.Value.Year,
                    month.Value.Month,
                    DateTime.DaysInMonth(month.Value.Year, month.Value.Month));
                while (startdate != enddate)
                {
                    if (startdate >= DateTime.Now.Date)
                    {
                        break;
                    }
                    foreach (var hik in emplist)
                    {
                        if (!tmlist.Exists(x => x.ID == hik.ID && x.date == startdate))
                        {
                            if (startdate.DayOfWeek != DayOfWeek.Saturday)
                            {
                                if (startdate.DayOfWeek != DayOfWeek.Sunday)
                                {
                                    var emp = new hik();
                                    var newdate = new DateTime(startdate.Year, startdate.Month, startdate.Day);
                                    emp.ID = hik.ID;
                                    emp.EMPID = hik.EMPID;
                                    emp.Person = hik.Person;
                                    emp.date = newdate;
                                    var qwEmployeeId = 0;
                                    int.TryParse(hik.ID, out qwEmployeeId);
                                    if (!leavelist.Exists(
                                            x => x.Start_leave <= startdate && x.End_leave >= startdate
                                                                            && x.master_file.employee_no
                                                                            == qwEmployeeId))
                                    {
                                        abslist.Add(emp);
                                    }
                                }
                            }
                        }

                    }
                    startdate = startdate.AddDays(1);

                }
            }

            passexel = abslist;
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add(getdate.ToString());
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "date ";
            var row = 2;
            foreach (var item in passexel.OrderBy(x => x.date).ThenBy(x=>x.EMPID))
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ID;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Person;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.date.Value.ToString("dd MMM yyyy");
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename =absences.xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
            return this.RedirectToAction("Index");
        }

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
        // // GET: hiks/Delete/5
        // public ActionResult Delete(int? id)
        // {
        // if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        // var hik = this.db.hiks.Find(id);
        // if (hik == null) return this.HttpNotFound();
        // return this.View(hik);
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
            this.ViewBag.eddate = getdate;
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

        public ActionResult index1(DateTime? getdate, DateTime? todate)
        {
            if (!todate.HasValue && getdate.HasValue)
            {
                getdate = new DateTime(getdate.Value.Year, getdate.Value.Month, 1);
                todate = new DateTime(
                    getdate.Value.Year,
                    getdate.Value.Month,
                    DateTime.DaysInMonth(getdate.Value.Year, getdate.Value.Month));
            }
            this.ViewBag.eddate = getdate;
            this.ViewBag.eddate2 = todate;
            var chin = new List<hik>();
            var chout = new List<hik>();
            var schin = new List<hik>();
            var schout = new List<hik>();
            List<hik> passexel = new List<hik>();
            var msname = this.db.master_file.ToList();
            var tmlist = this.db.hiks.OrderBy(x => x.datetime).ToList();
            foreach (var hik in tmlist.FindAll(x => x.date >= getdate && x.date <= todate))
            {
                int.TryParse(hik.ID, out var  qwEmployeeId);
                hik.EMPID = qwEmployeeId;
                if (msname.Exists(x => x.employee_no == qwEmployeeId)) { 
                    hik.Person = msname.Find(x => x.employee_no == qwEmployeeId).employee_name;
                }
                if (hik.Status == "CheckOut" && hik.date >= getdate && hik.date <= todate) chout.Add(hik);
                if (hik.Status == "CheckIN" && hik.date >= getdate && hik.date <= todate) chin.Add(hik);
            }

            foreach (var w1 in chin.OrderBy(x => x.EMPID).ThenBy(x => x.datetime))
                if (!schin.Exists(x => x.date == w1.date && x.EMPID == w1.EMPID))
                    schin.Add(w1);
            foreach (var w1 in chout.OrderBy(x => x.EMPID).ThenByDescending(x => x.datetime))
                if (!schout.Exists(x => x.date == w1.date && x.EMPID == w1.EMPID))
                    schout.Add(w1);
            var chdecin = new List<hik>();
            chdecin = chin.OrderBy(x => x.EMPID).ThenByDescending(x => x.datetime).ToList();
            var schinout = new List<hik>();
            foreach (var hik in schin.OrderBy(x => x.EMPID).ThenBy(x => x.datetime))
            {
                if (!schinout.Exists(x=>x.EMPID == hik.EMPID && x.date == hik.date) && !schout.Exists(x2=>x2.EMPID == hik.EMPID && x2.date == hik.date))
                {
                    var tempchin = new hik(); 
                    tempchin = chdecin.Find(c => c.EMPID == hik.EMPID);
                    if (!schin.Exists(x1 => x1.EMPID == tempchin.EMPID && x1.datetime == tempchin.datetime))
                    {
                        schinout.Add(tempchin);
                    }
                }
            }

            foreach (var hik1 in schinout)
            {
                if (hik1.Status == "CheckIN")
                {
                    hik1.Status = "CheckOut";
                }
            }
            passexel.AddRange(schinout);
            passexel.AddRange(schin);
            passexel.AddRange(schout);
            return this.View(passexel.OrderBy(x => x.date).ThenBy(x => x.EMPID));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}