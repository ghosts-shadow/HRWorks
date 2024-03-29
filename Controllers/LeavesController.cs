using System.Data.Entity.Infrastructure;

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
    using static Humanizer.On;
    using static Microsoft.Isam.Esent.Interop.EnumeratedColumn;
    using System.Collections;

    [Authorize]
    public class LeavesController : Controller
    {
        private readonly HREntities db = new HREntities();


        // GET: Leaves/Create               
        public ActionResult Create()
        {
            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Annual", Value = "1" },
                new ListItem { Text = "Sick(non industrial)", Value = "2" },
                new ListItem { Text = "Compassionate", Value = "3" },
                new ListItem { Text = "Maternity", Value = "4" },
                new ListItem { Text = "Haj", Value = "5" },
                new ListItem { Text = "Unpaid", Value = "6" },
                new ListItem { Text = "Sick(industrial)", Value = "7" },
                new ListItem { Text = "UDDAH", Value = "8" },
                new ListItem { Text = "ESCORT", Value = "9" },
                new ListItem { Text = "PATERNITY ", Value = "10" },
                new ListItem { Text = "SABBATICAL", Value = "11" },
                new ListItem { Text = "STUDY LEAVE ", Value = "12" },
                new ListItem { Text = "Compensatory", Value = "13" },
                new ListItem { Text = "balance rectification", Value = "14" }
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
                new ListItem { Text = "Annual", Value = "1" },
                new ListItem { Text = "Sick(non industrial)", Value = "2" },
                new ListItem { Text = "Compassionate", Value = "3" },
                new ListItem { Text = "Maternity", Value = "4" },
                new ListItem { Text = "Haj", Value = "5" },
                new ListItem { Text = "Unpaid", Value = "6" },
                new ListItem { Text = "Sick(industrial)", Value = "7" },
                new ListItem { Text = "UDDAH", Value = "8" },
                new ListItem { Text = "ESCORT", Value = "9" },
                new ListItem { Text = "PATERNITY ", Value = "10" },
                new ListItem { Text = "SABBATICAL", Value = "11" },
                new ListItem { Text = "STUDY LEAVE ", Value = "12" },
                new ListItem { Text = "Compensatory", Value = "13" },
                new ListItem { Text = "balance rectification", Value = "14" }
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


            if (leave.leave_type == "5")
            {
                var datediff = (leave.End_leave - leave.Start_leave).Value.TotalDays + 1;
                if (datediff > 10)
                {
                    ModelState.AddModelError("leave_type", "maximum days allowed for haj are 10 ");
                    goto jderr;
                }

                if (leavelistc.Exists(x => x.leave_type == "5" && x.Employee_id == leave.Employee_id))
                {
                    ModelState.AddModelError("leave_type", "already taken once");
                    goto jderr;
                }
            }

            var leavelist = this.db.Leaves.ToList();
            if (leavelist.Exists(
                    x => x.Employee_id == leave.Employee_id && x.Start_leave == leave.Start_leave
                                                            && x.End_leave == leave.End_leave))
            {
                ViewBag.exhist = "the entry already exists";
                return this.View(leave);
            }

            if (fileBase != null)
            {
                var i = 0;
                var imgname = Path.GetFileName(fileBase.FileName);
                var fileexe = Path.GetExtension(fileBase.FileName);
                var filepath = new DirectoryInfo("D:/HR/leave/");
                serverfile =
                    "D:/HR/leave/" + leave.master_file.employee_no; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/leave/" + leave.master_file.employee_no + "/" +
                                 leave.master_file.employee_no + "_(" + i + ")_" + DateTime.Now.ToString("dd-MM-YY") +
                                 fileexe;
                    i++;
                } while (System.IO.File.Exists(
                             serverfile = "D:/HR/leave/" + leave.master_file.employee_no + "/" +
                                          leave.master_file.employee_no + "_(" + i + ")_" +
                                          DateTime.Now.ToString("dd-MM-YY") +
                                          fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
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
            return this.RedirectToAction("getallorone", new { search = search, page = page });
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
            var duplist = new List<master_file>();
            foreach (var file in alist)
            {
                var temp = file.employee_no;
                var temp2 = file.last_working_day;
                var temp3 = file.status;
                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    if (file.status != "inactive" && !file.last_working_day.HasValue)
                    {
                        if (!duplist.Exists(x => x.employee_no == file.employee_no))
                        {
                            afinallist.Add(file);
                        }
                    }
                    else
                    {
                        duplist.Add(file);
                    }
                }
            }


            if (days.HasValue)
            {
                foreach (var file in afinallist)
                {
                    leavebalcalperyear(file.employee_id);
                    //forfitedbalence(file.employee_id);
                }

                var leaveballist = this.db.leavecalperyears.Where(x => x.leave_balance >= days.Value).ToList();
                //var leaveballist = this.db.leavecal2020.Where(x => x.leave_balance >= days.Value).ToList();
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

                passexel = leavesnew;
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
                double sab = 0;
                double study = 0;
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

                        if (leaf.leave_type == "11")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) sab += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) sab += times.Value.TotalDays + 1;
                            }
                        }

                        if (leaf.leave_type == "12")
                        {
                            if (leaf.half)
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) study += times.Value.TotalDays + 1 - 0.5;
                            }
                            else
                            {
                                times = leaf.End_leave - leaf.Start_leave;
                                if (times != null) study += times.Value.TotalDays + 1;
                            }
                        }
                    }

                    passexel = ump;
                    if (empjd.date_joined != null)
                    {
                        var forfeitedlist = db.leavecalperyears.ToList();
                        //var forfeitedlist = db.leavecal2020.ToList();
                        var forfeited = forfeitedlist.OrderByDescending(x => x.balances_of_year).ToList()
                            .FindAll(x => x.Employee_id == Employee_id);
                        netperiod = forfeited[0].net_period.Value;
                        lb = forfeited[0].leave_balance.Value;
                        period = 0;
                        ac = 0;
                        forfeitedbal = 0;
                        foreach (var leavecalperyear in forfeited)
                        {
                            period += leavecalperyear.period.Value;
                            ac += leavecalperyear.accrued.Value;
                            forfeitedbal += leavecalperyear.forfited_balance.Value;
                        }
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

                if (item.leave_type == "11")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = "Sabbatical";
                }

                if (item.leave_type == "12")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = "study";
                }
                if (item.leave_type == "13")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = "Compensatory";
                }
                if (item.leave_type == "14")
                {
                    Sheet.Cells[string.Format("h{0}", row)].Value = "balance rectificatiom";
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
            Sheet.Cells[string.Format("A{0}", row + 5)].Value = "annual leave applied =" + (favailed);
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
                new ListItem { Text = "Annual", Value = "1" },
                new ListItem { Text = "Sick(non industrial)", Value = "2" },
                new ListItem { Text = "Compassionate", Value = "3" },
                new ListItem { Text = "Maternity", Value = "4" },
                new ListItem { Text = "Haj", Value = "5" },
                new ListItem { Text = "Unpaid", Value = "6" },
                new ListItem { Text = "Sick(industrial)", Value = "7" },
                new ListItem { Text = "UDDAH", Value = "8" },
                new ListItem { Text = "ESCORT", Value = "9" },
                new ListItem { Text = "PATERNITY ", Value = "10" },
                new ListItem { Text = "SABBATICAL", Value = "11" },
                new ListItem { Text = "STUDY LEAVE ", Value = "12" },
                new ListItem { Text = "Compensatory", Value = "13" },
                new ListItem { Text = "balance rectification", Value = "14" }
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
                new ListItem { Text = "Annual", Value = "1" },
                new ListItem { Text = "Sick(non industrial)", Value = "2" },
                new ListItem { Text = "Compassionate", Value = "3" },
                new ListItem { Text = "Maternity", Value = "4" },
                new ListItem { Text = "Haj", Value = "5" },
                new ListItem { Text = "Unpaid", Value = "6" },
                new ListItem { Text = "Sick(industrial)", Value = "7" },
                new ListItem { Text = "UDDAH", Value = "8" },
                new ListItem { Text = "ESCORT", Value = "9" },
                new ListItem { Text = "PATERNITY ", Value = "10" },
                new ListItem { Text = "SABBATICAL", Value = "11" },
                new ListItem { Text = "STUDY LEAVE ", Value = "12" },
                new ListItem { Text = "Compensatory", Value = "13" },
                new ListItem { Text = "balance rectification", Value = "14" }
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
                serverfile =
                    "D:/HR/leave/" + leave.master_file.employee_no; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/leave/" + leave.master_file.employee_no + "/" +
                                 leave.master_file.employee_no + "_(" + i + ")_" + DateTime.Now.ToString("dd-MM-YY") +
                                 fileexe;
                    i++;
                } while (System.IO.File.Exists(
                             serverfile = "D:/HR/leave/" + leave.master_file.employee_no + "/" +
                                          leave.master_file.employee_no + "_(" + i + ")_" +
                                          DateTime.Now.ToString("dd-MM-YY") +
                                          fileexe));

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

                return this.RedirectToAction("getallorone", new { search = search, page = page });
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
                        a = (int)pagesize;
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
            leaves = this.db.Leaves.Where(x => x.actual_return_date == null && x.Start_leave < DateTime.Today)
                .Include(l => l.master_file)
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
                        a = (int)pagesize;
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
                leavebalcalperyear(Employee_id.Value);
                var leavecal2020list = db.leavecalperyears.OrderByDescending(x => x.balances_of_year).ToList();
                //var leavecal2020list = db.leavecal2020.ToList();
                var leavebal2020 = new List<leavecalperyear>();
                //forfitedbalence(empjd.employee_id);
                leavebal2020 = leavecal2020list.FindAll(x => x.Employee_id == empjd.employee_id);
                var asf = empjd.date_joined;
                var leaves = this.db.Leaves.Include(l => l.master_file).OrderBy(x => x.Id)
                    .Where(x => x.Employee_id == Employee_id && x.Start_leave >= asf).ToList();
                //var ump = leaves.ToList();
                //var rdate = new DateTime();
                //var times = new TimeSpan?();
                double sick = 0;
                double comp = 0;
                double mate = 0;
                double haj = 0;
                double udd = 0;
                double esco = 0;
                double pater = 0;
                double sab = 0;
                double study = 0;
                double availed = 0;
                var favailed = 0d;
                var ump1 = 0d;
                var accr = 0d;
                var forfited = 0d;
                var per = 0d;
                foreach (var leavecalperyear in leavebal2020)
                {
                    ump1 += leavecalperyear.unpaid.Value;
                    accr += leavecalperyear.accrued.Value;
                    forfited += leavecalperyear.forfited_balance.Value;
                    per += leavecalperyear.period.Value;
                    sick += leavecalperyear.sick_leave_balance_industrial.Value +
                            leavecalperyear.sick_leave_balance.Value;
                    comp += leavecalperyear.compassionate_leave_balance.Value;
                    mate += leavecalperyear.maternity_leave_balance.Value;
                    haj += leavecalperyear.haj_leave_balance.Value;
                    udd += leavecalperyear.UDDAH_leave_balance.Value;
                    esco += leavecalperyear.escort_leave_balance.Value;
                    pater += leavecalperyear.paternity_leave_balance.Value;
                    sab += leavecalperyear.sabbatical_leave_balance.Value;
                    availed += leavecalperyear.annual_leave_taken.Value;
                    favailed += leavecalperyear.Annual_Leave_Applied.Value;
                }

                {
                    /*foreach (var leaf in ump)
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
                    
                                        if (leaf.leave_type == "11")
                                        {
                                            if (leaf.half)
                                            {
                                                times = leaf.End_leave - leaf.Start_leave;
                                                if (times != null) sab += times.Value.TotalDays + 1 - 0.5;
                                            }
                                            else
                                            {
                                                times = leaf.End_leave - leaf.Start_leave;
                                                if (times != null) sab += times.Value.TotalDays + 1;
                                            }
                                        }
                    
                                        if (leaf.leave_type == "12")
                                        {
                                            if (leaf.half)
                                            {
                                                times = leaf.End_leave - leaf.Start_leave;
                                                if (times != null) study += times.Value.TotalDays + 1 - 0.5;
                                            }
                                            else
                                            {
                                                times = leaf.End_leave - leaf.Start_leave;
                                                if (times != null) study += times.Value.TotalDays + 1;
                                            }
                                        }
                                    }*/
                }
                this.ViewBag.udd = udd;
                this.ViewBag.esco = esco;
                this.ViewBag.pater = pater;
                this.ViewBag.mate = mate;
                this.ViewBag.haj = haj;
                this.ViewBag.sick = sick;
                this.ViewBag.comp = comp;
                this.ViewBag.sab = sab;
                this.ViewBag.study = study;
                this.ViewBag.lbal = leavebal2020[0].leave_balance;
                this.ViewBag.aval = availed;
                this.ViewBag.faval = favailed;
                this.ViewBag.taval = availed + favailed;
                this.ViewBag.netp = leavebal2020[0].net_period;
                this.ViewBag.name = empjd.employee_name;
                this.ViewBag.no = empjd.employee_no;
                ViewBag.datejoined = empjd.date_joined.Value.ToString("dd MMMM yyyy");
                this.ViewBag.ump = ump1;
                this.ViewBag.accr = accr;
                this.ViewBag.forfited = forfited;
                this.ViewBag.per = per;
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
            var periodtill2020 = 0d;
            // if (asf > new DateTime(2021, 1, 31))
            // {
            //     goto skip2020;
            // }
            periodtill2020 = (new DateTime(2020, 12, 31) - asf).Value.TotalDays + 1;
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

            //skip2020: ;
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

            var yearsinperiod = 0d;
            if (DateTime.Today.Month == 12)
            {
                yearsinperiod =
                    Math.Round(
                        ((new DateTime(DateTime.Today.Year + 1, 12, 31) - new DateTime(2021, 1, 1)).TotalDays + 1) /
                        365);
            }
            else
            {
                yearsinperiod =
                    Math.Round(
                        ((new DateTime(DateTime.Today.Year, 12, 31) - new DateTime(2021, 1, 1)).TotalDays + 1) /
                        365);
            }

            for (int i = 0; i < yearsinperiod; i++)
            {
                var anualleavetakenperyear = 0d;
                var unpaidperyear = 0d;
                // if (asf> new DateTime(2021 + i, 3, 31))
                // {
                //     goto skipyear;
                // }
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
                per2020lb.net_periodafter2020 +=
                    Math.Round(new DateTime(2021 + i, 12, 31).DayOfYear - (unpaidperyear * 30 / 360));
                per2020lb.accruedafter2020 += actillperyear;
                if (DateTime.Today > new DateTime(2022 + i, 3, 31))
                {
                    if (per2020lb.leave_balance <= 0)
                    {
                        if ((lbperyear + per2020lb.leave_balance) < 0)
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

                //skipyear: ;
            }

            var submitedleave = db.employeeleavesubmitions.ToList()
                .FindAll(x => x.Employee_id == empjd.employee_id && x.apstatus == "submitted");
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
                leavecalsave.periodafter2020
                    = /*per2020lb.net_periodafter2020 + (per2020lb.unpaid_leaveafter2020 * 30 / 360)*/
                    365 * yearsinperiod;
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
                leavecalsave.periodafter2020 = /*per2020lb.net_periodafter2020 - per2020lb.unpaid_leaveafter2020*/
                    365 * yearsinperiod;
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

        public static double RoundToNearestHalf(double number)
        {
            double nearestHalf = Math.Round(number * 2.0, MidpointRounding.AwayFromZero) / 2.0;
            return nearestHalf;
        }

        public void leavebalcalperyear(long Employee_id)
        {
            const double lbpd30 = (30.0 / 365.0);
            const double lbpd24 = (24.0 / 365.0);
            const double lbpd30f20 = (30.0 / 360.0);
            const double lbpd24f20 = (24.0 / 360.0);
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            var empjd = afinallist.Find(x => x.employee_id == Employee_id);
            var maxleaveperyear = new List<lbperyear>();
            if (db.lbperyears.OrderByDescending(y => y.year).ToList().Exists(x => x.Employee_id == Employee_id))
            {
                maxleaveperyear = db.lbperyears.OrderByDescending(y => y.year).ToList()
                    .FindAll(x => x.Employee_id == Employee_id);
            }

            var saveleaveperyear = new leavecalperyear();
            var savecheckleaveperyear = db.leavecalperyears.ToList();
            double accleave = 0;
            double netperiod = 0;
            if (empjd == null)
            {
                goto endfun;
            }

            var asf = empjd.date_joined;
            if (asf == null)
            {
                goto endfun;
            }

            var leaves = this.db.Leaves.OrderByDescending(x => x.Date).ThenBy(y => y.Start_leave).Where(
                x => x.Employee_id == Employee_id && x.Start_leave >= asf).ToList();
            var submittedleaves = db.employeeleavesubmitions.OrderByDescending(x => x.dateadded)
                .ThenBy(x => x.Start_leave).Where(x =>
                    x.Employee_id == Employee_id && x.Start_leave >= asf.Value && x.apstatus == "submitted").ToList();
            var leaveperyearlist = db.leavecalperyears.Where(x => x.Employee_id == empjd.employee_id)
                .OrderByDescending(x => x.balances_of_year).ToList();
            /*if (leaves.Count > 0)
            {
                var datecheck = leaves.First();
                var datecheckempsub = new employeeleavesubmition();
                if (submittedleaves.Count >0)
                {
                    goto frun;
                }
                
                if (datecheck.actualchangeddateby.HasValue)
                {
                    var leavelastupdated = leaves.First().actualchangeddateby.Value;
                    if (leaveperyearlist.Count > 0)
                    {
                        var leaveperyearlastupdated = leaveperyearlist.First().date_updated;
                        var leaveperyearlastupdatedy = leaveperyearlist.First().balances_of_year;
                        if ((!(leavelastupdated >= leaveperyearlastupdated.Date) && leavelastupdated.Year <= leaveperyearlastupdatedy.Year) ||
                            (leaveperyearlastupdated - DateTime.Today).TotalDays > 5)
                        {
                            if (maxleaveperyear.Count > 0 && maxleaveperyear[0].modified_date > leaveperyearlastupdated)
                            {
                                goto endfun;
                            }

                            if (maxleaveperyear.Count <= 0)
                            {
                                goto endfun;
                            }
                        }
                    }
                }
                else
                {
                    goto endfun;
                }
            }
            frun: ;*/
            var period = 0.0d;
            if (asf.Value.Year <= 2020)
            {
                double anualleavetakentill2020 = 0;
                double nonintsicktill2020 = 0;
                double compastill2020 = 0;
                double matertill2020 = 0;
                double hajtill2020 = 0;
                double unpaidtill2020 = 0;
                double intsicktill2020 = 0;
                double uddahtill2020 = 0;
                double escorttill2020 = 0;
                double patertill2020 = 0;
                double sabtill2020 = 0;
                double studytill2020 = 0;
                var leaveannualandunpaid2020 = leaves.FindAll(x =>
                    x.Date <= new DateTime(2021, 3, 31) && (x.leave_type == "1" || x.leave_type == "6"));
                var leaverest2020 = leaves.FindAll(x =>
                    x.Date <= new DateTime(2020, 12, 31) && !(x.leave_type == "1" || x.leave_type == "6"));
                var leave2020 = new List<Leave>();
                leave2020.AddRange(leaveannualandunpaid2020);
                leave2020.AddRange(leaverest2020);
                foreach (var leaf in leave2020)
                {
                    var anltaken = true;
                    if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                    if (leaf.Start_leave > DateTime.Now)
                    {
                        anltaken = false;
                    }

                    var times = leaf.End_leave - leaf.Start_leave;
                    if (times == null) continue;
                    var days = times.Value.TotalDays + 1;
                    if (leaf.half) days -= 0.5;
                    switch (leaf.leave_type)
                    {
                        case "1":
                            anualleavetakentill2020 += days;
                            break;
                        case "2":
                            nonintsicktill2020 += days;
                            break;
                        case "3":
                            compastill2020 += days;
                            break;
                        case "4":
                            matertill2020 += days;
                            break;
                        case "5":
                            hajtill2020 += days;
                            break;
                        case "6":
                            unpaidtill2020 += days;
                            break;
                        case "7":
                            intsicktill2020 += days;
                            break;
                        case "8":
                            uddahtill2020 += days;
                            break;
                        case "9":
                            escorttill2020 += days;
                            break;
                        case "10":
                            patertill2020 += days;
                            break;
                        case "11":
                            sabtill2020 += days;
                            break;
                        case "12":
                            studytill2020 += days;
                            break;
                    }
                }

                var temptime = new DateTime(2020, 12, 31) - asf.Value;
                period = temptime.TotalDays + 1;
                var temptime1 = /*new DateTime(i, 12, 31)*/ new DateTime(DateTime.Now.Year, 12, 31) - asf.Value;
                var joiningperiod = temptime1.TotalDays + 1;
                if (joiningperiod < 365)
                {
                    accleave = Math.Round((period - unpaidtill2020) * (lbpd24f20));
                }
                else
                {
                    ;
                    accleave = Math.Round((period - unpaidtill2020) * lbpd30f20);
                }

                var leavebal2020 = Math.Round(accleave) - anualleavetakentill2020;
                var savelbpy = new leavecalperyear();
                savelbpy.Employee_id = empjd.employee_id;
                savelbpy.balances_of_year = new DateTime(2020, 1, 1);
                savelbpy.period = period;
                savelbpy.unpaid = unpaidtill2020;
                savelbpy.net_period = period - unpaidtill2020;
                savelbpy.accrued = accleave;
                savelbpy.annual_leave_taken = anualleavetakentill2020;
                savelbpy.Annual_Leave_Applied = 0;
                savelbpy.Annual_Leave_total = anualleavetakentill2020;
                if (leavebal2020 <= 0)
                {
                    savelbpy.leave_balance = leavebal2020;
                    savelbpy.forfited_balance = 0;
                }
                else
                {
                    savelbpy.leave_balance = 0;
                    savelbpy.forfited_balance = leavebal2020;
                }

                savelbpy.sick_leave_balance = nonintsicktill2020;
                savelbpy.sick_leave_balance_industrial = intsicktill2020;
                savelbpy.compassionate_leave_balance = compastill2020;
                savelbpy.maternity_leave_balance = matertill2020;
                savelbpy.haj_leave_balance = hajtill2020;
                savelbpy.UDDAH_leave_balance = uddahtill2020;
                savelbpy.escort_leave_balance = escorttill2020;
                savelbpy.paternity_leave_balance = patertill2020;
                savelbpy.sabbatical_leave_balance = sabtill2020;
                savelbpy.study_leave_balance = sabtill2020;
                savelbpy.date_updated = DateTime.Now;
                savelbpy.leave_count = leave2020.Count;

                if (savecheckleaveperyear.Exists(x =>
                        x.balances_of_year == new DateTime(2020, 1, 1) && x.Employee_id == empjd.employee_id))
                {
                    var rewritelb = savecheckleaveperyear.Find(x =>
                        x.balances_of_year == new DateTime(2020, 1, 1) && x.Employee_id == empjd.employee_id);
                    rewritelb.Employee_id = empjd.employee_id;
                    rewritelb.balances_of_year = savelbpy.balances_of_year;
                    rewritelb.period = savelbpy.period;
                    rewritelb.unpaid = savelbpy.unpaid;
                    rewritelb.net_period = savelbpy.net_period;
                    rewritelb.accrued = savelbpy.accrued;
                    rewritelb.annual_leave_taken = savelbpy.annual_leave_taken;
                    rewritelb.Annual_Leave_Applied = savelbpy.Annual_Leave_Applied;
                    rewritelb.Annual_Leave_total = savelbpy.Annual_Leave_total;
                    rewritelb.leave_balance = savelbpy.leave_balance;
                    rewritelb.forfited_balance = savelbpy.forfited_balance;
                    rewritelb.sick_leave_balance = savelbpy.sick_leave_balance;
                    rewritelb.sick_leave_balance_industrial = savelbpy.sick_leave_balance_industrial;
                    rewritelb.compassionate_leave_balance = savelbpy.compassionate_leave_balance;
                    rewritelb.maternity_leave_balance = savelbpy.maternity_leave_balance;
                    rewritelb.haj_leave_balance = savelbpy.haj_leave_balance;
                    rewritelb.UDDAH_leave_balance = savelbpy.UDDAH_leave_balance;
                    rewritelb.escort_leave_balance = savelbpy.escort_leave_balance;
                    rewritelb.paternity_leave_balance = savelbpy.paternity_leave_balance;
                    rewritelb.sabbatical_leave_balance = savelbpy.sabbatical_leave_balance;
                    rewritelb.study_leave_balance = savelbpy.study_leave_balance;
                    rewritelb.date_updated = DateTime.Now;
                    rewritelb.leave_count = savelbpy.leave_count;
                    this.db.Entry(rewritelb).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        ex.Entries.Single().Reload();
                        db.SaveChanges();
                    }
                }
                else
                {
                    this.db.leavecalperyears.Add(savelbpy);
                    this.db.SaveChanges();
                }
            }

            if (asf.Value.Year <= 2023)
            {
                for (int i = 2021; i < 2024; i++)
                {
                    if (asf.Value.Year <= i)
                    {
                        savecheckleaveperyear = db.leavecalperyears.ToList();
                        var perviousyearleave = savecheckleaveperyear.Find(x =>
                            x.balances_of_year == new DateTime(i - 1, 1, 1) && x.Employee_id == empjd.employee_id);
                        var ifnewleavelist = new List<Leave>();
                        if (perviousyearleave == null)
                        {
                            ifnewleavelist = leaves.FindAll(x =>
                                x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1) &&
                                (x.leave_type == "1" || x.leave_type == "6"));
                        }

                        var leaveannualandunpaidpy = leaves.FindAll(x =>
                            x.Date <= new DateTime(i + 1, 3, 31) && x.Date > new DateTime(i, 3, 31) &&
                            (x.leave_type == "1" || x.leave_type == "6"));
                        var anun2024remove = leaves.FindAll(x =>
                            x.Start_leave <= new DateTime(2025, 3, 31) && x.Start_leave > new DateTime(2024, 3, 31) &&
                            (x.leave_type == "1" || x.leave_type == "6"));
                        foreach (var leaf in anun2024remove)
                        {
                            leaveannualandunpaidpy.Remove(leaf);
                        }
                        var leaverestpy = leaves.FindAll(x =>
                            x.Date <= new DateTime(i, 12, 31) && x.Date >= new DateTime(i, 1, 1) &&
                            !(x.leave_type == "1" || x.leave_type == "6"));
                        var leavepy = new List<Leave>();
                        var savelbpy = new leavecalperyear
                        {
                            balances_of_year = DateTime.Now,
                            period = 0,
                            unpaid = 0,
                            net_period = 0,
                            accrued = 0,
                            annual_leave_taken = 0,
                            Annual_Leave_Applied = 0,
                            Annual_Leave_total = 0,
                            leave_balance = 0,
                            forfited_balance = 0,
                            sick_leave_balance = 0,
                            compassionate_leave_balance = 0,
                            maternity_leave_balance = 0,
                            haj_leave_balance = 0,
                            UDDAH_leave_balance = 0,
                            escort_leave_balance = 0,
                            paternity_leave_balance = 0,
                            sabbatical_leave_balance = 0,
                            study_leave_balance = 0,
                            date_updated = DateTime.Now,
                            sick_leave_balance_industrial = 0,
                            sumittedleavebal = 0
                        };
                        savelbpy.Employee_id = empjd.employee_id;
                        savelbpy.balances_of_year = new DateTime(i, 1, 1);
                        savelbpy.date_updated = DateTime.Now;
                        if (ifnewleavelist.Count != 0)
                        {
                            leavepy.AddRange(ifnewleavelist);
                        }

                        leavepy.AddRange(leaveannualandunpaidpy);
                        leavepy.AddRange(leaverestpy);
                        foreach (var leaf in leavepy)
                        {
                            var anlnottaken = false;
                            if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                            if (leaf.Start_leave > DateTime.Now)
                            {
                                anlnottaken = true;
                            }

                            var times = leaf.End_leave - leaf.Start_leave;
                            if (times == null) continue;
                            var days = times.Value.TotalDays + 1;
                            if (leaf.half) days -= 0.5;
                            switch (leaf.leave_type)
                            {
                                case "1":
                                    if (anlnottaken)
                                    {
                                        savelbpy.Annual_Leave_Applied += days;
                                    }
                                    else
                                    {
                                        savelbpy.annual_leave_taken += days;
                                    }

                                    break;
                                case "2":
                                    savelbpy.sick_leave_balance += days;
                                    break;
                                case "3":
                                    savelbpy.compassionate_leave_balance += days;
                                    break;
                                case "4":
                                    savelbpy.maternity_leave_balance += days;
                                    break;
                                case "5":
                                    savelbpy.haj_leave_balance += days;
                                    break;
                                case "6":
                                    savelbpy.unpaid += days;
                                    break;
                                case "7":
                                    savelbpy.sick_leave_balance_industrial += days;
                                    break;
                                case "8":
                                    savelbpy.UDDAH_leave_balance += days;
                                    break;
                                case "9":
                                    savelbpy.escort_leave_balance += days;
                                    break;
                                case "10":
                                    savelbpy.paternity_leave_balance += days;
                                    break;
                                case "11":
                                    savelbpy.sabbatical_leave_balance += days;
                                    break;
                                case "12":
                                    savelbpy.study_leave_balance += days;
                                    break;
                            }
                        }

                        var temptime = /*new DateTime(i, 12, 31)*/ new DateTime(DateTime.Now.Year, 12, 31) - asf.Value;
                        var joiningperiod = temptime.TotalDays + 1;
                        if (perviousyearleave == null)
                        {
                            var temptime1 = new DateTime(i, 12, 31) - asf.Value;
                            period = temptime1.TotalDays + 1;
                        }
                        else
                        {
                            period = 365;
                        }

                        if (joiningperiod < 365)
                        {
                            if (!savelbpy.unpaid.HasValue)
                            {
                                savelbpy.unpaid = 0;
                            }

                            if (i == 2023)
                            {
                                accleave = RoundToNearestHalf((joiningperiod - savelbpy.unpaid.Value) * (lbpd24f20));
                            }
                            else
                            {
                                accleave = Math.Round((joiningperiod - savelbpy.unpaid.Value) * (lbpd24f20));
                            }
                        }
                        else
                        {
                            if (!savelbpy.unpaid.HasValue)
                            {
                                savelbpy.unpaid = 0;
                            }

                            var temp1 = (savelbpy.unpaid.Value);
                            var temp2 = (period);
                            if (i == 2023)
                            {
                                accleave = RoundToNearestHalf((temp2 - temp1) * lbpd30);
                            }
                            else
                            {
                                accleave = Math.Round((temp2 - temp1) * lbpd30f20);
                            }
                        }

                        if (maxleaveperyear.Count > 0)
                        {
                            if (maxleaveperyear.Exists(x => x.year.Year <= i))
                            {
                                var maxleaveperyeartemp = maxleaveperyear.Find(x => x.year.Year <= i);
                                if (period - savelbpy.unpaid.Value < 365)
                                {
                                    accleave = Math.Round((period - savelbpy.unpaid.Value) *
                                        maxleaveperyeartemp.total_leave_balance / 365);
                                }
                                else
                                {
                                    accleave = maxleaveperyeartemp.total_leave_balance;
                                }
                            }
                        }

                        savelbpy.accrued = accleave;
                        savelbpy.Annual_Leave_total = savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied;
                        savelbpy.period = period;
                        savelbpy.leave_count = leavepy.Count;
                        if (perviousyearleave != null)
                        {
                            if (savelbpy.net_period == null)
                            {
                                savelbpy.net_period = 0;
                            }

                            savelbpy.net_period += perviousyearleave.net_period + period - savelbpy.unpaid;
                            savelbpy.leave_balance =
                                accleave - savelbpy.Annual_Leave_total + perviousyearleave.leave_balance;

                            if (DateTime.Now >= new DateTime(i + 1, 3, 31))
                            {
                                if (savelbpy.leave_balance <= 0)
                                {
                                    savelbpy.leave_balance = savelbpy.leave_balance;
                                    savelbpy.forfited_balance = 0;
                                }
                                else
                                {
                                    savelbpy.forfited_balance = savelbpy.leave_balance;
                                    savelbpy.leave_balance = 0;
                                }
                            }
                        }
                        else
                        {
                            if (savelbpy.net_period == null)
                            {
                                savelbpy.net_period = 0;
                            }

                            savelbpy.leave_balance =
                                accleave - (savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied);

                            if (DateTime.Now >= new DateTime(i + 1, 3, 31))
                            {
                                if (savelbpy.leave_balance <= 0)
                                {
                                    savelbpy.leave_balance = savelbpy.leave_balance;
                                    savelbpy.forfited_balance = 0;
                                }
                                else
                                {
                                    savelbpy.forfited_balance = savelbpy.leave_balance;
                                    savelbpy.leave_balance = 0;
                                }
                            }

                            savelbpy.net_period += period - savelbpy.unpaid;
                        }


                        if (savecheckleaveperyear.Exists(x =>
                                x.balances_of_year == new DateTime(i, 1, 1) && x.Employee_id == empjd.employee_id))
                        {
                            var rewritelb = savecheckleaveperyear.Find(x =>
                                x.balances_of_year == new DateTime(i, 1, 1) && x.Employee_id == empjd.employee_id);
                            rewritelb.Employee_id = empjd.employee_id;
                            rewritelb.balances_of_year = savelbpy.balances_of_year;
                            rewritelb.period = savelbpy.period;
                            rewritelb.unpaid = savelbpy.unpaid;
                            rewritelb.net_period = savelbpy.net_period;
                            rewritelb.accrued = savelbpy.accrued;
                            rewritelb.annual_leave_taken = savelbpy.annual_leave_taken;
                            rewritelb.Annual_Leave_Applied = savelbpy.Annual_Leave_Applied;
                            rewritelb.Annual_Leave_total = savelbpy.Annual_Leave_total;
                            rewritelb.leave_balance = savelbpy.leave_balance;
                            rewritelb.forfited_balance = savelbpy.forfited_balance;
                            rewritelb.sick_leave_balance = savelbpy.sick_leave_balance;
                            rewritelb.sick_leave_balance_industrial = savelbpy.sick_leave_balance_industrial;
                            rewritelb.compassionate_leave_balance = savelbpy.compassionate_leave_balance;
                            rewritelb.maternity_leave_balance = savelbpy.maternity_leave_balance;
                            rewritelb.haj_leave_balance = savelbpy.haj_leave_balance;
                            rewritelb.UDDAH_leave_balance = savelbpy.UDDAH_leave_balance;
                            rewritelb.escort_leave_balance = savelbpy.escort_leave_balance;
                            rewritelb.paternity_leave_balance = savelbpy.paternity_leave_balance;
                            rewritelb.sabbatical_leave_balance = savelbpy.sabbatical_leave_balance;
                            rewritelb.study_leave_balance = savelbpy.study_leave_balance;
                            rewritelb.date_updated = DateTime.Now;
                            rewritelb.leave_count = savelbpy.leave_count;
                            this.db.Entry(rewritelb).State = EntityState.Modified;
                            this.db.SaveChanges();
                        }
                        else
                        {
                            this.db.leavecalperyears.Add(savelbpy);
                            this.db.SaveChanges();
                        }

                        var submitedleave = db.employeeleavesubmitions.ToList()
                            .FindAll(x =>
                                x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                x.Date <= new DateTime(i + 1, 3, 31) && x.Date >= new DateTime(i, 4, 1));

                        var anun2024sub = db.employeeleavesubmitions.ToList()
                            .FindAll(x =>
                                x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                x.Date <= new DateTime(2025, 3, 31) && x.Date >= new DateTime(2024, 4, 1));

                        foreach (var leaf in anun2024sub)
                        {
                            submitedleave.Remove(leaf);
                        }
                        var ifnewsublist = new List<employeeleavesubmition>();
                        if (perviousyearleave == null)
                        {
                            ifnewsublist = db.employeeleavesubmitions.ToList().FindAll(x =>
                                x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1));
                        }

                        var yearrecord = db.leavecalperyears.ToList().Find(x =>
                            x.Employee_id == empjd.employee_id && x.balances_of_year.Year == i);
                        if (ifnewsublist.Count > 0)
                        {
                            submitedleave.AddRange(ifnewsublist);
                        }

                        if (submitedleave.Count == 0)
                        {
                            yearrecord.sumittedleavebal = yearrecord.leave_balance;
                            this.db.Entry(yearrecord).State = EntityState.Modified;
                            this.db.SaveChanges();
                            goto endfun1;
                        }

                        var anualleavesub = 0d;
                        var unpaidsub = 0d;
                        foreach (var leaf in submitedleave)
                        {
                            var anltaken = true;
                            if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                            if (leaf.Start_leave > DateTime.Now)
                            {
                                anltaken = false;
                            }

                            var times = leaf.End_leave - leaf.Start_leave;
                            if (times == null) continue;
                            var days = times.Value.TotalDays + 1.0;
                            if (leaf.half) days -= 0.5;
                            switch (leaf.leave_type)
                            {
                                case "1":
                                    anualleavesub += days;
                                    break;
                                case "6":
                                    unpaidsub += days;
                                    break;
                            }
                        }

                        if (submitedleave.Count > 0)
                        {
                            yearrecord.sumittedleavebal = yearrecord.leave_balance - anualleavesub -
                                                          RoundToNearestHalf(unpaidsub * lbpd30f20);
                        }
                        else
                        {
                            yearrecord.sumittedleavebal = yearrecord.leave_balance;
                        }

                        this.db.Entry(yearrecord).State = EntityState.Modified;
                        this.db.SaveChanges();
                        endfun1: ;
                    }
                }
            }

            if (asf.Value.Year <= 2024)
            {
                var nextyearleave = 0;
                if (DateTime.Now.Month == 12)
                {
                    nextyearleave = 1;
                }
                for (int i = 2024; i <= DateTime.Now.Year + nextyearleave; i++)
                {
                    if (asf.Value.Year <= i)
                    {
                        var leaveannualandunpaidpy = leaves.FindAll(x =>
                            x.Start_leave <= new DateTime(i + 1, 3, 31) && x.Start_leave > new DateTime(i, 3, 31) &&
                            (x.leave_type == "1" || x.leave_type == "6"));
                        var leaverestpy = leaves.FindAll(x =>
                            x.Start_leave <= new DateTime(i, 12, 31) && x.Start_leave >= new DateTime(i, 1, 1) &&
                            !(x.leave_type == "1" || x.leave_type == "6"));
                        var ifnewleavelist = new List<Leave>();
                        var perviousyearleave = savecheckleaveperyear.Find(x =>
                            x.balances_of_year == new DateTime(i - 1, 1, 1) && x.Employee_id == empjd.employee_id);
                        if (perviousyearleave == null)
                        {
                            ifnewleavelist = leaves.FindAll(x =>
                                x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1) &&
                                (x.leave_type == "1" || x.leave_type == "6"));
                        }

                        var leavepy = new List<Leave>();
                        var savelbpy = new leavecalperyear
                        {
                            balances_of_year = DateTime.Now,
                            period = 0,
                            unpaid = 0,
                            net_period = 0,
                            accrued = 0,
                            annual_leave_taken = 0,
                            Annual_Leave_Applied = 0,
                            Annual_Leave_total = 0,
                            leave_balance = 0,
                            forfited_balance = 0,
                            sick_leave_balance = 0,
                            compassionate_leave_balance = 0,
                            maternity_leave_balance = 0,
                            haj_leave_balance = 0,
                            UDDAH_leave_balance = 0,
                            escort_leave_balance = 0,
                            paternity_leave_balance = 0,
                            sabbatical_leave_balance = 0,
                            study_leave_balance = 0,
                            date_updated = DateTime.Now,
                            sick_leave_balance_industrial = 0,
                            sumittedleavebal = 0
                        };
                        savelbpy.Employee_id = empjd.employee_id;
                        savelbpy.balances_of_year = new DateTime(i, 1, 1);
                        savelbpy.date_updated = DateTime.Now;
                        if (ifnewleavelist.Count != 0)
                        {
                            leavepy.AddRange(ifnewleavelist);
                        }

                        leavepy.AddRange(leaveannualandunpaidpy);
                        leavepy.AddRange(leaverestpy);
                        savelbpy.leave_count = leavepy.Count;
                        foreach (var leaf in leavepy)
                        {
                            var anlnottaken = false;
                            if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                            if (leaf.Start_leave > DateTime.Now)
                            {
                                anlnottaken = true;
                            }

                            var times = leaf.End_leave - leaf.Start_leave;
                            if (times == null) continue;
                            var days = times.Value.TotalDays + 1;
                            if (leaf.half) days -= 0.5;
                            switch (leaf.leave_type)
                            {
                                case "1":
                                    if (anlnottaken)
                                    {
                                        savelbpy.Annual_Leave_Applied += days;
                                    }
                                    else
                                    {
                                        savelbpy.annual_leave_taken += days;
                                    }

                                    break;
                                case "2":
                                    savelbpy.sick_leave_balance += days;
                                    break;
                                case "3":
                                    savelbpy.compassionate_leave_balance += days;
                                    break;
                                case "4":
                                    savelbpy.maternity_leave_balance += days;
                                    break;
                                case "5":
                                    savelbpy.haj_leave_balance += days;
                                    break;
                                case "6":
                                    savelbpy.unpaid += days;
                                    break;
                                case "7":
                                    savelbpy.sick_leave_balance_industrial += days;
                                    break;
                                case "8":
                                    savelbpy.UDDAH_leave_balance += days;
                                    break;
                                case "9":
                                    savelbpy.escort_leave_balance += days;
                                    break;
                                case "10":
                                    savelbpy.paternity_leave_balance += days;
                                    break;
                                case "11":
                                    savelbpy.sabbatical_leave_balance += days;
                                    break;
                                case "12":
                                    savelbpy.study_leave_balance += days;
                                    break;
                            }
                        }

                        var temptime = new DateTime(i, 12, 31) - asf.Value;
                        var joiningperiod = temptime.TotalDays + 1;
                        period = 365;
                        if (joiningperiod < 365)
                        {
                            if (!savelbpy.unpaid.HasValue)
                            {
                                savelbpy.unpaid = 0;
                            }

                            accleave = Math.Round((joiningperiod) * lbpd24);
                        }
                        else
                        {
                            if (!savelbpy.unpaid.HasValue)
                            {
                                savelbpy.unpaid = 0;
                            }

                            var temp1 = (savelbpy.unpaid.Value);
                            var temp2 = (period);
                            accleave = Math.Round((temp2) * lbpd30);
                        }

                        if (maxleaveperyear.Count > 0)
                        {
                            if (maxleaveperyear.Exists(x => x.year.Year <= i))
                            {
                                var maxleaveperyeartemp = maxleaveperyear.Find(x => x.year.Year <= i);
                                if (period - savelbpy.unpaid.Value < 365)
                                {
                                    accleave = Math.Round((period - savelbpy.unpaid.Value) *
                                        maxleaveperyeartemp.total_leave_balance / 365);
                                }
                                else
                                {
                                    accleave = maxleaveperyeartemp.total_leave_balance;
                                }
                            }
                        }

                        savelbpy.Annual_Leave_total = savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied;
                        savelbpy.period = 365;
                        savelbpy.accrued = accleave;
                        if (perviousyearleave != null)
                        {
                            if (savelbpy.net_period == null)
                            {
                                savelbpy.net_period = 0;
                            }

                            savelbpy.net_period += perviousyearleave.net_period + savelbpy.period - savelbpy.unpaid;
                            savelbpy.leave_balance = accleave -
                                                     (savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied) +
                                                     perviousyearleave.leave_balance;
                            

                            if (DateTime.Now >= new DateTime(i + 1, 3, 31))
                            {
                                if (savelbpy.leave_balance <= 0)
                                {
                                    savelbpy.leave_balance = savelbpy.leave_balance;
                                    savelbpy.forfited_balance = 0;
                                }
                                else
                                {
                                    savelbpy.forfited_balance = savelbpy.leave_balance;
                                    savelbpy.leave_balance = 0;
                                }
                            }
                        }
                        else
                        {
                            if (savelbpy.net_period == null)
                            {
                                savelbpy.net_period = 0;
                            }

                            savelbpy.leave_balance = Math.Round(accleave) -
                                                     (savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied);
                            if (DateTime.Now >= new DateTime(i + 1, 3, 31))
                            {
                                if (savelbpy.leave_balance <= 0)
                                {
                                    savelbpy.leave_balance = savelbpy.leave_balance;
                                    savelbpy.forfited_balance = 0;
                                }
                                else
                                {
                                    savelbpy.forfited_balance = savelbpy.leave_balance;
                                    savelbpy.leave_balance = 0;
                                }
                            }

                            savelbpy.net_period += period - savelbpy.unpaid;
                        }


                        if (savecheckleaveperyear.Exists(x =>
                                x.balances_of_year == new DateTime(i, 1, 1) && x.Employee_id == empjd.employee_id))
                        {
                            var rewritelb = savecheckleaveperyear.Find(x =>
                                x.balances_of_year == new DateTime(i, 1, 1) && x.Employee_id == empjd.employee_id);
                            rewritelb.Employee_id = empjd.employee_id;
                            rewritelb.balances_of_year = savelbpy.balances_of_year;
                            rewritelb.period = savelbpy.period;
                            rewritelb.unpaid = savelbpy.unpaid;
                            rewritelb.net_period = savelbpy.net_period;
                            rewritelb.accrued = savelbpy.accrued;
                            rewritelb.annual_leave_taken = savelbpy.annual_leave_taken;
                            rewritelb.Annual_Leave_Applied = savelbpy.Annual_Leave_Applied;
                            rewritelb.Annual_Leave_total = savelbpy.Annual_Leave_total;
                            rewritelb.leave_balance = savelbpy.leave_balance;
                            rewritelb.forfited_balance = savelbpy.forfited_balance;
                            rewritelb.sick_leave_balance = savelbpy.sick_leave_balance;
                            rewritelb.sick_leave_balance_industrial = savelbpy.sick_leave_balance_industrial;
                            rewritelb.compassionate_leave_balance = savelbpy.compassionate_leave_balance;
                            rewritelb.maternity_leave_balance = savelbpy.maternity_leave_balance;
                            rewritelb.haj_leave_balance = savelbpy.haj_leave_balance;
                            rewritelb.UDDAH_leave_balance = savelbpy.UDDAH_leave_balance;
                            rewritelb.escort_leave_balance = savelbpy.escort_leave_balance;
                            rewritelb.paternity_leave_balance = savelbpy.paternity_leave_balance;
                            rewritelb.sabbatical_leave_balance = savelbpy.sabbatical_leave_balance;
                            rewritelb.study_leave_balance = savelbpy.study_leave_balance;
                            rewritelb.date_updated = DateTime.Now;
                            this.db.Entry(rewritelb).State = EntityState.Modified;
                            this.db.SaveChanges();
                        }
                        else
                        {
                            this.db.leavecalperyears.Add(savelbpy);
                            this.db.SaveChanges();
                        }

                        var submitedleave = db.employeeleavesubmitions.ToList()
                            .FindAll(x =>
                                x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                x.Date <= new DateTime(i+1, 3, 31) && x.Date >= new DateTime(i, 1, 1));

                        var ifnewsublist = new List<employeeleavesubmition>();
                        if (perviousyearleave == null)
                        {
                            ifnewsublist = db.employeeleavesubmitions.ToList().FindAll(x => x.Employee_id == empjd.employee_id &&
                                x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1) &&
                                (x.leave_type == "1" || x.leave_type == "6"));
                        }

                        var yearrecord = db.leavecalperyears.ToList().Find(x =>
                            x.Employee_id == empjd.employee_id && x.balances_of_year.Year == i);
                        if (ifnewsublist.Count > 0)
                        {
                            submitedleave.AddRange(ifnewsublist);
                        }

                        if (submitedleave.Count == 0)
                        {
                            if (perviousyearleave != null)
                            {

                                if (perviousyearleave.leave_balance <= perviousyearleave.sumittedleavebal)
                                {
                                    yearrecord.sumittedleavebal = yearrecord.leave_balance;
                                    this.db.Entry(yearrecord).State = EntityState.Modified;
                                    this.db.SaveChanges();
                                }
                                else
                                {
                                    yearrecord.sumittedleavebal = yearrecord.leave_balance - perviousyearleave.leave_balance + perviousyearleave.sumittedleavebal;
                                    this.db.Entry(yearrecord).State = EntityState.Modified;
                                    this.db.SaveChanges();
                                }
                            }
                            else
                            {
                                yearrecord.sumittedleavebal = yearrecord.leave_balance;
                                this.db.Entry(yearrecord).State = EntityState.Modified;
                                this.db.SaveChanges();
                            }
                            goto endfun1;
                        }

                        var anualleavesub = 0d;
                        var unpaidsub = 0d;
                        foreach (var leaf in submitedleave)
                        {
                            var anltaken = true;
                            if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                            if (leaf.Start_leave > DateTime.Now)
                            {
                                anltaken = false;
                            }

                            var times = leaf.End_leave - leaf.Start_leave;
                            if (times == null) continue;
                            var days = times.Value.TotalDays + 1.0;
                            if (leaf.half) days -= 0.5;
                            switch (leaf.leave_type)
                            {
                                case "1":
                                    anualleavesub += days;
                                    break;
                                case "6":
                                    unpaidsub += days;
                                    break;
                            }
                        }

                        if (submitedleave.Count > 0)
                        {
                            yearrecord.sumittedleavebal =
                                yearrecord.leave_balance - anualleavesub - (unpaidsub * lbpd30f20);
                        }
                        else
                        {
                            yearrecord.sumittedleavebal = yearrecord.leave_balance;
                        }

                        this.db.Entry(yearrecord).State = EntityState.Modified;
                        this.db.SaveChanges();
                        endfun1: ;
                    }
                }
            }


            endfun: ;
        }

        public ActionResult reportserch(int? days)
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            var duplist = new List<master_file>();
            foreach (var file in alist)
            {
                var temp = file.employee_no;
                var temp2 = file.last_working_day;
                var temp3 = file.status;
                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    if (file.status != "inactive" && !file.last_working_day.HasValue)
                    {
                        if (!duplist.Exists(x => x.employee_no == file.employee_no))
                        {
                            afinallist.Add(file);
                        }
                    }
                    else
                    {
                        duplist.Add(file);
                    }
                }
            }

            ViewBag.days = days;
            var leaves = new List<Leave>();

            if (days.HasValue)
            {
                foreach (var file in afinallist)
                {
                    this.leavebalcalperyear(file.employee_id);
                    //this.forfitedbalence(file.employee_id);
                }

                var leaveballisttemp = this.db.leavecalperyears.OrderByDescending(x => x.balances_of_year)
                    .ThenBy(x => x.Employee_id).Where(x =>
                        (x.balances_of_year.Year == DateTime.Today.Year ||
                         x.balances_of_year.Year == DateTime.Today.Year - 1) && x.leave_balance >= days.Value).ToList();
                var leaveballist = new List<leavecalperyear>();
                foreach (var file in leaveballisttemp)
                {
                    if (!leaveballist.Exists(x => x.Employee_id == file.Employee_id))
                    {
                        var templist = leaveballisttemp.FindAll(x => x.Employee_id == file.Employee_id);
                        var tempver = new leavecalperyear();
                        foreach (var leavecalperyear in templist)
                        {
                            tempver.Employee_id = leavecalperyear.Employee_id;
                            if (tempver.leave_balance == null)
                            {
                                tempver.leave_balance = 0d;
                            }

                            if (DateTime.Today < new DateTime(DateTime.Today.Year, 3, 31))
                            {
                                tempver.leave_balance += leavecalperyear.leave_balance;
                            }
                            else
                            {
                                tempver.leave_balance = leavecalperyear.leave_balance;
                                break;
                            }
                        }

                        leaveballist.Add(tempver);
                    }
                }

                foreach (var leavecal in leaveballist)
                {
                    var leaveempid = this.db.Leaves.Where(x => x.Employee_id == leavecal.Employee_id)
                        .Include(l => l.master_file).OrderByDescending(x => x.leave_bal).ToList();
                    foreach (var leaf in leaveempid)
                    {
                        leaf.leave_bal = leavecal.leave_balance;
                        if (!leaves.Exists(x => x.Employee_id == leaf.Employee_id) &&
                            afinallist.Exists(x => x.employee_id == leaf.Employee_id)) leaves.Add(leaf);
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

        public ActionResult lbpyindex(string search)

        {
            var lbdisplaylist = new List<leavecalperyear>();
            if (!string.IsNullOrEmpty(search))
            {
                var Lbpyearlist = db.leavecalperyears.ToList();
                if (int.TryParse(search, out var idk))
                {
                    lbdisplaylist = Lbpyearlist.FindAll(x => x.master_file.employee_no == idk);
                    if (lbdisplaylist.Count > 0)
                    {
                        leavebalcalperyear(lbdisplaylist.First().Employee_id);
                    }
                }
                else
                {
                    // var svalue = search.ToUpperInvariant();
                    // lbdisplaylist = Lbpyearlist.FindAll(x => x.master_file.employee_name.ToUpperInvariant().Contains(svalue));
                    // if (lbdisplaylist.Count > 0)
                    // {
                    //     leavebalcalperyear(lbdisplaylist.First().Employee_id);
                    // }
                }
            }

            return View(lbdisplaylist);
        }

        public ActionResult ImportExcel()
        {
            return View();
        }

        /*public ActionResult lbtillyear(DateTime? date)
        {
            const double lbpd30 = (30.0 / 365.0);
            const double lbpd24 = (24.0 / 365.0);
            const double lbpd30f20 = (30.0 / 360.0);
            const double lbpd24f20 = (24.0 / 360.0);
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            
        }*/

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
                                        x.Employee_id == pro.Employee_id &&
                                        x.month.Value.Year == pro.month.Value.Year &&
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
                new ListItem { Text = "Annual", Value = "1" },
                new ListItem { Text = "Sick(non industrial)", Value = "2" },
                new ListItem { Text = "Compassionate", Value = "3" },
                new ListItem { Text = "Maternity", Value = "4" },
                new ListItem { Text = "Haj", Value = "5" },
                new ListItem { Text = "Unpaid", Value = "6" },
                new ListItem { Text = "Sick(industrial)", Value = "7" },
                new ListItem { Text = "UDDAH", Value = "8" },
                new ListItem { Text = "ESCORT", Value = "9" },
                new ListItem { Text = "PATERNITY ", Value = "10" },
                new ListItem { Text = "SABBATICAL", Value = "11" },
                new ListItem { Text = "STUDY LEAVE ", Value = "12" }
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
                // return this.View(cllist
                //     .Where(x => ((eddate >= x.Start_leave && eddate1 <= x.Start_leave ) || (eddate <= x.End_leave && eddate1 >= x.End_leave)) &&
                //                 x.leave_type == leave_type.ToString()).OrderBy(x => x.departmant_project).ThenBy(x => x.employee_no));
                var leaveconlistfinal = new List<con_leavemodel>();
                for (DateTime j = eddate.Value; j <= eddate1.Value; j = j.AddDays(1))
                {
                    var templeavelist = cllist.FindAll(x => x.Start_leave <= j && x.End_leave >= j);
                    foreach (var leaf in templeavelist)
                    {
                        if (!leaveconlistfinal.Exists(x => x.id == leaf.id) && leaf.leave_type == leave_type.ToString())
                        {
                            leaveconlistfinal.Add(leaf);
                        }
                    }
                }

                return this.View(leaveconlistfinal.OrderBy(x => x.departmant_project)
                    .ThenBy(x => x.employee_no)); /*return this.View(cllist
                    .Where(x => ((eddate <= x.Start_leave && eddate1 >= x.Start_leave) || (eddate >= x.End_leave && eddate1 <= x.End_leave)) &&
                                x.leave_type == leave_type.ToString()).OrderBy(x => x.departmant_project).ThenBy(x => x.employee_no));*/
            }

            return this.View(new List<con_leavemodel>());
        }

        public ActionResult periodic(DateTime? eddate, DateTime? eddate1, int? leave_type)
        {
            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Annual", Value = "1" },
                new ListItem { Text = "Sick(non industrial)", Value = "2" },
                new ListItem { Text = "Compassionate", Value = "3" },
                new ListItem { Text = "Maternity", Value = "4" },
                new ListItem { Text = "Haj", Value = "5" },
                new ListItem { Text = "Unpaid", Value = "6" },
                new ListItem { Text = "Sick(industrial)", Value = "7" },
                new ListItem { Text = "UDDAH", Value = "8" },
                new ListItem { Text = "ESCORT", Value = "9" },
                new ListItem { Text = "PATERNITY ", Value = "10" },
                new ListItem { Text = "SABBATICAL", Value = "11" },
                new ListItem { Text = "STUDY LEAVE ", Value = "12" }
            };
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            var leavelist = this.db.Leaves.ToList();
            var searchresults = new List<Leave>();
            if (eddate.HasValue && !eddate1.HasValue)
            {
                searchresults = leavelist.FindAll(x =>
                    x.Start_leave <= eddate && x.End_leave >= eddate && x.leave_type == leave_type.ToString() &&
                    x.master_file != null && !x.master_file.last_working_day.HasValue);
            }

            if (eddate.HasValue && eddate1.HasValue)
            {
                // searchresults = leavelist.FindAll(x => x.Start_leave <= eddate1 && x.Start_leave >= eddate); 
                searchresults = leavelist.FindAll(x =>
                    x.Start_leave <= eddate1 && x.End_leave >= eddate && x.leave_type == leave_type.ToString() &&
                    x.master_file != null && !x.master_file.last_working_day.HasValue);
            }

            if (!eddate.HasValue && !eddate1.HasValue && leave_type.HasValue)
            {
                searchresults = leavelist.FindAll(x => x.leave_type == leave_type.ToString());
            }

            return View(searchresults.OrderByDescending(x => x.Start_leave).ToList());
        }

        public ActionResult leavebaltill(DateTime? caltill)
        {
            var leavelist = new List<leavecalperyear>();
            if (caltill == null)
            {
                goto end;
            }

            const double lbpd30 = (30.0 / 365.0);
            const double lbpd24 = (24.0 / 365.0);
            const double lbpd30f20 = (30.0 / 360.0);
            const double lbpd24f20 = (24.0 / 360.0);
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var inalist = this.db.master_file.Where(x => x.last_working_day.HasValue == true).OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    if (!inalist.Exists(x => x.employee_no == file.employee_no))
                    {
                        afinallist.Add(file);
                    }
                }
            }


            foreach (var file in afinallist)
            {
                var empjd = afinallist.Find(x => x.employee_id == file.employee_id);
                var maxleaveperyear = new List<lbperyear>();
                if (db.lbperyears.OrderByDescending(y => y.year).ToList()
                    .Exists(x => x.Employee_id == file.employee_id))
                {
                    maxleaveperyear = db.lbperyears.OrderByDescending(y => y.year).ToList()
                        .FindAll(x => x.Employee_id == file.employee_id);
                }

                double accleave = 0;
                double netperiod = 0;
                if (empjd == null)
                {
                    goto endfun;
                }

                var asf = empjd.date_joined;
                if (asf == null)
                {
                    goto endfun;
                }


                var leaves2 = this.db.Leaves.OrderByDescending(x => x.Date).ThenBy(y => y.Start_leave).Where(
                        x => x.Employee_id == file.employee_id && x.Start_leave >= asf && x.End_leave < caltill.Value)
                    .ToList();
                var leaves = this.db.Leaves.OrderByDescending(x => x.Date).ThenBy(y => y.Start_leave).Where(
                        x => x.Employee_id == file.employee_id && x.Start_leave >= asf && x.Start_leave < caltill.Value)
                    .ToList();

                var period = 0.0d;

                if (asf.Value.Year <= 2020)
                {
                    double anualleavetakentill2020 = 0;
                    double nonintsicktill2020 = 0;
                    double compastill2020 = 0;
                    double matertill2020 = 0;
                    double hajtill2020 = 0;
                    double unpaidtill2020 = 0;
                    double intsicktill2020 = 0;
                    double uddahtill2020 = 0;
                    double escorttill2020 = 0;
                    double patertill2020 = 0;
                    double sabtill2020 = 0;
                    double studytill2020 = 0;
                    var leaveannualandunpaid2020 = new List<Leave>();
                    var leaverest2020 = new List<Leave>();
                    if (caltill.Value.Year >= 2020)
                    {
                        leaveannualandunpaid2020 = leaves.FindAll(x =>
                            x.Date <= new DateTime(2021, 3, 31) && (x.leave_type == "1" || x.leave_type == "6"));
                        leaverest2020 = leaves.FindAll(x =>
                            x.Date <= new DateTime(2020, 12, 31) && !(x.leave_type == "1" || x.leave_type == "6"));
                    }
                    else
                    {
                        leaveannualandunpaid2020 = leaves.FindAll(x =>
                            x.Date <= caltill.Value && (x.leave_type == "1" || x.leave_type == "6"));
                        leaverest2020 = leaves.FindAll(x =>
                            x.Date <= caltill.Value && !(x.leave_type == "1" || x.leave_type == "6"));
                    }

                    var leave2020 = new List<Leave>();
                    leave2020.AddRange(leaveannualandunpaid2020);
                    leave2020.AddRange(leaverest2020);
                    foreach (var leaf in leave2020)
                    {
                        var anltaken = true;
                        if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                        if (leaf.Start_leave > DateTime.Now)
                        {
                            anltaken = false;
                        }

                        var times = leaf.End_leave - leaf.Start_leave;
                        if (times == null) continue;
                        var days = times.Value.TotalDays + 1;
                        if (leaf.half) days -= 0.5;
                        switch (leaf.leave_type)
                        {
                            case "1":
                                anualleavetakentill2020 += days;
                                break;
                            case "2":
                                nonintsicktill2020 += days;
                                break;
                            case "3":
                                compastill2020 += days;
                                break;
                            case "4":
                                matertill2020 += days;
                                break;
                            case "5":
                                hajtill2020 += days;
                                break;
                            case "6":
                                unpaidtill2020 += days;
                                break;
                            case "7":
                                intsicktill2020 += days;
                                break;
                            case "8":
                                uddahtill2020 += days;
                                break;
                            case "9":
                                escorttill2020 += days;
                                break;
                            case "10":
                                patertill2020 += days;
                                break;
                            case "11":
                                sabtill2020 += days;
                                break;
                            case "12":
                                studytill2020 += days;
                                break;
                        }
                    }

                    var temptime = new DateTime(2020, 12, 31) - asf.Value;
                    var temptime2 = new TimeSpan();
                    period = temptime.TotalDays + 1;
                    var temptime1 = /*new DateTime(i, 12, 31)*/ new DateTime(DateTime.Now.Year, 12, 31) - asf.Value;
                    var joiningperiod = temptime1.TotalDays + 1;
                    if (joiningperiod < 365)
                    {
                        accleave = Math.Round((period - unpaidtill2020) * (lbpd24f20));
                    }
                    else
                    {
                        accleave = Math.Round((period - unpaidtill2020) * lbpd30f20);
                    }

                    var leavebal2020 = Math.Round(accleave) - anualleavetakentill2020;
                    var savelbpy = new leavecalperyear();
                    savelbpy.Employee_id = empjd.employee_id;
                    savelbpy.balances_of_year = new DateTime(2020, 1, 1);
                    savelbpy.period = period;
                    savelbpy.unpaid = unpaidtill2020;
                    savelbpy.net_period = period - unpaidtill2020;
                    savelbpy.accrued = accleave;
                    savelbpy.annual_leave_taken = anualleavetakentill2020;
                    savelbpy.Annual_Leave_Applied = 0;
                    savelbpy.Annual_Leave_total = anualleavetakentill2020;
                    if (leavebal2020 <= 0)
                    {
                        savelbpy.leave_balance = leavebal2020;
                        savelbpy.forfited_balance = 0;
                    }
                    else
                    {
                        savelbpy.leave_balance = 0;
                        savelbpy.forfited_balance = leavebal2020;
                    }

                    savelbpy.sick_leave_balance = nonintsicktill2020;
                    savelbpy.sick_leave_balance_industrial = intsicktill2020;
                    savelbpy.compassionate_leave_balance = compastill2020;
                    savelbpy.maternity_leave_balance = matertill2020;
                    savelbpy.haj_leave_balance = hajtill2020;
                    savelbpy.UDDAH_leave_balance = uddahtill2020;
                    savelbpy.escort_leave_balance = escorttill2020;
                    savelbpy.paternity_leave_balance = patertill2020;
                    savelbpy.sabbatical_leave_balance = sabtill2020;
                    savelbpy.study_leave_balance = sabtill2020;
                    savelbpy.date_updated = DateTime.Now;
                    savelbpy.leave_count = leave2020.Count;
                    savelbpy.master_file = file;
                    leavelist.Add(savelbpy);
                }

                if (asf.Value.Year <= 2023)
                {
                    for (int i = 2021; i < 2024; i++)
                    {
                        if (asf.Value.Year <= i)
                        {
                            var perviousyearleave = leavelist.Find(x =>
                                x.balances_of_year == new DateTime(i - 1, 1, 1) && x.Employee_id == empjd.employee_id);
                            var ifnewleavelist = new List<Leave>();
                            var ifnewsublist = new List<employeeleavesubmition>();
                            var submitedleave = new List<employeeleavesubmition>();
                            var leaveannualandunpaidpy = new List<Leave>();
                            var leaverestpy = new List<Leave>();

                            if (caltill.Value.Year >= i)
                            {
                                leaveannualandunpaidpy = leaves.FindAll(x =>
                                    x.Date <= new DateTime(i + 1, 3, 31) && x.Date > new DateTime(i, 3, 31) &&
                                    (x.leave_type == "1" || x.leave_type == "6"));
                                leaverestpy = leaves.FindAll(x =>
                                    x.Date <= new DateTime(i, 12, 31) && x.Date >= new DateTime(i, 1, 1) &&
                                    !(x.leave_type == "1" || x.leave_type == "6"));
                                submitedleave = db.employeeleavesubmitions.ToList()
                                    .FindAll(x =>
                                        x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                        x.Date <= new DateTime(i + 1, 3, 31) && x.Date >= new DateTime(i, 4, 1));
                                if (perviousyearleave == null)
                                {
                                    ifnewleavelist = leaves.FindAll(x =>
                                        x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1) &&
                                        (x.leave_type == "1" || x.leave_type == "6"));
                                    ifnewsublist = db.employeeleavesubmitions.ToList().FindAll(x =>
                                        x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                        x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1));
                                }
                            }
                            else
                            {
                                if (caltill.Value.Month > 3)
                                {
                                    leaveannualandunpaidpy = leaves.FindAll(x =>
                                        x.Date <= caltill.Value && x.Date > new DateTime(i, 3, 31) &&
                                        (x.leave_type == "1" || x.leave_type == "6"));
                                    leaverestpy = leaves.FindAll(x =>
                                        x.Date <= caltill.Value && x.Date >= new DateTime(i, 1, 1) &&
                                        !(x.leave_type == "1" || x.leave_type == "6"));
                                    submitedleave = db.employeeleavesubmitions.ToList()
                                        .FindAll(x =>
                                            x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                            x.Date <= caltill.Value && x.Date >= new DateTime(i, 4, 1));
                                    if (perviousyearleave == null)
                                    {
                                        ifnewleavelist = leaves.FindAll(x =>
                                            x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1) &&
                                            (x.leave_type == "1" || x.leave_type == "6"));
                                        ifnewsublist = db.employeeleavesubmitions.ToList().FindAll(x =>
                                            x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                            x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1));
                                    }
                                }
                                else
                                {
                                    if (perviousyearleave == null)
                                    {
                                        ifnewleavelist = leaves.FindAll(x =>
                                            x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1) &&
                                            (x.leave_type == "1" || x.leave_type == "6"));
                                        ifnewsublist = db.employeeleavesubmitions.ToList().FindAll(x =>
                                            x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                            x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1));
                                    }
                                }
                            }

                            var leavepy = new List<Leave>();
                            var savelbpy = new leavecalperyear
                            {
                                balances_of_year = DateTime.Now,
                                period = 0,
                                unpaid = 0,
                                net_period = 0,
                                accrued = 0,
                                annual_leave_taken = 0,
                                Annual_Leave_Applied = 0,
                                Annual_Leave_total = 0,
                                leave_balance = 0,
                                forfited_balance = 0,
                                sick_leave_balance = 0,
                                compassionate_leave_balance = 0,
                                maternity_leave_balance = 0,
                                haj_leave_balance = 0,
                                UDDAH_leave_balance = 0,
                                escort_leave_balance = 0,
                                paternity_leave_balance = 0,
                                sabbatical_leave_balance = 0,
                                study_leave_balance = 0,
                                date_updated = DateTime.Now,
                                sick_leave_balance_industrial = 0,
                                sumittedleavebal = 0
                            };
                            savelbpy.Employee_id = empjd.employee_id;
                            savelbpy.balances_of_year = new DateTime(i, 1, 1);
                            savelbpy.date_updated = DateTime.Now;
                            if (ifnewleavelist.Count != 0)
                            {
                                leavepy.AddRange(ifnewleavelist);
                            }

                            leavepy.AddRange(leaveannualandunpaidpy);
                            leavepy.AddRange(leaverestpy);
                            foreach (var leaf in leavepy)
                            {
                                var anlnottaken = false;
                                if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                                if (leaf.Start_leave > DateTime.Now)
                                {
                                    anlnottaken = true;
                                }

                                var times = leaf.End_leave - leaf.Start_leave;
                                if (times == null) continue;
                                var days = times.Value.TotalDays + 1;
                                if (leaf.half) days -= 0.5;
                                switch (leaf.leave_type)
                                {
                                    case "1":
                                        if (anlnottaken)
                                        {
                                            savelbpy.Annual_Leave_Applied += days;
                                        }
                                        else
                                        {
                                            savelbpy.annual_leave_taken += days;
                                        }

                                        break;
                                    case "2":
                                        savelbpy.sick_leave_balance += days;
                                        break;
                                    case "3":
                                        savelbpy.compassionate_leave_balance += days;
                                        break;
                                    case "4":
                                        savelbpy.maternity_leave_balance += days;
                                        break;
                                    case "5":
                                        savelbpy.haj_leave_balance += days;
                                        break;
                                    case "6":
                                        savelbpy.unpaid += days;
                                        break;
                                    case "7":
                                        savelbpy.sick_leave_balance_industrial += days;
                                        break;
                                    case "8":
                                        savelbpy.UDDAH_leave_balance += days;
                                        break;
                                    case "9":
                                        savelbpy.escort_leave_balance += days;
                                        break;
                                    case "10":
                                        savelbpy.paternity_leave_balance += days;
                                        break;
                                    case "11":
                                        savelbpy.sabbatical_leave_balance += days;
                                        break;
                                    case "12":
                                        savelbpy.study_leave_balance += days;
                                        break;
                                }
                            }

                            var temptime = new DateTime(DateTime.Now.Year, 12, 31) - asf.Value;
                            var joiningperiod = temptime.TotalDays + 1;
                            if (perviousyearleave == null)
                            {
                                var temptime1 = new DateTime(i, 12, 31) - asf.Value;
                                period = temptime1.TotalDays + 1;
                            }
                            else
                            {
                                period = 365;
                            }

                            if (joiningperiod < 365)
                            {
                                if (!savelbpy.unpaid.HasValue)
                                {
                                    savelbpy.unpaid = 0;
                                }

                                if (i == 2023)
                                {
                                    accleave = RoundToNearestHalf((joiningperiod - savelbpy.unpaid.Value) *
                                                                  (lbpd24f20));
                                }
                                else
                                {
                                    accleave = Math.Round((joiningperiod - savelbpy.unpaid.Value) * (lbpd24f20));
                                }
                            }
                            else
                            {
                                if (!savelbpy.unpaid.HasValue)
                                {
                                    savelbpy.unpaid = 0;
                                }

                                var temp1 = (savelbpy.unpaid.Value);
                                var temp2 = (period);
                                if (i == 2023)
                                {
                                    accleave = RoundToNearestHalf((temp2 - temp1) * lbpd30);
                                }
                                else
                                {
                                    accleave = Math.Round((temp2 - temp1) * lbpd30f20);
                                }
                            }

                            if (maxleaveperyear.Count > 0)
                            {
                                if (maxleaveperyear.Exists(x => x.year.Year <= i))
                                {
                                    var maxleaveperyeartemp = maxleaveperyear.Find(x => x.year.Year <= i);
                                    if (period - savelbpy.unpaid.Value < 365)
                                    {
                                        accleave = Math.Round((period - savelbpy.unpaid.Value) *
                                            maxleaveperyeartemp.total_leave_balance / 365);
                                    }
                                    else
                                    {
                                        accleave = maxleaveperyeartemp.total_leave_balance;
                                    }
                                }
                            }

                            savelbpy.accrued = accleave;
                            savelbpy.Annual_Leave_total = savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied;
                            savelbpy.period = period;
                            savelbpy.leave_count = leavepy.Count;
                            if (perviousyearleave != null)
                            {
                                if (savelbpy.net_period == null)
                                {
                                    savelbpy.net_period = 0;
                                }

                                savelbpy.net_period += perviousyearleave.net_period + period - savelbpy.unpaid;
                                savelbpy.leave_balance = accleave - savelbpy.Annual_Leave_total +
                                                         perviousyearleave.leave_balance;

                                if (DateTime.Now >= new DateTime(i + 1, 3, 31))
                                {
                                    if (savelbpy.leave_balance <= 0)
                                    {
                                        savelbpy.leave_balance = savelbpy.leave_balance;
                                        savelbpy.forfited_balance = 0;
                                    }
                                    else
                                    {
                                        savelbpy.forfited_balance = savelbpy.leave_balance;
                                        savelbpy.leave_balance = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (savelbpy.net_period == null)
                                {
                                    savelbpy.net_period = 0;
                                }

                                savelbpy.leave_balance =
                                    accleave - (savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied);

                                if (DateTime.Now >= new DateTime(i + 1, 3, 31))
                                {
                                    if (savelbpy.leave_balance <= 0)
                                    {
                                        savelbpy.leave_balance = savelbpy.leave_balance;
                                        savelbpy.forfited_balance = 0;
                                    }
                                    else
                                    {
                                        savelbpy.forfited_balance = savelbpy.leave_balance;
                                        savelbpy.leave_balance = 0;
                                    }
                                }

                                savelbpy.net_period += period - savelbpy.unpaid;
                            }

                            savelbpy.master_file = file;
                            leavelist.Add(savelbpy);
                            var yearrecord = leavelist.Find(x =>
                                x.Employee_id == empjd.employee_id && x.balances_of_year.Year == i);
                            if (ifnewsublist.Count > 0)
                            {
                                submitedleave.AddRange(ifnewsublist);
                            }

                            if (submitedleave.Count == 0)
                            {
                                yearrecord.sumittedleavebal = yearrecord.leave_balance;
                                goto endfun1;
                            }

                            var anualleavesub = 0d;
                            var unpaidsub = 0d;
                            foreach (var leaf in submitedleave)
                            {
                                var anltaken = true;
                                if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                                if (leaf.Start_leave > DateTime.Now)
                                {
                                    anltaken = false;
                                }

                                var times = leaf.End_leave - leaf.Start_leave;
                                if (times == null) continue;
                                var days = times.Value.TotalDays + 1.0;
                                if (leaf.half) days -= 0.5;
                                switch (leaf.leave_type)
                                {
                                    case "1":
                                        anualleavesub += days;
                                        break;
                                    case "6":
                                        unpaidsub += days;
                                        break;
                                }
                            }

                            if (submitedleave.Count > 0)
                            {
                                yearrecord.sumittedleavebal = yearrecord.leave_balance - anualleavesub -
                                                              RoundToNearestHalf(unpaidsub * lbpd30f20);
                            }
                            else
                            {
                                yearrecord.sumittedleavebal = yearrecord.leave_balance;
                            }

                            endfun1: ;
                        }
                    }
                }

                if (asf.Value.Year <= 2024)
                {
                    var nextyearleave = 0;
                    if (DateTime.Now.Month == 12)
                    {
                        nextyearleave = 1;
                    }
                    for (int i = 2024; i <= DateTime.Now.Year + nextyearleave; i++)
                    {
                        if (asf.Value.Year <= i)
                        {
                            var leaveannualandunpaidpy = new List<Leave>();
                            var leaverestpy = new List<Leave>();
                            var ifnewleavelist = new List<Leave>();
                            var perviousyearleave = leavelist.Find(x =>
                                x.balances_of_year == new DateTime(i - 1, 1, 1) && x.Employee_id == empjd.employee_id);
                            var ifnewsublist = new List<employeeleavesubmition>();
                            var submitedleave = new List<employeeleavesubmition>();
                            if (caltill.Value.Year >= i)
                            {
                                leaveannualandunpaidpy = leaves.FindAll(x =>
                                    x.Start_leave <= new DateTime(i + 1, 3, 31) &&
                                    x.Start_leave > new DateTime(i, 3, 31) &&
                                    (x.leave_type == "1" || x.leave_type == "6"));
                                leaverestpy = leaves.FindAll(x =>
                                    x.Start_leave <= new DateTime(i, 12, 31) &&
                                    x.Start_leave >= new DateTime(i, 1, 1) &&
                                    !(x.leave_type == "1" || x.leave_type == "6"));
                                submitedleave = db.employeeleavesubmitions.ToList()
                                    .FindAll(x => x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                                  x.Start_leave <= new DateTime(i, 12, 31) &&
                                                  x.Start_leave >= new DateTime(i, 1, 1) );


                                if (perviousyearleave == null)
                                {
                                    ifnewleavelist = leaves.FindAll(x =>
                                        x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1) &&
                                        (x.leave_type == "1" || x.leave_type == "6"));
                                    ifnewsublist = db.employeeleavesubmitions.ToList().FindAll(x =>
                                        x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1) && (x.leave_type == "1" || x.leave_type == "6"));

                                }
                            }
                            else
                            {
                                if (caltill.Value.Month > 3)
                                {
                                    leaveannualandunpaidpy = leaves.FindAll(x =>
                                        x.Start_leave <= caltill.Value && x.Start_leave > new DateTime(i, 3, 31) &&
                                        (x.leave_type == "1" || x.leave_type == "6"));
                                    leaverestpy = leaves.FindAll(x =>
                                        x.Start_leave <= caltill.Value && x.Start_leave >= new DateTime(i, 1, 1) &&
                                        !(x.leave_type == "1" || x.leave_type == "6"));
                                    submitedleave = db.employeeleavesubmitions.ToList()
                                        .FindAll(x => x.Employee_id == empjd.employee_id && x.apstatus == "submitted" &&
                                                      x.Start_leave <= new DateTime(i, 12, 31) &&
                                                      x.Start_leave >= new DateTime(i, 1, 1));
                                    if (perviousyearleave == null)
                                    {
                                        ifnewleavelist = leaves.FindAll(x =>
                                            x.Date <= caltill.Value && x.Date >= new DateTime(i, 1, 1) &&
                                            (x.leave_type == "1" || x.leave_type == "6"));
                                        ifnewsublist = db.employeeleavesubmitions.ToList().FindAll(x =>
                                            x.Date <= new DateTime(i, 3, 31) && x.Date >= new DateTime(i, 1, 1) && (x.leave_type == "1" || x.leave_type == "6"));

                                    }
                                }
                                else
                                {
                                    if (perviousyearleave == null)
                                    {
                                        ifnewleavelist = leaves.FindAll(x =>
                                            x.Date <= caltill.Value && x.Date >= new DateTime(i, 1, 1) &&
                                            (x.leave_type == "1" || x.leave_type == "6"));
                                        ifnewsublist = db.employeeleavesubmitions.ToList().FindAll(x =>
                                            x.Date <= caltill.Value && x.Date >= new DateTime(i, 1, 1) && (x.leave_type == "1" || x.leave_type == "6"));

                                    }
                                }
                            }

                            var leavepy = new List<Leave>();
                            var savelbpy = new leavecalperyear
                            {
                                balances_of_year = DateTime.Now,
                                period = 0,
                                unpaid = 0,
                                net_period = 0,
                                accrued = 0,
                                annual_leave_taken = 0,
                                Annual_Leave_Applied = 0,
                                Annual_Leave_total = 0,
                                leave_balance = 0,
                                forfited_balance = 0,
                                sick_leave_balance = 0,
                                compassionate_leave_balance = 0,
                                maternity_leave_balance = 0,
                                haj_leave_balance = 0,
                                UDDAH_leave_balance = 0,
                                escort_leave_balance = 0,
                                paternity_leave_balance = 0,
                                sabbatical_leave_balance = 0,
                                study_leave_balance = 0,
                                date_updated = DateTime.Now,
                                sick_leave_balance_industrial = 0,
                                sumittedleavebal = 0
                            };
                            savelbpy.Employee_id = empjd.employee_id;
                            savelbpy.balances_of_year = new DateTime(i, 1, 1);
                            savelbpy.date_updated = DateTime.Now;
                            if (ifnewleavelist.Count != 0)
                            {
                                leavepy.AddRange(ifnewleavelist);
                            }

                            leavepy.AddRange(leaveannualandunpaidpy);
                            leavepy.AddRange(leaverestpy);
                            savelbpy.leave_count = leavepy.Count;
                            foreach (var leaf in leavepy)
                            {
                                var anlnottaken = false;
                                if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                                if (leaf.Start_leave > DateTime.Now)
                                {
                                    anlnottaken = true;
                                }

                                var times = leaf.End_leave - leaf.Start_leave;
                                if (times == null) continue;
                                var days = times.Value.TotalDays + 1;
                                if (leaf.half) days -= 0.5;
                                switch (leaf.leave_type)
                                {
                                    case "1":
                                        if (anlnottaken)
                                        {
                                            savelbpy.Annual_Leave_Applied += days;
                                        }
                                        else
                                        {
                                            savelbpy.annual_leave_taken += days;
                                        }

                                        break;
                                    case "2":
                                        savelbpy.sick_leave_balance += days;
                                        break;
                                    case "3":
                                        savelbpy.compassionate_leave_balance += days;
                                        break;
                                    case "4":
                                        savelbpy.maternity_leave_balance += days;
                                        break;
                                    case "5":
                                        savelbpy.haj_leave_balance += days;
                                        break;
                                    case "6":
                                        savelbpy.unpaid += days;
                                        break;
                                    case "7":
                                        savelbpy.sick_leave_balance_industrial += days;
                                        break;
                                    case "8":
                                        savelbpy.UDDAH_leave_balance += days;
                                        break;
                                    case "9":
                                        savelbpy.escort_leave_balance += days;
                                        break;
                                    case "10":
                                        savelbpy.paternity_leave_balance += days;
                                        break;
                                    case "11":
                                        savelbpy.sabbatical_leave_balance += days;
                                        break;
                                    case "12":
                                        savelbpy.study_leave_balance += days;
                                        break;
                                }
                            }

                            var temptime = new DateTime(i, 12, 31) - asf.Value;
                            var joiningperiod = temptime.TotalDays + 1;
                            period = 365;
                            if (joiningperiod < 365)
                            {
                                if (!savelbpy.unpaid.HasValue)
                                {
                                    savelbpy.unpaid = 0;
                                }

                                accleave = Math.Round((joiningperiod) * lbpd24);
                            }
                            else
                            {
                                if (!savelbpy.unpaid.HasValue)
                                {
                                    savelbpy.unpaid = 0;
                                }

                                var temp1 = (savelbpy.unpaid.Value);
                                var temp2 = (period);
                                accleave = Math.Round((temp2) * lbpd30);
                            }

                            if (maxleaveperyear.Count > 0)
                            {
                                if (maxleaveperyear.Exists(x => x.year.Year <= i))
                                {
                                    var maxleaveperyeartemp = maxleaveperyear.Find(x => x.year.Year <= i);
                                    if (period - savelbpy.unpaid.Value < 365)
                                    {
                                        accleave = Math.Round((period - savelbpy.unpaid.Value) *
                                            maxleaveperyeartemp.total_leave_balance / 365);
                                    }
                                    else
                                    {
                                        accleave = maxleaveperyeartemp.total_leave_balance;
                                    }
                                }
                            }

                            savelbpy.Annual_Leave_total = savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied;
                            savelbpy.period = 365;
                            savelbpy.accrued = accleave;
                            if (perviousyearleave != null)
                            {
                                if (savelbpy.net_period == null)
                                {
                                    savelbpy.net_period = 0;
                                }

                                savelbpy.net_period += perviousyearleave.net_period + savelbpy.period - savelbpy.unpaid;
                                savelbpy.leave_balance = accleave -
                                                         (savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied) +
                                                         perviousyearleave.leave_balance;
                                if (DateTime.Now >= new DateTime(i + 1, 3, 31))
                                {
                                    if (savelbpy.leave_balance <= 0)
                                    {
                                        savelbpy.leave_balance = savelbpy.leave_balance;
                                        savelbpy.forfited_balance = 0;
                                    }
                                    else
                                    {
                                        savelbpy.forfited_balance = savelbpy.leave_balance;
                                        savelbpy.leave_balance = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (savelbpy.net_period == null)
                                {
                                    savelbpy.net_period = 0;
                                }

                                savelbpy.leave_balance = Math.Round(accleave) -
                                                         (savelbpy.annual_leave_taken + savelbpy.Annual_Leave_Applied);
                                if (DateTime.Now >= new DateTime(i + 1, 3, 31))
                                {
                                    if (savelbpy.leave_balance <= 0)
                                    {
                                        savelbpy.leave_balance = savelbpy.leave_balance;
                                        savelbpy.forfited_balance = 0;
                                    }
                                    else
                                    {
                                        savelbpy.forfited_balance = savelbpy.leave_balance;
                                        savelbpy.leave_balance = 0;
                                    }
                                }

                                savelbpy.net_period += period - savelbpy.unpaid;
                            }
                            savelbpy.master_file = file;
                            leavelist.Add(savelbpy);
                            if (submitedleave.Count == 0)
                            {
                                goto endfun1;
                            }
                            var yearrecord = leavelist.Find(x =>
                                x.Employee_id == empjd.employee_id && x.balances_of_year.Year == i);
                            if (ifnewsublist.Count > 0)
                            {
                                submitedleave.AddRange(ifnewsublist);
                            }
                            var anualleavesub = 0d;
                            var unpaidsub = 0d;
                            foreach (var leaf in submitedleave)
                            {
                                var anltaken = true;
                                if (leaf.End_leave == null || leaf.Start_leave == null) continue;
                                if (leaf.Start_leave > DateTime.Now)
                                {
                                    anltaken = false;
                                }
                                var times = leaf.End_leave - leaf.Start_leave;
                                if (times == null) continue;
                                var days = times.Value.TotalDays + 1.0;
                                if (leaf.half) days -= 0.5;
                                switch (leaf.leave_type)
                                {
                                    case "1":
                                        anualleavesub += days;
                                        break;
                                    case "6":
                                        unpaidsub += days;
                                        break;
                                }
                            }

                            if (submitedleave.Count > 0)
                            {
                                yearrecord.sumittedleavebal = yearrecord.leave_balance - anualleavesub - (unpaidsub * lbpd30f20);
                            }
                            else
                            {
                                yearrecord.sumittedleavebal = yearrecord.leave_balance;
                            }

                        endfun1:;
                        }
                    }
                }

                endfun: ;
            }

            return View(leavelist.OrderBy(x => x.master_file.employee_no).ThenBy(x => x.balances_of_year));

        end: ;
        return View(new List<leavecalperyear>());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}