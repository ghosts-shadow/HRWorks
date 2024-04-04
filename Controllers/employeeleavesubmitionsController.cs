using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using HRworks.Models;
using Microsoft.Ajax.Utilities;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNet.Identity;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace HRworks.Controllers
{
    public class employeeleavesubmitionsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: employeeleavesubmitions
        [Authorize]
        //[Authorize(Roles = "HOD,employee,Manager,super_admin")]
        public ActionResult Index()
        {
            var emprel = db.emprels.ToList();
            var leafcon = new LeavesController();
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);

            var employeeleavesubmitions = new List<employeeleavesubmition>();
            if (empuser == null)
            {
                goto logend;
            }

            var empjd = empuser.master_file;
            var rel = emprel.Find(x => x.Employee_id == empjd.employee_id);
            ViewBag.rel = "";
            if (rel != null && rel.HOD.HasValue)
            {
                ViewBag.rel = "line_man";
            }

            employeeleavesubmitions =
                db.employeeleavesubmitions.ToList()
                    .FindAll(x => x.Employee_id == empjd.employee_id && !(x.apstatus == "approved" || x.apstatus == "already exists"));
            leafcon.leavebalcalperyear(empjd.employee_id);
            //leafcon.forfitedbalence(empjd.employee_id);
            var leavecal2020list = db.leavecalperyears.ToList();
            //var leavecal2020list = db.leavecal2020.ToList();
            //var leavebal2020 = new leavecal2020();
            var leavebal2020 = leavecal2020list.OrderByDescending(x=>x.balances_of_year).ToList().FindAll(x =>
                x.Employee_id == empjd.employee_id);
            var asf = empjd.date_joined;
            var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                x => x.Employee_id == empjd.employee_id && x.Start_leave >= asf).ToList();
            var ump = leaves.ToList();
            var rdate = new DateTime();
            var times = new TimeSpan?();
            foreach (var leaf in ump)
            {
                var leavetoemp = new employeeleavesubmition();
                leavetoemp.Id = leaf.Id;
                leavetoemp.Date = leaf.Date;
                leavetoemp.Start_leave = leaf.Start_leave;
                leavetoemp.End_leave = leaf.End_leave;
                leavetoemp.Employee_id = leaf.Employee_id;
                leavetoemp.Return_leave = leaf.Return_leave;
                leavetoemp.leave_type = leaf.leave_type;
                leavetoemp.toltal_requested_days = leaf.toltal_requested_days;
                leavetoemp.half = leaf.half;
                leavetoemp.toltal_requested_days = leaf.days;
                leavetoemp.apstatus = "approved";
                leavetoemp.empreturnfromleavesubs = new List<empreturnfromleavesub>();
                var erllist = db.empreturnfromleavesubs.Where(y => y.leaveid == leaf.Id)
                    .OrderByDescending(x => x.dateadded).ToList();
                var empleavesubcheck = new empreturnfromleavesub();
                var empreturn = new empreturnfromleavesub();
                if (erllist.Count != 0)
                {
                    empleavesubcheck = erllist.First();
                    empreturn.actualreturnleave = empleavesubcheck.actualreturnleave;
                    empreturn.apstatus = empleavesubcheck.apstatus;
                }
                else
                {
                    empreturn.actualreturnleave = leaf.actual_return_date;
                }

                empreturn.Date = leavetoemp.Date;
                empreturn.Employee_id = leavetoemp.Employee_id;
                if (empreturn.actualreturnleave.HasValue)
                {
                    if (empleavesubcheck.apstatus == null && leaf.actual_return_date.HasValue)
                    {
                        empreturn.apstatus = "approved";
                    }
                }

                leavetoemp.empreturnfromleavesubs.Add(empreturn);
                employeeleavesubmitions.Add(leavetoemp);
                if (leaf.Reference == null)
                {
                    leaf.Reference = DateTime.Now.ToString("F");
                }

                rdate = Convert.ToDateTime(leaf.Reference);
                {
                    /*if (leaf.leave_type == "1")
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
                    }*/
                }
            }

            /*
            if (DateTime.Today <= new DateTime(DateTime.Today.Year,3,31))
            {
                if (leavebal2020.Count > 1 && (leavebal2020[1].leave_balance > 0 || leavebal2020[1].sumittedleavebal > 0))
                {

                    if (leavebal2020[0].leave_balance + leavebal2020[1].leave_balance <
                        leavebal2020[0].sumittedleavebal + leavebal2020[1].sumittedleavebal)
                    {
                        this.ViewBag.lbal = leavebal2020[0].leave_balance + leavebal2020[1].leave_balance;
                    }
                    else
                    {
                        this.ViewBag.lbal = leavebal2020[0].sumittedleavebal + leavebal2020[1].sumittedleavebal;
                    }
                }
                else
                {
                    if (leavebal2020[0].leave_balance < leavebal2020[0].sumittedleavebal)
                    {
                        this.ViewBag.lbal = leavebal2020[0].leave_balance;
                    }
                    else
                    {
                        this.ViewBag.lbal = leavebal2020[0].sumittedleavebal;
                    }
                }
            }
            else
            {
            }*/
                if (leavebal2020[0].leave_balance < leavebal2020[0].sumittedleavebal)
                {
                    this.ViewBag.lbal = leavebal2020[0].leave_balance;
                }
                else
                {
                    this.ViewBag.lbal = leavebal2020[0].sumittedleavebal;
                }

            var per = 0d;
            var ump1 = 0d;
            var accr = 0d;
            var forfited = 0d;
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
            foreach (var leavecalperyear in leavebal2020)
            {
                per+=leavecalperyear.period.Value;
                ump1+=leavecalperyear.unpaid.Value;
                accr+=leavecalperyear.accrued.Value;
                forfited+=leavecalperyear.forfited_balance.Value;
                sick+=leavecalperyear.sick_leave_balance.Value+leavecalperyear.sick_leave_balance_industrial.Value;
                comp+=leavecalperyear.compassionate_leave_balance.Value;
                mate+=leavecalperyear.maternity_leave_balance.Value;
                haj+=leavecalperyear.haj_leave_balance.Value;
                udd+=leavecalperyear.UDDAH_leave_balance.Value;
                esco+=leavecalperyear.escort_leave_balance.Value;
                pater+=leavecalperyear.paternity_leave_balance.Value;
                sab+=leavecalperyear.sabbatical_leave_balance.Value;
                study+=leavecalperyear.study_leave_balance.Value;
                availed+=leavecalperyear.annual_leave_taken.Value;
                favailed+=leavecalperyear.Annual_Leave_Applied.Value;
            }
            this.ViewBag.per = per;
            this.ViewBag.ump = ump1;
            this.ViewBag.accr = accr;
            this.ViewBag.forfited = forfited;
            this.ViewBag.aval = availed;
            this.ViewBag.faval = favailed;
            this.ViewBag.taval = availed + favailed;
            this.ViewBag.name = empjd.employee_name;
            this.ViewBag.no = empjd.employee_no;
            this.ViewBag.netp = leavebal2020[0].net_period;
            this.ViewBag.udd = udd;
            this.ViewBag.esco = esco;
            this.ViewBag.pater = pater;
            this.ViewBag.mate = mate;
            this.ViewBag.haj = haj;
            this.ViewBag.sick = sick;
            this.ViewBag.comp = comp;
            this.ViewBag.sab = sab;
            this.ViewBag.study = study;
            /*if (leavebal2020.leave_balance < leavebal2020.ifslbal)
            {
                this.ViewBag.lbal = leavebal2020.leave_balance;
            }
            else
            {
                this.ViewBag.lbal = leavebal2020.ifslbal;
            }

            this.ViewBag.per = leavebal2020.periodtill2020 + leavebal2020.periodafter2020;
            this.ViewBag.aval = availed;
            this.ViewBag.faval = favailed;
            this.ViewBag.taval = availed + favailed;
            this.ViewBag.netp = leavebal2020.net_periodtill2020 + leavebal2020.net_periodafter2020;
            this.ViewBag.ump = leavebal2020.unpaid_leavetill2020 + leavebal2020.unpaid_leaveafter2020;
            this.ViewBag.accr = leavebal2020.accruedafter2020 + leavebal2020.accruedtill2020;
            this.ViewBag.name = empjd.employee_name;
            this.ViewBag.no = empjd.employee_no;
            this.ViewBag.forfited = leavebal2020.forfitedafter2020 + leavebal2020.forfitedtill2020;*/
            return View(employeeleavesubmitions.OrderByDescending(c=>c.Start_leave).ToList());

            logend:;
            ViewBag.logend = "forcelogout";
            return View(employeeleavesubmitions);
        }

        public ActionResult displayall()
        {
            var leavelist = db.employeeleavesubmitions.OrderByDescending(x=>x.dateadded).ThenBy(x=>x.master_file.employee_no).ToList();
            return View(leavelist);
        }

        // GET: employeeleavesubmitions/Details/5
        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            employeeleavesubmition employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            if (employeeleavesubmition == null)
            {
                return HttpNotFound();
            }

            return View(employeeleavesubmition);
        }

        // GET: employeeleavesubmitions/Create
        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        public ActionResult Create()
        {
            var userempnolists = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empusers = userempnolists.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var leafcon = new LeavesController();
            var empjd = empusers.master_file;
            leafcon.leavebalcalperyear(empjd.employee_id);
            //leafcon.forfitedbalence(empjd.employee_id);
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "Annual", Value = "1"},
                new ListItem {Text = "Sick(non industrial)", Value = "2"},
                new ListItem {Text = "Compassionate", Value = "3"},
                new ListItem {Text = "Maternity", Value = "4"},
                new ListItem {Text = "Haj", Value = "5"},
                new ListItem {Text = "Unpaid", Value = "6"},
                new ListItem {Text = "Sick(industrial)", Value = "7"},
                new ListItem {Text = "AL UDDAH", Value = "8"},
                new ListItem {Text = "ESCORT", Value = "9"},
                new ListItem {Text = "PATERNITY ", Value = "10"},
                new ListItem {Text = "SABBATICAL", Value = "11"},
                new ListItem {Text = "STUDY LEAVE ", Value = "12"},
                new ListItem { Text = "Compensatory", Value = "13" }
            };
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            this.ViewBag.employee_id = empuser.master_file.employee_id;
            return View();
        }

        // POST: employeeleavesubmitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        public ActionResult Create([Bind(Include =
                "Id,Employee_id,Date,Start_leave,End_leave,Return_leave,leave_type,toltal_requested_days,submitted_by,apstatus,half,approved_byline,approved_byhod")]
            employeeleavesubmition employeeleavesubmition, HttpPostedFileBase fileBase)
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
                new ListItem {Text = "AL UDDAH", Value = "8"},
                new ListItem {Text = "ESCORT", Value = "9"},
                new ListItem {Text = "PATERNITY ", Value = "10"},
                new ListItem {Text = "SABBATICAL", Value = "11"},
                new ListItem {Text = "STUDY LEAVE ", Value = "12"},
                new ListItem { Text = "Compensatory", Value = "13" }
            };
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            this.ViewBag.leave_type = new SelectList(listItems, "Value", "Text");
            employeeleavesubmition.Employee_id = empuser.master_file.employee_id;
            employeeleavesubmition.dateadded = DateTime.Now;
            this.ViewBag.employee_id = empuser.master_file.employee_id;
            var jd = empuser.master_file.date_joined;
            var leavelistc = this.db.Leaves.Where(x=>x.Employee_id == employeeleavesubmition.Employee_id).ToList();
            var empsubleave = db.employeeleavesubmitions.ToList();
            var leaveabal = db.leavecalperyears.OrderBy(x=>x.balances_of_year).ToList().FindAll(x => x.Employee_id == empuser.master_file.employee_id /*&& (x.balances_of_year.Year == DateTime.Today.Year - 1 || x.balances_of_year.Year == DateTime.Today.Year )*/).Last();
            //var leaveabal = db.leavecal2020.ToList().Find(x => x.Employee_id == empuser.master_file.employee_id);
            var leaveperyeartemp = new leavecalperyear();
            leaveperyeartemp.Employee_id = empuser.master_file.employee_id;
            leaveperyeartemp.leave_balance = leaveabal.leave_balance;
            leaveperyeartemp.sumittedleavebal = leaveabal.sumittedleavebal;
            if (employeeleavesubmition.leave_type == "1")
            {
                var leavebalsub = 0d;
                if (leaveperyeartemp.sumittedleavebal < leaveperyeartemp.leave_balance)
                {
                    if (leaveperyeartemp.sumittedleavebal != null) leavebalsub = leaveperyeartemp.sumittedleavebal.Value;
                }
                else
                {
                    if (leaveperyeartemp.leave_balance != null) leavebalsub = leaveperyeartemp.leave_balance.Value;
                }

                if (employeeleavesubmition.toltal_requested_days != null)
                {
                    var leave_balhalfif = employeeleavesubmition.toltal_requested_days;
                    if (employeeleavesubmition.half)
                    {
                        leave_balhalfif -= 0.5;
                    }

                    if (leavebalsub - leave_balhalfif < 0)
                    {
                        ModelState.AddModelError("toltal_requested_days", "insufficient balance");
                        goto jderr;
                    }
                }
            }
                var compballist = db.companleaveBals.ToList();

            if (employeeleavesubmition.leave_type == "13")
            {
                if (compballist.Count > 0)
                {
                    var compbalcheck = compballist.Find(x=>x.EmpNo == employeeleavesubmition.Employee_id);
                    
                    if (compbalcheck != null ||employeeleavesubmition.toltal_requested_days != null)
                    {
                        var compbalhalfif = employeeleavesubmition.toltal_requested_days;
                        if (true)
                        {
                            compbalhalfif -= 0.5;
                        }

                        if (compbalcheck.balance - compbalhalfif < 0)
                        {
                            ModelState.AddModelError("toltal_requested_days", "insufficient balance");
                            goto jderr;
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("toltal_requested_days", "insufficient balance");
                    goto jderr;
                }
            }
            
            foreach (var leaf in empsubleave)
            {
                if (leaf.apstatus == "submitted" || leaf.apstatus == "submitted for HR" /*|| leaf.apstatus.Contains("rejected")*/ ||
                    leaf.apstatus == "approved by line manager")
                {
                    var templeave = new Leave();
                    templeave.Date = leaf.Date;
                    templeave.Start_leave = leaf.Start_leave;
                    templeave.End_leave = leaf.End_leave;
                    templeave.Return_leave = leaf.Return_leave;
                    templeave.half = leaf.half;
                    templeave.Employee_id = leaf.Employee_id;
                    templeave.leave_type = leaf.leave_type;
                    templeave.actualchangedby = User.Identity.Name;
                    leavelistc.Add(templeave);
                }
            }

            if (employeeleavesubmition.leave_type == "12")
            {
                var diff = 0d;
                var difftemp = 0d;
                var tempaluddah = leavelistc.FindAll(x => x.leave_type == "12");
                foreach (var leaf in tempaluddah)
                {
                    diff += (leaf.End_leave.Value - leaf.Start_leave.Value).Days + 1;
                    difftemp += (leaf.End_leave.Value - leaf.Start_leave.Value).Days + 1;
                }

                if (employeeleavesubmition.toltal_requested_days.HasValue)
                {
                    diff += employeeleavesubmition.toltal_requested_days.Value;
                }
                if (diff > 10)
                {
                    // if (diff - difftemp < 10)
                    // {
                    //     ModelState.AddModelError("toltal_requested_days", "insufficient balance ");
                    //     goto jderr;
                    // }
                    // else
                    {
                        ModelState.AddModelError("toltal_requested_days", "insufficient balance");
                        goto jderr;
                    }
                }
            }

            if (leavelistc.Exists(x =>
                ((x.Start_leave <= employeeleavesubmition.Start_leave &&
                  x.End_leave >= employeeleavesubmition.Start_leave) ||
                 (x.Start_leave <= employeeleavesubmition.End_leave &&
                  x.End_leave >= employeeleavesubmition.End_leave)) &&
                x.Employee_id == employeeleavesubmition.Employee_id))
            {
                var testl = leavelistc.FindAll(x =>
                    ((x.Start_leave <= employeeleavesubmition.Start_leave &&
                      x.End_leave >= employeeleavesubmition.Start_leave) ||
                     (x.Start_leave <= employeeleavesubmition.End_leave &&
                      x.End_leave >= employeeleavesubmition.End_leave)) &&
                    x.Employee_id == employeeleavesubmition.Employee_id);
                ModelState.AddModelError("Start_leave", "already exists");
                goto jderr;
            }
            if (employeeleavesubmition.leave_type == "5")
            {
                var datediff = (employeeleavesubmition.End_leave - employeeleavesubmition.Start_leave).Value.TotalDays +
                               1;
                if (datediff > 10)
                {
                    ModelState.AddModelError("leave_type", "maximum days allowed for haj are 10 ");
                    goto jderr;
                }
                if (leavelistc.Exists(x => x.leave_type == "5" && x.Employee_id == employeeleavesubmition.Employee_id))
                {
                    ModelState.AddModelError("leave_type", "already taken once");
                    goto jderr;
                }            

            }

            if (leavelistc.Exists(x =>
                (x.Start_leave >= employeeleavesubmition.Start_leave &&
                 x.End_leave <= employeeleavesubmition.End_leave) &&
                x.Employee_id == employeeleavesubmition.Employee_id))
            {
                ModelState.AddModelError("Start_leave", "already exists");

                goto jderr;
            }


            //            if (leavelistc.Exists(x=>x.End_leave >= employeeleavesubmition.Start_leave && x.Employee_id == employeeleavesubmition.Employee_id))
            //            {
            //                ModelState.AddModelError("Start_leave", employeeleavesubmition.Start_leave+" already exists");
            //
            //                goto jderr;
            //            }
            if (employeeleavesubmition.Start_leave < jd && employeeleavesubmition.Start_leave != default)
            {
                ModelState.AddModelError("Start_leave", "selected date is before " + jd);
                goto jderr;
            }

            if (employeeleavesubmition.End_leave < jd && employeeleavesubmition.End_leave != default)
            {
                ModelState.AddModelError("Start_leave", "selected date is before " + jd);
                goto jderr;
            }

            if (employeeleavesubmition.End_leave < employeeleavesubmition.Start_leave)
            {
                ModelState.AddModelError("Start_leave", "cannot add date in reverse");
                goto jderr;
            }

            var leavelist = this.db.Leaves.ToList();
            if (leavelist.Exists(
                x => x.Employee_id == employeeleavesubmition.Employee_id && x.Start_leave ==
                                                                         employeeleavesubmition.Start_leave
                                                                         && x.End_leave == employeeleavesubmition
                                                                             .End_leave))
            {
                ModelState.AddModelError("Start_leave", "the entry already exists");
                goto jderr;
            }

            string serverfile;
            if (fileBase != null)
            {
                var i = 0;
                var imgname = Path.GetFileName(fileBase.FileName);
                var fileexe = Path.GetExtension(fileBase.FileName);
                var filepath = new DirectoryInfo("D:/HR/leave/");
                serverfile =
                    "D:/HR/leave/" + empuser.master_file.employee_no; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                do
                {
                    serverfile = "D:/HR/leave/" + empuser.master_file.employee_no + "/" +
                                 empuser.master_file.employee_no + "_(" + i +")_"+DateTime.Now.ToString("dd-MM-YY")+ fileexe;
                    i++;
                } while (System.IO.File.Exists(
                    serverfile = "D:/HR/leave/" + empuser.master_file.employee_no + "/" +
                                 empuser.master_file.employee_no + "_(" + i + ")_" + DateTime.Now.ToString("dd-MM-YY") + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }

            if (ModelState.IsValid)
            {
                employeeleavesubmition.Employee_id = empuser.master_file.employee_id;
                employeeleavesubmition.submitted_by = User.Identity.Name;

                employeeleavesubmition.Date = DateTime.Today; 
                if ((employeeleavesubmition.leave_type == "2" || employeeleavesubmition.leave_type == "7")|| employeeleavesubmition.leave_type == "11")
                {
                    employeeleavesubmition.apstatus = "submitted for HR";
                    employeeleavesubmition.imgpath = serverfile;
                    db.employeeleavesubmitions.Add(employeeleavesubmition);
                    db.SaveChanges();
                    var sendmailtridsick = db.employeeleavesubmitions.ToList().Last();
                    SendMail("", "submitted for HR", sendmailtridsick.Id);
                    return RedirectToAction("Index");
                }

                if (employeeleavesubmition.leave_type == "13")
                {
                    var compbalcheck = compballist.Find(x => x.EmpNo == employeeleavesubmition.Employee_id);
                    //compbalcheck.balance -= employeeleavesubmition.toltal_requested_days
                }
                employeeleavesubmition.apstatus = "submitted";
                employeeleavesubmition.imgpath = serverfile;
                db.employeeleavesubmitions.Add(employeeleavesubmition);
                db.SaveChanges();
                var sendmailtrid = db.employeeleavesubmitions.ToList().Last();
                SendMail("", "submitted", sendmailtrid.Id);
                return RedirectToAction("Index");
            }

            jderr: ;
            return View(employeeleavesubmition);
        }

        // GET: employeeleavesubmitions/Edit/5
        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            employeeleavesubmition employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            if (employeeleavesubmition == null)
            {
                return HttpNotFound();
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name",
                employeeleavesubmition.Employee_id);
            return View(employeeleavesubmition);
        }

        // POST: employeeleavesubmitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        public ActionResult Edit([Bind(Include =
                "Id,Employee_id,Date,Start_leave,End_leave,Return_leave,leave_type,toltal_requested_days,submitted_by,apstatus,half,approved_byline,approved_byhod")]
            employeeleavesubmition employeeleavesubmition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_id = new SelectList(db.master_file, "employee_id", "employee_name",
                employeeleavesubmition.Employee_id);
            return View(employeeleavesubmition);
        }

        // GET: employeeleavesubmitions/Delete/5
        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            employeeleavesubmition employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            if (employeeleavesubmition == null)
            {
                return HttpNotFound();
            }

            return View(employeeleavesubmition);
        }

        // POST: employeeleavesubmitions/Delete/5
        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            employeeleavesubmition employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            db.employeeleavesubmitions.Remove(employeeleavesubmition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //[Authorize(Roles = "HOD,Manager,EXTHOD,super_admin")]
        [Authorize]
        public ActionResult empleaveap()
        {
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var employeeleavesubmitions = new List<employeeleavesubmition>();
            var conlist = db.contracts.ToList();
            if (User.IsInRole("super_admin") || User.IsInRole("slapproval"))
            {
                goto superlogin;
            }

            if (empuser == null)
            {
                goto logend;
            }

            var empjd = empuser.master_file;
            var relationlist = db.emprels.ToList();
            var logedinsrels = relationlist.FindAll(x => x.line_man == empjd.employee_id || x.HOD == empjd.employee_id);
            foreach (var emprel in logedinsrels)
            {
                if (emprel.line_man == empjd.employee_id)
                {
                    employeeleavesubmitions.AddRange(db.employeeleavesubmitions.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "submitted"));
                }
                if (emprel.HOD.HasValue && emprel.HOD == empjd.employee_id)
                {
                    employeeleavesubmitions.AddRange(db.employeeleavesubmitions.ToList()
                        .FindAll(x => x.Employee_id == emprel.Employee_id && x.apstatus == "approved by line manager"));
                }

            }
                

            foreach (var empdep in employeeleavesubmitions)
            {
                var empcon = conlist.Find(x => x.employee_no == empdep.Employee_id);
                if (empcon != null)
                {
                    empdep.dep = empcon.designation;
                }
                else
                {
                    empdep.dep = "no entry in contract table";
                }
            }
            return View(employeeleavesubmitions);
            logend: ;
            ViewBag.logend = "forcelogout";
            return View(employeeleavesubmitions);
            superlogin:;
            foreach (var empdep in employeeleavesubmitions)
            {
                var empcon = conlist.Find(x => x.employee_no == empdep.Employee_id);
                if (empcon != null)
                {
                    empdep.dep = empcon.designation;
                }
                else
                {
                    empdep.dep = "no entry in contract table";
                }
            }
            employeeleavesubmitions.AddRange(db.employeeleavesubmitions.ToList()
                .FindAll(x => x.apstatus == "submitted for HR"));
            return View(employeeleavesubmitions);
        }
        
        public ActionResult allpendingapprovals()
        {
            var pendingempapp = db.employeeleavesubmitions
                .Where(x => x.apstatus == "submitted" || x.apstatus == "approved by line manager" || x.apstatus == "submitted for HR").ToList();
            var empid = db.master_file.ToList();
            foreach (var pea in pendingempapp)
            {
                var emprellist = db.emprels.ToList();
                var emprel = emprellist.Find(x => x.Employee_id == pea.Employee_id);
                var conlist = db.contracts.ToList();
                var empcon = conlist.Find(x => x.employee_no == pea.Employee_id);
                if (emprel != null)
                {
                    if (emprel.HOD != null)
                    {
                        pea.rel = "line_man";
                    }
                    else
                    {
                        pea.rel = "";
                    }

                    if (pea.rel == "line_man")
                    {
                        if (pea.apstatus == "submitted")
                        {
                            pea.relwho = empid.Find(x=>x.employee_id == emprel.line_man).employee_name;
                        }
                        if (pea.apstatus == "approved by line manager")
                        {
                            pea.relwho = empid.Find(x => x.employee_id == emprel.HOD).employee_name;
                        }
                    }
                    else
                    {
                        pea.relwho = empid.Find(x => x.employee_id == emprel.line_man).employee_name;
                    }

                }
                else
                {
                    pea.rel = "";
                    pea.relwho = "no approval flow";
                }
                if (empcon != null)
                {
                    pea.dep = empcon.designation;
                }
                else
                {
                    pea.dep = "";
                }
                
            }
            return View(pendingempapp);
        }

        /*
        public ActionResult approve(int id)
        {
            if (User.IsInRole("Manager"))
            {
                var employeeleavesubmition = db.employeeleavesubmitions.Find(id);
                if (employeeleavesubmition.apstatus == "approved by line manager")
                {
                    goto back;
                }

                employeeleavesubmition.apstatus = "approved by line manager";
                var higherupchecklist = db.emprels.ToList();
                var hihigherupcheck = higherupchecklist.Find(x => x.Employee_id == employeeleavesubmition.Employee_id);
                if (!hihigherupcheck.HOD.HasValue)
                {
                    employeeleavesubmition.apstatus = "approved";
                    var leaveentry = new Leave();
                    leaveentry.Date = employeeleavesubmition.Date;
                    leaveentry.Start_leave = employeeleavesubmition.Start_leave;
                    leaveentry.End_leave = employeeleavesubmition.End_leave;
                    leaveentry.Return_leave = employeeleavesubmition.Return_leave;
                    leaveentry.half = employeeleavesubmition.half;
                    leaveentry.Employee_id = employeeleavesubmition.Employee_id;
                    leaveentry.leave_type = employeeleavesubmition.leave_type;
                    leaveentry.actualchangeddateby = DateTime.Now;
                    leaveentry.actualchangedby = "added by system after approval " + User.Identity.Name;
                    leaveentry.approved_by = User.Identity.Name;
                    leaveentry.imgpath = employeeleavesubmition.imgpath;
                    db.Leaves.Add(leaveentry);
                    db.SaveChanges();
                    employeeleavesubmition.approved_byline = "N/A";
                    employeeleavesubmition.approved_byhod = User.Identity.Name;
                    db.Entry(employeeleavesubmition).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                    return RedirectToAction("empleaveap");
                }

                employeeleavesubmition.approved_byline = User.Identity.Name;
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail("", "approved by line manager", id);
                return RedirectToAction("empleaveap");
            }

        back:;
            if (User.IsInRole("HOD") || User.IsInRole("EXTHOD"))
            {
                var employeeleavesubmition = db.employeeleavesubmitions.Find(id);
                employeeleavesubmition.apstatus = "approved";
                var leaveentry = new Leave();
                leaveentry.Date = employeeleavesubmition.Date;
                leaveentry.Start_leave = employeeleavesubmition.Start_leave;
                leaveentry.End_leave = employeeleavesubmition.End_leave;
                leaveentry.Return_leave = employeeleavesubmition.Return_leave;
                leaveentry.half = employeeleavesubmition.half;
                leaveentry.Employee_id = employeeleavesubmition.Employee_id;
                leaveentry.leave_type = employeeleavesubmition.leave_type;
                leaveentry.actualchangeddateby = DateTime.Now;
                leaveentry.actualchangedby = "added by system after approval " + User.Identity.Name;
                leaveentry.approved_by = User.Identity.Name;
                leaveentry.imgpath = employeeleavesubmition.imgpath;
                db.Leaves.Add(leaveentry);
                db.SaveChanges();
                employeeleavesubmition.approved_byline = User.Identity.Name;
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail("", "approved", id);
                return RedirectToAction("empleaveap");
            }

            if (User.IsInRole("super_admin") || User.IsInRole("slapproval"))
            {
                var employeeleavesubmition = db.employeeleavesubmitions.Find(id);
                employeeleavesubmition.apstatus = "approved";
                var leaveentry = new Leave();
                leaveentry.Date = employeeleavesubmition.Date;
                leaveentry.Start_leave = employeeleavesubmition.Start_leave;
                leaveentry.End_leave = employeeleavesubmition.End_leave;
                leaveentry.Return_leave = employeeleavesubmition.Return_leave;
                leaveentry.half = employeeleavesubmition.half;
                leaveentry.Employee_id = employeeleavesubmition.Employee_id;
                leaveentry.leave_type = employeeleavesubmition.leave_type;
                leaveentry.actualchangeddateby = DateTime.Now;
                leaveentry.actualchangedby = "added by system after approval " + User.Identity.Name;
                leaveentry.approved_by = User.Identity.Name;
                leaveentry.imgpath = employeeleavesubmition.imgpath;
                db.Leaves.Add(leaveentry);
                db.SaveChanges();
                employeeleavesubmition.approved_byline = User.Identity.Name;
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail("", "approved", id);
                return RedirectToAction("empleaveap");
            }

            return RedirectToAction("empleaveap");
        }*/



        [Authorize(Roles = "HOD,employee,Manager")]
        public ActionResult Dleavereport(string search)
        {
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var empjd = empuser.master_file;
            var relationlist = db.emprels.ToList();
            var logedinsrels = relationlist.FindAll(x => x.line_man == empjd.employee_id || x.HOD == empjd.employee_id);
            var DLR = new List<Leave>();
            var DLRtemplist = new List<Leave>();
            foreach (var emprel in logedinsrels)
            {
                var relationlist1 =
                    relationlist.FindAll(x => x.line_man == emprel.Employee_id || x.HOD == emprel.Employee_id);
                foreach (var emprel2 in relationlist1)
                {
                    var DLRtemp1 = db.Leaves.Where(x =>
                        x.Employee_id == emprel2.Employee_id && x.Start_leave.Value.Year <= DateTime.Now.Year && x.End_leave.Value.Year >= DateTime.Now.Year /*&&
                        x.leave_type == "1"*/);
                    DLRtemplist.AddRange(DLRtemp1);
                }
                var DLRtemp = db.Leaves.Where(x =>
                    x.Employee_id == emprel.Employee_id && x.Start_leave.Value.Year <= DateTime.Now.Year && x.End_leave.Value.Year >= DateTime.Now.Year /*&&
                    x.leave_type == "1"*/).ToList();
                if (DLRtemp.Count > 0)
                {
                    DLRtemplist.AddRange(DLRtemp);
                }
            }
            foreach(var empin in DLRtemplist)
            {
                if(DLR.Count == 0)
                {
                    DLR.Add(empin);
                }
                if(!DLR.Exists(x=>x.Id == empin.Id))
                {
                    DLR.Add(empin);
                }
            }

            if (!string.IsNullOrEmpty(search) && DLR.Count > 0)
            {
                if (int.TryParse(search, out var idk))
                {
                    DLR = DLR.FindAll(x => x.master_file.employee_no == idk);
                }
                else
                {

                    DLR = DLR.FindAll(x => x.master_file.employee_name.Contains(search));
                }
            }

            return View(DLR.OrderBy(x=>x.End_leave).ThenByDescending(x=>x.Date).ThenBy(x=>x.master_file.employee_no).ToList());
        }


        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        public ActionResult approve(int id)
        {
            var employeeleavesubmitionlist = db.employeeleavesubmitions.ToList();
            var employeeleavesubmition = employeeleavesubmitionlist.Find(x=>x.Id == id);
            employeeleavesubmition.dateadded = DateTime.Now;
            var leaveentry = new Leave();
            leaveentry.Date = employeeleavesubmition.Date;
            leaveentry.Start_leave = employeeleavesubmition.Start_leave;
            leaveentry.End_leave = employeeleavesubmition.End_leave;
            leaveentry.Return_leave = employeeleavesubmition.Return_leave;
            leaveentry.half = employeeleavesubmition.half;
            leaveentry.Employee_id = employeeleavesubmition.Employee_id;
            leaveentry.leave_type = employeeleavesubmition.leave_type;
            leaveentry.actualchangeddateby = DateTime.Now;
            leaveentry.imgpath = employeeleavesubmition.imgpath;
            if (User.IsInRole("super_admin") || User.IsInRole("slapproval"))
            {
                var leavedupcheck = db.Leaves.ToList();
                if (!leavedupcheck.Exists(x => x.Start_leave == leaveentry.Start_leave && x.Employee_id == leaveentry.Employee_id))
                {
                    leaveentry.actualchangedby = "added after approval " + User.Identity.Name;
                    leaveentry.approved_by = User.Identity.Name;
                    leaveentry.imgpath = employeeleavesubmition.imgpath;
                    db.Leaves.Add(leaveentry);
                    db.SaveChanges();
                    var tempdate = leaveentry.Start_leave.Value.AddDays(-1);
                    var leavelist = db.Leaves.Where(x => x.Employee_id == leaveentry.Employee_id && x.End_leave == tempdate && x.actual_return_date == null).ToList();
                    if (leavelist.Count > 0)
                    {
                        var leavevar = leavelist[0];
                        leavevar.actual_return_date = leavevar.Return_leave;
                        leavevar.actualchangedby = "system";
                        leavevar.actualchangeddateby = DateTime.Now;
                        db.Entry(leavevar).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    employeeleavesubmition.approved_byhod = User.Identity.Name;
                    employeeleavesubmition.apstatus = "approved";
                    db.Entry(employeeleavesubmition).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }
                else
                {
                    employeeleavesubmition.apstatus = "already exists";
                    db.Entry(employeeleavesubmition).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("empleaveap");
            }
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var relationlist = db.emprels.ToList();
            var empjd = empuser.master_file;
            var logedinsrels = relationlist.FindAll(x => x.line_man == empjd.employee_id || x.HOD == empjd.employee_id);
            var currentrel = logedinsrels.Find(x => x.Employee_id == employeeleavesubmition.Employee_id);
            if (currentrel.line_man == empjd.employee_id)
            {
                if (employeeleavesubmition.apstatus == "approved by line manager")
                {
                    goto back;
                }
                
                if (!currentrel.HOD.HasValue)
                {
                    var leavedupcheck = db.Leaves.ToList();
                    if (!leavedupcheck.Exists(x=>x.Start_leave == leaveentry.Start_leave && x.Employee_id == leaveentry.Employee_id))
                    {
                        leaveentry.actualchangedby = "added after approval " + User.Identity.Name;
                        leaveentry.approved_by = User.Identity.Name;
                        leaveentry.imgpath = employeeleavesubmition.imgpath;
                        db.Leaves.Add(leaveentry);
                        db.SaveChanges();
                        var tempdate = leaveentry.Start_leave.Value.AddDays(-1);
                        var leavelist = db.Leaves.Where(x => x.Employee_id == leaveentry.Employee_id && x.End_leave == tempdate && x.actual_return_date == null).ToList();
                        if (leavelist.Count > 0)
                        {
                            var leavevar = leavelist[0];
                            leavevar.actual_return_date = leavevar.Return_leave;
                            leavevar.actualchangedby = "system";
                            leavevar.actualchangeddateby = DateTime.Now;
                            db.Entry(leavevar).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        employeeleavesubmition.apstatus = "approved";
                        employeeleavesubmition.approved_byline = "N/A";
                        employeeleavesubmition.approved_byhod = User.Identity.Name;
                        db.Entry(employeeleavesubmition).State = EntityState.Modified;
                        db.SaveChanges();
                        SendMail("", "approved", id);
                    }
                    else
                    {
                        employeeleavesubmition.apstatus = "already exists";
                        db.Entry(employeeleavesubmition).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("empleaveap");
                }
                employeeleavesubmition.approved_byline = User.Identity.Name;
                employeeleavesubmition.apstatus = "approved by line manager";
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail("", "approved by line manager", id);
                return RedirectToAction("empleaveap");
            }
        back:;
            if (currentrel.HOD == empjd.employee_id )
            {
                var leavedupcheck = db.Leaves.ToList();
                if (!leavedupcheck.Exists(x => x.Start_leave == leaveentry.Start_leave && x.Employee_id == leaveentry.Employee_id))
                {
                    leaveentry.actualchangedby = "added after approval " + User.Identity.Name;
                    leaveentry.approved_by = User.Identity.Name;
                    leaveentry.imgpath = employeeleavesubmition.imgpath;
                    db.Leaves.Add(leaveentry);
                    db.SaveChanges();
                    var tempdate = leaveentry.Start_leave.Value.AddDays(-1);
                    var leavelist = db.Leaves.Where(x => x.Employee_id == leaveentry.Employee_id && x.End_leave == tempdate && x.actual_return_date == null).ToList();
                    if (leavelist.Count > 0)
                    {
                        var leavevar = leavelist[0];
                        leavevar.actual_return_date = leavevar.Return_leave;
                        leavevar.actualchangedby = "system";
                        leavevar.actualchangeddateby = DateTime.Now;
                        db.Entry(leavevar).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    employeeleavesubmition.approved_byhod = User.Identity.Name;
                    employeeleavesubmition.apstatus = "approved";
                    db.Entry(employeeleavesubmition).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("", "approved", id);
                }
                else
                {
                    employeeleavesubmition.apstatus = "already exists";
                    db.Entry(employeeleavesubmition).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("empleaveap");
            }

            return RedirectToAction("empleaveap");
        }
        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        public ActionResult reject(int id, string message)
        {
            var employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            var userempnolist = db.usernames
                .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
            var empuser = userempnolist.Find(x => x.AspNetUser.UserName == User.Identity.Name);
            var relationlist = db.emprels.ToList();
            if (User.IsInRole("super_admin") || User.IsInRole("slapproval"))
            {
                employeeleavesubmition.apstatus = "rejected by HR for " + message;
                employeeleavesubmition.approved_byhod = User.Identity.Name;
                employeeleavesubmition.approved_byline = "N/A";
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail(message, "rejected by HR for ", id);
                return RedirectToAction("empleaveap");
            }
            var empjd = empuser.master_file;
            var logedinsrels = relationlist.FindAll(x => x.line_man == empjd.employee_id || x.HOD == empjd.employee_id);
            var currentrel = logedinsrels.Find(x => x.Employee_id == employeeleavesubmition.Employee_id);
            if (currentrel.line_man == empjd.employee_id)
            {
                if (currentrel.HOD == empjd.employee_id)
                {
                    goto hodtra;
                }
                employeeleavesubmition.apstatus = "rejected by line manager for " + message;
                employeeleavesubmition.approved_byline = User.Identity.Name;
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail(message, "rejected by line manager for ", id);
                return RedirectToAction("empleaveap");
            }

            hodtra: ;
            if (currentrel.HOD == empjd.employee_id)
            {
                employeeleavesubmition.apstatus = "rejected by HOD for " + message;
                employeeleavesubmition.approved_byhod = User.Identity.Name;
                db.Entry(employeeleavesubmition).State = EntityState.Modified;
                db.SaveChanges();
                SendMail(message, "rejected by HOD for ", id);
                return RedirectToAction("empleaveap");
            }


            return RedirectToAction("empleaveap");
        }

        [Authorize(Roles = "HOD,employee,Manager,super_admin,slapproval")]
        public FileResult Download(int id)
        {
            var employeeleavesubmition = db.employeeleavesubmitions.Find(id);
            byte[] fileBytes = System.IO.File.ReadAllBytes(employeeleavesubmition.imgpath);
            string fileName = Path.GetFileName(employeeleavesubmition.imgpath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        
        public void SendMail(string msg, string action, int elsid)
        {

            var empleavelist = db.employeeleavesubmitions.ToList();
            var empleave = empleavelist.Find(x => x.Id == elsid);
            var message = new MimeMessage();
            var emprellist = db.emprels.ToList();
            var emprel = emprellist.Find(x => x.Employee_id == empleave.Employee_id);
            var userlist = db.AspNetUsers.ToList();
            var usernamelist = db.usernames.ToList();
            var contractlist = db.contracts.OrderByDescending(x => x.date_changed).ToList();
            var desig = "";
            if (contractlist.Exists(x => x.employee_no == empleave.Employee_id))
            {
                var temp = contractlist.Find(x => x.employee_no == empleave.Employee_id);
                if (!temp.designation.IsNullOrWhiteSpace())
                {
                    desig = temp.designation;
                }
            }

            var emplusersname = usernamelist.Find(x => x.employee_no == emprel.Employee_id);
            message.From.Add(new MailboxAddress("Hrworks", "leave@citiscapegroup.com"));

            if (action.Equals("submitted"))
            {
                var nextusersname = usernamelist.Find(x => x.employee_no == emprel.line_man);
                var nextuser = userlist.Find(x => x.Id == nextusersname.aspnet_uid);
                message.To.Add((new MailboxAddress(nextusersname.full_name, nextuser.Email)));
                message.Subject = "leave approval";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for   (" +
                           emplusersname.master_file.employee_no + ") " +
                           emplusersname.full_name + "-" + desig + " has been submitted for your approval" + "\n\n\n" +
                           "http://hrworks.ddns.net:6333/citiworks/employeeleavesubmitions" + "\n\n\n" +
                           "Thanks Best Regards, "
                };
            }

            if (action.Equals("submitted for HR"))
            {
                message.To.Add((new MailboxAddress("Yahya Rashid", "yrashid@citiscapegroup.com")));
                message.Subject = "leave approval";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for   (" +
                           emplusersname.master_file.employee_no + ") " +
                           emplusersname.full_name + "-" + desig + " has been submitted for your approval" + "\n\n\n" +
                           "http://hrworks.ddns.net:6333/citiworks/employeeleavesubmitions" + "\n\n\n" +
                           "Thanks Best Regards, "
                };
            }

            if (action.Contains("approved"))
            {
                if (action == "approved by line manager")
                {
                    var previoususersname = usernamelist.Find(x => x.employee_no == emprel.line_man);
                    var nextusersname = usernamelist.Find(x => x.employee_no == emprel.HOD);
                    var nextuser = userlist.Find(x => x.Id == nextusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(nextusersname.full_name, nextuser.Email)));
                    message.Subject = "leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for  (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.full_name + "-"  + desig + " has been approved by line manager " +
                               previoususersname.master_file.employee_name + " and forwarded for your approval" +
                               "\n\n\n" +
                               "http://hrworks.ddns.net:6333/citiworks/employeeleavesubmitions" + "\n\n\n" +
                               "Thanks Best Regards, "
                    };
                }
                else if (action == "approved")
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Cc.Add((new MailboxAddress("Yahya Rashid", "yrashid@citiscapegroup.com")));
                    message.Subject = "leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for  (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.full_name + "-" + desig + " has been approved" + "\n\n\n" +
                               "http://hrworks.ddns.net:6333/citiworks/employeeleavesubmitions" + "\n\n\n" +
                               "Thanks Best Regards, "
                    };
                }
            }

            if (action.Contains("rejected"))

            {
                if (action.Contains("rejected by line manager"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for  (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.full_name + "-" + desig + " has been rejected by line manager for " +
                               msg + "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }

                if (action.Contains("rejected by HOD"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for   (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.full_name + "-" + desig + " has been rejected by HOD for " + msg +
                               "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }

                if (action.Contains("rejected by HR"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "leave approval";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for  (" +
                               emplusersname.master_file.employee_no + ") " +
                               emplusersname.full_name + "-" + desig + " has been rejected by HR for " + msg +
                               "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }
            }

            /*if (message.To.Count != 0)
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("outlook.office365.com", 587, false);
                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("leave@citiscapegroup.com", "Tak98020");
                    client.Send(message);
                    client.Disconnect(true);
                }
            }*/
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