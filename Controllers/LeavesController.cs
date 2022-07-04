namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;
    using System.Data;
    using HRworks.Models;
    using Microsoft.Ajax.Utilities;
    using OfficeOpenXml;
    using PagedList;

    public class LeavesController : Controller
    {
        private readonly HREntities db = new HREntities();


        // GET: Leaves/Create               
        public ActionResult Create()
        {
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "Annual", Value = "1"},
                new ListItem {Text = "Sick(non industrial)", Value = "2"},
                new ListItem {Text = "Compassionate", Value = "3"},
                new ListItem {Text = "Maternity", Value = "4"},
                new ListItem {Text = "Haj", Value = "5"},
                new ListItem {Text = "Unpaid", Value = "6"},
                new ListItem {Text = "Sick(industrial)", Value = "7"},
                new ListItem {Text = "UDDAH", Value = "8"},
                new ListItem {Text = "ESCORT", Value = "9"},
                new ListItem {Text = "PATERNITY ", Value = "10"}
            };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_id = new SelectList(
                afinallist.OrderBy(x => x.employee_no),
                "employee_id",
                "employee_no");
            ViewBag.employee_id1 = new SelectList(
                afinallist.OrderBy(e => e.employee_name),
                "employee_id",
                "employee_name");
            return this.View();
        }

        // POST: Leaves/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                Include =
                    "Employee_id,Id,Date,Reference,Start_leave,End_leave,Return_leave,days,leave_type,actual_return_date,half")]
            Leave leave,
            HttpPostedFileBase fileBase)
        {
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "Annual", Value = "1"},
                new ListItem {Text = "Sick(non industrial)", Value = "2"},
                new ListItem {Text = "Compassionate", Value = "3"},
                new ListItem {Text = "Maternity", Value = "4"},
                new ListItem {Text = "Haj", Value = "5"},
                new ListItem {Text = "Unpaid", Value = "6"},
                new ListItem {Text = "Sick(industrial)", Value = "7"},
                new ListItem {Text = "UDDAH", Value = "8"},
                new ListItem {Text = "ESCORT", Value = "9"},
                new ListItem {Text = "PATERNITY ", Value = "10"}
            };
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_id = new SelectList(
                afinallist.OrderBy(x => x.employee_no),
                "employee_id",
                "employee_no");
            ViewBag.employee_id1 = new SelectList(
                afinallist.OrderBy(e => e.employee_name),
                "employee_id",
                "employee_name");
            var leaveid = afinallist.Find(x => x.employee_id == leave.Employee_id);
            var jd = leaveid.date_joined;
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            string serverfile;
            var leavelistc = this.db.Leaves.ToList();
            if (leavelistc.Exists(x =>
                ((x.Start_leave <= leave.Start_leave && x.End_leave >= leave.Start_leave) ||
                 (x.Start_leave <= leave.End_leave && x.End_leave >= leave.End_leave)) &&
                x.Employee_id == leave.Employee_id))
            {
                ModelState.AddModelError("Start_leave", "already exists");

                goto jderr;
            }

            if (leavelistc.Exists(x =>
                (x.Start_leave >= leave.Start_leave && x.End_leave <= leave.End_leave) &&
                x.Employee_id == leave.Employee_id))
            {
                ModelState.AddModelError("Start_leave", "already exists");

                goto jderr;
            }


            //            if (leavelistc.Exists(x=>x.End_leave >= leave.Start_leave && x.Employee_id == leave.Employee_id))
            //            {
            //                ModelState.AddModelError("Start_leave", leave.Start_leave+" already exists");
            //
            //                goto jderr;
            //            }
            if (leave.Start_leave < jd && leave.Start_leave != default)
            {
                ModelState.AddModelError("Start_leave", "selected date is before " + jd);
                goto jderr;
            }

            if (leave.End_leave < jd && leave.End_leave != default)
            {
                ModelState.AddModelError("Start_leave", "selected date is before " + jd);
                goto jderr;
            }

            if (leave.End_leave < leave.Start_leave)
            {
                ModelState.AddModelError("Start_leave", "cannot add date in reverse");
                goto jderr;
            }

            if (fileBase != null)
            {
                var i = 0;
                var imgname = Path.GetFileName(fileBase.FileName);
                var fileexe = Path.GetExtension(fileBase.FileName);
                var filepath = new DirectoryInfo("D:/HR/leave/");
                serverfile = "D:/HR/leave/" + leave.Employee_id; /*+ "/"+ passport.employee_no + fileexe;*/
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

            var leavelist = this.db.Leaves.ToList();
            if (leavelist.Exists(
                x => x.Employee_id == leave.Employee_id && x.Start_leave == leave.Start_leave
                                                        && x.End_leave == leave.End_leave))
            {
                ViewBag.exhist = "the entry already exists";
                return this.View(leave);
            }

            ViewBag.exhist = "";
            if (this.ModelState.IsValid)
            {
                var file1 = new Leave();
                file1.Employee_id = leave.Employee_id;
                var masterstatus = this.db.master_file.Find(leave.Employee_id);
                file1.Date = leave.Date;
                file1.Reference = serverfile;
                file1.half = leave.half;
                file1.Start_leave = leave.Start_leave;
                file1.End_leave = leave.End_leave;
                if (leave.Return_leave == null)
                {
                    file1.Return_leave = file1.End_leave.Value.AddDays(1);
                }
                else
                {
                    file1.Return_leave = leave.Return_leave;
                }

                file1.leave_type = leave.leave_type;
                file1.days = leave.days;
                file1.data_o_n = "New";
                file1.actual_return_date = leave.actual_return_date;
                if (leave.actual_return_date == null) masterstatus.status = "on leave";
                else masterstatus.status = "active";
                file1.actualchangedby = User.Identity.Name;
                file1.actualchangeddateby = DateTime.Now;
                this.db.Leaves.Add(file1);
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            jderr: ;
            return this.View(leave);
        }

        // GET: Leaves/Delete/5
        public ActionResult Delete(int? id, string search, int? page)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var leave = this.db.Leaves.Find(id);
            if (leave == null) return this.HttpNotFound();
            ViewBag.search = search;
            ViewBag.page = page;
            return this.View(leave);
        }

        // POST: Leaves/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string search, int? page)
        {
            var leave = this.db.Leaves.Find(id);
            var masterstatus = this.db.master_file.Find(leave.Employee_id);
            masterstatus.status = "active";
            this.db.Leaves.Remove(leave);
            this.db.SaveChanges();
            ViewBag.search = search;
            ViewBag.page = page;
            return this.RedirectToAction("getallorone", new {search = search, page = page});
        }

        // GET: Leaves/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var leave = this.db.Leaves.Find(id);
            if (leave == null) return this.HttpNotFound();
            return this.View(leave);
        }

        public void DownloadExcel1(int? days)
        {
            List<Leave> passexel = new List<Leave>();
            var leaves = new List<Leave>();
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
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

            if (days.HasValue)
            {
                foreach (var file in afinallist)
                {
                    forfitedbalence(file.employee_id);
                }
                var leaveballist = this.db.leavecal2020.Where(x => x.leave_balance <= days.Value).ToList();
                foreach (var leavecal in leaveballist)
                {
                    var leaveempid = this.db.Leaves.Where(x => x.Employee_id == leavecal.Employee_id)
                        .Include(l => l.master_file).OrderByDescending(x => x.Id).ToList();
                    foreach (var leaf in leaveempid)
                    {
                        leaf.leave_bal = leavecal.leave_balance;
                        if (!leaves.Exists(x => x.Employee_id == leaf.Employee_id)) leaves.Add(leaf);
                    }
                }

                foreach (var emp in afinallist)
                {
                    if (emp.date_joined != null)
                    {
                        var leaf = new Leave();
                        if (leaveballist.Exists(x => x.Employee_id == emp.employee_id))
                        {
                            var lb = leaveballist.Find(x => x.Employee_id == emp.employee_id).leave_balance;
                            leaf.leave_bal = lb;
                            leaf.Employee_id = emp.employee_id;
                            leaf.master_file = emp;
                            if (!leaves.Exists(x => x.Employee_id == leaf.Employee_id)) leaves.Add(leaf);
                        }
                    }
                }

                passexel = leaves;
            }
            
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("leaves_report".ToUpper());
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "leave bal ";
            var row = 2;
            foreach (var item in passexel.OrderBy(x => x.leave_bal))
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.leave_bal;
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename =leaves_report.xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
        }

        public void DownloadExcel(long? Employee_id, DateTime? eddate)
        {
            double unpaid = 0;
            double availed = 0;
            double favailed = 0;
            double period = 0;
            double netperiod = 0;
            double lb = 0;
            double ac = 0;
            double forfeitedbal = 0;
            List<Leave> passexel = new List<Leave>();
            var emp_list = this.db.master_file.ToList();
            var emp_listfinal = new List<master_file>();
            foreach (var empid in emp_list)
            {
                var emp_unfilted = emp_list.FindAll(x => x.employee_no == empid.employee_no)
                    .OrderBy(x => x.employee_id);
                if (emp_unfilted != null) emp_listfinal.Add(emp_unfilted.First());
            }

            if (Employee_id == null && eddate == null)
            {
                passexel = this.db.Leaves.ToList();
                goto all;
            }

            if (Employee_id != null && eddate != null)
            {
                var empjd = emp_listfinal.Find(x => x.employee_no == Employee_id);
                Employee_id = empjd.employee_id;
                var asf = empjd.date_joined;
                var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                    x => x.Employee_id == Employee_id && x.Start_leave >= asf && x.End_leave <= eddate);
                var times = eddate - asf;
                double haj = 0;
                double mate = 0;
                double comp = 0;
                double sick = 0;
                double udd = 0;
                double esco = 0;
                double pater = 0;
                if (times != null)
                {
                    var ump = leaves.ToList();
                    foreach (var leaf in ump)
                    {
                        if (leaf.leave_type == "1")
                        {
                            if (leaf.half)
                            {
                                if (DateTime.Today > leaf.Start_leave)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) availed += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) favailed += times.Value.TotalDays + 1 - 0.5;
                                }
                            }
                            else
                            {
                                if (DateTime.Today > leaf.Start_leave)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) availed += times.Value.TotalDays + 1;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) favailed += times.Value.TotalDays + 1;
                                }
                            }

                        }

                        if (leaf.leave_type == "2" || leaf.leave_type == "7")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) sick += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) sick += times.Value.TotalDays + 1;
                            }
                        }

                        if (leaf.leave_type == "3")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) comp += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) comp += times.Value.TotalDays + 1;
                            }
                        }

                        if (leaf.leave_type == "4")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) mate += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) mate += times.Value.TotalDays + 1;
                            }
                        }

                        if (leaf.leave_type == "5")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) haj += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) haj += times.Value.TotalDays + 1;
                            }
                        }

                        if (leaf.leave_type == "6")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) unpaid += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) unpaid += times.Value.TotalDays + 1;
                            }
                        }

                        if (leaf.leave_type == "8")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) udd += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) udd += times.Value.TotalDays + 1;
                            }
                        }

                        if (leaf.leave_type == "9")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) esco += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) esco += times.Value.TotalDays + 1;
                            }
                        }

                        if (leaf.leave_type == "10")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) pater += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) pater += times.Value.TotalDays + 1;
                            }
                        }
                    }
                    passexel = ump;
                    if (empjd.date_joined != null)
                    {
                        var forfeitedlist = db.leavecal2020.ToList();
                        var forfeited = forfeitedlist.Find(x => x.Employee_id == Employee_id);
                        period = forfeited.periodafter2020.Value + forfeited.periodtill2020.Value;
                        netperiod = forfeited.net_periodafter2020.Value + forfeited.net_periodtill2020.Value;
                        ac = forfeited.accruedafter2020.Value + forfeited.accruedtill2020.Value;
                        lb = forfeited.leave_balance.Value;
                        forfeitedbal = forfeited.forfitedafter2020.Value+forfeited.forfitedtill2020.Value;
                    }
                }
            }

            all:
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("leaves_report".ToUpper());
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "date";
            Sheet.Cells["D1"].Value = "from ";
            Sheet.Cells["E1"].Value = "to";
            Sheet.Cells["F1"].Value = "actual return date ";
            Sheet.Cells["G1"].Value = "days ";
            Sheet.Cells["H1"].Value = "leave type ";
            Sheet.Cells["I1"].Value = "is half-day included";
            var row = 2;
            foreach (var item in passexel.OrderByDescending(x => x.Start_leave).ToList())
            {
                var stday = new DateTime();
                var edday = new DateTime();
                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Date.ToString("yy-MMM-dd");
                if (item.Start_leave.HasValue)
                {
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Start_leave.Value.ToString("yy-MMM-dd");
                    stday = item.Start_leave.Value;
                }

                if (item.End_leave.HasValue)
                {
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.End_leave.Value.ToString("yy-MMM-dd");
                    edday = item.End_leave.Value;
                }

                if (item.actual_return_date.HasValue)
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.actual_return_date.Value.ToString("yy-MMM-dd");
                if (item.half)
                {
                    Sheet.Cells[string.Format("G{0}", row)].Value =
                        ((edday - stday).Days + 1 - 0.5);
                }
                else
                {
                    Sheet.Cells[string.Format("G{0}", row)].Value =
                        ((edday - stday).Days + 1);
                }

                if (item.leave_type == "1")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = " Annual";
                }

                if (item.leave_type == "2")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = " Sick";
                }

                if (item.leave_type == "3")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = " Compassionate";
                }

                if (item.leave_type == "4")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = " Maternity";
                }

                if (item.leave_type == "5")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = " Haj";
                }

                if (item.leave_type == "6")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = " Unpaid";
                }

                if (item.leave_type == "7")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = " others";
                }

                if (item.leave_type == "8")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = "UDDAH";
                }

                if (item.leave_type == "9")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = "Escort";
                }

                if (item.leave_type == "10")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = "Paternity";
                }

                if (item.half)
                {
                    Sheet.Cells[string.Format("I{0}", row)].Value = "yes";
                }
                else
                {
                    Sheet.Cells[string.Format("I{0}", row)].Value = "no";
                }

                row++;
            }

            row += 5;
            Sheet.Cells[string.Format("A{0}", row)].Value = "Period =" + period;
            Sheet.Cells[string.Format("A{0}", row + 1)].Value = "Unpaid =" + unpaid;
            Sheet.Cells[string.Format("A{0}", row + 2)].Value = "Netperiod =" + netperiod;
            Sheet.Cells[string.Format("A{0}", row + 3)].Value = "Accrued =" + ac;
            Sheet.Cells[string.Format("A{0}", row + 4)].Value = "annual leave taken =" + (availed);
            Sheet.Cells[string.Format("A{0}", row + 5)].Value = "annual leave applied =" + (favailed );
            Sheet.Cells[string.Format("A{0}", row + 6)].Value = "annual leave total =" + (favailed + availed);
            Sheet.Cells[string.Format("A{0}", row + 7)].Value = "leave balance =" + lb;
            Sheet.Cells[string.Format("A{0}", row + 8)].Value = "leave balance =" + forfeitedbal;
            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition",
                "filename = leaves_report_summery_till_" + eddate.ToString() + ".xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
        }

        // GET: Leaves/Edit/5
        public ActionResult Edit(int? id, string search, int? page)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var leave = this.db.Leaves.Find(id);
            if (leave == null) return this.HttpNotFound();
            ViewBag.search = search;
            ViewBag.page = page;
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "Annual", Value = "1"},
                new ListItem {Text = "Sick(non industrial)", Value = "2"},
                new ListItem {Text = "Compassionate", Value = "3"},
                new ListItem {Text = "Maternity", Value = "4"},
                new ListItem {Text = "Haj", Value = "5"},
                new ListItem {Text = "Unpaid", Value = "6"},
                new ListItem {Text = "Sick(industrial)", Value = "7"},
                new ListItem {Text = "UDDAH", Value = "8"},
                new ListItem {Text = "ESCORT", Value = "9"},
                new ListItem {Text = "PATERNITY ", Value = "10"},
            };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text", leave.leave_type);
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            ViewBag.leavety = leave.leave_type;
            return this.View(leave);
        }

        // POST: Leaves/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Leave leave,
            HttpPostedFileBase fileBase, string search, int? page)
        {
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "Annual", Value = "1"},
                new ListItem {Text = "Sick(non industrial)", Value = "2"},
                new ListItem {Text = "Compassionate", Value = "3"},
                new ListItem {Text = "Maternity", Value = "4"},
                new ListItem {Text = "Haj", Value = "5"},
                new ListItem {Text = "Unpaid", Value = "6"},
                new ListItem {Text = "Sick(industrial)", Value = "7"},
                new ListItem {Text = "UDDAH", Value = "8"},
                new ListItem {Text = "ESCORT", Value = "9"},
                new ListItem {Text = "PATERNITY ", Value = "10"}
            };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            ViewBag.search = search;
            ViewBag.page = page;
            string serverfile;
            if (fileBase != null)
            {
                var i = 0;
                var imgname = Path.GetFileName(fileBase.FileName);
                var fileexe = Path.GetExtension(fileBase.FileName);
                var filepath = new DirectoryInfo("D:/HR/leave/");
                serverfile = "D:/HR/leave/" + leave.Employee_id; /*+ "/"+ passport.employee_no + fileexe;*/
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
                var filepath = db.Leaves.ToList().FindAll(x => x.Employee_id == leave.Employee_id)
                    .OrderByDescending(x => x.Start_leave).ToList();
                var filepath1 = filepath.FindAll(c => c.Reference != null);
                if (filepath1.Count != 0)
                {
                    var imgpath = filepath.FindAll(c => c.Reference != null).OrderByDescending(x => x.Start_leave)
                        .First().Reference;
                    serverfile = imgpath;
                }
            }

            if (this.ModelState.IsValid)
            {
                var leavelistc = this.db.Leaves.AsNoTracking().ToList();
                var editleave = leavelistc.Find(x => x.Id == leave.Id);
                leavelistc.Remove(editleave);
                if (leavelistc.Exists(x =>
                    ((x.Start_leave <= leave.Start_leave && x.End_leave >= leave.Start_leave) ||
                     (x.Start_leave <= leave.End_leave && x.End_leave >= leave.End_leave)) &&
                    x.Employee_id == leave.Employee_id))
                {
                    ModelState.AddModelError("Start_leave", "already exists");

                    goto jderr;
                }

                if (leavelistc.Exists(x =>
                    (x.Start_leave >= leave.Start_leave && x.End_leave <= leave.End_leave) &&
                    x.Employee_id == leave.Employee_id))
                {
                    ModelState.AddModelError("Start_leave", "already exists");

                    goto jderr;
                }


                leavelistc.Add(editleave);

                if (leave.End_leave < leave.Start_leave)
                {
                    ModelState.AddModelError("End_leave", "end date should not be less then start date");
                    goto jderr;
                }

                if (leave.Return_leave < leave.Start_leave)
                {
                    ModelState.AddModelError("Return_leave", "return date should not be less then start date");
                    goto jderr;
                }

                if (leave.Return_leave < leave.End_leave)
                {
                    ModelState.AddModelError("Return_leave", "return date should not be less then end date ");
                    goto jderr;
                }

                if (leave.actual_return_date < leave.Start_leave)
                {
                    ModelState.AddModelError("actual_return_date",
                        "actual return date should not be less then start date ");
                    goto jderr;
                }

                var actdate = new DateTime();
                var file1 = new Leave();
                file1.Employee_id = leave.Employee_id;
                var masterstatus = this.db.master_file.Find(leave.Employee_id);
                if (leave.actual_return_date < leave.Return_leave)
                {
                    actdate = leave.actual_return_date.Value;
                    file1 = leave;
                    file1.Date = leave.Date;
                    file1.Reference = serverfile;
                    file1.Start_leave = leave.Start_leave;
                    file1.End_leave = actdate.AddDays(-1);
                    file1.Return_leave = leave.actual_return_date;
                    file1.leave_type = leave.leave_type;
                    file1.actual_return_date = leave.actual_return_date;
                    file1.half = leave.half;
                    if (leave.actual_return_date == null)
                    {
                        masterstatus.status = "on leave";
                    }
                    else masterstatus.status = "active";

                    file1.actualchangedby = User.Identity.Name;
                    file1.actualchangeddateby = DateTime.Now;
                    var attachedEntity = db.Leaves.Find(file1.Id);
                    if (attachedEntity != null && db.Entry(attachedEntity).State != EntityState.Detached)
                    {
                        db.Entry(attachedEntity).State = EntityState.Detached;
                    }

                    file1.master_file = masterstatus;
                    this.db.Entry(file1).State = EntityState.Modified;
                    this.db.SaveChanges();
                }
                else if (leave.actual_return_date == leave.Return_leave || leave.actual_return_date == null)
                {
                    file1 = leave;
                    file1.Date = leave.Date;
                    file1.Reference = serverfile;
                    file1.Start_leave = leave.Start_leave;
                    file1.End_leave = leave.End_leave;
                    file1.Return_leave = leave.Return_leave;
                    file1.leave_type = leave.leave_type;
                    file1.actual_return_date = leave.actual_return_date;
                    file1.half = leave.half;
                    if (leave.actual_return_date == null) masterstatus.status = "on leave";
                    else
                    {
                        masterstatus.status = "active";
                    }

                    file1.actualchangedby = User.Identity.Name;
                    file1.actualchangeddateby = DateTime.Now;
                    var attachedEntity = db.Leaves.Find(file1.Id);
                    if (attachedEntity != null && db.Entry(attachedEntity).State != EntityState.Detached)
                    {
                        db.Entry(attachedEntity).State = EntityState.Detached;
                    }

                    file1.master_file = masterstatus;
                    this.db.Entry(file1).State = EntityState.Modified;
                    this.db.SaveChanges();
                }
                else if (leave.actual_return_date > leave.Return_leave)
                {
                    actdate = leave.actual_return_date.Value;
                    file1 = leave;
                    file1.actualchangedby = "system";
                    file1.actualchangeddateby = DateTime.Now;
                    var todate = leave.End_leave;
                    file1.Return_leave = todate.Value.AddDays(1);
                    file1.actual_return_date = todate.Value.AddDays(1);
                    var attachedEntity = db.Leaves.Find(file1.Id);
                    if (attachedEntity != null && db.Entry(attachedEntity).State != EntityState.Detached)
                    {
                        db.Entry(attachedEntity).State = EntityState.Detached;
                    }

                    file1.master_file = masterstatus;
                    this.db.Entry(file1).State = EntityState.Modified;
                    this.db.SaveChanges();
                    var unpaidauto = new Leave();
                    unpaidauto = file1;
                    unpaidauto.leave_type = "6";
                    unpaidauto.Start_leave = todate.Value.AddDays(1);
                    var actdate_1 = actdate.AddDays(-1);
                    unpaidauto.End_leave = actdate_1;
                    unpaidauto.Return_leave = actdate;
                    unpaidauto.actual_return_date = actdate;
                    file1.half = leave.half;
                    masterstatus.status = "active";
                    unpaidauto.actualchangedby = User.Identity.Name;
                    unpaidauto.actualchangeddateby = DateTime.Now;
                    this.db.Leaves.Add(unpaidauto);
                    this.db.SaveChanges();
                }

                return this.RedirectToAction("getallorone", new {search = search, page = page});
            }

            jderr: ;
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_no");
            var leavemasterfile = afinallist.Find(x => x.employee_id == leave.Employee_id);
            leave.master_file = leavemasterfile;
            return this.View(leave);
        }

        // GET: Leaves
        public ActionResult Index()
        {
            var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).ToList();
            return this.View(leaves);
        }

        public ActionResult getallorone(string search, int? page, int? pagesize, int? idsearch)
        {
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize = 100;
            List<Leave> leaves = new List<Leave>();
            leaves = this.db.Leaves.Include(l => l.master_file).OrderBy(x => x.master_file.employee_no)
                .ThenByDescending(x => x.Start_leave)
                .ThenByDescending(x => x.master_file.employee_no).ToList();
            if (pagesize != 0)
            {
                int a = 100;
                if (pagesize != null)
                {
                    if (pagesize != 0)
                    {
                        a = (int) pagesize;
                    }
                }

                defaSize = a;
            }

            ViewBag.search = search;
            ViewBag.page = page;
            ViewBag.pagesize = defaSize;
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out var idk))
                {
                    leaves = db.Leaves
                        .Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenByDescending(x => x.Start_leave).ToList();
                }
                else
                {
                    leaves = db.Leaves
                        .Where(
                            x => x.master_file.employee_name
                                .Contains(search) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenByDescending(x => x.Start_leave).ToList();
                }
            }

            return this.View(leaves.OrderBy(x => x.master_file.employee_no).ThenByDescending(x => x.End_leave)
                .ToPagedList(page ?? 1, defaSize));
        }

        public ActionResult getallonleave(string search, int? page, int? pagesize, int? idsearch)
        {
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize = 100;
            List<Leave> leaves = new List<Leave>();
            leaves = this.db.Leaves.Where(x => x.actual_return_date == null).Include(l => l.master_file)
                .OrderBy(x => x.master_file.employee_no)
                .ThenByDescending(x => x.Start_leave)
                .ThenByDescending(x => x.master_file.employee_no).ToList();
            if (pagesize != 0)
            {
                int a = 100;
                if (pagesize != null)
                {
                    if (pagesize != 0)
                    {
                        a = (int) pagesize;
                    }
                }

                defaSize = a;
            }

            ViewBag.search = search;
            ViewBag.pagesize = defaSize;
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out var idk))
                {
                    leaves = db.Leaves
                        .Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenByDescending(x => x.Start_leave).ToList();
                }
                else
                {
                    leaves = db.Leaves
                        .Where(
                            x => x.master_file.employee_name
                                .Contains(search) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenByDescending(x => x.Start_leave).ToList();
                }
            }

            return this.View(leaves.OrderBy(x => x.master_file.employee_no).ThenByDescending(x => x.End_leave)
                .ToPagedList(page ?? 1, defaSize));
        }

        public ActionResult leave_absence_Index(DateTime? eddate)
        {
            ViewBag.eddate = eddate;
            var leaves = new List<leave_absence>();
            if (eddate != null)
            {
                leaves = this.db.leave_absence
                    .Where(x => x.month.Value.Month == eddate.Value.Month && x.month.Value.Year == eddate.Value.Year)
                    .OrderByDescending(x => x.Id).ToList();
            }

            return this.View(leaves.OrderBy(x => x.master_file.employee_no));
        }

        public string humanize(double days)
        {
            // The string we're working with to create the representation
            string str = "";
            // Map lengths of `diff` to different time periods
            var values = new List<Tuple<string, double>>();
            values.Add(new Tuple<string, double>("year", 365));
            values.Add(new Tuple<string, double>("month", 30));
            values.Add(new Tuple<string, double>("day", 1));
            // Iterate over the values...
            for (var i = 0; i < values.Count; i++)
            {
                var amount = Math.Floor(days / values[i].Item2);
                // ... and find the largest time value that fits into the diff
                if (amount >= 1)
                {
                    // If we match, add to the string ('s' is for pluralization)
                    str += amount + " " + values[i].Item1 + (amount > 1 ? "s" : " ") + " ";

                    // and subtract from the diff
                    days -= amount * values[i].Item2;
                }
            }

            if (string.IsNullOrEmpty(str))
            {
                str = "0 days";
            }

            return str;
        }

        public ActionResult report(long? Employee_id, DateTime? eddate)
        {
            eddate = new DateTime(DateTime.Now.Year, 12, DateTime.DaysInMonth(DateTime.Now.Year, 12));
            ViewBag.Employee_id = Employee_id;
            ViewBag.eddate = eddate;
            var emp_listfinal = new List<master_file>();
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            emp_listfinal = afinallist;
            var empjd = afinallist.Find(x => x.employee_id == Employee_id);
            this.ViewBag.employee_id = new SelectList(
                afinallist,
                "employee_id",
                "employee_no");

            if (Employee_id != null)
            {
                var leavecal2020list = db.leavecal2020.ToList();
                var leavebal2020 = new leavecal2020();
                forfitedbalence(empjd.employee_id);
                leavebal2020 = leavecal2020list.Find(x =>
                    x.Employee_id == empjd.employee_id && x.dateupdated == DateTime.Today);

                var asf = empjd.date_joined;
                var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                    x => x.Employee_id == Employee_id && x.Start_leave >= asf).ToList();
                var ump = leaves.ToList();
                var rdate = new DateTime();
                var times = new TimeSpan?();
                double sick = 0;
                double comp = 0;
                double mate = 0;
                double haj = 0;
                double udd = 0;
                double esco = 0;
                double pater = 0;
                double availed = 0;
                var favailed = 0d;
                foreach (var leaf in ump)
                {
                    if (leaf.Reference == null)
                    {
                        leaf.Reference = DateTime.Now.ToString("F");
                    }

                    rdate = Convert.ToDateTime(leaf.Reference);

                    if (leaf.leave_type == "1")
                    {
                        if (leaf.half)
                        {
                            if (DateTime.Today > leaf.Start_leave)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) availed += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) favailed += times.Value.TotalDays + 1 - 0.5;
                            }
                        }
                        else
                        {
                            if (DateTime.Today > leaf.Start_leave)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) availed += times.Value.TotalDays + 1;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) favailed += times.Value.TotalDays + 1;
                            }
                        }
                    }

                    if (leaf.leave_type == "2" || leaf.leave_type == "7")
                    {
                        if (leaf.half)
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) sick += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) sick += times.Value.TotalDays + 1;
                        }
                    }

                    if (leaf.leave_type == "3")
                    {
                        if (leaf.half)
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) comp += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) comp += times.Value.TotalDays + 1;
                        }
                    }

                    if (leaf.leave_type == "4")
                    {
                        if (leaf.half)
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) mate += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) mate += times.Value.TotalDays + 1;
                        }
                    }

                    if (leaf.leave_type == "5")
                    {
                        if (leaf.half)
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) haj += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) haj += times.Value.TotalDays + 1;
                        }
                    }

                    if (leaf.leave_type == "8")
                    {
                        if (leaf.half)
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) udd += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) udd += times.Value.TotalDays + 1;
                        }
                    }

                    if (leaf.leave_type == "9")
                    {
                        if (leaf.half)
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) esco += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) esco += times.Value.TotalDays + 1;
                        }
                    }

                    if (leaf.leave_type == "10")
                    {
                        if (leaf.half)
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) pater += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            times = leaf.End_leave - leaf.Start_leave;
                            if (times != null) pater += times.Value.TotalDays + 1;
                        }
                    }
                }

                this.ViewBag.udd = udd;
                this.ViewBag.esco = esco;
                this.ViewBag.pater = pater;
                this.ViewBag.mate = mate;
                this.ViewBag.haj = haj;
                this.ViewBag.sick = sick;
                this.ViewBag.comp = comp;
                this.ViewBag.lbal = leavebal2020.leave_balance;
                this.ViewBag.per = leavebal2020.periodtill2020 + leavebal2020.periodafter2020;
                this.ViewBag.aval = availed;
                this.ViewBag.faval = favailed;
                this.ViewBag.taval = availed + favailed;
                this.ViewBag.netp = leavebal2020.net_periodtill2020 + leavebal2020.net_periodafter2020;
                this.ViewBag.ump = leavebal2020.unpaid_leavetill2020 + leavebal2020.unpaid_leaveafter2020;
                this.ViewBag.accr = leavebal2020.accruedafter2020 + leavebal2020.accruedtill2020;
                this.ViewBag.name = empjd.employee_name;
                this.ViewBag.no = empjd.employee_no;
                this.ViewBag.forfited = leavebal2020.forfitedafter2020 + leavebal2020.forfitedtill2020;
                return this.View(leaves.OrderByDescending(x => x.Start_leave).ToList());
            }

            var leave = new List<Leave>();
            return this.View(leave);
        }

        public void forfitedbalence(long Employee_id)
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            var empjd = afinallist.Find(x => x.employee_id == Employee_id);
            if (empjd == null)
            {
                goto endfun;
            }
            var asf = empjd.date_joined;
            if (asf == null)
            {
                goto endfun;
            }
            var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                x => x.Employee_id == Employee_id && x.Start_leave >= asf).ToList();
            var anualleavetakentill2020 = 0d;
            var unpaidtill2020 = 0d;
            var periodtill2020 = (new DateTime(2020, 12, 31) - asf).Value.TotalDays + 1;
            foreach (var leaf in leaves)
            {
                var times = new TimeSpan?();
                if (leaf.leave_type == "1")
                {
                    times = leaf.End_leave - leaf.Start_leave;
                    if (leaf.half)
                    {
                        if (leaf.Date <= new DateTime(2021, 3, 31))
                        {
                            if (times != null) anualleavetakentill2020 += times.Value.TotalDays + 1 - 0.5;
                        }
                    }
                    else
                    {
                        if (leaf.Date <= new DateTime(2021, 3, 31))
                        {
                            if (times != null) anualleavetakentill2020 += times.Value.TotalDays + 1;
                        }
                    }
                }

                if (leaf.leave_type == "6")
                {
                    if (leaf.half)
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (leaf.Date <= new DateTime(2021, 3, 31))
                        {
                            if (times != null) unpaidtill2020 += times.Value.TotalDays + 1 - 0.5;
                        }
                    }
                    else
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (leaf.Date <= new DateTime(2021, 3, 31))
                        {
                            if (times != null) unpaidtill2020 += times.Value.TotalDays + 1;
                        }
                    }
                }
            }

            var netperiodtill2020 = periodtill2020 - unpaidtill2020;
            var actill2020 = Math.Round(netperiodtill2020 * 30 / 360);
            var per2020lb = new leavecal2020();
            var lb2020 = actill2020 - anualleavetakentill2020;
            if (lb2020 > 0)
            {
                per2020lb.forfitedtill2020 = lb2020;
                per2020lb.leave_balance = 0;
                per2020lb.accruedtill2020 = actill2020;
                per2020lb.anual_leave_takentill2020 = anualleavetakentill2020;
                per2020lb.unpaid_leavetill2020 = unpaidtill2020;
                per2020lb.net_periodtill2020 = netperiodtill2020;
                per2020lb.periodtill2020 = periodtill2020;
                per2020lb.Employee_id = empjd.employee_id;
                per2020lb.periodafter2020 = 0d;
                per2020lb.anual_leave_takenafter2020 = 0d;
                per2020lb.unpaid_leaveafter2020 = 0d;
                per2020lb.net_periodafter2020 = 0d;
                per2020lb.accruedafter2020 = 0d;
                per2020lb.forfitedafter2020 = 0d;
            }
            else
            {
                per2020lb.leave_balance = lb2020;
                per2020lb.forfitedtill2020 = 0;
                per2020lb.accruedtill2020 = actill2020;
                per2020lb.anual_leave_takentill2020 = anualleavetakentill2020;
                per2020lb.unpaid_leavetill2020 = unpaidtill2020;
                per2020lb.net_periodtill2020 = netperiodtill2020;
                per2020lb.periodtill2020 = periodtill2020;
                per2020lb.Employee_id = empjd.employee_id;
                per2020lb.periodafter2020 = 0d;
                per2020lb.anual_leave_takenafter2020 = 0d;
                per2020lb.unpaid_leaveafter2020 = 0d;
                per2020lb.net_periodafter2020 = 0d;
                per2020lb.accruedafter2020 = 0d;
                per2020lb.forfitedafter2020 = 0d;
            }

            var yearsinperiod =
                Math.Round(((new DateTime(DateTime.Today.Year, 12, 31) - new DateTime(2021, 1, 1)).TotalDays + 1) /
                           365);
            for (int i = 0; i < yearsinperiod; i++)
            {
                var anualleavetakenperyear = 0d;
                var unpaidperyear = 0d;
                var iterationvar = leaves.FindAll(x =>
                    x.Date > new DateTime(2021 + i, 3, 31) && x.Date <= new DateTime(2022 + i, 3, 31));
                foreach (var leaf in iterationvar)
                {
                    var times = new TimeSpan?();
                    if (leaf.leave_type == "1")
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (leaf.half)
                        {
                            if (times != null) anualleavetakenperyear += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            if (times != null) anualleavetakenperyear += times.Value.TotalDays + 1;
                        }
                    }

                    if (leaf.leave_type == "6")
                    {
                        times = leaf.End_leave - leaf.Start_leave;
                        if (leaf.half)
                        {
                            if (times != null) unpaidperyear += times.Value.TotalDays + 1 - 0.5;
                        }
                        else
                        {
                            if (times != null) unpaidperyear += times.Value.TotalDays + 1;
                        }
                    }
                }

                var actillperyear = Math.Round(30 - (unpaidperyear * 30 / 360));
                var lbperyear = actillperyear - anualleavetakenperyear;

                per2020lb.anual_leave_takenafter2020 += anualleavetakenperyear;
                per2020lb.unpaid_leaveafter2020 += unpaidperyear;
                per2020lb.net_periodafter2020 += Math.Round(new DateTime(2021 + i, 12, 31).DayOfYear - (unpaidperyear * 30 / 360));
                per2020lb.accruedafter2020 += actillperyear;
                if (DateTime.Today > new DateTime(2022 + i, 3, 31))
                {
                    if (per2020lb.leave_balance <= 0)
                    {
                        if((lbperyear + per2020lb.leave_balance) < 0)
                        {
                            per2020lb.forfitedafter2020 += 0;
                            per2020lb.leave_balance = lbperyear + per2020lb.leave_balance;
                        }
                        else
                        {
                            per2020lb.forfitedafter2020 += lbperyear + per2020lb.leave_balance;
                            per2020lb.leave_balance = 0;
                        }
                    }
                    else
                    {
                        per2020lb.forfitedafter2020 += lbperyear;
                        per2020lb.leave_balance = 0;
                    }
                }
                else
                {
                    per2020lb.leave_balance += lbperyear;
                }
            }

            var submitedleave = db.employeeleavesubmitions.ToList().FindAll(x => x.Employee_id == empjd.employee_id && x.apstatus == "submitted");
            var Unsubmitedlb = 0d;
            var Ansubmitedlb = 0d;
            foreach (var leaf in submitedleave)
            {
                var times = new TimeSpan?();
                if (leaf.leave_type == "1")
                {
                    times = leaf.End_leave - leaf.Start_leave;
                    if (leaf.half)
                    {
                        if (times != null) Ansubmitedlb += times.Value.TotalDays + 1 - 0.5;
                    }
                    else
                    {
                        if (times != null) Ansubmitedlb += times.Value.TotalDays + 1;
                    }
                }
                if (leaf.leave_type == "6")
                {
                    times = leaf.End_leave - leaf.Start_leave;
                    if (leaf.half)
                    {
                        if (times != null) Unsubmitedlb += times.Value.TotalDays + 1 - 0.5;
                    }
                    else
                    {
                        if (times != null) Unsubmitedlb += times.Value.TotalDays + 1;
                    }
                }
            }
            var leavecal202list = db.leavecal2020.ToList();
            if (leavecal202list.Exists(x => x.Employee_id == empjd.employee_id))
            {
                var leavecalsave = leavecal202list.Find(x => x.Employee_id == empjd.employee_id);
                leavecalsave.anual_leave_takenafter2020 = per2020lb.anual_leave_takenafter2020;
                leavecalsave.unpaid_leaveafter2020 = per2020lb.unpaid_leaveafter2020;
                leavecalsave.net_periodafter2020 = per2020lb.net_periodafter2020;
                leavecalsave.accruedafter2020 = per2020lb.accruedafter2020;
                leavecalsave.periodafter2020 = /*per2020lb.net_periodafter2020 + (per2020lb.unpaid_leaveafter2020 * 30 / 360)*/ 365 * yearsinperiod;
                leavecalsave.dateupdated = DateTime.Today;
                leavecalsave.forfitedafter2020 = per2020lb.forfitedafter2020;
                leavecalsave.periodtill2020 = per2020lb.periodtill2020;
                leavecalsave.anual_leave_takentill2020 = per2020lb.anual_leave_takentill2020;
                leavecalsave.unpaid_leavetill2020 = per2020lb.unpaid_leavetill2020;
                leavecalsave.net_periodtill2020 = per2020lb.net_periodtill2020;
                leavecalsave.accruedtill2020 = per2020lb.accruedtill2020;
                leavecalsave.forfitedtill2020 = per2020lb.forfitedtill2020;
                leavecalsave.leave_balance = per2020lb.leave_balance;
                leavecalsave.ifslbal = per2020lb.leave_balance - Ansubmitedlb - (Unsubmitedlb * 30 / 360);
                this.db.Entry(leavecalsave).State = EntityState.Modified;
                this.db.SaveChanges();
            }
            else
            {
                var leavecalsave = new leavecal2020();
                leavecalsave.Employee_id = per2020lb.Employee_id;
                leavecalsave.leave_balance = per2020lb.leave_balance;
                leavecalsave.leave_balance = per2020lb.leave_balance;
                leavecalsave.ifslbal = per2020lb.leave_balance - Ansubmitedlb - (Unsubmitedlb * 30 / 360);
                leavecalsave.periodtill2020 = per2020lb.periodtill2020;
                leavecalsave.anual_leave_takentill2020 = per2020lb.anual_leave_takentill2020;
                leavecalsave.unpaid_leavetill2020 = per2020lb.unpaid_leavetill2020;
                leavecalsave.net_periodtill2020 = per2020lb.net_periodtill2020;
                leavecalsave.accruedtill2020 = per2020lb.accruedtill2020;
                leavecalsave.forfitedtill2020 = per2020lb.forfitedtill2020;
                leavecalsave.periodafter2020 = /*per2020lb.net_periodafter2020 - per2020lb.unpaid_leaveafter2020*/365 * yearsinperiod;
                leavecalsave.anual_leave_takenafter2020 = per2020lb.anual_leave_takenafter2020;
                leavecalsave.unpaid_leaveafter2020 = per2020lb.unpaid_leaveafter2020;
                leavecalsave.net_periodafter2020 = per2020lb.net_periodafter2020;
                leavecalsave.accruedafter2020 = per2020lb.accruedafter2020;
                leavecalsave.forfitedafter2020 = per2020lb.forfitedafter2020;
                leavecalsave.dateupdated = DateTime.Today;
                this.db.leavecal2020.Add(leavecalsave);
                this.db.SaveChanges();
            }
            endfun: ;
        }

        public ActionResult reportserch(int? days)
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0)
                {
                    afinallist.Add(file);
                }

                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    if (file.status != "inactive")
                    {
                        afinallist.Add(file);
                    }
                }
            }

            ViewBag.days = days;
            var leaves = new List<Leave>();
            
            if (days.HasValue)
            {
                foreach (var file in afinallist)
                {
                    this.forfitedbalence(file.employee_id);
                }
                var leaveballist = this.db.leavecal2020.Where(x => x.leave_balance >= days.Value).ToList();
                foreach (var leavecal in leaveballist)
                {
                    var leaveempid = this.db.Leaves.Where(x => x.Employee_id == leavecal.Employee_id)
                        .Include(l => l.master_file).OrderByDescending(x => x.leave_bal).ToList();
                    foreach (var leaf in leaveempid)
                    {
                        leaf.leave_bal = leavecal.leave_balance;
                        if (!leaves.Exists(x => x.Employee_id == leaf.Employee_id)) leaves.Add(leaf);
                    }
                }


                foreach (var emp in afinallist)
                {
                    if (emp.date_joined != null && emp.status != "inactive")
                    {
                        var leaf = new Leave();
                        if (leaveballist.Exists(x => x.Employee_id == emp.employee_id))
                        {
                            var lb = leaveballist.Find(x => x.Employee_id == emp.employee_id).leave_balance;
                            leaf.leave_bal = lb;
                            leaf.Employee_id = emp.employee_id;
                            leaf.master_file = emp;
                            if (!leaves.Exists(x => x.Employee_id == leaf.Employee_id)) leaves.Add(leaf);
                        }
                    }
                }

                var leavesnew = new List<Leave>();
                foreach (var leaf in leaves)
                {
                    if (leaf.leave_bal < days.Value)
                    {
                        continue;
                    }
                    else
                    {
                        leavesnew.Add(leaf);
                    }
                }

                return this.View(leavesnew.OrderBy(x => x.leave_bal));
            }
            
            return this.View(leaves);
        }

        HREntities hrdb = new HREntities();

        public ActionResult ImportExcel()
        {
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

                string[] validFileTypes = {".csv"};

                var path1 = string.Format(
                    "{0}/{1}",
                    this.Server.MapPath("~/Content/Uploads"),
                    this.Request.Files["FileUpload1"].FileName);
                if (!Directory.Exists(path1)) Directory.CreateDirectory(this.Server.MapPath("~/Content/Uploads"));

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
                            var leavecheck = this.db.leave_absence.ToList();
                            var pro = new leave_absence();
                            foreach (DataRow dr in dt.Rows)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (dr[column] == null || dr[column].ToString() == " ") goto e;

                                    if (column.ColumnName == "Month")
                                    {
                                        DateTime.TryParse(dr[column].ToString(), out var dtt);
                                        pro.month = dtt;
                                    }

                                    if (column.ColumnName == "from")
                                    {
                                        DateTime.TryParse(dr[column].ToString(), out var dtt);
                                        pro.fromd = dtt;
                                    }

                                    if (column.ColumnName == "to")
                                    {
                                        DateTime.TryParse(dr[column].ToString(), out var dtt);
                                        pro.tod = dtt;
                                    }

                                    if (column.ColumnName == "Absents")
                                    {
                                        float.TryParse(dr[column].ToString(), out var dtt);
                                        pro.absence = dtt;
                                    }

                                    if (column.ColumnName == "EMPNO")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var dbmf = this.hrdb.master_file.ToList();
                                            var epid = dbmf.Find(x => x.employee_no == idm);
                                            if (epid == null) goto e;
                                            pro.Employee_id = epid.employee_id;
                                        }
                                    }
                                }

                                if (leavecheck.Exists(x =>
                                    x.Employee_id == pro.Employee_id && x.month.Value.Year == pro.month.Value.Year &&
                                    x.month.Value.Month == pro.month.Value.Month))
                                {
                                    var leaveab = leavecheck.Find(
                                        x => x.Employee_id == pro.Employee_id
                                             && x.month.Value.Year == pro.month.Value.Year
                                             && x.month.Value.Month == pro.month.Value.Month);
                                    leaveab.absence = pro.absence;
                                    this.db.Entry(leaveab).State = EntityState.Modified;
                                    this.db.SaveChanges();
                                }
                                else
                                {
                                    this.hrdb.leave_absence.Add(pro);
                                    this.hrdb.SaveChanges();
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

        public ActionResult mrv(DateTime? eddate, DateTime? eddate1, int? leave_type)
        {
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "Annual", Value = "1"},
                new ListItem {Text = "Sick(non industrial)", Value = "2"},
                new ListItem {Text = "Compassionate", Value = "3"},
                new ListItem {Text = "Maternity", Value = "4"},
                new ListItem {Text = "Haj", Value = "5"},
                new ListItem {Text = "Unpaid", Value = "6"},
                new ListItem {Text = "Sick(industrial)", Value = "7"},
                new ListItem {Text = "UDDAH", Value = "8"},
                new ListItem {Text = "ESCORT", Value = "9"},
                new ListItem {Text = "PATERNITY ", Value = "10"}
            };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            ViewBag.leaveval = leave_type;
            var con_leave = from l in this.db.Leaves
                join con in this.db.contracts on l.master_file.employee_no equals con.master_file.employee_no
                select new
                {
                    l.master_file.employee_no,
                    l.master_file.employee_name,
                    con.designation,
                    con.departmant_project,
                    l.Start_leave,
                    l.End_leave,
                    l.actual_return_date,
                    l.leave_type,
                    l.half
                };
            var leavelist = this.db.Leaves.ToList();
            var listconleave = con_leave.OrderBy(x => x.departmant_project).ToList();
            var cllist = new List<con_leavemodel>();
            int i = 0;
            foreach (var cl in listconleave)
            {
                var clitem = new con_leavemodel();
                clitem.id = ++i;
                clitem.employee_no = cl.employee_no;
                clitem.employee_name = cl.employee_name;
                clitem.designation = cl.designation;
                clitem.departmant_project = cl.departmant_project;
                clitem.Start_leave = cl.Start_leave;
                clitem.End_leave = cl.End_leave;
                clitem.leave_type = cl.leave_type;
                clitem.half = cl.half;
                clitem.Areturn_leave = cl.actual_return_date;
                cllist.Add(clitem);
            }

            if (eddate.HasValue && !eddate1.HasValue)
            {
                return this.View(cllist
                    .Where(x => (x.Start_leave >= eddate || x.Areturn_leave == null) &&
                                x.leave_type == leave_type.ToString())
                    .OrderBy(x => x.departmant_project).ThenBy(x => x.employee_no));
            }

            if (eddate.HasValue && eddate1.HasValue)
            {
                // return this.View(cllist
                //     .Where(x => ((x.Start_leave >= eddate && x.Start_leave <= eddate1) ||
                //                  (x.Start_leave >= eddate && x.Start_leave <= eddate1 && x.Areturn_leave == null) ||
                //                  (x.Areturn_leave >= eddate && x.Areturn_leave <= eddate1)) &&
                //                 x.leave_type == leave_type.ToString())
                //     .OrderBy(x => x.departmant_project).ThenBy(x => x.employee_no));
                return this.View(cllist
                    .Where(x => ((eddate >= x.Start_leave && eddate1 <= x.Start_leave ) || (eddate <= x.End_leave && eddate1 >= x.End_leave)) &&
                                x.leave_type == leave_type.ToString()).OrderBy(x => x.departmant_project).ThenBy(x => x.employee_no));
            }

            return this.View(new List<con_leavemodel>());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}