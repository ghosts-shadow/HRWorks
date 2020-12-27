using Microsoft.Ajax.Utilities;

namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using System.Web.Security;

    using HRworks.Models;

    [NoDirectAccess]
   // [Authorize(Roles = "super_admin,payrole,employee_con")]
    public class payrolesController : Controller
    {
        private const string Purpose = "equalizer";

        private readonly HREntities db = new HREntities();

        private readonly LogisticsSoftEntities db1 = new LogisticsSoftEntities();

        public static string Protect(string unprotectedText)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedText);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }

        public static bool IsBase64Encoded(string str)
        {
            if (str.Replace(" ", "").Length % 4 != 0)
            {
                return false;
            }

            try
            {
                var str1 = Convert.FromBase64String(str);
                MachineKey.Unprotect(str1, Purpose);
                return true;
            }
            catch (Exception exception)
            {
                // Handle the exception
            }

            return false;
        }
        public static string Unprotect(string protectedText)
        {
            var protectedBytes = Convert.FromBase64String(protectedText);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
            var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
            return unprotectedText;
        }

        // GET: payroles/Create
        public ActionResult Create()
        {
            this.ViewBag.con_id = new SelectList(this.db.contracts, "employee_id", "designation");
            this.ViewBag.LWOP = new SelectList(this.db.Leaves, "Id", "Reference");
            this.ViewBag.employee_no = new SelectList(this.db.master_file, "employee_id", "employee_name");
            return this.View();
        }

        // POST: payroles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(payrole payrole)
        {
            if (this.ModelState.IsValid)
            {
                payrole.totalpayable = Protect(payrole.totalpayable);
                payrole.OTRegular = Protect(payrole.OTRegular);
                payrole.OTFriday = Protect(payrole.OTFriday);
                payrole.HolidayOT = Protect(payrole.HolidayOT);
                payrole.TotalOT = Protect(payrole.TotalOT);
                payrole.NetPay = Protect(payrole.NetPay);
                payrole.TotalDedution = Protect(payrole.TotalDedution);
                this.db.payroles.Add(payrole);
                this.db.SaveChanges();
            }

            this.ViewBag.con_id = new SelectList(this.db.contracts, "employee_id", "designation", payrole.con_id);
            this.ViewBag.LWOP = new SelectList(this.db.Leaves, "Id", "Reference", payrole.LWOP);
            this.ViewBag.employee_no = new SelectList(
                this.db.master_file,
                "employee_id",
                "employee_name",
                payrole.employee_no);
            return this.View(payrole);
        }

        // GET: payroles/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var payrole = this.db.payroles.Find(id);
            if (payrole == null) return this.HttpNotFound();
            return this.View(payrole);
        }

        // POST: payroles/Delete/5
        [Authorize(Roles = "super_admin")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var payrole = this.db.payroles.Find(id);
            this.db.payroles.Remove(payrole);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        // GET: payroles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var payrole = this.db.payroles.Find(id);
            if (payrole == null) return this.HttpNotFound();
            return this.View(payrole);
        }

        // GET: payroles/Edit/5
        //[Authorize(Roles = "super_admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var payrole = this.db.payroles.Find(id);
            if (payrole == null) return this.HttpNotFound();
            this.ViewBag.con_id = new SelectList(this.db.contracts, "employee_id", "employee_id", payrole.con_id);
            this.ViewBag.Absents = new SelectList(this.db.leave_absence, "Id", "Id", payrole.Absents);
            this.ViewBag.LWOP = new SelectList(this.db.Leaves, "Id", "ID", payrole.LWOP);
            this.ViewBag.employee_no = new SelectList(
                this.db.master_file,
                "employee_id",
                "employee_name",
                payrole.employee_no);

            if (!payrole.totalpayable.Contains(" ")  && IsBase64Encoded(payrole.totalpayable))
            {
                payrole.totalpayable = Unprotect(payrole.totalpayable);
            }

            if (!payrole.OTRegular.Contains(" ")  && IsBase64Encoded(payrole.OTRegular))
            {
                payrole.OTRegular = Unprotect(payrole.OTRegular);
            }

            if(!payrole.OTFriday.Contains(" ")  && IsBase64Encoded(payrole.OTFriday))
            {
                payrole.OTFriday = Unprotect(payrole.OTFriday);
            }

            if(!payrole.OTNight.Contains(" ")  && IsBase64Encoded(payrole.OTNight))
            {
                payrole.OTNight = Unprotect(payrole.OTNight);
            }

            if(!payrole.HolidayOT.Contains(" ")  && IsBase64Encoded(payrole.HolidayOT))
            {
                payrole.HolidayOT = Unprotect(payrole.HolidayOT);
            }

            if(!payrole.TotalOT.Contains(" ")  && IsBase64Encoded(payrole.TotalOT))
            {
                payrole.TotalOT = Unprotect(payrole.TotalOT);
            }

            if (!String.IsNullOrWhiteSpace(payrole.TransportationAllowance_))
            {
                if(!payrole.TransportationAllowance_.Contains(" ")  && IsBase64Encoded(payrole.TransportationAllowance_))
                {
                    payrole.TransportationAllowance_ = Unprotect(payrole.TransportationAllowance_);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.TicketAllowance_))
            {
                if(!payrole.TicketAllowance_.Contains(" ")  && IsBase64Encoded(payrole.TicketAllowance_))
                {
                    payrole.TicketAllowance_ = Unprotect(payrole.TicketAllowance_);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.Arrears))
            {
                if(!payrole.Arrears.Contains(" ")  && IsBase64Encoded(payrole.Arrears))
                {
                    payrole.Arrears = Unprotect(payrole.Arrears);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.cashAdvances))
            {
                if(!payrole.cashAdvances.Contains(" ")  && IsBase64Encoded(payrole.cashAdvances))
                {
                    payrole.cashAdvances = Unprotect(payrole.cashAdvances);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.HouseAllow))
            {
                if(!payrole.HouseAllow.Contains(" ")  && IsBase64Encoded(payrole.HouseAllow))
                {
                    payrole.HouseAllow = Unprotect(payrole.HouseAllow);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.FoodAllow))
            {
                if(!payrole.FoodAllow.Contains(" ")  && IsBase64Encoded(payrole.FoodAllow))
                {
                    payrole.FoodAllow = Unprotect(payrole.FoodAllow);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.Timekeeping))
            {
                if(!payrole.Timekeeping.Contains(" ")  && IsBase64Encoded(payrole.Timekeeping))
                {
                    payrole.Timekeeping = Unprotect(payrole.Timekeeping);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.Communication))
            {
                if(!payrole.Communication.Contains(" ")  && IsBase64Encoded(payrole.Communication))
                {
                    payrole.Communication = Unprotect(payrole.Communication);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.TrafficFines))
            {
                if(!payrole.TrafficFines.Contains(" ")  && IsBase64Encoded(payrole.TrafficFines))
                {
                    payrole.TrafficFines = Unprotect(payrole.TrafficFines);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.others))
            {
                if(!payrole.others.Contains(" ")  && IsBase64Encoded(payrole.others))
                {
                    payrole.others = Unprotect(payrole.others);
                }
            }
            if (!String.IsNullOrWhiteSpace(payrole.TotalDedution))
            {
                if(!payrole.TotalDedution.Contains(" ")  && IsBase64Encoded(payrole.TotalDedution))
                {
                    payrole.TotalDedution = Unprotect(payrole.TotalDedution);
                }
            }

            if (!String.IsNullOrWhiteSpace(payrole.NetPay))
            {
                if(!payrole.NetPay.Contains(" ")  && IsBase64Encoded(payrole.NetPay))
                {
                    payrole.NetPay = Unprotect(payrole.NetPay);
                }
            }
            return this.View(payrole);
        }

        // POST: payroles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "super_admin")]
        public ActionResult Edit(
            payrole payrole)
        {
            if (this.ModelState.IsValid)
            {

                if(!payrole.totalpayable.Contains(" ") && IsBase64Encoded(payrole.totalpayable))
                {
                    payrole.totalpayable = Unprotect(payrole.totalpayable);                    
                }
                if(!payrole.OTRegular.Contains(" ")  && IsBase64Encoded(payrole.OTRegular))
                {
                    payrole.OTRegular = Unprotect(payrole.OTRegular);                    
                }
                if(!payrole.OTFriday.Contains(" ")  && IsBase64Encoded(payrole.OTFriday))
                {
                    payrole.OTFriday = Unprotect(payrole.OTFriday);                    
                }
                if(!payrole.OTNight.Contains(" ")  && IsBase64Encoded(payrole.OTNight))
                {
                    payrole.OTNight = Unprotect(payrole.OTNight);                    
                }
                if(!payrole.HolidayOT.Contains(" ")  && IsBase64Encoded(payrole.HolidayOT))
                {
                    payrole.HolidayOT = Unprotect(payrole.HolidayOT);                    
                }
                if(!payrole.TotalOT.Contains(" ")  && IsBase64Encoded(payrole.TotalOT))
                {
                    payrole.TotalOT = Unprotect(payrole.TotalOT);
                }

                if (payrole.NetPay != null)
                {
                    if (payrole.Leave != null)
                    {
                        if (payrole.forthemonth != null && payrole.Leave.days >= DateTime.DaysInMonth(payrole.forthemonth.Value.Year,payrole.forthemonth.Value.Month))
                        {
                            payrole.NetPay = (0).ToString();
                            payrole.NetPay = Protect(payrole.NetPay);
                        }
                        else
                        {
                            double.TryParse(payrole.totalpayable, out var a);
                            double.TryParse(payrole.TotalOT, out var b);
                            double.TryParse(payrole.TotalDedution, out var c);
                            payrole.NetPay = (a + b - c).ToString();
                            payrole.NetPay = Protect(payrole.NetPay);
                        }
                    }
                    else
                    {
                        double.TryParse(payrole.totalpayable, out var a);
                        double.TryParse(payrole.TotalOT, out var b);
                        double.TryParse(payrole.TotalDedution, out var c);
                        payrole.NetPay = (a + b - c).ToString();
                        payrole.NetPay = Protect(payrole.NetPay);
                    }

                }
                payrole.totalpayable = Protect(payrole.totalpayable);
                payrole.OTRegular = Protect(payrole.OTRegular);
                payrole.OTFriday = Protect(payrole.OTFriday);
                payrole.OTNight = Protect(payrole.OTNight);
                payrole.HolidayOT = Protect(payrole.HolidayOT);
                payrole.TotalOT = Protect(payrole.TotalOT);
                if (payrole.TransportationAllowance_ != null && !IsBase64Encoded(payrole.TransportationAllowance_))
                {
                    payrole.TransportationAllowance_ = Protect(payrole.TransportationAllowance_);
                }
                if (payrole.TicketAllowance_ != null && !IsBase64Encoded(payrole.TicketAllowance_))
                {
                    payrole.TicketAllowance_ = Protect(payrole.TicketAllowance_);
                }
                if (payrole.Arrears != null && !IsBase64Encoded(payrole.Arrears))
                {
                    payrole.Arrears = Protect(payrole.Arrears);
                }
                if (payrole.cashAdvances != null && !IsBase64Encoded(payrole.cashAdvances))
                {
                    payrole.cashAdvances = Protect(payrole.cashAdvances);
                }
                if (payrole.HouseAllow != null && !IsBase64Encoded(payrole.HouseAllow))
                {
                    payrole.HouseAllow = Protect(payrole.HouseAllow);

                }
                if (payrole.FoodAllow != null && !IsBase64Encoded(payrole.FoodAllow))
                {
                    payrole.FoodAllow = Protect(payrole.FoodAllow);

                }
                if (payrole.Timekeeping != null && !IsBase64Encoded(payrole.Timekeeping))
                {
                    payrole.Timekeeping = Protect(payrole.Timekeeping);

                }
                if (payrole.Communication != null && !IsBase64Encoded(payrole.Communication))
                {
                payrole.Communication = Protect(payrole.Communication);

                }
                if (payrole.TrafficFines != null && !IsBase64Encoded(payrole.TrafficFines))
                {
                    payrole.TrafficFines = Protect(payrole.TrafficFines);

                }
                if (payrole.TotalDedution != null && !IsBase64Encoded(payrole.TotalDedution))
                {
                    payrole.TotalDedution = Protect(payrole.TotalDedution);
                }
                if (payrole.others != null && !IsBase64Encoded(payrole.others))
                {
                    payrole.others = Protect(payrole.others);
                }
                this.db.Entry(payrole).State = EntityState.Modified;
                this.db.SaveChanges();
                return this.RedirectToAction("Index",new { month = payrole.forthemonth});
            }

            this.ViewBag.con_id = new SelectList(this.db.contracts, "employee_id", "employee_id", payrole.con_id);
            this.ViewBag.Absents = new SelectList(this.db.leave_absence, "Id", "Id", payrole.Absents);
            this.ViewBag.LWOP = new SelectList(this.db.Leaves, "Id", "ID", payrole.LWOP);
            this.ViewBag.employee_no = new SelectList(
                this.db.master_file,
                "employee_id",
                "employee_name",
                payrole.employee_no);
            return this.View(payrole);
        }

        public List<int> GetAll(DateTime date)
        {
            var month = date.Month;
            var lastDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDay = DateTime.DaysInMonth(date.Year, date.Month);
            var array = new List<int>(); // dd/mm/yy
            var count = -1;
            for (var i = 1; i <= lastDay; i++)
            {
                var temp = new DateTime(date.Year, month, i);
                var day = temp.DayOfWeek;
                if (day == DayOfWeek.Friday)
                {
                    count++;
                    var dd = temp.Day;
                    array.Add(dd);
                }
            }

            return array;
        }

        public List<int> GetAllholi(DateTime date)
        {
            var holilist = this.db1.Holidays
                .Where(x => x.Date.Value.Month == date.Month && x.Date.Value.Year == date.Year).ToList();
            var array = new List<int>();
            foreach (var ho in holilist) array.Add(ho.Date.Value.Day);

            return array;
        }

        // GET: payroles
        public ActionResult Index(DateTime? month ,string save)
        {
            
            var payroles = this.db.payroles.Include(p => p.contract).Include(p => p.Leave).Include(p => p.master_file);
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            var paylist = new List<payrole>();
            var lab = this.db1.LabourMasters.ToList();
            var mts = new List<MainTimeSheet>();
            var att1 = new List<Attendance>();
            var att = new List<Attendance>();

            if (month.HasValue)
            {
                ViewBag.payday = month;
                var paylisteisting = this.db.payroles.ToList();
                mts = this.db1.MainTimeSheets.Where(
                    x => x.TMonth.Month == month.Value.Month && x.TMonth.Year == month.Value.Year
                                                             && x.ManPowerSupplier == 1).ToList();
                var atlist = this.db1.Attendances.ToList();
                var endmo = new DateTime(
                    month.Value.Year,
                    month.Value.Month,
                    DateTime.DaysInMonth(month.Value.Year, month.Value.Month));
                var Msum = this.db1.MainTimeSheets.Where(
                        y => y.ManPowerSupplier == 1 && y.TMonth.Month == month.Value.Month && y.TMonth.Year == month
                                 .Value.Year)
                    .ToList();
                var cony = 0;
                var passexel = new List<Attendance>();
                foreach (var sum in Msum)
                {
                   var listat = this.db1.Attendances.Where(x => x.SubMain.Equals(sum.ID)).OrderByDescending(x => x.ID)
                        .ToList();

                    foreach (var VA in listat.OrderBy(x => x.ID))
                    {
                        if (!passexel.Exists(
                                x => x.MainTimeSheet.ProjectList.ID == VA.MainTimeSheet.ProjectList.ID
                                     && x.EmpID == VA.EmpID))
                        {
                            passexel.Add(VA);
                        }
                        else
                        {
                            cony++;
                        }
                    }
                }

                att = passexel;
                foreach (var masterFile in afinallist)
                {
                    if (paylisteisting.Exists(
                        x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1)
                             && x.employee_no == masterFile.employee_id))
                    {
                        var payr = paylisteisting.Find(
                            x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1)
                                 && x.employee_no == masterFile.employee_id);
                        if (payr.save == true)
                        {
                            goto sav;
                        }
////                        var leave1 = this.db.Leaves.Where(
////                            x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
////                                                                         && x.Start_leave >= payr.forthemonth
////                                                                         && x.Start_leave <= endmo
////                                                                         && x.End_leave >= payr.forthemonth
////                                                                         && x.End_leave <= endmo).ToList();
//                        var leave1 = this.db.Leaves.Where(
//                            x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
//                                                                         && x.Start_leave <= payr.forthemonth
//                                                                         && x.End_leave >= payr.forthemonth).ToList();
////                        var leave2 = this.db.Leaves.Where(
////                            x => x.Employee_id == masterFile.employee_id && x.Start_leave >= payr.forthemonth
////                                                                         && x.Start_leave <= endmo
////                                                                         && x.End_leave >= payr.forthemonth
////                                                                         && x.End_leave <= endmo).ToList();
//                        var leave2 = this.db.Leaves.Where(
//                            x => x.Employee_id == masterFile.employee_id && x.Start_leave <= payr.forthemonth
//                                                                         && x.End_leave >= payr.forthemonth).ToList();

                        var leavedate1 = payr.forthemonth;
                        var leavedateend = new DateTime(payr.forthemonth.Value.Year,payr.forthemonth.Value.Month,DateTime.DaysInMonth(payr.forthemonth.Value.Year,payr.forthemonth.Value.Month));
                        leavedateend = leavedateend.AddDays(1);
                        var leave1 = new List<Leave>();
                        var leave2 = new List<Leave>();
                        do
                        {
                            var leave1_1 = this.db.Leaves.Where(
                                x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
                                                                             && x.Start_leave <= leavedate1
                                                                             && x.End_leave >= leavedate1).ToList();
                            var leave2_1 = this.db.Leaves.Where(
                                x => x.Employee_id == masterFile.employee_id && x.Start_leave <= leavedate1
                                                                             && x.End_leave >= leavedate1).ToList();
                            foreach (var leaf in leave1_1)
                            {
                                if (!leave1.Exists(x=>x.Id ==leaf.Id))
                                {
                                    leave1.Add(leaf);
                                }
                            }
                            foreach (var leaf in leave2_1)
                            {
                                if (!leave2.Exists(x=>x.Id ==leaf.Id))
                                {
                                    leave2.Add(leaf);
                                }
                            }

                            leavedate1=leavedate1.Value.AddDays(1);
                        } while (leavedate1 != leavedateend);
                        var lowp = 0;
                        foreach (var leaf in leave1)
                        {
                            var dif = leaf.End_leave - leaf.Start_leave;
                            var date1 = leaf.Start_leave;
                            var date2 = leaf.End_leave;
                            while (date1 != date2)
                            {
                                if (date1.Value.Month == payr.forthemonth.Value.Month)
                                {
                                    lowp += 1;
                                }
                                date1 = date1.Value.AddDays(1);
                            }

                            var daysinm = DateTime.DaysInMonth(payr.forthemonth.Value.Year,
                                payr.forthemonth.Value.Month);
                            if (lowp != daysinm)
                            {
                                lowp += 1;
                            }
                            //lowp += dif.Value.Days + 1;
                        }

                        var abslist1 = this.db.leave_absence.Where(
                                x => x.month.Value.Month == month.Value.Month && x.month.Value.Year == month.Value.Year
                                                                              && x.Employee_id
                                                                              == masterFile.employee_id)
                            .ToList();
                        var absd = 0;
                        foreach (var leaf in abslist1)
                        {
                            var dif = leaf.tod - leaf.fromd;
                            absd += dif.Value.Days + 1;
                        }

                        var lab1 = lab.Find(x => x.EMPNO == masterFile.employee_no);
                        var aqt = 0l;
                        var aqf = 0l;
                        var aqh = 0l;
                        if (lab1 == null) goto tos1;
                        var attd = att.FindAll(x => x.EmpID == lab1.ID).ToList();
                        var fdaylist = this.GetAll(month.Value);
                        var hlistday = this.GetAllholi(month.Value);
                        foreach (var aq in attd)
                        {
                            var aft1 = 0d;
                            var x = 0l;
                            var x1 = 0l;
                            if (IsBase64Encoded(payr.contract.FOT))
                            {
                                double.TryParse(Unprotect(payr.contract.FOT), out aft1);
                            }

                            var leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                            if (aft1 != 0)
                            {
                                var y = 0l;
                                leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) && !hlistday.Exists(f => f.Equals(1)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    
                                    long.TryParse(aq.C1, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;

                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) && !hlistday.Exists(f => f.Equals(2)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C2, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) && !hlistday.Exists(f => f.Equals(3)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C3, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) && !hlistday.Exists(f => f.Equals(4)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C4, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) && !hlistday.Exists(f => f.Equals(5)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C5, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) && !hlistday.Exists(f => f.Equals(6)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C6, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) && !hlistday.Exists(f => f.Equals(7)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C7, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) && !hlistday.Exists(f => f.Equals(8)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C8, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) && !hlistday.Exists(f => f.Equals(9)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C9, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) && !hlistday.Exists(f => f.Equals(10)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C10, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) && !hlistday.Exists(f => f.Equals(11)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C11, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) && !hlistday.Exists(f => f.Equals(12)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C12, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) && !hlistday.Exists(f => f.Equals(13)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C13, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) && !hlistday.Exists(f => f.Equals(14)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C14, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) && !hlistday.Exists(f => f.Equals(15)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C15, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) && !hlistday.Exists(f => f.Equals(16)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C16, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) && !hlistday.Exists(f => f.Equals(17)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C17, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) && !hlistday.Exists(f => f.Equals(18)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C18, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) && !hlistday.Exists(f => f.Equals(19)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C19, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) && !hlistday.Exists(f => f.Equals(20)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C20, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) && !hlistday.Exists(f => f.Equals(21)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C21, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) && !hlistday.Exists(f => f.Equals(22)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C22, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) && !hlistday.Exists(f => f.Equals(23)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C23, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) && !hlistday.Exists(f => f.Equals(24)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C24, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) && !hlistday.Exists(f => f.Equals(25)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C25, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) && !hlistday.Exists(f => f.Equals(26)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C26, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) && !hlistday.Exists(f => f.Equals(27)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C27, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) && !hlistday.Exists(f => f.Equals(28)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C28, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }

                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) && !hlistday.Exists(f => f.Equals(29)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C29, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) && !hlistday.Exists(f => f.Equals(30)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C30, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) && !hlistday.Exists(f => f.Equals(31)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C31, out y);
                                    if (y > 9)
                                    {
                                        x += y - 9;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                            }
                            else if (aft1 == 0)
                            {
                                leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                var y = 0l;
                                if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) && !hlistday.Exists(f => f.Equals(1)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C1, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) && !hlistday.Exists(f => f.Equals(2)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C2, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) && !hlistday.Exists(f => f.Equals(3)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C3, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) && !hlistday.Exists(f => f.Equals(4)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C4, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) && !hlistday.Exists(f => f.Equals(5)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C5, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) && !hlistday.Exists(f => f.Equals(6)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C6, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) && !hlistday.Exists(f => f.Equals(7)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C7, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) && !hlistday.Exists(f => f.Equals(8)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C8, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) && !hlistday.Exists(f => f.Equals(9)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C9, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) && !hlistday.Exists(f => f.Equals(10)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C10, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) && !hlistday.Exists(f => f.Equals(11)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C11, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) && !hlistday.Exists(f => f.Equals(12)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C12, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) && !hlistday.Exists(f => f.Equals(13)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C13, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) && !hlistday.Exists(f => f.Equals(14)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C14, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) && !hlistday.Exists(f => f.Equals(15)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C15, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) && !hlistday.Exists(f => f.Equals(16)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C16, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) && !hlistday.Exists(f => f.Equals(17)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C17, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) && !hlistday.Exists(f => f.Equals(18)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C18, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) && !hlistday.Exists(f => f.Equals(19)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C19, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) && !hlistday.Exists(f => f.Equals(20)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C20, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);

                                if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) && !hlistday.Exists(f => f.Equals(21)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C21, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) && !hlistday.Exists(f => f.Equals(22)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C22, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) && !hlistday.Exists(f => f.Equals(23)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C23, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) && !hlistday.Exists(f => f.Equals(24)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C24, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) && !hlistday.Exists(f => f.Equals(25)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C25, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) && !hlistday.Exists(f => f.Equals(26)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C26, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) && !hlistday.Exists(f => f.Equals(27)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C27, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) && !hlistday.Exists(f => f.Equals(28)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C28, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) && !hlistday.Exists(f => f.Equals(29)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C29, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) && !hlistday.Exists(f => f.Equals(30)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C30, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) && !hlistday.Exists(f => f.Equals(31)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C31, out y);
                                    if (y > 8)
                                    {
                                        x += y - 8;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                            }

                            leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                            var y1 = 0l;x1 = 0l;
                            if (aq.C1 != null && fdaylist.Exists(g => g.Equals(1)) && !hlistday.Exists(f => f.Equals(1)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C1, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C2 != null && fdaylist.Exists(g => g.Equals(2)) && !hlistday.Exists(f => f.Equals(2)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C2, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C3 != null && fdaylist.Exists(g => g.Equals(3)) && !hlistday.Exists(f => f.Equals(3)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C3, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C4 != null && fdaylist.Exists(g => g.Equals(4)) && !hlistday.Exists(f => f.Equals(4)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C4, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C5 != null && fdaylist.Exists(g => g.Equals(5)) && !hlistday.Exists(f => f.Equals(5)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C5, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C6 != null && fdaylist.Exists(g => g.Equals(6)) && !hlistday.Exists(f => f.Equals(6)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C6, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C7 != null && fdaylist.Exists(g => g.Equals(7)) && !hlistday.Exists(f => f.Equals(7)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C7, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C8 != null && fdaylist.Exists(g => g.Equals(8)) && !hlistday.Exists(f => f.Equals(8)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C8, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C9 != null && fdaylist.Exists(g => g.Equals(9)) && !hlistday.Exists(f => f.Equals(9)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C9, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C10 != null && fdaylist.Exists(g => g.Equals(10)) && !hlistday.Exists(f => f.Equals(10)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C10, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C11 != null && fdaylist.Exists(g => g.Equals(11)) && !hlistday.Exists(f => f.Equals(11)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C11, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C12 != null && fdaylist.Exists(g => g.Equals(12)) && !hlistday.Exists(f => f.Equals(12)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C12, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C13 != null && fdaylist.Exists(g => g.Equals(13)) && !hlistday.Exists(f => f.Equals(13)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C13, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C14 != null && fdaylist.Exists(g => g.Equals(14)) && !hlistday.Exists(f => f.Equals(14)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C14, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C15 != null && fdaylist.Exists(g => g.Equals(15)) && !hlistday.Exists(f => f.Equals(15)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C15, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C16 != null && fdaylist.Exists(g => g.Equals(16)) && !hlistday.Exists(f => f.Equals(16)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C16, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C17 != null && fdaylist.Exists(g => g.Equals(17)) && !hlistday.Exists(f => f.Equals(17)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C17, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C18 != null && fdaylist.Exists(g => g.Equals(18)) && !hlistday.Exists(f => f.Equals(18)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C18, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C19 != null && fdaylist.Exists(g => g.Equals(19)) && !hlistday.Exists(f => f.Equals(19)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C19, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C20 != null && fdaylist.Exists(g => g.Equals(20)) && !hlistday.Exists(f => f.Equals(20)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C20, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);

                            if (aq.C21 != null && fdaylist.Exists(g => g.Equals(21)) && !hlistday.Exists(f => f.Equals(21)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C21, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C22 != null && fdaylist.Exists(g => g.Equals(22)) && !hlistday.Exists(f => f.Equals(22)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C22, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C23 != null && fdaylist.Exists(g => g.Equals(23)) && !hlistday.Exists(f => f.Equals(23)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C23, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C24 != null && fdaylist.Exists(g => g.Equals(24)) && !hlistday.Exists(f => f.Equals(24)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C24, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C25 != null && fdaylist.Exists(g => g.Equals(25)) && !hlistday.Exists(f => f.Equals(25)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C25, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C26 != null && fdaylist.Exists(g => g.Equals(26)) && !hlistday.Exists(f => f.Equals(26)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C26, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C27 != null && fdaylist.Exists(g => g.Equals(27)) && !hlistday.Exists(f => f.Equals(27)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C27, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C28 != null && fdaylist.Exists(g => g.Equals(28)) && !hlistday.Exists(f => f.Equals(28)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C28, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C29 != null && fdaylist.Exists(g => g.Equals(29)) && !hlistday.Exists(f => f.Equals(29)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C29, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C30 != null && fdaylist.Exists(g => g.Equals(30)) && !hlistday.Exists(f => f.Equals(30)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C30, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C31 != null && fdaylist.Exists(g => g.Equals(31)) && !hlistday.Exists(f => f.Equals(31)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C31, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }
                            leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                            aqf += x1;
                            y1 = 0l;x1 = 0l;
                            if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) && hlistday.Exists(f => f.Equals(1)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C1, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) && hlistday.Exists(f => f.Equals(2)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C2, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) && hlistday.Exists(f => f.Equals(3)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C3, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) && hlistday.Exists(f => f.Equals(4)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C4, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) && hlistday.Exists(f => f.Equals(5)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C5, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) && hlistday.Exists(f => f.Equals(6)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C6, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) && hlistday.Exists(f => f.Equals(7)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C7, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) && hlistday.Exists(f => f.Equals(8)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C8, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) && hlistday.Exists(f => f.Equals(9)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C9, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) && hlistday.Exists(f => f.Equals(10)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C10, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) && hlistday.Exists(f => f.Equals(11)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C11, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) && hlistday.Exists(f => f.Equals(12)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C12, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) && hlistday.Exists(f => f.Equals(13)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C13, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) && hlistday.Exists(f => f.Equals(14)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C14, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) && hlistday.Exists(f => f.Equals(15)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C15, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) && hlistday.Exists(f => f.Equals(16)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C16, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) && hlistday.Exists(f => f.Equals(17)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C17, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) && hlistday.Exists(f => f.Equals(18)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C18, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) && hlistday.Exists(f => f.Equals(19)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C19, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) && hlistday.Exists(f => f.Equals(20)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C20, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);

                            if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) && hlistday.Exists(f => f.Equals(21)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C21, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) && hlistday.Exists(f => f.Equals(22)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C22, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) && hlistday.Exists(f => f.Equals(23)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C23, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) && hlistday.Exists(f => f.Equals(24)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C24, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) && hlistday.Exists(f => f.Equals(25)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C25, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) && hlistday.Exists(f => f.Equals(26)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C26, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) && hlistday.Exists(f => f.Equals(27)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C27, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) && hlistday.Exists(f => f.Equals(28)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C28, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) && hlistday.Exists(f => f.Equals(29)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C29, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) && hlistday.Exists(f => f.Equals(30)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C30, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) && hlistday.Exists(f => f.Equals(31)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                            {
                                long.TryParse(aq.C31, out y1);
                                
                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            aqh += x1;
                            aqt += x;
                        }

                        tos1:
                        payr.OTRegular = aqt.ToString();
                        payr.OTFriday = aqf.ToString();
                        payr.HolidayOT = aqh.ToString();
                        var aft = 0d;
                        var ant = 0d;
                        var m = 0d;
                        if (payr.contract.FOT != null && !payr.contract.FOT.Contains(" ")  && IsBase64Encoded(payr.contract.FOT))
                        {
                            double.TryParse(Unprotect(payr.contract.FOT), out  aft);
                        }
                        if (payr.OTNight != null && !payr.OTNight.Contains(" ")  && IsBase64Encoded(payr.OTNight) )
                        {
                            double.TryParse(Unprotect(payr.OTNight), out ant);
                        }
                        else
                        {
                            payr.OTNight = "0";
                        }

                        var comrat = 0d;
                        var totded = 0d;
                        if (payr.cashAdvances != null )
                        {
                             double.TryParse(Unprotect(payr.cashAdvances),out comrat);
                             totded += comrat;
                        }
                        if (payr.HouseAllow != null )
                        {
                             double.TryParse(Unprotect(payr.HouseAllow),out comrat);
                             totded += comrat;
                        }
                        if (payr.TrafficFines != null )
                        {
                             double.TryParse(Unprotect(payr.TrafficFines),out comrat);
                             totded += comrat;
                        }
                        if (payr.TransportationAllowance_ != null )
                        {
                             double.TryParse(Unprotect(payr.TransportationAllowance_),out comrat);
                             totded += comrat;
                        }
                        if (payr.FoodAllow != null )
                        {
                             double.TryParse(Unprotect(payr.FoodAllow),out comrat);
                             totded += comrat;
                        }
                        if (payr.Timekeeping != null )
                        {
                             double.TryParse(Unprotect(payr.Timekeeping),out comrat);
                             totded += comrat;
                        }
                        if (payr.Communication != null )
                        {
                             double.TryParse(Unprotect(payr.Communication),out comrat);
                             totded += comrat;
                        }
                        if (payr.others != null )
                        {
                             double.TryParse(Unprotect(payr.others),out comrat);
                             totded += comrat;
                        }
                        if (payr.TrafficFines != null )
                        {
                             double.TryParse(Unprotect(payr.TrafficFines),out comrat);
                             totded += comrat;
                        }

                        if (payr.leave_absence != null)
                        {
                            payr.leave_absence.absence = absd;
                        }

                        if (payr.Leave != null)
                        {
                            payr.Leave.days = lowp;
                            double.TryParse(Unprotect(payr.contract.salary_details), out var gross);
                            var TLWOP = (absd + payr.Leave.days) * (gross * 12 / 365);
                            if (TLWOP != null)
                            {
                                totded +=  TLWOP.Value;
                            }
                        }
                        payr.TotalDedution = totded.ToString();

                        var conlist = this.db.contracts.ToList();
                        var con = conlist.Find(c1 => c1.employee_no == masterFile.employee_id);
                        double.TryParse(Unprotect(con.basic), out var bac);
                        var basperh = ((bac * 12) / 365) / 8;var leave21 = leave2.FindAll(
                            x => x.leave_type == "1").ToList();
                        var al = 0;
                        foreach (var leaf in leave21)
                        {
                            //var dif = leaf.End_leave - leaf.Start_leave;
                            //al += dif.Value.Days + 1;
                            var date1 = leaf.Start_leave;
                            var date2 = leaf.End_leave;
                            while (date1 != date2)
                            {
                                if (date1.Value.Month == payr.forthemonth.Value.Month)
                                {
                                    al += 1;
                                }
                                date1 = date1.Value.AddDays(1);
                            }
                            al += 1;
                        }
                        var fotded = (aft * 12 / 365) * (absd + lowp +  al);
                        if ((absd + lowp +  al) != 0 )
                        {
                            payr.Fot = (((aft * 12 / 365) * (DateTime.DaysInMonth(payr.forthemonth.Value.Year,payr.forthemonth.Value.Month) - lowp - al - absd))).ToString();
                        }
                        else
                        {
                            payr.Fot = aft.ToString();
                        }

                        double.TryParse(payr.Fot, out var ffot);
                        payr.TotalOT =
                            ((aqf * 1.5 * basperh) + (aqh * 2.5 * basperh) + (aqt * 1.25 * basperh) + ant + ffot)
                            .ToString();
                        double.TryParse(Unprotect(con.salary_details), out var sal);
                        var tac = 0d;
                        var arr = 0d;
                        if (payr.TicketAllowance_ != null && IsBase64Encoded(payr.TicketAllowance_))
                        {
                            double.TryParse(Unprotect(payr.TicketAllowance_), out tac);
                        }

                        if (payr.Arrears != null && IsBase64Encoded(payr.Arrears))
                        {
                            double.TryParse(Unprotect(payr.Arrears), out arr);
                        }
                        payr.totalpayable = (sal + tac + arr).ToString();
                        double.TryParse(payr.totalpayable, out var a);
                        double.TryParse(payr.TotalOT, out var b);
                        double.TryParse(payr.TotalDedution, out var c);
                        var endday = new DateTime( payr.forthemonth.Value.Year,payr.forthemonth.Value.Month,DateTime.DaysInMonth(payr.forthemonth.Value.Year,payr.forthemonth.Value.Month));
                        var stday = new DateTime( payr.forthemonth.Value.Year,payr.forthemonth.Value.Month,1);
                        
                        
                        
                        if ((absd + lowp) >= DateTime.DaysInMonth(payr.forthemonth.Value.Year,payr.forthemonth.Value.Month))
                        {
                            payr.NetPay = 0.ToString();
                        }
                        else
                        {
                            payr.NetPay = (a + b - c - fotded).ToString();
                        }

                        if (save.IsNullOrWhiteSpace())
                        {
                            payr.save = false;
                        }
                        else
                        {
                            payr.save = true;
                        }
                        Edit(payr);
                        paylist.Add(payr);
                        sav: ;
                        if (!save.IsNullOrWhiteSpace())
                        {
                            var paysavedlist = db.payrollsaveds.ToList();
                            if (paysavedlist.Exists(x=>x.forthemonth == payr.forthemonth && x.employee_no == payr.master_file.employee_no))
                            {
                                goto save_end;
                            }
                            var paysave = new payrollsaved();
                            double.TryParse(payrolesController.Unprotect(payr.HolidayOT), out var b1);
                            var bas = payrolesController.Unprotect(payr.contract.basic);
                            double.TryParse(bas, out var bas1);
                            var basperh1 = ((bas1 * 12) / 365) / 8;
                            var bdays = b1;
                            b1 = b1 * 2.5 * basperh1;
                            double.TryParse(payrolesController.Unprotect(payr.OTFriday), out var c1);
                            var cdays1 = c1;
                            c1 = c1 * 1.5 * basperh1;
                            double.TryParse(payrolesController.Unprotect(payr.OTRegular), out var a1);
                            var adays1 = a1;
                            a1 = a1 * 1.25 * basperh1;
                            paysave.employee_no = payr.master_file.employee_no;
                            paysave.employee_name = payr.master_file.employee_name;
                            paysave.Basic = payr.contract.basic;
                            paysave.Gross = payr.contract.salary_details;
                            paysave.TicketAllowance_ = payr.TicketAllowance_;
                            paysave.Arrears = payr.Arrears;
                            paysave.totalpayable = payr.totalpayable;
                            paysave.OTRegular = payr.OTRegular;
                            paysave.OTRegularamt = a1.ToString();
                            paysave.OTFriday = payr.OTFriday;
                            paysave.OTFridayamt = c1.ToString();
                            paysave.OTNight = payr.OTNight;
                            paysave.HolidayOT = payr.HolidayOT;
                            paysave.HolidayOTamt = b1.ToString();
                            paysave.Fot = payr.contract.FOT;
                            paysave.TotalOT = payr.TotalOT;
                            paysave.cashAdvances = payr.cashAdvances;
                            paysave.HouseAllow = payr.HouseAllow;
                            paysave.TransportationAllowance_ = payr.TransportationAllowance_;
                            paysave.FoodAllow = payr.FoodAllow;
                            paysave.Timekeeping = payr.Timekeeping;
                            paysave.Communication = payr.Communication;
                            paysave.TrafficFines = payr.TrafficFines;
                            if (payr.leave_absence != null)
                            {
                                paysave.Absents = (int?) payr.leave_absence.absence;
                            }
                            else
                            {
                                paysave.Absents = 0;
                            }

                            if (payr.Leave != null)
                            {
                                paysave.LWOP = payr.Leave.days;
                            }
                            else
                            {
                                paysave.LWOP = 0;
                            }

                            double.TryParse(Unprotect(payr.contract.salary_details), out var gross);
                            var TLWOP = (paysave.Absents + paysave.LWOP) * (gross * 12 / 365);
                            paysave.TotalLWOP = TLWOP.ToString();
                            paysave.others = payr.others;
                            paysave.TotalDedution = payr.TotalDedution;
                            paysave.NetPay = payr.NetPay;
                            paysave.remarks = payr.remarks;
                            paysave.forthemonth = payr.forthemonth;
                            db.payrollsaveds.Add(paysave);
                            db.SaveChanges();
                            save_end: ;
                        }
                    }
                    else
                    {
                        var payr = new payrole();
                        payr.save = false;
                        payr.master_file = masterFile;
                        payr.employee_no = masterFile.employee_id;
                        payr.forthemonth = new DateTime(month.Value.Year, month.Value.Month, 1);
                        if (masterFile.contracts.Count != 0)
                        {
                            var conlist = this.db.contracts.ToList();
                            var alist1 = this.db.contracts.OrderByDescending(e => e.date_changed).ToList();
                            var afinallist1 = new List<contract>();
                            foreach (var file in alist1)
                            {
                                if (afinallist1.Count == 0) afinallist1.Add(file);

                                if (!afinallist1.Exists(x => x.employee_no == file.employee_no)) afinallist1.Add(file);
                            }

                            conlist = afinallist1;
                            payr.contract = conlist.Find(c => c.employee_no == masterFile.employee_id);
                            var con = conlist.Find(c => c.employee_no == masterFile.employee_id);
                            
//                        var leave1 = this.db.Leaves.Where(
//                            x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
//                                                                         && x.Start_leave >= payr.forthemonth
//                                                                         && x.Start_leave <= endmo
//                                                                         && x.End_leave >= payr.forthemonth
//                                                                         && x.End_leave <= endmo).ToList();
                            var leavedate1 = payr.forthemonth;
                            var leavedateend = new DateTime(payr.forthemonth.Value.Year,payr.forthemonth.Value.Month,DateTime.DaysInMonth(payr.forthemonth.Value.Year,payr.forthemonth.Value.Month));
                            leavedateend = leavedateend.AddDays(1);
                            var leave1 = new List<Leave>();
                            var leave2 = new List<Leave>();
                            do
                            {
                            var leave1_1 = this.db.Leaves.Where(
                            x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
                                                                         && x.Start_leave <= leavedate1
                                                                         && x.End_leave >= leavedate1).ToList();
                            var leave2_1 = this.db.Leaves.Where(
                            x => x.Employee_id == masterFile.employee_id && x.Start_leave <= leavedate1
                                                                         && x.End_leave >= leavedate1).ToList();
                            foreach (var leaf in leave1_1)
                            {
                                if (!leave1.Exists(x=>x.Id ==leaf.Id))
                                {
                                    leave1.Add(leaf);
                                }
                            }
                            foreach (var leaf in leave2_1)
                            {
                                if (!leave2.Exists(x=>x.Id ==leaf.Id))
                                {
                                    leave2.Add(leaf);
                                }
                            }
                            
                                leavedate1=leavedate1.Value.AddDays(1);
                            } while (leavedate1 < leavedateend);
//                        var leave2 = this.db.Leaves.Where(
//                            x => x.Employee_id == masterFile.employee_id && x.Start_leave >= payr.forthemonth
//                                                                         && x.Start_leave <= endmo
//                                                                         && x.End_leave >= payr.forthemonth
//                                                                         && x.End_leave <= endmo).ToList();
                        var lowp = 0;
                        foreach (var leaf in leave1)
                        {
                            var dif = leaf.End_leave - leaf.Start_leave;
                            var date1 = leaf.Start_leave;
                            var date2 = leaf.End_leave;
                            while (date1 != date2)
                            {
                                if (date1.Value.Month == payr.forthemonth.Value.Month)
                                {
                                    lowp += 1;
                                }
                                date1 = date1.Value.AddDays(1);
                            }
                            var daysinm = DateTime.DaysInMonth(payr.forthemonth.Value.Year,
                                payr.forthemonth.Value.Month);
                            if (lowp != daysinm)
                            {
                                lowp += 1;
                            }
                            //lowp += dif.Value.Days + 1;
                        }
                            var abslist1 = this.db.leave_absence.Where(
                                x => x.month.Value.Month == month.Value.Month && x.month.Value.Year == month.Value.Year
                                                                              && x.Employee_id
                                                                              == masterFile.employee_id).ToList();
                            var absd = 0;
                            foreach (var leaf in abslist1)
                            {
                                var dif = leaf.tod - leaf.fromd;
                                absd += dif.Value.Days + 1;
                            }

                            var lab1 = lab.Find(x => x.EMPNO == masterFile.employee_no);
                            var aqt = 0l;
                            var aqf = 0l;
                            var aqh = 0l;
                            if (lab1 == null) goto tos;
                            var attd = att.FindAll(x => x.EmpID == lab1.ID).ToList();
                            var fdaylist = this.GetAll(month.Value);
                            var hlistday = this.GetAllholi(month.Value);
                            foreach (var aq in attd)
                            {
                                var aft1 = 0d;
                                var x = 0l;
                                var x1 = 0l;
                                if (IsBase64Encoded(payr.contract.FOT))
                                {
                                    double.TryParse(Unprotect(payr.contract.FOT), out aft1);
                                }

                                var leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                if (aft1 != 0)
                                {
                                    var y = 0l;
                                    leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                    if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) && !hlistday.Exists(f => f.Equals(1)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C1, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;

                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) && !hlistday.Exists(f => f.Equals(2)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C2, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) && !hlistday.Exists(f => f.Equals(3)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C3, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) && !hlistday.Exists(f => f.Equals(4)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C4, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) && !hlistday.Exists(f => f.Equals(5)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C5, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) && !hlistday.Exists(f => f.Equals(6)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C6, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) && !hlistday.Exists(f => f.Equals(7)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C7, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) && !hlistday.Exists(f => f.Equals(8)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C8, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) && !hlistday.Exists(f => f.Equals(9)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C9, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) && !hlistday.Exists(f => f.Equals(10)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C10, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) && !hlistday.Exists(f => f.Equals(11)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C11, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) && !hlistday.Exists(f => f.Equals(12)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C12, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) && !hlistday.Exists(f => f.Equals(13)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C13, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) && !hlistday.Exists(f => f.Equals(14)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C14, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) && !hlistday.Exists(f => f.Equals(15)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C15, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) && !hlistday.Exists(f => f.Equals(16)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C16, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) && !hlistday.Exists(f => f.Equals(17)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C17, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) && !hlistday.Exists(f => f.Equals(18)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C18, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) && !hlistday.Exists(f => f.Equals(19)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C19, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) && !hlistday.Exists(f => f.Equals(20)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C20, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) && !hlistday.Exists(f => f.Equals(21)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C21, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) && !hlistday.Exists(f => f.Equals(22)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C22, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) && !hlistday.Exists(f => f.Equals(23)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C23, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) && !hlistday.Exists(f => f.Equals(24)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C24, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) && !hlistday.Exists(f => f.Equals(25)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C25, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) && !hlistday.Exists(f => f.Equals(26)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C26, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) && !hlistday.Exists(f => f.Equals(27)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C27, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) && !hlistday.Exists(f => f.Equals(28)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C28, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }

                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) && !hlistday.Exists(f => f.Equals(29)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C29, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) && !hlistday.Exists(f => f.Equals(30)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C30, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) && !hlistday.Exists(f => f.Equals(31)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C31, out y);
                                        if (y > 9)
                                        {
                                            x += y - 9;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                }
                                else if (aft1 == 0)
                                {
                                    leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                    var y = 0l;
                                    if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) && !hlistday.Exists(f => f.Equals(1)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C1, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) && !hlistday.Exists(f => f.Equals(2)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C2, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) && !hlistday.Exists(f => f.Equals(3)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C3, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) && !hlistday.Exists(f => f.Equals(4)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C4, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) && !hlistday.Exists(f => f.Equals(5)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C5, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) && !hlistday.Exists(f => f.Equals(6)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C6, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) && !hlistday.Exists(f => f.Equals(7)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C7, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) && !hlistday.Exists(f => f.Equals(8)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C8, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) && !hlistday.Exists(f => f.Equals(9)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C9, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) && !hlistday.Exists(f => f.Equals(10)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C10, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) && !hlistday.Exists(f => f.Equals(11)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C11, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) && !hlistday.Exists(f => f.Equals(12)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C12, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) && !hlistday.Exists(f => f.Equals(13)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C13, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) && !hlistday.Exists(f => f.Equals(14)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C14, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) && !hlistday.Exists(f => f.Equals(15)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C15, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) && !hlistday.Exists(f => f.Equals(16)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C16, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) && !hlistday.Exists(f => f.Equals(17)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C17, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) && !hlistday.Exists(f => f.Equals(18)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C18, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) && !hlistday.Exists(f => f.Equals(19)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C19, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) && !hlistday.Exists(f => f.Equals(20)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C20, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);

                                    if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) && !hlistday.Exists(f => f.Equals(21)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C21, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) && !hlistday.Exists(f => f.Equals(22)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C22, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) && !hlistday.Exists(f => f.Equals(23)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C23, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) && !hlistday.Exists(f => f.Equals(24)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C24, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) && !hlistday.Exists(f => f.Equals(25)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C25, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) && !hlistday.Exists(f => f.Equals(26)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C26, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) && !hlistday.Exists(f => f.Equals(27)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C27, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) && !hlistday.Exists(f => f.Equals(28)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C28, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) && !hlistday.Exists(f => f.Equals(29)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C29, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) && !hlistday.Exists(f => f.Equals(30)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C30, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                    if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) && !hlistday.Exists(f => f.Equals(31)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                    {
                                        long.TryParse(aq.C31, out y);
                                        if (y > 8)
                                        {
                                            x += y - 8;
                                        }
                                    }

                                    leavedate = leavedate.AddDays(1);
                                }

                                leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                var y1 = 0l;x1 = 0l;
                                if (aq.C1 != null && fdaylist.Exists(g => g.Equals(1)) && !hlistday.Exists(f => f.Equals(1)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C1, out y1);
                                    
                                    {
                                        x1 +=y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C2 != null && fdaylist.Exists(g => g.Equals(2)) && !hlistday.Exists(f => f.Equals(2)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C2, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C3 != null && fdaylist.Exists(g => g.Equals(3)) && !hlistday.Exists(f => f.Equals(3)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C3, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C4 != null && fdaylist.Exists(g => g.Equals(4)) && !hlistday.Exists(f => f.Equals(4)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C4, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C5 != null && fdaylist.Exists(g => g.Equals(5)) && !hlistday.Exists(f => f.Equals(5)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C5, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C6 != null && fdaylist.Exists(g => g.Equals(6)) && !hlistday.Exists(f => f.Equals(6)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C6, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C7 != null && fdaylist.Exists(g => g.Equals(7)) && !hlistday.Exists(f => f.Equals(7)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C7, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C8 != null && fdaylist.Exists(g => g.Equals(8)) && !hlistday.Exists(f => f.Equals(8)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C8, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C9 != null && fdaylist.Exists(g => g.Equals(9)) && !hlistday.Exists(f => f.Equals(9)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C9, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C10 != null && fdaylist.Exists(g => g.Equals(10)) && !hlistday.Exists(f => f.Equals(10)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C10, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C11 != null && fdaylist.Exists(g => g.Equals(11)) && !hlistday.Exists(f => f.Equals(11)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C11, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C12 != null && fdaylist.Exists(g => g.Equals(12)) && !hlistday.Exists(f => f.Equals(12)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C12, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C13 != null && fdaylist.Exists(g => g.Equals(13)) && !hlistday.Exists(f => f.Equals(13)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C13, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C14 != null && fdaylist.Exists(g => g.Equals(14)) && !hlistday.Exists(f => f.Equals(14)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C14, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C15 != null && fdaylist.Exists(g => g.Equals(15)) && !hlistday.Exists(f => f.Equals(15)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C15, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C16 != null && fdaylist.Exists(g => g.Equals(16)) && !hlistday.Exists(f => f.Equals(16)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C16, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C17 != null && fdaylist.Exists(g => g.Equals(17)) && !hlistday.Exists(f => f.Equals(17)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C17, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C18 != null && fdaylist.Exists(g => g.Equals(18)) && !hlistday.Exists(f => f.Equals(18)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C18, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C19 != null && fdaylist.Exists(g => g.Equals(19)) && !hlistday.Exists(f => f.Equals(19)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C19, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C20 != null && fdaylist.Exists(g => g.Equals(20)) && !hlistday.Exists(f => f.Equals(20)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C20, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);

                                if (aq.C21 != null && fdaylist.Exists(g => g.Equals(21)) && !hlistday.Exists(f => f.Equals(21)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C21, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C22 != null && fdaylist.Exists(g => g.Equals(22)) && !hlistday.Exists(f => f.Equals(22)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C22, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C23 != null && fdaylist.Exists(g => g.Equals(23)) && !hlistday.Exists(f => f.Equals(23)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C23, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C24 != null && fdaylist.Exists(g => g.Equals(24)) && !hlistday.Exists(f => f.Equals(24)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C24, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C25 != null && fdaylist.Exists(g => g.Equals(25)) && !hlistday.Exists(f => f.Equals(25)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C25, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C26 != null && fdaylist.Exists(g => g.Equals(26)) && !hlistday.Exists(f => f.Equals(26)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C26, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C27 != null && fdaylist.Exists(g => g.Equals(27)) && !hlistday.Exists(f => f.Equals(27)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C27, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C28 != null && fdaylist.Exists(g => g.Equals(28)) && !hlistday.Exists(f => f.Equals(28)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C28, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C29 != null && fdaylist.Exists(g => g.Equals(29)) && !hlistday.Exists(f => f.Equals(29)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C29, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C30 != null && fdaylist.Exists(g => g.Equals(30)) && !hlistday.Exists(f => f.Equals(30)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C30, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C31 != null && fdaylist.Exists(g => g.Equals(31)) && !hlistday.Exists(f => f.Equals(31)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C31, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }
                                leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                if (aq.FridayHours.HasValue) aqf += x1;
                                y1 = 0l;x1 = 0l;
                                if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) && hlistday.Exists(f => f.Equals(1)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C1, out y1);
                                    
                                    {
                                        x1 +=y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) && hlistday.Exists(f => f.Equals(2)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C2, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) && hlistday.Exists(f => f.Equals(3)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C3, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) && hlistday.Exists(f => f.Equals(4)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C4, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) && hlistday.Exists(f => f.Equals(5)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C5, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) && hlistday.Exists(f => f.Equals(6)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C6, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) && hlistday.Exists(f => f.Equals(7)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C7, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) && hlistday.Exists(f => f.Equals(8)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C8, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) && hlistday.Exists(f => f.Equals(9)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C9, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) && hlistday.Exists(f => f.Equals(10)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C10, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) && hlistday.Exists(f => f.Equals(11)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C11, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) && hlistday.Exists(f => f.Equals(12)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C12, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) && hlistday.Exists(f => f.Equals(13)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C13, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) && hlistday.Exists(f => f.Equals(14)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C14, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) && hlistday.Exists(f => f.Equals(15)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C15, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) && hlistday.Exists(f => f.Equals(16)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C16, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) && hlistday.Exists(f => f.Equals(17)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C17, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) && hlistday.Exists(f => f.Equals(18)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C18, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) && hlistday.Exists(f => f.Equals(19)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C19, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) && hlistday.Exists(f => f.Equals(20)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C20, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);

                                if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) && hlistday.Exists(f => f.Equals(21)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C21, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) && hlistday.Exists(f => f.Equals(22)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C22, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) && hlistday.Exists(f => f.Equals(23)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C23, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) && hlistday.Exists(f => f.Equals(24)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C24, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) && hlistday.Exists(f => f.Equals(25)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C25, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) && hlistday.Exists(f => f.Equals(26)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C26, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) && hlistday.Exists(f => f.Equals(27)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C27, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) && hlistday.Exists(f => f.Equals(28)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C28, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) && hlistday.Exists(f => f.Equals(29)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C29, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) && hlistday.Exists(f => f.Equals(30)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C30, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) && hlistday.Exists(f => f.Equals(31)) && !leave2.Exists(z =>z.Start_leave <= leavedate && z.End_leave >= leavedate) && !leave2.Exists(z => z.actual_return_date == null))
                                {
                                    long.TryParse(aq.C31, out y1);
                                    
                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.Holidays.HasValue) aqh += x1;
                                aqt += x;
                            }

                            tos:
                            payr.OTRegular = aqt.ToString();
                            payr.OTFriday = aqf.ToString();
                            payr.HolidayOT = aqh.ToString();
                            var aft = 0d;
                            var ant = 0d;
                            if (payr.contract.FOT != null)
                            {
                                double.TryParse(Unprotect(payr.contract.FOT), out aft);
                            }

                            if (payr.OTNight != null)
                            {
                                double.TryParse(Unprotect(payr.OTNight), out ant);
                            }
                            else
                            {
                                payr.OTNight = "0";
                            }

                            if (abslist1.Count != 0)
                            {
                                    payr.Absents = abslist1.OrderByDescending(x => x.month).First().Id;
                                    payr.leave_absence = abslist1.OrderByDescending(x => x.month).First();
                                    payr.leave_absence.absence = absd;
                            }

                            if (leave1.Count != 0)
                            {
                                    payr.LWOP = leave1.OrderByDescending(x => x.Start_leave).First().Id;
                                    payr.Leave = leave1.OrderByDescending(x => x.Start_leave).First();
                                    payr.Leave.days = lowp;
                            }
                            
                            double.TryParse(Unprotect(con.basic), out var bac);
                            var basperh = ((bac * 12) / 365) / 8;
                            var leave21 = leave2.FindAll(
                                x => x.leave_type == "1").ToList();
                            var al = 0;
                            foreach (var leaf in leave21)
                            {
                                //var dif = leaf.End_leave - leaf.Start_leave;
                                //al += dif.Value.Days + 1;
                                var date1 = leaf.Start_leave;
                                var date2 = leaf.End_leave;
                                while (date1 != date2)
                                {
                                    if (date1.Value.Month == payr.forthemonth.Value.Month)
                                    {
                                        al += 1;
                                    }
                                    date1 = date1.Value.AddDays(1);
                                }
                                al += 1;
                            }

                            var fotded = (aft * 12 / 365) * (lowp +al +absd);
                            
                            if ((absd + lowp +  al) != 0 )
                            {
                                payr.Fot = (((aft * 12 / 365) * (DateTime.DaysInMonth(payr.forthemonth.Value.Year,payr.forthemonth.Value.Month) - lowp - al - absd))).ToString();
                            }
                            else
                            {
                                payr.Fot = aft.ToString();
                            }
                            double.TryParse(payr.Fot, out var ffot);
                            payr.TotalOT = ((aqf * 1.5 * basperh) + (aqh * 2.5 * basperh) + (aqt * 1.25 * basperh) + ffot + ant).ToString();
                            double.TryParse(Unprotect(con.salary_details), out var sal);
                            var labs = 0d;
                            var ldays = 0d;
                            if (payr.leave_absence != null)
                            {
                                labs = payr.leave_absence.absence.Value;
                            }

                            if (payr.Leave != null)
                            {
                                ldays = payr.Leave.days.Value;
                            }

                            var TLWOP = (labs + ldays) * (sal * 12 / 365);
                            var tac = 0d;
                            var arr = 0d;
                            if (payr.TicketAllowance_ != null && IsBase64Encoded(payr.TicketAllowance_))
                            {
                                double.TryParse(Unprotect(payr.TicketAllowance_), out tac);
                            }
                            if (payr.Arrears != null && IsBase64Encoded(payr.Arrears))
                            {
                                double.TryParse(Unprotect(payr.Arrears), out arr);
                            }
                            payr.totalpayable = (sal + tac + arr).ToString();
                            double.TryParse(payr.totalpayable, out var a);
                            double.TryParse(payr.TotalOT, out var b);
                            payr.TotalDedution = TLWOP.ToString();
                            double.TryParse(payr.TotalDedution, out var c10);
                            if ((labs + ldays) >= DateTime.DaysInMonth(payr.forthemonth.Value.Year,payr.forthemonth.Value.Month))
                            {
                                payr.NetPay = 0.ToString();
                            }
                            else
                            {
                                payr.NetPay = (a + b - c10 - fotded).ToString();
                            
                            }
                            paylist.Add(payr);
                            this.Create(payr);
                        }
                    }
                }
                var model12 = new paysavedlist();
                var savedlist1 = db.payrollsaveds.ToList();
                var savedlist = savedlist1
                    .FindAll(x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1)).ToList();
                if (savedlist.Count !=0)
                {
                    model12 = new paysavedlist
                    {
                        Payrollsaved = savedlist, Payroll = paylist
                    };
                }
                else
                {
                    model12 = new paysavedlist
                    {
                        Payroll = paylist,Payrollsaved = savedlist
                    };
                }
                return this.View(model12);
            }
            
            var model11 = new paysavedlist
            {
                Payroll = new List<payrole>(),Payrollsaved = new List<payrollsaved>()
            };
            return this.View(model11);
        }

        public ActionResult payslip(int? Employee_id, DateTime? eddate)
        {
            ViewBag.Employee_id = Employee_id;
            ViewBag.eddate = eddate;
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            var pay = this.db.payrollsaveds.ToList();
            afinallist = afinallist.FindAll(x => x.contracts.Count != 0);
            this.ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_no");
            var payslip = new payrollsaved();
                var model111 = new payslipmodel
                {
                    contract = new contract(),master_file = new master_file(),paysaved = new payrollsaved()
                };
            if (eddate.HasValue && Employee_id.HasValue)
            {
                eddate = new DateTime(eddate.Value.Year,eddate.Value.Month,1);
                var endmo = new DateTime(
                    eddate.Value.Year,
                    eddate.Value.Month,
                    DateTime.DaysInMonth(eddate.Value.Year, eddate.Value.Month));
                var empname10 = afinallist.Find(x=>x.employee_id == Employee_id);
                payslip = pay.Find(x => x.employee_no == empname10.employee_no && x.forthemonth == eddate);
                if (payslip == null)
                {
                    ViewBag.eddate = null;
                    goto xe;
                }
                var leave1 = this.db.Leaves.Where(
                    x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "6"
                                                                 && x.Start_leave <= payslip.forthemonth
                                                                 && x.End_leave >= payslip.forthemonth).ToList();
                var leave2 = this.db.Leaves.Where(
                    x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "1"
                                                              && x.Start_leave <= payslip.forthemonth
                                                              && x.End_leave >= payslip.forthemonth).ToList();
                var leave3 = this.db.Leaves.Where(
                    x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "2"
                                                              && x.Start_leave <= payslip.forthemonth
                                                              && x.End_leave >= payslip.forthemonth).ToList();
                var leave4 = this.db.Leaves.Where(
                    x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "3"
                                                              && x.Start_leave <= payslip.forthemonth
                                                              && x.End_leave >= payslip.forthemonth).ToList();
                var leave5 = this.db.Leaves.Where(
                    x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "4"
                                                              && x.Start_leave <= payslip.forthemonth
                                                              && x.End_leave >= payslip.forthemonth).ToList();
                var leave6 = this.db.Leaves.Where(
                    x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "5"
                                                              && x.Start_leave <= payslip.forthemonth
                                                              && x.End_leave >= payslip.forthemonth).ToList();
                var lowp = 0;
                foreach (var leaf in leave1)
                {
                    //var dif = leaf.End_leave - leaf.Start_leave;
                    //lowp += dif.Value.Days + 1;
                    var date1 = leaf.Start_leave;
                    var date2 = leaf.End_leave;
                    while (date1 != date2)
                    {
                        if (date1.Value.Month == payslip.forthemonth.Value.Month)
                        {
                            lowp += 1;
                        }
                        date1 = date1.Value.AddDays(1);
                    }
                    var daysinm = DateTime.DaysInMonth(payslip.forthemonth.Value.Year,
                        payslip.forthemonth.Value.Month);
                    if (lowp != daysinm)
                    {
                        lowp += 1;
                    }
                }
                var al = 0;
                foreach (var leaf in leave2)
                {
                    //var dif = leaf.End_leave - leaf.Start_leave;
                    //al += dif.Value.Days + 1;
                    var date1 = leaf.Start_leave;
                    var date2 = leaf.End_leave;
                    while (date1 != date2)
                    {
                        if (date1.Value.Month == payslip.forthemonth.Value.Month)
                        {
                            al += 1;
                        }
                        date1 = date1.Value.AddDays(1);
                    }
                    al += 1;
                }
                var sl = 0;
                foreach (var leaf in leave3)
                {
//                    var dif = leaf.End_leave - leaf.Start_leave;
//                    sl += dif.Value.Days + 1;
                    var date1 = leaf.Start_leave;
                    var date2 = leaf.End_leave;
                    while (date1 != date2)
                    {
                        if (date1.Value.Month == payslip.forthemonth.Value.Month)
                        {
                            sl += 1;
                        }
                        date1 = date1.Value.AddDays(1);
                    }
                    sl += 1;
                }
                var com = 0;
                foreach (var leaf in leave4)
                {
//                    var dif = leaf.End_leave - leaf.Start_leave;
//                    com += dif.Value.Days + 1;
                    var date1 = leaf.Start_leave;
                    var date2 = leaf.End_leave;
                    while (date1 != date2)
                    {
                        if (date1.Value.Month == payslip.forthemonth.Value.Month)
                        {
                            com += 1;
                        }
                        date1 = date1.Value.AddDays(1);
                    }
                    com += 1;
                }
                var mat = 0;
                foreach (var leaf in leave5)
                {
//                    var dif = leaf.End_leave - leaf.Start_leave;
//                    mat += dif.Value.Days + 1;
                    var date1 = leaf.Start_leave;
                    var date2 = leaf.End_leave;
                    while (date1 != date2)
                    {
                        if (date1.Value.Month == payslip.forthemonth.Value.Month)
                        {
                            mat += 1;
                        }
                        date1 = date1.Value.AddDays(1);
                    }
                    mat += 1;
                }
                var haj = 0;
                foreach (var leaf in leave6)
                {
//                    var dif = leaf.End_leave - leaf.Start_leave;
//                    haj += dif.Value.Days + 1;
                    var date1 = leaf.Start_leave;
                    var date2 = leaf.End_leave;
                    while (date1 != date2)
                    {
                        if (date1.Value.Month == payslip.forthemonth.Value.Month)
                        {
                            haj += 1;
                        }
                        date1 = date1.Value.AddDays(1);
                    }
                    haj += 1;
                }
                {
                    double unpaid = 0;
                    double netperiod = 0;
                    double accrued = 0;
                    double avalied = 0;
                    double lbal = 0;
                    var empname1 = afinallist.Find(x=>x.employee_no == payslip.employee_no);
                    var asf = empname1.date_joined;
                    var leaves = this.db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                        x => x.Employee_id == Employee_id && x.Start_leave >= asf );
                    var tempdate = new DateTime(2020,12,31);
                    var times = tempdate - asf;
                    var period = times.Value.TotalDays + 1;
                    var ump = leaves.ToList();
                    foreach (var leaf in ump)
                    {
                        if (leaf.leave_type == "6")
                        {
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
                        }


                        if (leaf.leave_type == "1")
                        {
                            
                            {
                                if (leaf.half)
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) avalied += times.Value.TotalDays + 1 - 0.5;
                                }
                                else
                                {
                                    times = leaf.End_leave - leaf.Start_leave;
                                    if (times != null) avalied += times.Value.TotalDays + 1;
                                }
                            }
                        }
                    }

                    netperiod = period - unpaid;
                    accrued = Math.Round(netperiod * 30 / 360);
                    lbal = accrued - avalied;
                    this.ViewBag.lbal = lbal;
                }
                ViewBag.al = al;
                ViewBag.sl = sl;
                ViewBag.com = com;
                ViewBag.mat = mat;
                ViewBag.haj = haj;
                ViewBag.holi = GetAllholi(eddate.Value).Count;
                
                var empname = afinallist.Find(x=>x.employee_no == payslip.employee_no);
                var abslist1 = this.db.leave_absence.Where(
                    x => x.month.Value.Month == eddate.Value.Month && x.month.Value.Year == eddate.Value.Year
                                                                  && x.Employee_id == empname.employee_id).ToList();
                var absd = 0;
                foreach (var leaf in abslist1)
                {
                    var dif = leaf.tod - leaf.fromd;
                    absd += dif.Value.Days + 1;
                }

                var con = db.contracts.OrderByDescending(x=>x.date_changed).ToList();
                var conemp = con.Find(x => x.employee_no == empname.employee_id);
                model111.master_file = empname;
                model111.paysaved = payslip;
                model111.contract = conemp;
            }
            xe: ;
            return this.View(model111);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}