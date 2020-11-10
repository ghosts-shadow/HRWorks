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
    [Authorize(Roles = "super_admin,payrole,employee_con")]
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
                    double.TryParse(payrole.totalpayable, out var a);
                    double.TryParse(payrole.TotalOT, out var b);
                    double.TryParse(payrole.TotalDedution, out var c);
                    payrole.NetPay = (a + b - c).ToString();
                    payrole.NetPay = Protect(payrole.NetPay);

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

        // GET: payroles
        public ActionResult Index(DateTime? month)
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
            var att = new List<Attendance>();

            if (month.HasValue)
            {
                var paylisteisting = this.db.payroles.ToList();
                mts = this.db1.MainTimeSheets.Where(
                    x => x.TMonth.Month == month.Value.Month && x.TMonth.Year == month.Value.Year
                                                             && x.ManPowerSupplier == 1).ToList();
                var endmo = new DateTime(
                    month.Value.Year,
                    month.Value.Month,
                    DateTime.DaysInMonth(month.Value.Year, month.Value.Month));
                foreach (var mt in mts)
                    if (mt.Attendances.Count != 0)
                        att.AddRange(mt.Attendances);
                foreach (var masterFile in afinallist)
                {
                    if (paylisteisting.Exists(
                        x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1)
                             && x.employee_no == masterFile.employee_id))
                    {
                        var payr = paylisteisting.Find(
                            x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1)
                                 && x.employee_no == masterFile.employee_id);
                        var leave1 = this.db.Leaves.Where(
                            x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
                                                                         && x.Start_leave >= payr.forthemonth
                                                                         && x.Start_leave <= endmo
                                                                         && x.End_leave >= payr.forthemonth
                                                                         && x.End_leave <= endmo).ToList();
                        var lowp = 0;
                        foreach (var leaf in leave1)
                        {
                            var dif = leaf.End_leave - leaf.Start_leave;
                            lowp += dif.Value.Days + 1;
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
                        foreach (var aq in attd)
                        {
                            if (aq.TotalOverTime.HasValue) aqt += aq.TotalOverTime.Value;
                            if (aq.FridayHours.HasValue) aqf += aq.FridayHours.Value;
                            if (aq.Holidays.HasValue) aqh += aq.Holidays.Value;
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
                        if (payr.TrafficFines != null )
                        {
                             double.TryParse(Unprotect(payr.TrafficFines),out comrat);

                            double.TryParse(Unprotect(payr.contract.salary_details), out var gross);
                            var TLWOP = (payr.leave_absence.absence + payr.Leave.days) * (gross / (DateTime.DaysInMonth(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month)));
                            if (TLWOP != null)
                            {
                                totded += comrat + TLWOP.Value;
                            }
                            else
                            {
                                totded += comrat;
                            }
                        }

                        payr.TotalDedution = totded.ToString();
                        payr.TotalOT = ((aqf *1.5 ) + (aqh * 2.5) + (aqt*1.25) + (aft) + (ant)).ToString();
                        if (payr.leave_absence != null)
                        {
                            payr.leave_absence.absence = absd;
                        }

                        if (payr.Leave != null)
                        {
                            payr.Leave.days = lowp;
                        }

                        var conlist = this.db.contracts.ToList();
                        var con = conlist.Find(c1 => c1.employee_no == masterFile.employee_id);
                        double.TryParse(Unprotect(con.basic), out var bac);
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
                        payr.NetPay = (a + b - c).ToString();
                        paylist.Add(payr);
                        Edit(payr);
                    }
                    else
                    {
                        var payr = new payrole();
                        payr.master_file = masterFile;
                        payr.employee_no = masterFile.employee_id;
                        payr.forthemonth = new DateTime(month.Value.Year, month.Value.Month, 1);
                        if (masterFile.contracts.Count != 0)
                        {
                            var conlist = this.db.contracts.ToList();
                            payr.contract = conlist.Find(c => c.employee_no == masterFile.employee_id);
                            var con = conlist.Find(c => c.employee_no == masterFile.employee_id);
                            var leave1 = this.db.Leaves.Where(
                                x => x.Employee_id == masterFile.employee_id && x.leave_type == "6" && x.Start_leave >= payr.forthemonth && x.Start_leave <= endmo && x.End_leave >= payr.forthemonth && x.End_leave <= endmo).ToList();
                            var lowp = 0;
                            foreach (var leaf in leave1)
                            {
                                var dif = leaf.End_leave - leaf.Start_leave;
                                lowp += dif.Value.Days + 1;
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
                            foreach (var aq in attd)
                            {
                                if (aq.TotalOverTime.HasValue) aqt += aq.TotalOverTime.Value;
                                if (aq.FridayHours.HasValue) aqf += aq.FridayHours.Value;
                                if (aq.Holidays.HasValue) aqh += aq.Holidays.Value;
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

                            payr.TotalOT = (aqf + aqh + aqt + aft + ant).ToString();
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
                            paylist.Add(payr);
                            this.Create(payr);
                        }
                    }
                }

                return this.View(paylist);
            }

            return this.View(paylist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}