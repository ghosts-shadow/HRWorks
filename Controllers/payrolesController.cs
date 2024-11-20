using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using System.Web.UI;
using System.Windows.Navigation;
using HRworks.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.Office.Interop.Word;
using OfficeOpenXml;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace HRworks.Controllers
{
    [NoDirectAccess]
    [Authorize(Roles = "super_admin,payrole,employee_con")]
    public class payrolesController : Controller
    {
        private const string Purpose = "equalizer";

        private readonly HREntities db = new HREntities();

        private readonly biometrics_DBEntities dbbio = new biometrics_DBEntities();

        private readonly LogisticsSoftEntities db1 = new LogisticsSoftEntities();

        public static string Protect(string unprotectedText)
        {
            if (unprotectedText.IsNullOrWhiteSpace())
            {
                unprotectedText = "0";
            }

            var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedText);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }

        public static bool IsBase64Encoded(string str)
        {
            if (str == null)
            {
                return false;
            }

            if (str.Replace(" ", "").Length % 4 != 0) return false;

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
            try
            {
                var protectedBytes = Convert.FromBase64String(protectedText);
                var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
                var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
                return unprotectedText;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        // GET: payroles/Create
        public ActionResult Create()
        {
            ViewBag.con_id = new SelectList(db.contracts, "employee_id", "designation");
            ViewBag.LWOP = new SelectList(db.Leaves, "Id", "Reference");
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: payroles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(payrole payrole)
        {
            if (ModelState.IsValid)
            {
                payrole.totalpayable = Protect(payrole.totalpayable);
                payrole.OTRegular = Protect(payrole.OTRegular);
                payrole.OTFriday = Protect(payrole.OTFriday);
                payrole.HolidayOT = Protect(payrole.HolidayOT);
                payrole.TotalOT = Protect(payrole.TotalOT);
                payrole.NetPay = Protect(payrole.NetPay);
                payrole.TotalDedution = Protect(payrole.TotalDedution);
                db.payroles.Add(payrole);
                db.SaveChanges();
            }

            ViewBag.con_id = new SelectList(db.contracts, "employee_id", "designation", payrole.con_id);
            ViewBag.LWOP = new SelectList(db.Leaves, "Id", "Reference", payrole.LWOP);
            ViewBag.employee_no = new SelectList(
                db.master_file,
                "employee_id",
                "employee_name",
                payrole.employee_no);
            return View(payrole);
        }

        // GET: payroles/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var payrole = db.payroles.Find(id);
            if (payrole == null) return HttpNotFound();
            return View(payrole);
        }

        // POST: payroles/Delete/5
        [Authorize(Roles = "super_admin")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var payrole = db.payroles.Find(id);
            db.payroles.Remove(payrole);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: payroles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var payrole = db.payroles.Find(id);
            if (payrole == null) return HttpNotFound();
            return View(payrole);
        }

        // GET: payroles/Edit/5
        //[Authorize(Roles = "super_admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var payrole = db.payroles.Find(id);
            if (payrole == null) return HttpNotFound();
            ViewBag.con_id = new SelectList(db.contracts, "employee_id", "employee_id", payrole.con_id);
            ViewBag.Absents = new SelectList(db.leave_absence, "Id", "Id", payrole.Absents);
            ViewBag.LWOP = new SelectList(db.Leaves, "Id", "ID", payrole.LWOP);
            ViewBag.employee_no = new SelectList(
                db.master_file,
                "employee_id",
                "employee_name",
                payrole.employee_no); /*
            ViewBag.ded_add = new List<SelectListItem>()
            {
                new SelectListItem() {Value = "Addition", Text = "Addition"},
                new SelectListItem() {Value = "deduction", Text = "deduction"}
            };*/
            if (!payrole.totalpayable.Contains(" ") && IsBase64Encoded(payrole.totalpayable))
                payrole.totalpayable = Unprotect(payrole.totalpayable);

            if (!payrole.OTRegular.Contains(" ") && IsBase64Encoded(payrole.OTRegular))
                payrole.OTRegular = Unprotect(payrole.OTRegular);

            if (!payrole.OTFriday.Contains(" ") && IsBase64Encoded(payrole.OTFriday))
                payrole.OTFriday = Unprotect(payrole.OTFriday);


            if (!payrole.HolidayOT.Contains(" ") && IsBase64Encoded(payrole.HolidayOT))
                payrole.HolidayOT = Unprotect(payrole.HolidayOT);

            if (!payrole.TotalOT.Contains(" ") && IsBase64Encoded(payrole.TotalOT))
                payrole.TotalOT = Unprotect(payrole.TotalOT);

            if (!string.IsNullOrWhiteSpace(payrole.TransportationAllowance_))
            {
                if (!payrole.TransportationAllowance_.Contains(" ") &&
                    IsBase64Encoded(payrole.TransportationAllowance_))
                    payrole.TransportationAllowance_ = Unprotect(payrole.TransportationAllowance_);
            }
            else
            {
                payrole.TransportationAllowance_ = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.OTNight))
            {
                if (!payrole.OTNight.Contains(" ") && IsBase64Encoded(payrole.OTNight))
                    payrole.OTNight = Unprotect(payrole.OTNight);
            }
            else
            {
                payrole.OTNight = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.TicketAllowance_))
            {
                if (!payrole.TicketAllowance_.Contains(" ") && IsBase64Encoded(payrole.TicketAllowance_))
                    payrole.TicketAllowance_ = Unprotect(payrole.TicketAllowance_);
            }
            else
            {
                payrole.TicketAllowance_ = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.Arrears))
            {
                if (!payrole.Arrears.Contains(" ") && IsBase64Encoded(payrole.Arrears))
                    payrole.Arrears = Unprotect(payrole.Arrears);
            }
            else
            {
                payrole.Arrears = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.cashAdvances))
            {
                if (!payrole.cashAdvances.Contains(" ") && IsBase64Encoded(payrole.cashAdvances))
                    payrole.cashAdvances = Unprotect(payrole.cashAdvances);
            }
            else
            {
                payrole.cashAdvances = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.HouseAllow))
            {
                if (!payrole.HouseAllow.Contains(" ") && IsBase64Encoded(payrole.HouseAllow))
                    payrole.HouseAllow = Unprotect(payrole.HouseAllow);
            }
            else
            {
                payrole.HouseAllow = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.FoodAllow))
            {
                if (!payrole.FoodAllow.Contains(" ") && IsBase64Encoded(payrole.FoodAllow))
                    payrole.FoodAllow = Unprotect(payrole.FoodAllow);
            }
            else
            {
                payrole.FoodAllow = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.Timekeeping))
            {
                if (!payrole.Timekeeping.Contains(" ") && IsBase64Encoded(payrole.Timekeeping))
                    payrole.Timekeeping = Unprotect(payrole.Timekeeping);
            }
            else
            {
                payrole.Timekeeping = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.Communication))
            {
                if (!payrole.Communication.Contains(" ") && IsBase64Encoded(payrole.Communication))
                    payrole.Communication = Unprotect(payrole.Communication);
            }
            else
            {
                payrole.Communication = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.TrafficFines))
            {
                if (!payrole.TrafficFines.Contains(" ") && IsBase64Encoded(payrole.TrafficFines))
                    payrole.TrafficFines = Unprotect(payrole.TrafficFines);
            }
            else
            {
                payrole.TrafficFines = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.others))
            {
                if (!payrole.others.Contains(" ") && IsBase64Encoded(payrole.others))
                    payrole.others = Unprotect(payrole.others);
            }
            else
            {
                payrole.others = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.amount))
            {
                if (!payrole.amount.Contains(" ") && IsBase64Encoded(payrole.amount))
                    payrole.amount = Unprotect(payrole.amount);
            }
            else
            {
                payrole.amount = 0.ToString();
            }

            if (!string.IsNullOrWhiteSpace(payrole.TotalDedution))
                if (!payrole.TotalDedution.Contains(" ") && IsBase64Encoded(payrole.TotalDedution))
                    payrole.TotalDedution = Unprotect(payrole.TotalDedution);

            if (!string.IsNullOrWhiteSpace(payrole.NetPay))
                if (!payrole.NetPay.Contains(" ") && IsBase64Encoded(payrole.NetPay))
                    payrole.NetPay = Unprotect(payrole.NetPay);
            return View(payrole);
        }

        // POST: payroles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "super_admin")]
        public ActionResult Edit(
            payrole payrole, string whichfun)
        {
            if (payrole == null)
            {
                goto endf;
            }

            if (ModelState.IsValid)
            {
                if (payrole.totalpayable != null && !payrole.totalpayable.Contains(" ") &&
                    IsBase64Encoded(payrole.totalpayable))
                    payrole.totalpayable = Unprotect(payrole.totalpayable);
                if (payrole.OTRegular != null && !payrole.OTRegular.Contains(" ") && IsBase64Encoded(payrole.OTRegular))
                    payrole.OTRegular = Unprotect(payrole.OTRegular);
                if (payrole.OTFriday != null && !payrole.OTFriday.Contains(" ") && IsBase64Encoded(payrole.OTFriday))
                    payrole.OTFriday = Unprotect(payrole.OTFriday);
                if (payrole.OTNight != null && !payrole.OTNight.Contains(" ") && IsBase64Encoded(payrole.OTNight))
                    payrole.OTNight = Unprotect(payrole.OTNight);
                if (payrole.HolidayOT != null && !payrole.HolidayOT.Contains(" ") && IsBase64Encoded(payrole.HolidayOT))
                    payrole.HolidayOT = Unprotect(payrole.HolidayOT);
                if (payrole.TotalOT != null && !payrole.TotalOT.Contains(" ") && IsBase64Encoded(payrole.TotalOT))
                    payrole.TotalOT = Unprotect(payrole.TotalOT);

                if (payrole.NetPay != null)
                {
                    if (payrole.Leave != null)
                    {
                        if (payrole.forthemonth != null && payrole.Leave.days >=
                            DateTime.DaysInMonth(payrole.forthemonth.Value.Year, payrole.forthemonth.Value.Month))
                        {
                            payrole.NetPay = 0.ToString();
                            payrole.NetPay = Protect(payrole.NetPay);
                        }
                        else
                        {
                            double.TryParse(payrole.totalpayable, out var a);
                            double.TryParse(payrole.TotalOT, out var b);
                            double.TryParse(payrole.TotalDedution, out var c);
                            double.TryParse(payrole.amount, out var d);
                            payrole.NetPay = (a + b - c - d).ToString();
                            payrole.NetPay = Protect(payrole.NetPay);
                        }
                    }
                    else
                    {
                        double.TryParse(payrole.totalpayable, out var a);
                        double.TryParse(payrole.TotalOT, out var b);
                        double.TryParse(payrole.TotalDedution, out var c);
                        double.TryParse(payrole.amount, out var d);
                        payrole.NetPay = (a + b - c - d).ToString();
                        payrole.NetPay = Protect(payrole.NetPay);
                    }
                }

                payrole.totalpayable = Protect(payrole.totalpayable);
                payrole.OTRegular = Protect(payrole.OTRegular);
                payrole.OTFriday = Protect(payrole.OTFriday);
                payrole.HolidayOT = Protect(payrole.HolidayOT);
                payrole.TotalOT = Protect(payrole.TotalOT);
                if (whichfun.IsNullOrWhiteSpace())
                {
                    payrole.Rstate = "E";
                }

                if (payrole.OTNight != null && !IsBase64Encoded(payrole.OTNight))
                    payrole.OTNight = Protect(payrole.OTNight);
                if (payrole.TransportationAllowance_ != null && !IsBase64Encoded(payrole.TransportationAllowance_))
                    payrole.TransportationAllowance_ = Protect(payrole.TransportationAllowance_);
                if (payrole.TicketAllowance_ != null && !IsBase64Encoded(payrole.TicketAllowance_))
                    payrole.TicketAllowance_ = Protect(payrole.TicketAllowance_);
                if (payrole.Arrears != null && !IsBase64Encoded(payrole.Arrears))
                    payrole.Arrears = Protect(payrole.Arrears);
                if (payrole.cashAdvances != null && !IsBase64Encoded(payrole.cashAdvances))
                    payrole.cashAdvances = Protect(payrole.cashAdvances);
                if (payrole.HouseAllow != null && !IsBase64Encoded(payrole.HouseAllow))
                    payrole.HouseAllow = Protect(payrole.HouseAllow);
                if (payrole.FoodAllow != null && !IsBase64Encoded(payrole.FoodAllow))
                    payrole.FoodAllow = Protect(payrole.FoodAllow);
                if (payrole.Timekeeping != null && !IsBase64Encoded(payrole.Timekeeping))
                    payrole.Timekeeping = Protect(payrole.Timekeeping);
                if (payrole.Communication != null && !IsBase64Encoded(payrole.Communication))
                    payrole.Communication = Protect(payrole.Communication);
                if (payrole.TrafficFines != null && !IsBase64Encoded(payrole.TrafficFines))
                    payrole.TrafficFines = Protect(payrole.TrafficFines);
                if (payrole.TotalDedution != null && !IsBase64Encoded(payrole.TotalDedution))
                    payrole.TotalDedution = Protect(payrole.TotalDedution);
                if (payrole.others != null && !IsBase64Encoded(payrole.others))
                    payrole.others = Protect(payrole.others);
                if (payrole.amount != null && !IsBase64Encoded(payrole.amount))
                    payrole.amount = Protect(payrole.amount);
                payrole.save = payrole.save;
                db.Entry(payrole).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { month = payrole.forthemonth });
            }

            ViewBag.con_id = new SelectList(db.contracts, "employee_id", "employee_id", payrole.con_id);
            ViewBag.Absents = new SelectList(db.leave_absence, "Id", "Id", payrole.Absents);
            ViewBag.LWOP = new SelectList(db.Leaves, "Id", "ID", payrole.LWOP);
            ViewBag.employee_no = new SelectList(
                db.master_file,
                "employee_id",
                "employee_name",
                payrole.employee_no);
            return View(payrole);
            endf: ;
            return RedirectToAction("Index");
        }

        public List<int> GetAll(DateTime date, long id)
        {
            var atweekendpro = id;
            var weekdaylist = db1.weekendlists.Where(x => x.project_id == atweekendpro).ToList();
            var month = date.Month;
            var lastDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDay = DateTime.DaysInMonth(date.Year, date.Month);
            var array = new List<int>(); // dd/mm/yy
            var count = -1;
            for (var i = 1; i <= lastDay; i++)
            {
                var temp = new DateTime(date.Year, month, i);
                var day = temp.DayOfWeek;
                if (weekdaylist.Count != 0)
                {
                    foreach (var weekend in weekdaylist)
                    {
                        /*
                                                int.TryParse(weekend.weekend, out var weekendday);
                                                if (day.ToString() == Enum.GetName(typeof(DayOfWeek), weekendday))*/
                        if (day.ToString() == weekend.weekend)
                        {
                            count++;
                            var dd = temp.Day;
                            array.Add(dd);
                        }
                    }
                }
                else
                {
                    if (day == DayOfWeek.Sunday)
                    {
                        count++;
                        var dd = temp.Day;
                        array.Add(dd);
                    }
                }
            }

            return array;
        }

        public List<int> GetAllholi(DateTime date)
        {
            var holilist = db1.Holidays
                .Where(x => x.Date.Value.Month == date.Month && x.Date.Value.Year == date.Year).ToList();
            var array = new List<int>();
            foreach (var ho in holilist) array.Add(ho.Date.Value.Day);

            return array;
        }

        // GET: payroles
        public ActionResult Index(DateTime? month, string save, string refresh)
        {
            var payroles = db.payroles.Include(p => p.contract).Include(p => p.Leave).Include(p => p.master_file);
            /*var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist.OrderByDescending(x => x.date_changed))
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no && x.status != "inactive"))
                    afinallist.Add(file);
            }*/

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

            var paylist = new List<payrole>();
            var lab = db1.LabourMasters.ToList();
            var mts = new List<MainTimeSheet>();
            var mts_1 = new List<MainTimeSheet>();
            var att_1 = new List<Attendance>();
            var att = new List<Attendance>();

            if (month.HasValue)
            {
                ViewBag.payday = month;
                var paylisteisting = db.payroles.ToList();
                mts = db1.MainTimeSheets.Where(
                        x => x.TMonth.Month == month.Value.Month && x.TMonth.Year == month.Value.Year
                                                                 && (x.ManPowerSupplier == 1 ||
                                                                     x.ManPowerSupplier == 8 ||
                                                                     x.ManPowerSupplier == 9))
                    .ToList();

                var atlist = db1.Attendances.ToList();
                var endmo = new DateTime(
                    month.Value.Year,
                    month.Value.Month,
                    DateTime.DaysInMonth(month.Value.Year, month.Value.Month));
                var Msum = db1.MainTimeSheets.Where(
                        y => (y.ManPowerSupplier == 1 || y.ManPowerSupplier == 8 ||
                              y.ManPowerSupplier == 9) &&
                             y.TMonth.Month == month.Value.Month && y.TMonth.Year == month
                                 .Value.Year)
                    .ToList();
                if (month.Value.Month == 1)
                {
                    mts_1 = db1.MainTimeSheets.Where(
                        x => x.TMonth.Month == 12 && x.TMonth.Year == (month.Value.Year - 1)
                                                  && (x.ManPowerSupplier == 1 ||
                                                      x.ManPowerSupplier == 8 ||
                                                      x.ManPowerSupplier == 9)).ToList();
                }
                else
                {
                    mts_1 = db1.MainTimeSheets.Where(
                        x => x.TMonth.Month == (month.Value.Month - 1) && x.TMonth.Year == (month.Value.Year)
                                                                       && (x.ManPowerSupplier == 1 ||
                                                                           x.ManPowerSupplier == 8 ||
                                                                           x.ManPowerSupplier == 9)).ToList();
                }

                var Msum_1 = mts_1;
                var cony = 0;
                var cony_1 = 0;
                var passexel = new List<Attendance>();
                var passexel_1 = new List<Attendance>();
                foreach (var sum in Msum)
                {
                    var listat = db1.Attendances.Where(x => x.SubMain.Equals(sum.ID)).OrderByDescending(x => x.ID)
                        .ToList();

                    foreach (var VA in listat.OrderBy(x => x.ID))
                        if (!passexel.Exists(
                                x => x.MainTimeSheet.ProjectList.ID == VA.MainTimeSheet.ProjectList.ID
                                     && x.EmpID == VA.EmpID))
                            passexel.Add(VA);
                        else
                            cony++;
                }

                att = passexel;
                foreach (var sum in Msum_1)
                {
                    var listat_1 = db1.Attendances.Where(x => x.SubMain.Equals(sum.ID)).OrderByDescending(x => x.ID)
                        .ToList();

                    foreach (var VA in listat_1.OrderBy(x => x.ID))
                        if (!passexel_1.Exists(
                                x => x.MainTimeSheet.ProjectList.ID == VA.MainTimeSheet.ProjectList.ID
                                     && x.EmpID == VA.EmpID))
                            passexel_1.Add(VA);
                        else
                            cony_1++;
                }


                att_1 = passexel_1;
//                var temp = afinallist.Find(x => x.employee_no == 101);
//                var temp2 = afinallist.Find(x => x.employee_no == 102);
//                var temp3 = afinallist.Find(x => x.employee_no == 110);
//                afinallist.Remove(temp);
//                afinallist.Remove(temp2);
//                afinallist.Remove(temp3);
                foreach (var masterFile in afinallist.OrderBy(x => x.employee_no))
                    if (paylisteisting.Exists(
                            x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1)
                                 && x.employee_no == masterFile.employee_id))
                    {
                        var payr = paylisteisting.Find(
                            x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1)
                                 && x.employee_no == masterFile.employee_id);
                        if (payr.save) goto sav;
                        if (!refresh.IsNullOrWhiteSpace())
                        {
                            payr.Rstate = "C";
                        }

                        if (payr.Rstate == "R")
                        {
                            goto R;
                        }

                        //                        var leave1 = db.Leaves.Where(
                        //                            x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
                        //                                                                         && x.Start_leave >= payr.forthemonth
                        //                                                                         && x.Start_leave <= endmo
                        //                                                                         && x.End_leave >= payr.forthemonth
                        //                                                                         && x.End_leave <= endmo).ToList();
                        //                        var leave1 = db.Leaves.Where(
                        //                            x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
                        //                                                                         && x.Start_leave <= payr.forthemonth
                        //                                                                         && x.End_leave >= payr.forthemonth).ToList();
                        //                        var leave2 = db.Leaves.Where(
                        //                            x => x.Employee_id == masterFile.employee_id && x.Start_leave >= payr.forthemonth
                        //                                                                         && x.Start_leave <= endmo
                        //                                                                        && x.End_leave >= payr.forthemonth
                        //                                                                         && x.End_leave <= endmo).ToList();
                        //                        var leave2 = db.Leaves.Where(
                        //                            x => x.Employee_id == masterFile.employee_id && x.Start_leave <= payr.forthemonth
                        //                                                                         && x.End_leave >= payr.forthemonth).ToList();
                        var leavedate1 = new DateTime();
                        if (payr.forthemonth.Value.Month == 1)
                        {
                            leavedate1 = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                        }
                        else
                        {
                            leavedate1 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1,
                                21);
                        }

                        var leavedateend = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month,
                            20);
                        var leave1 = new List<Leave>();
                        var leave2 = new List<Leave>();
                        var abslist1 = new List<leave_absence>();
                        var currentyear = DateTime.Today.Year;
                        var sickleaveind = new List<Leave>();
                        var sickleavenonind = new List<Leave>();
                        var sickleaveindhp = new List<Leave>();
                        var sickleavenonindhp = new List<Leave>();
                        var sickleaveindup = new List<Leave>();
                        var sickleavenonindup = new List<Leave>();
                        var maternityleave = new List<Leave>();
                        var maternityleavehp = new List<Leave>();
                        var maternityleaveup = new List<Leave>();
                        var yearstart = new DateTime(currentyear, 1, 1);
                        var yearend = new DateTime(currentyear, 12, 31);
                        var sickleaveindlist = new List<Leave>();
                        var sickleavenonindlist = new List<Leave>();
                        var datecount = yearstart;
                        var slindcount = 0d;
                        var slindcounthalfex = 0d;
                        var slnonindcount = 0d;
                        var slnonindcounthalfex = 0d;
                        var mlcount = 0d;
                        var mlcounthalfex = 0d;
                        do
                        {
                            var sickleaveindlist1 = db.Leaves.Where(
                                x => x.Employee_id == masterFile.employee_id && x.leave_type == "7"
                                                                             && x.Start_leave <= datecount
                                                                             && x.End_leave >= datecount).ToList();
                            var sickleavenonindlist1 = db.Leaves.Where(
                                x => x.Employee_id == masterFile.employee_id && x.leave_type == "2"
                                                                             && x.Start_leave <= datecount
                                                                             && x.End_leave >= datecount).ToList();
                            var maternityleavelist1 = db.Leaves.Where(
                                x => x.Employee_id == masterFile.employee_id && x.leave_type == "4"
                                                                             && x.Start_leave <= datecount
                                                                             && x.End_leave >= datecount).ToList();
                            foreach (var leaf in sickleaveindlist1)
                            {
                                slindcount++;
                                slindcounthalfex++;
                                if (!sickleaveind.Exists(x => x.Id == leaf.Id))
                                {
                                    sickleaveind.Add(leaf);
                                    if (leaf.half)
                                    {
                                        slindcount -= 0.5;
                                        slindcounthalfex -= 0.5;
                                    }
                                }

                                if (slindcount > 180)
                                {
                                    var slindcounttemp = slindcount - 180;
                                    var slindcounthalfextemp = slindcounthalfex - 180;
                                    if (slindcounttemp > 180)
                                    {
                                        if (sickleaveindup.Exists(x => x.Start_leave == leaf.Start_leave))
                                        {
                                            var daysepleave = new Leave();
                                            daysepleave.Start_leave = datecount;
                                            daysepleave.End_leave = datecount;
                                            daysepleave.leave_type = leaf.leave_type;
                                            if ((slindcounthalfextemp % 1) == 0)
                                            {
                                                daysepleave.half = false;
                                            }
                                            else
                                            {
                                                daysepleave.half = true;
                                                slindcounthalfextemp -= 0.5;
                                            }

                                            sickleaveindup.Add(daysepleave);
                                        }
                                    }
                                    else
                                    {
                                        if (sickleaveindhp.Exists(x => x.Start_leave == leaf.Start_leave))
                                        {
                                            var daysepleave = new Leave();
                                            daysepleave.Start_leave = datecount;
                                            daysepleave.End_leave = datecount;
                                            daysepleave.leave_type = leaf.leave_type;
                                            if ((slindcounthalfex % 1) == 0)
                                            {
                                                daysepleave.half = false;
                                            }
                                            else
                                            {
                                                daysepleave.half = true;
                                                slindcounthalfex -= 0.5;
                                            }

                                            sickleaveindhp.Add(daysepleave);
                                        }
                                    }
                                }
                            }

                            foreach (var leaf in sickleavenonindlist1)
                            {
                                slnonindcount++;
                                slnonindcounthalfex++;
                                if (!sickleavenonind.Exists(x => x.Id == leaf.Id))
                                {
                                    sickleavenonind.Add(leaf);
                                    if (leaf.half)
                                    {
                                        slnonindcount -= 0.5;
                                        slnonindcounthalfex -= 0.5;
                                    }
                                }

                                if (slnonindcount > 15)
                                {
                                    var slnonindcounttemp = slindcount - 15;
                                    var slnonindcounthalfextemp = slnonindcounthalfex - 15;
                                    if (slnonindcounttemp > 30)
                                    {
                                        if (sickleaveindup.Exists(x => x.Start_leave == leaf.Start_leave))
                                        {
                                            var daysepleave = new Leave();
                                            daysepleave.Start_leave = datecount;
                                            daysepleave.End_leave = datecount;
                                            daysepleave.leave_type = leaf.leave_type;
                                            if ((slnonindcounthalfextemp % 1) == 0)
                                            {
                                                daysepleave.half = false;
                                            }
                                            else
                                            {
                                                daysepleave.half = true;
                                                slnonindcounthalfextemp -= 0.5;
                                            }

                                            sickleaveindup.Add(daysepleave);
                                        }
                                    }
                                    else
                                    {
                                        if (sickleaveindhp.Exists(x => x.Start_leave == leaf.Start_leave))
                                        {
                                            var daysepleave = new Leave();
                                            daysepleave.Start_leave = datecount;
                                            daysepleave.End_leave = datecount;
                                            daysepleave.leave_type = leaf.leave_type;
                                            if ((slnonindcounthalfex % 1) == 0)
                                            {
                                                daysepleave.half = false;
                                            }
                                            else
                                            {
                                                daysepleave.half = true;
                                                slnonindcounthalfex -= 0.5;
                                            }

                                            sickleaveindhp.Add(daysepleave);
                                        }
                                    }
                                }
                            }

                            foreach (var leaf in maternityleavelist1)
                            {
                                mlcount++;
                                mlcounthalfex++;
                                if (!maternityleave.Exists(x => x.Id == leaf.Id))
                                {
                                    maternityleave.Add(leaf);
                                    if (leaf.half)
                                    {
                                        mlcount -= 0.5;
                                        mlcounthalfex -= 0.5;
                                    }
                                }

                                if (mlcount > 45)
                                {
                                    var mlcounttemp = slindcount - 45;
                                    var mlcounthalfextemp = mlcounthalfex - 45;
                                    if (mlcounttemp > 15)
                                    {
                                        if (maternityleaveup.Exists(x => x.Start_leave == leaf.Start_leave))
                                        {
                                            var daysepleave = new Leave();
                                            daysepleave.Start_leave = datecount;
                                            daysepleave.End_leave = datecount;
                                            daysepleave.leave_type = leaf.leave_type;
                                            if ((mlcounthalfextemp % 1) == 0)
                                            {
                                                daysepleave.half = false;
                                            }
                                            else
                                            {
                                                daysepleave.half = true;
                                                mlcounthalfextemp -= 0.5;
                                            }

                                            maternityleaveup.Add(daysepleave);
                                        }
                                    }
                                    else
                                    {
                                        if (maternityleavehp.Exists(x => x.Start_leave == leaf.Start_leave))
                                        {
                                            var daysepleave = new Leave();
                                            daysepleave.Start_leave = datecount;
                                            daysepleave.End_leave = datecount;
                                            daysepleave.leave_type = leaf.leave_type;
                                            if ((mlcounthalfex % 1) == 0)
                                            {
                                                daysepleave.half = false;
                                            }
                                            else
                                            {
                                                daysepleave.half = true;
                                                mlcounthalfex -= 0.5;
                                            }

                                            maternityleavehp.Add(daysepleave);
                                        }
                                    }
                                }
                            }

                            datecount = datecount.AddDays(1);
                        } while (datecount < yearend);

                        var absd = 0;
                        var mlcounthp = 0d;
                        var mlcountup = 0d;
                        var slindcounthp = 0d;
                        var slindcountup = 0d;
                        var slnonindcounthp = 0d;
                        var slnonindcountup = 0d;
                        do
                        {
                            var leave1_1 = db.Leaves.Where(
                                x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
                                                                             && x.Start_leave <= leavedate1
                                                                             && x.End_leave >= leavedate1).ToList();
                            var leave2_1 = db.Leaves.Where(
                                x => x.Employee_id == masterFile.employee_id && x.Start_leave <= leavedate1
                                                                             && x.End_leave >= leavedate1).ToList();
                            var abslist1_1 = db.leave_absence.Where(
                                x => x.Employee_id == masterFile.employee_id && x.fromd <= leavedate1
                                                                             && x.tod >= leavedate1).ToList();

                            if (maternityleavehp.Exists(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                            {
                                var temp = maternityleavehp.Find(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                mlcounthp++;
                                if (temp.half)
                                {
                                    mlcounthp -= 0.5;
                                }
                            }

                            if (maternityleaveup.Exists(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                            {
                                var temp = maternityleaveup.Find(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                mlcountup++;
                                if (temp.half)
                                {
                                    mlcountup -= 0.5;
                                }
                            }

                            if (sickleaveindhp.Exists(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                            {
                                var temp = sickleaveindhp.Find(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                slindcounthp++;
                                if (temp.half)
                                {
                                    slindcounthp -= 0.5;
                                }
                            }

                            if (sickleaveindup.Exists(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                            {
                                var temp = sickleaveindup.Find(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                slindcountup++;
                                if (temp.half)
                                {
                                    slindcountup -= 0.5;
                                }
                            }

                            if (sickleavenonindhp.Exists(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                            {
                                var temp = sickleavenonindhp.Find(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                slnonindcounthp++;
                                if (temp.half)
                                {
                                    slnonindcounthp -= 0.5;
                                }
                            }

                            if (sickleavenonindup.Exists(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                            {
                                var temp = sickleavenonindup.Find(x =>
                                    x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                slnonindcountup++;
                                if (temp.half)
                                {
                                    slnonindcountup -= 0.5;
                                }
                            }

                            foreach (var leaf in leave1_1)
                                if (!leave1.Exists(x => x.Id == leaf.Id))
                                    leave1.Add(leaf);
                            foreach (var leaf1 in abslist1_1)
                                if (!leave1.Exists(x => x.Id == leaf1.Id))
                                    abslist1.Add(leaf1);
                            foreach (var leaf in leave2_1)
                                if (!leave2.Exists(x => x.Id == leaf.Id))
                                    leave2.Add(leaf);

                            leavedate1 = leavedate1.AddDays(1);
                        } while (leavedate1 < leavedateend);

                        var lowp = 0;
                        var datediff1 = new DateTime();
                        var datediff2 = new DateTime();
                        if (payr.forthemonth.Value.Month == 1)
                        {
                            datediff1 = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                        }
                        else
                        {
                            datediff1 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1, 21);
                        }

                        datediff2 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month, 20);
                        var datediff = (datediff2 - datediff1).Days + 1;
                        foreach (var leaf in leave1)
                        {
                            var dif = leaf.End_leave - leaf.Start_leave;
                            var date1 = leaf.Start_leave;
                            var date2 = leaf.End_leave;
                            while (date1 != date2)
                            {
                                if (date1 >= datediff1 && date1 < datediff2) lowp += 1;
                                date1 = date1.Value.AddDays(1);
                            }

                            var daysinm = DateTime.DaysInMonth(payr.forthemonth.Value.Year,
                                payr.forthemonth.Value.Month);
                            if (lowp != datediff) lowp += 1;
                            //lowp += dif.Value.Days + 1;
                        }

//                        var abslist1 = db.leave_absence.Where(
//                                x => x.tod <=  && x.month.Value.Year == month.Value.Year
//                                                                              && x.Employee_id
//                                                                              == masterFile.employee_id)
//                            .ToList();
//                        var absd = 0;
//                        foreach (var leaf in abslist1)
//                        {
//                            var dif = leaf.tod - leaf.fromd;
//                            absd += dif.Value.Days + 1;
//                        }

                        foreach (var absence in abslist1)
                        {
                            var dif = absence.tod - absence.fromd;
                            var date1 = absence.fromd;
                            var date2 = absence.tod;
                            while (date1 != date2)
                            {
                                if (date1 >= datediff1 && date1 < datediff2) absd += 1;
                                date1 = date1.Value.AddDays(1);
                            }

                            var daysinm = DateTime.DaysInMonth(payr.forthemonth.Value.Year,
                                payr.forthemonth.Value.Month);
                            if (absd != datediff) absd += 1;
                        }

                        var lab1 = lab.Find(x => x.EMPNO == masterFile.employee_no);
                        var aqt = 0L;
                        var aqf = 0L;
                        var aqh = 0L;
                        if (lab1 == null) goto tos1;
                        var attd1 = att.FindAll(x => x.EmpID == lab1.ID).ToList();
                        var attd = new List<Attendance>();
                        var attd1_1 = att_1.FindAll(x => x.EmpID == lab1.ID).ToList();
                        var attd_1 = new List<Attendance>();
                        foreach (var atq in attd1)
                        {
                            var attq1 = new Attendance();
                            if (attd.Exists(x => x.EmpID == atq.EmpID))
                            {
                                attq1 = attd.First();
                                attd.Remove(attq1);
                                long.TryParse(attq1.C1, out var hrs1);
                                long.TryParse(atq.C1, out var hrs2);
                                var sum = hrs1 + hrs2;
                                attq1.C1 = sum.ToString();
                                long.TryParse(attq1.C2, out hrs1);
                                long.TryParse(atq.C2, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C2 = sum.ToString();
                                long.TryParse(attq1.C3, out hrs1);
                                long.TryParse(atq.C3, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C3 = sum.ToString();
                                long.TryParse(attq1.C4, out hrs1);
                                long.TryParse(atq.C4, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C4 = sum.ToString();
                                long.TryParse(attq1.C5, out hrs1);
                                long.TryParse(atq.C5, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C5 = sum.ToString();
                                long.TryParse(attq1.C6, out hrs1);
                                long.TryParse(atq.C6, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C6 = sum.ToString();
                                long.TryParse(attq1.C7, out hrs1);
                                long.TryParse(atq.C7, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C7 = sum.ToString();
                                long.TryParse(attq1.C8, out hrs1);
                                long.TryParse(atq.C8, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C8 = sum.ToString();
                                long.TryParse(attq1.C9, out hrs1);
                                long.TryParse(atq.C9, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C9 = sum.ToString();
                                long.TryParse(attq1.C10, out hrs1);
                                long.TryParse(atq.C10, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C10 = sum.ToString();
                                long.TryParse(attq1.C11, out hrs1);
                                long.TryParse(atq.C11, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C11 = sum.ToString();
                                long.TryParse(attq1.C12, out hrs1);
                                long.TryParse(atq.C12, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C12 = sum.ToString();
                                long.TryParse(attq1.C13, out hrs1);
                                long.TryParse(atq.C13, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C13 = sum.ToString();
                                long.TryParse(attq1.C14, out hrs1);
                                long.TryParse(atq.C14, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C14 = sum.ToString();
                                long.TryParse(attq1.C15, out hrs1);
                                long.TryParse(atq.C15, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C15 = sum.ToString();
                                long.TryParse(attq1.C16, out hrs1);
                                long.TryParse(atq.C16, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C16 = sum.ToString();
                                long.TryParse(attq1.C17, out hrs1);
                                long.TryParse(atq.C17, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C17 = sum.ToString();
                                long.TryParse(attq1.C18, out hrs1);
                                long.TryParse(atq.C18, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C18 = sum.ToString();
                                long.TryParse(attq1.C19, out hrs1);
                                long.TryParse(atq.C19, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C19 = sum.ToString();
                                long.TryParse(attq1.C20, out hrs1);
                                long.TryParse(atq.C20, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C20 = sum.ToString();
                                long.TryParse(attq1.C21, out hrs1);
                                long.TryParse(atq.C21, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C21 = sum.ToString();
                                long.TryParse(attq1.C22, out hrs1);
                                long.TryParse(atq.C22, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C22 = sum.ToString();
                                long.TryParse(attq1.C23, out hrs1);
                                long.TryParse(atq.C23, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C23 = sum.ToString();
                                long.TryParse(attq1.C24, out hrs1);
                                long.TryParse(atq.C24, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C24 = sum.ToString();
                                long.TryParse(attq1.C25, out hrs1);
                                long.TryParse(atq.C25, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C25 = sum.ToString();
                                long.TryParse(attq1.C26, out hrs1);
                                long.TryParse(atq.C26, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C26 = sum.ToString();
                                long.TryParse(attq1.C27, out hrs1);
                                long.TryParse(atq.C27, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C27 = sum.ToString();
                                long.TryParse(attq1.C28, out hrs1);
                                long.TryParse(atq.C28, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C28 = sum.ToString();
                                long.TryParse(attq1.C29, out hrs1);
                                long.TryParse(atq.C29, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C29 = sum.ToString();
                                long.TryParse(attq1.C30, out hrs1);
                                long.TryParse(atq.C30, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C30 = sum.ToString();
                                long.TryParse(attq1.C31, out hrs1);
                                long.TryParse(atq.C31, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C31 = sum.ToString();
                                attd.Add(attq1);
                            }
                            else
                            {
                                attd.Add(atq);
                            }
                        }

                        foreach (var atq in attd1_1)
                        {
                            var attq1 = new Attendance();
                            if (attd_1.Exists(x => x.EmpID == atq.EmpID))
                            {
                                attq1 = attd_1.First();
                                attd_1.Remove(attq1);
                                long.TryParse(attq1.C1, out var hrs1);
                                long.TryParse(atq.C1, out var hrs2);
                                var sum = hrs1 + hrs2;
                                attq1.C1 = sum.ToString();
                                long.TryParse(attq1.C2, out hrs1);
                                long.TryParse(atq.C2, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C2 = sum.ToString();
                                long.TryParse(attq1.C3, out hrs1);
                                long.TryParse(atq.C3, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C3 = sum.ToString();
                                long.TryParse(attq1.C4, out hrs1);
                                long.TryParse(atq.C4, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C4 = sum.ToString();
                                long.TryParse(attq1.C5, out hrs1);
                                long.TryParse(atq.C5, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C5 = sum.ToString();
                                long.TryParse(attq1.C6, out hrs1);
                                long.TryParse(atq.C6, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C6 = sum.ToString();
                                long.TryParse(attq1.C7, out hrs1);
                                long.TryParse(atq.C7, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C7 = sum.ToString();
                                long.TryParse(attq1.C8, out hrs1);
                                long.TryParse(atq.C8, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C8 = sum.ToString();
                                long.TryParse(attq1.C9, out hrs1);
                                long.TryParse(atq.C9, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C9 = sum.ToString();
                                long.TryParse(attq1.C10, out hrs1);
                                long.TryParse(atq.C10, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C10 = sum.ToString();
                                long.TryParse(attq1.C11, out hrs1);
                                long.TryParse(atq.C11, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C11 = sum.ToString();
                                long.TryParse(attq1.C12, out hrs1);
                                long.TryParse(atq.C12, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C12 = sum.ToString();
                                long.TryParse(attq1.C13, out hrs1);
                                long.TryParse(atq.C13, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C13 = sum.ToString();
                                long.TryParse(attq1.C14, out hrs1);
                                long.TryParse(atq.C14, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C14 = sum.ToString();
                                long.TryParse(attq1.C15, out hrs1);
                                long.TryParse(atq.C15, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C15 = sum.ToString();
                                long.TryParse(attq1.C16, out hrs1);
                                long.TryParse(atq.C16, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C16 = sum.ToString();
                                long.TryParse(attq1.C17, out hrs1);
                                long.TryParse(atq.C17, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C17 = sum.ToString();
                                long.TryParse(attq1.C18, out hrs1);
                                long.TryParse(atq.C18, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C18 = sum.ToString();
                                long.TryParse(attq1.C19, out hrs1);
                                long.TryParse(atq.C19, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C19 = sum.ToString();
                                long.TryParse(attq1.C20, out hrs1);
                                long.TryParse(atq.C20, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C20 = sum.ToString();
                                long.TryParse(attq1.C21, out hrs1);
                                long.TryParse(atq.C21, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C21 = sum.ToString();
                                long.TryParse(attq1.C22, out hrs1);
                                long.TryParse(atq.C22, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C22 = sum.ToString();
                                long.TryParse(attq1.C23, out hrs1);
                                long.TryParse(atq.C23, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C23 = sum.ToString();
                                long.TryParse(attq1.C24, out hrs1);
                                long.TryParse(atq.C24, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C24 = sum.ToString();
                                long.TryParse(attq1.C25, out hrs1);
                                long.TryParse(atq.C25, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C25 = sum.ToString();
                                long.TryParse(attq1.C26, out hrs1);
                                long.TryParse(atq.C26, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C26 = sum.ToString();
                                long.TryParse(attq1.C27, out hrs1);
                                long.TryParse(atq.C27, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C27 = sum.ToString();
                                long.TryParse(attq1.C28, out hrs1);
                                long.TryParse(atq.C28, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C28 = sum.ToString();
                                long.TryParse(attq1.C29, out hrs1);
                                long.TryParse(atq.C29, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C29 = sum.ToString();
                                long.TryParse(attq1.C30, out hrs1);
                                long.TryParse(atq.C30, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C30 = sum.ToString();
                                long.TryParse(attq1.C31, out hrs1);
                                long.TryParse(atq.C31, out hrs2);
                                sum = hrs1 + hrs2;
                                attq1.C31 = sum.ToString();
                                attd_1.Add(attq1);
                            }
                            else
                            {
                                attd_1.Add(atq);
                            }
                        }

                        var attd_final = new List<Attendance>();
                        var attd_final1 = new List<Attendance>();
                        attd_final.AddRange(attd);
                        attd_final.AddRange(attd_1);
                        foreach (var aq1 in attd_final.OrderByDescending(x => x.MainTimeSheet.TMonth)
                                     .ThenBy(x => x.EmpID))
                        {
                            var empnam = new Attendance();
                            if (aq1.MainTimeSheet.TMonth.Month == 1)
                            {
                                empnam = attd_final.Find(x =>
                                    x.EmpID == aq1.EmpID &&
                                    x.MainTimeSheet.TMonth.Month == 12);
                            }
                            else
                            {
                                empnam = attd_final.Find(x =>
                                    x.EmpID == aq1.EmpID &&
                                    x.MainTimeSheet.TMonth.Month == aq1.MainTimeSheet.TMonth.Month - 1);
                            }

                            if (empnam != null)
                            {
                                aq1.C21 = empnam.C21;
                                aq1.C22 = empnam.C22;
                                aq1.C23 = empnam.C23;
                                aq1.C24 = empnam.C24;
                                aq1.C25 = empnam.C25;
                                aq1.C26 = empnam.C26;
                                aq1.C27 = empnam.C27;
                                aq1.C28 = empnam.C28;
                                aq1.C29 = empnam.C29;
                                aq1.C30 = empnam.C30;
                                aq1.C31 = empnam.C31;
                                if (!attd_final1.Exists(x => x.EmpID == aq1.EmpID))
                                {
                                    attd_final1.Add(aq1);
                                }
                            }
                        }

                        var newdate = new DateTime();
                        if (payr.forthemonth.Value.Month == 1)
                        {
                            newdate = new DateTime(payr.forthemonth.Value.Year - 1, 12, 1);
                        }
                        else
                        {
                            newdate = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1, 1);
                        }

                        var datestart = new DateTime();
                        var dateend = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month, 20);
                        if (payr.forthemonth.Value.Month == 1)
                        {
                            datestart = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                        }
                        else
                        {
                            datestart = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1, 21);
                        }

                        var hlistday = new List<int>();
                        var hlistday1 = GetAllholi(month.Value);
                        foreach (var i in hlistday1)
                        {
                            var dt1 = new DateTime(dateend.Year, dateend.Month, i);
                            if (datestart <= dt1 && dt1 <= dateend)
                            {
                                hlistday.Add(i);
                            }
                        }

                        hlistday1 = GetAllholi(newdate);
                        foreach (var i in hlistday1)
                        {
                            var dt1 = new DateTime(datestart.Year, datestart.Month, i);
                            if (datestart <= dt1 && dt1 <= dateend)
                            {
                                hlistday.Add(i);
                            }
                        }

                        foreach (var aq in attd_final1)
                        {
                            var fdaylist = new List<int>();
                            var fdaylist1 = GetAll(month.Value, aq.MainTimeSheet.Project);
                            foreach (var i in fdaylist1)
                            {
                                var dt1 = new DateTime(dateend.Year, dateend.Month, i);
                                if (datestart <= dt1 && dt1 <= dateend)
                                {
                                    fdaylist.Add(i);
                                }
                            }

                            fdaylist1 = GetAll(newdate, aq.MainTimeSheet.Project);
                            foreach (var i in fdaylist1)
                            {
                                var dt1 = new DateTime(datestart.Year, datestart.Month, i);
                                if (datestart <= dt1 && dt1 <= dateend)
                                {
                                    fdaylist.Add(i);
                                }
                            }

                            var x = 0L;
                            var x1 = 0L;
                            var leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                            leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                            var y = 0L;
                            if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) &&
                                !hlistday.Exists(f => f.Equals(1)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C1, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) &&
                                !hlistday.Exists(f => f.Equals(2)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C2, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) &&
                                !hlistday.Exists(f => f.Equals(3)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C3, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) &&
                                !hlistday.Exists(f => f.Equals(4)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C4, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) &&
                                !hlistday.Exists(f => f.Equals(5)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C5, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) &&
                                !hlistday.Exists(f => f.Equals(6)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C6, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) &&
                                !hlistday.Exists(f => f.Equals(7)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C7, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) &&
                                !hlistday.Exists(f => f.Equals(8)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C8, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) &&
                                !hlistday.Exists(f => f.Equals(9)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C9, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) &&
                                !hlistday.Exists(f => f.Equals(10)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C10, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) &&
                                !hlistday.Exists(f => f.Equals(11)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C11, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) &&
                                !hlistday.Exists(f => f.Equals(12)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C12, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) &&
                                !hlistday.Exists(f => f.Equals(13)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C13, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) &&
                                !hlistday.Exists(f => f.Equals(14)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C14, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) &&
                                !hlistday.Exists(f => f.Equals(15)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C15, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) &&
                                !hlistday.Exists(f => f.Equals(16)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C16, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) &&
                                !hlistday.Exists(f => f.Equals(17)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C17, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) &&
                                !hlistday.Exists(f => f.Equals(18)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C18, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) &&
                                !hlistday.Exists(f => f.Equals(19)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C19, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) &&
                                !hlistday.Exists(f => f.Equals(20)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C20, out y);
                                if (y > 8) x += y - 8;
                            }

                            if (leavedate.Month == 1)
                            {
                                leavedate = new DateTime(month.Value.Year - 1, 12, 21);
                            }
                            else
                            {
                                leavedate = new DateTime(month.Value.Year, month.Value.Month - 1, 21);
                            }


                            if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) &&
                                !hlistday.Exists(f => f.Equals(21)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C21, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) &&
                                !hlistday.Exists(f => f.Equals(22)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C22, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) &&
                                !hlistday.Exists(f => f.Equals(23)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C23, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) &&
                                !hlistday.Exists(f => f.Equals(24)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C24, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) &&
                                !hlistday.Exists(f => f.Equals(25)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C25, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) &&
                                !hlistday.Exists(f => f.Equals(26)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C26, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) &&
                                !hlistday.Exists(f => f.Equals(27)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C27, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) &&
                                !hlistday.Exists(f => f.Equals(28)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C28, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) &&
                                !hlistday.Exists(f => f.Equals(29)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C29, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) &&
                                !hlistday.Exists(f => f.Equals(30)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C30, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) &&
                                !hlistday.Exists(f => f.Equals(31)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C31, out y);
                                if (y > 8) x += y - 8;
                            }

                            leavedate = leavedate.AddDays(1);

                            leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                            var y1 = 0L;
                            x1 = 0L;
                            if (aq.C1 != null && fdaylist.Exists(g => g.Equals(1)) &&
                                !hlistday.Exists(f => f.Equals(1)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C1, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C2 != null && fdaylist.Exists(g => g.Equals(2)) &&
                                !hlistday.Exists(f => f.Equals(2)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C2, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C3 != null && fdaylist.Exists(g => g.Equals(3)) &&
                                !hlistday.Exists(f => f.Equals(3)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C3, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C4 != null && fdaylist.Exists(g => g.Equals(4)) &&
                                !hlistday.Exists(f => f.Equals(4)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C4, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C5 != null && fdaylist.Exists(g => g.Equals(5)) &&
                                !hlistday.Exists(f => f.Equals(5)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C5, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C6 != null && fdaylist.Exists(g => g.Equals(6)) &&
                                !hlistday.Exists(f => f.Equals(6)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C6, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C7 != null && fdaylist.Exists(g => g.Equals(7)) &&
                                !hlistday.Exists(f => f.Equals(7)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C7, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C8 != null && fdaylist.Exists(g => g.Equals(8)) &&
                                !hlistday.Exists(f => f.Equals(8)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C8, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C9 != null && fdaylist.Exists(g => g.Equals(9)) &&
                                !hlistday.Exists(f => f.Equals(9)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C9, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C10 != null && fdaylist.Exists(g => g.Equals(10)) &&
                                !hlistday.Exists(f => f.Equals(10)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C10, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C11 != null && fdaylist.Exists(g => g.Equals(11)) &&
                                !hlistday.Exists(f => f.Equals(11)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C11, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C12 != null && fdaylist.Exists(g => g.Equals(12)) &&
                                !hlistday.Exists(f => f.Equals(12)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C12, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C13 != null && fdaylist.Exists(g => g.Equals(13)) &&
                                !hlistday.Exists(f => f.Equals(13)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C13, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C14 != null && fdaylist.Exists(g => g.Equals(14)) &&
                                !hlistday.Exists(f => f.Equals(14)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C14, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C15 != null && fdaylist.Exists(g => g.Equals(15)) &&
                                !hlistday.Exists(f => f.Equals(15)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C15, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C16 != null && fdaylist.Exists(g => g.Equals(16)) &&
                                !hlistday.Exists(f => f.Equals(16)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C16, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C17 != null && fdaylist.Exists(g => g.Equals(17)) &&
                                !hlistday.Exists(f => f.Equals(17)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C17, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C18 != null && fdaylist.Exists(g => g.Equals(18)) &&
                                !hlistday.Exists(f => f.Equals(18)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C18, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C19 != null && fdaylist.Exists(g => g.Equals(19)) &&
                                !hlistday.Exists(f => f.Equals(19)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C19, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C20 != null && fdaylist.Exists(g => g.Equals(20)) &&
                                !hlistday.Exists(f => f.Equals(20)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C20, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            if (leavedate.Month == 1)
                            {
                                leavedate = new DateTime(month.Value.Year - 1, 12, 21);
                            }
                            else
                            {
                                leavedate = new DateTime(month.Value.Year, month.Value.Month - 1, 21);
                            }


                            if (aq.C21 != null && fdaylist.Exists(g => g.Equals(21)) &&
                                !hlistday.Exists(f => f.Equals(21)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C21, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C22 != null && fdaylist.Exists(g => g.Equals(22)) &&
                                !hlistday.Exists(f => f.Equals(22)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C22, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C23 != null && fdaylist.Exists(g => g.Equals(23)) &&
                                !hlistday.Exists(f => f.Equals(23)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C23, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C24 != null && fdaylist.Exists(g => g.Equals(24)) &&
                                !hlistday.Exists(f => f.Equals(24)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C24, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C25 != null && fdaylist.Exists(g => g.Equals(25)) &&
                                !hlistday.Exists(f => f.Equals(25)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C25, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C26 != null && fdaylist.Exists(g => g.Equals(26)) &&
                                !hlistday.Exists(f => f.Equals(26)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C26, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C27 != null && fdaylist.Exists(g => g.Equals(27)) &&
                                !hlistday.Exists(f => f.Equals(27)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C27, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C28 != null && fdaylist.Exists(g => g.Equals(28)) &&
                                !hlistday.Exists(f => f.Equals(28)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C28, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C29 != null && fdaylist.Exists(g => g.Equals(29)) &&
                                !hlistday.Exists(f => f.Equals(29)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C29, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C30 != null && fdaylist.Exists(g => g.Equals(30)) &&
                                !hlistday.Exists(f => f.Equals(30)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C30, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C31 != null && fdaylist.Exists(g => g.Equals(31)) &&
                                !hlistday.Exists(f => f.Equals(31)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C31, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                            aqf += x1;
                            y1 = 0L;
                            x1 = 0L;
                            if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) &&
                                hlistday.Exists(f => f.Equals(1)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C1, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) &&
                                hlistday.Exists(f => f.Equals(2)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C2, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) &&
                                hlistday.Exists(f => f.Equals(3)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C3, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) &&
                                hlistday.Exists(f => f.Equals(4)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C4, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) &&
                                hlistday.Exists(f => f.Equals(5)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C5, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) &&
                                hlistday.Exists(f => f.Equals(6)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C6, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) &&
                                hlistday.Exists(f => f.Equals(7)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C7, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) &&
                                hlistday.Exists(f => f.Equals(8)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C8, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) &&
                                hlistday.Exists(f => f.Equals(9)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C9, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) &&
                                hlistday.Exists(f => f.Equals(10)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C10, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) &&
                                hlistday.Exists(f => f.Equals(11)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C11, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) &&
                                hlistday.Exists(f => f.Equals(12)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C12, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) &&
                                hlistday.Exists(f => f.Equals(13)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C13, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) &&
                                hlistday.Exists(f => f.Equals(14)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C14, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) &&
                                hlistday.Exists(f => f.Equals(15)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C15, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) &&
                                hlistday.Exists(f => f.Equals(16)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C16, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) &&
                                hlistday.Exists(f => f.Equals(17)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C17, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) &&
                                hlistday.Exists(f => f.Equals(18)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C18, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) &&
                                hlistday.Exists(f => f.Equals(19)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C19, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) &&
                                hlistday.Exists(f => f.Equals(20)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C20, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            if (leavedate.Month == 1)
                            {
                                leavedate = new DateTime(month.Value.Year - 1, 12, 21);
                            }
                            else
                            {
                                leavedate = new DateTime(month.Value.Year, month.Value.Month - 1, 21);
                            }


                            if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) &&
                                hlistday.Exists(f => f.Equals(21)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C21, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) &&
                                hlistday.Exists(f => f.Equals(22)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C22, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) &&
                                hlistday.Exists(f => f.Equals(23)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C23, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) &&
                                hlistday.Exists(f => f.Equals(24)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C24, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) &&
                                hlistday.Exists(f => f.Equals(25)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C25, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) &&
                                hlistday.Exists(f => f.Equals(26)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C26, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) &&
                                hlistday.Exists(f => f.Equals(27)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C27, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) &&
                                hlistday.Exists(f => f.Equals(28)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C28, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) &&
                                hlistday.Exists(f => f.Equals(29)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C29, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) &&
                                hlistday.Exists(f => f.Equals(30)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
                            {
                                long.TryParse(aq.C30, out y1);

                                {
                                    x1 += y1;
                                }
                            }

                            leavedate = leavedate.AddDays(1);
                            if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) &&
                                hlistday.Exists(f => f.Equals(31)) &&
                                !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                !leave2.Exists(z =>
                                    z.actual_return_date == null &&
                                    z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                               )
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

                        payr.OTRegular = aqt.ToString();
                        payr.OTFriday = aqf.ToString();
                        payr.HolidayOT = aqh.ToString();
                        payr.Rstate = "R";
                        var ant = 0d;
                        var m = 0d;
                        if (payr.OTNight != null && IsBase64Encoded(payr.OTNight))
                        {
                            double.TryParse(Unprotect(payr.OTNight), out ant);
                        }

                        var comrat = 0d;
                        var totded = 0d;
                        if (payr.cashAdvances != null)
                        {
                            double.TryParse(Unprotect(payr.cashAdvances), out comrat);
                            totded += comrat;
                        }

                        if (payr.HouseAllow != null)
                        {
                            double.TryParse(Unprotect(payr.HouseAllow), out comrat);
                            totded += comrat;
                        }

                        if (payr.TrafficFines != null)
                        {
                            double.TryParse(Unprotect(payr.TrafficFines), out comrat);
                            totded += comrat;
                        }

                        if (payr.TransportationAllowance_ != null)
                        {
                            double.TryParse(Unprotect(payr.TransportationAllowance_), out comrat);
                            totded += comrat;
                        }

                        {
                            /*
                            var fad = 0d;
                            var fadamount = 0d;


                            if (payr.contract != null)
                            {
                                var confa = 0d;
                                double.TryParse(Unprotect(payr.contract.food_allowance), out confa);
                                if (confa == 0)
                                {
                                    fadamount = 0;
                                }
                                else
                                {

                                    var mindate = new DateTime();
                                    if (payr.forthemonth.Value.Month == 1)
                                    {
                                        mindate = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                                    }
                                    else
                                    {
                                        mindate = new DateTime(payr.forthemonth.Value.Year,
                                            payr.forthemonth.Value.Month - 1,
                                            21);
                                    }

                                    var moutdate = new DateTime(payr.forthemonth.Value.Year,
                                        payr.forthemonth.Value.Month,
                                        20);
                                    var minoutlist = new List<Manpowerinoutform>();
                                    if (lab1 != null)
                                    {
                                        do
                                        {
                                            var minout = db1.Manpowerinoutforms
                                                .Where(x => x.check_in < mindate && x.EmpID == lab1.ID &&
                                                            x.camp != "Camp Musaffah").ToList();
                                            foreach (var inout in minout)
                                            {
                                                if (!minoutlist.Exists(
                                                    x => x.EmpID == inout.EmpID && x.Project == inout.Project))
                                                {
                                                    minoutlist.Add(inout);
                                                }
                                            }

                                            mindate = mindate.AddDays(1);
                                        } while (mindate <= moutdate);
                                    }

                                    if (payr.forthemonth.Value.Month == 1)
                                    {
                                        datediff1 = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                                    }
                                    else
                                    {
                                        datediff1 = new DateTime(payr.forthemonth.Value.Year,
                                            payr.forthemonth.Value.Month - 1,
                                            21);
                                    }

                                    datediff2 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month,
                                        20);
                                    datediff = (datediff2 - datediff1).Days + 1;
                                    var faddays = 0;
                                    var fadextrastop = 0;
                                    foreach (var inout in minoutlist)
                                    {
                                        var date1 = inout.check_in;
                                        var date2 = inout.check_out;
                                        if (date2 == null)
                                        {
                                            date2 = datediff2;
                                        }

                                        while (date1 != date2)
                                        {
                                            fadextrastop++;
                                            if (date1 >= datediff1 && date1 < datediff2) faddays += 1;
                                            date1 = date1.Value.AddDays(1);
                                        }

                                        if (faddays != datediff && fadextrastop > 0)
                                        {
                                            faddays += 1;
                                            fadextrastop = 0;
                                        }
                                    }

                                    var fadamountpday = 250d * 12d / 360d;
                                    fadamount = fadamountpday * (datediff - faddays);
                                    //fadamount = fadamountpday * faddays;
                                    if (fadamount > 250)
                                    {
                                        fadamount = 250;
                                    }
                                }
                            }
                            else
                            {
                                fadamount = 0;
                            }
                            payr.FoodAllow = Protect(fadamount.ToString());

                        if (payr.FoodAllow != null)
                        {
                            double.TryParse(Unprotect(payr.FoodAllow), out comrat);
                            totded += comrat;
                        }*/
                        }

                        if (payr.Timekeeping != null)
                        {
                            double.TryParse(Unprotect(payr.Timekeeping), out comrat);
                            totded += comrat;
                        }

                        if (payr.Communication != null)
                        {
                            double.TryParse(Unprotect(payr.Communication), out comrat);
                            totded += comrat;
                        }

                        if (payr.others != null)
                        {
                            double.TryParse(Unprotect(payr.others), out comrat);
                            totded += comrat;
                        }

                        if (payr.amount != null)
                        {
                            double.TryParse(Unprotect(payr.amount), out comrat);
                            totded += comrat;
                        }


                        if (payr.leave_absence != null) payr.leave_absence.absence = absd;

                        if (payr.Leave != null)
                        {
                            payr.Leave.days = lowp;
                        }

                        var gross1 = 0d;

                        double.TryParse(Unprotect(payr.contract.salary_details), out gross1);
                        var TLWOP1 = (absd + lowp) * (gross1 * 12 / 360);
                        totded += TLWOP1;

                        payr.TotalDedution = totded.ToString();

                        var conlist = db.contracts.ToList();
                        var con = conlist.Find(c1 => c1.employee_no == masterFile.employee_id);
                        var bac = 0d;
                        double.TryParse(Unprotect(con.basic), out bac);
                        var basperh = bac * 12 / 360 / 8;
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
                                if (date1.Value.Month == payr.forthemonth.Value.Month) al += 1;
                                date1 = date1.Value.AddDays(1);
                            }

                            al += 1;
                        }

                        payr.TotalOT =
                            (aqf * 1.5 * basperh + aqh * 2.5 * basperh + aqt * 1.25 * basperh + ant)
                            .ToString();
                        var sal = 0d;
                        double.TryParse(Unprotect(con.salary_details), out sal);
                        var tac = 0d;
                        var arr = 0d;
                        var foo = 0d;
                        if (payr.TicketAllowance_ != null && IsBase64Encoded(payr.TicketAllowance_))
                            double.TryParse(Unprotect(payr.TicketAllowance_), out tac);

                        if (payr.Arrears != null && IsBase64Encoded(payr.Arrears))
                            double.TryParse(Unprotect(payr.Arrears), out arr);
                        if (payr.FoodAllow != null && IsBase64Encoded(payr.FoodAllow))
                            double.TryParse(Unprotect(payr.FoodAllow), out foo);
                        payr.totalpayable = (sal + tac + arr + foo).ToString();
                        double.TryParse(payr.totalpayable, out var a);
                        double.TryParse(payr.TotalOT, out var b);
                        double.TryParse(payr.TotalDedution, out var c);
                        var endday = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month,
                            DateTime.DaysInMonth(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month));
                        var stday = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month, 1);
                        var d = 0d;
                        if (payr.amount != null && IsBase64Encoded(payr.amount))
                            double.TryParse(Unprotect(payr.amount), out d);
                        if (absd + lowp >=
                            DateTime.DaysInMonth(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month))
                            payr.NetPay = 0.ToString();
                        else
                            payr.NetPay = (a + b - c - d).ToString();

                        if (save.IsNullOrWhiteSpace())
                            payr.save = false;
                        else
                            payr.save = true;
                        Edit(payr, "edit");
                        R: ;
                        if (payr.Rstate == "R")
                        {
                            paylist.Add(payr);
                        }

                        sav: ;
                        if (!save.IsNullOrWhiteSpace())
                        {
                            var paysavedlist = db.payrollsaveds.ToList();
                            if (paysavedlist.Exists(x =>
                                    x.forthemonth == payr.forthemonth && x.employee_no == payr.master_file.employee_no))
                                goto save_end;
                            var paysave = new payrollsaved();
                            if (payr.master_file != null)
                            {
                                paysave.employee_no = payr.master_file.employee_no;
                                paysave.employee_name = payr.master_file.employee_name;
                                if (payr.master_file.labour_card.Count > 0)
                                {
                                    paysave.establishment = payr.master_file.labour_card.Last().establishment;
                                }
                            }

                            var a1 = 0d;
                            var b1 = 0d;
                            var c1 = 0d;
                            var d1 = 0d;
                            var e1 = 0d;
                            var f1 = 0d;
                            if (payr.contract != null)
                            {
                                if (payr.HolidayOT != null)
                                {
                                    double.TryParse(Unprotect(payr.HolidayOT), out b1);
                                }

                                if (payr.contract != null)
                                {
                                    var bas = "0";
                                    bas = Unprotect(payr.contract.basic);
                                    double.TryParse(bas, out var bas1);
                                    var basperh1 = bas1 * 12 / 360 / 8;
                                    var bdays = b1;
                                    b1 = b1 * 2.5 * basperh1;
                                    if (payr.HolidayOT != null)
                                    {
                                        double.TryParse(Unprotect(payr.OTFriday), out c1);
                                    }

                                    var cdays1 = c1;
                                    c1 = c1 * 1.5 * basperh1;
                                    if (payr.HolidayOT != null)
                                    {
                                        double.TryParse(Unprotect(payr.OTRegular), out a1);
                                    }

                                    var adays1 = a1;
                                    a1 = a1 * 1.25 * basperh1;
                                }

                                if (payr.contract.basic != null)
                                    paysave.Basic = payr.contract.basic;
                                if (payr.contract.housing_allowance != null)
                                    paysave.CHouseAllow = payr.contract.housing_allowance;
                                if (payr.contract.transportation_allowance != null)
                                    paysave.CTransportationAllowance = payr.contract.transportation_allowance;
                                if (payr.contract.food_allowance != null)
                                    paysave.CFoodAllow = payr.contract.food_allowance;
                                if (payr.amount != null)
                                    paysave.amount = payr.amount;
                                if (payr.ded_add != null)
                                    paysave.ded_add = payr.ded_add;
                                if (payr.contract.salary_details != null)
                                {
                                    paysave.Gross = payr.contract.salary_details;
                                    double.TryParse(Unprotect(payr.contract.salary_details), out d1);
                                    double.TryParse(Unprotect(payr.TotalOT), out e1);
                                    paysave.Grosstotal = Protect((d1 + e1).ToString());
                                }
                            }

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
                            paysave.TotalOT = payr.TotalOT;
                            paysave.cashAdvances = payr.cashAdvances;
                            paysave.HouseAllow = payr.HouseAllow;
                            paysave.TransportationAllowance_ = payr.TransportationAllowance_;
                            paysave.FoodAllow = payr.FoodAllow;
                            paysave.Timekeeping = payr.Timekeeping;
                            paysave.Communication = payr.Communication;
                            paysave.TrafficFines = payr.TrafficFines;
                            if (payr.leave_absence != null)
                                paysave.Absents = (int?)payr.leave_absence.absence;
                            else
                                paysave.Absents = 0;

                            if (payr.Leave != null)
                                paysave.LWOP = payr.Leave.days;
                            else
                                paysave.LWOP = 0;

                            if (payr.amount != null)
                                paysave.amount = payr.amount;
                            var gross = 0d;
                            double.TryParse(Unprotect(payr.contract.salary_details), out gross);
                            var TLWOP = (paysave.Absents + paysave.LWOP) * (gross * 12 / 360);
                            paysave.TotalLWOP = TLWOP.ToString();
                            paysave.others = payr.others;
                            paysave.TotalDedution = payr.TotalDedution;
                            paysave.NetPay = payr.NetPay;
                            paysave.remarks = payr.remarks;
                            paysave.forthemonth = payr.forthemonth;
                            payr.save = true;
                            db.Entry(payr).State = EntityState.Modified;
                            db.SaveChanges();
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
                        payr.Rstate = "R";
                        if (masterFile.contracts.Count != 0 &&
                            (masterFile.last_working_day == null ||
                             masterFile.last_working_day.Value.Month != month.Value.Month))
                        {
                            var conlist = db.contracts.ToList();
                            var alist1 = db.contracts.OrderByDescending(e => e.date_changed).ToList();
                            var afinallist1 = new List<contract>();
                            foreach (var file in alist1)
                            {
                                if (afinallist1.Count == 0) afinallist1.Add(file);

                                if (!afinallist1.Exists(x => x.employee_no == file.employee_no)) afinallist1.Add(file);
                            }

                            conlist = afinallist1;
                            payr.contract = conlist.Find(c => c.employee_no == masterFile.employee_id);
                            var con = conlist.Find(c => c.employee_no == masterFile.employee_id);

//                        var leave1 = db.Leaves.Where(
//                            x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
//                                                                         && x.Start_leave >= payr.forthemonth
//                                                                         && x.Start_leave <= endmo
//                                                                         && x.End_leave >= payr.forthemonth
//                                                                         && x.End_leave <= endmo).ToList();
                            var leavedate1 = new DateTime();
                            if (payr.forthemonth.Value.Month == 1)
                            {
                                leavedate1 = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                            }
                            else
                            {
                                leavedate1 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1,
                                    21);
                            }

                            var leavedateend = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month,
                                20);
                            leavedateend = leavedateend.AddDays(1);
                            var leave1 = new List<Leave>();
                            var leave2 = new List<Leave>();
                            var abslist1 = new List<leave_absence>();
                            var currentyear = DateTime.Today.Year;
                            var sickleaveind = new List<Leave>();
                            var sickleavenonind = new List<Leave>();
                            var sickleaveindhp = new List<Leave>();
                            var sickleavenonindhp = new List<Leave>();
                            var sickleaveindup = new List<Leave>();
                            var sickleavenonindup = new List<Leave>();
                            var maternityleave = new List<Leave>();
                            var maternityleavehp = new List<Leave>();
                            var maternityleaveup = new List<Leave>();
                            var yearstart = new DateTime(currentyear, 1, 1);
                            var yearend = new DateTime(currentyear, 12, 31);
                            var sickleaveindlist = new List<Leave>();
                            var sickleavenonindlist = new List<Leave>();
                            var datecount = yearstart;
                            var slindcount = 0d;
                            var slindcounthalfex = 0d;
                            var slnonindcount = 0d;
                            var slnonindcounthalfex = 0d;
                            var mlcount = 0d;
                            var mlcounthalfex = 0d;
                            do
                            {
                                var sickleaveindlist1 = db.Leaves.Where(
                                    x => x.Employee_id == masterFile.employee_id && x.leave_type == "7"
                                        && x.Start_leave <= datecount
                                        && x.End_leave >= datecount).ToList();
                                var sickleavenonindlist1 = db.Leaves.Where(
                                    x => x.Employee_id == masterFile.employee_id && x.leave_type == "2"
                                        && x.Start_leave <= datecount
                                        && x.End_leave >= datecount).ToList();
                                var maternityleavelist1 = db.Leaves.Where(
                                    x => x.Employee_id == masterFile.employee_id && x.leave_type == "4"
                                        && x.Start_leave <= datecount
                                        && x.End_leave >= datecount).ToList();
                                foreach (var leaf in sickleaveindlist1)
                                {
                                    slindcount++;
                                    slindcounthalfex++;
                                    if (!sickleaveind.Exists(x => x.Id == leaf.Id))
                                    {
                                        sickleaveind.Add(leaf);
                                        if (leaf.half)
                                        {
                                            slindcount -= 0.5;
                                            slindcounthalfex -= 0.5;
                                        }
                                    }

                                    if (slindcount > 180)
                                    {
                                        var slindcounttemp = slindcount - 180;
                                        var slindcounthalfextemp = slindcounthalfex - 180;
                                        if (slindcounttemp > 180)
                                        {
                                            if (sickleaveindup.Exists(x => x.Start_leave == leaf.Start_leave))
                                            {
                                                var daysepleave = new Leave();
                                                daysepleave.Start_leave = datecount;
                                                daysepleave.End_leave = datecount;
                                                daysepleave.leave_type = leaf.leave_type;
                                                if ((slindcounthalfextemp % 1) == 0)
                                                {
                                                    daysepleave.half = false;
                                                }
                                                else
                                                {
                                                    daysepleave.half = true;
                                                    slindcounthalfextemp -= 0.5;
                                                }

                                                sickleaveindup.Add(daysepleave);
                                            }
                                        }
                                        else
                                        {
                                            if (sickleaveindhp.Exists(x => x.Start_leave == leaf.Start_leave))
                                            {
                                                var daysepleave = new Leave();
                                                daysepleave.Start_leave = datecount;
                                                daysepleave.End_leave = datecount;
                                                daysepleave.leave_type = leaf.leave_type;
                                                if ((slindcounthalfex % 1) == 0)
                                                {
                                                    daysepleave.half = false;
                                                }
                                                else
                                                {
                                                    daysepleave.half = true;
                                                    slindcounthalfex -= 0.5;
                                                }

                                                sickleaveindhp.Add(daysepleave);
                                            }
                                        }
                                    }
                                }

                                foreach (var leaf in sickleavenonindlist1)
                                {
                                    slnonindcount++;
                                    slnonindcounthalfex++;
                                    if (!sickleavenonind.Exists(x => x.Id == leaf.Id))
                                    {
                                        sickleavenonind.Add(leaf);
                                        if (leaf.half)
                                        {
                                            slnonindcount -= 0.5;
                                            slnonindcounthalfex -= 0.5;
                                        }
                                    }

                                    if (slnonindcount > 15)
                                    {
                                        var slnonindcounttemp = slindcount - 15;
                                        var slnonindcounthalfextemp = slnonindcounthalfex - 15;
                                        if (slnonindcounttemp > 30)
                                        {
                                            if (sickleaveindup.Exists(x => x.Start_leave == leaf.Start_leave))
                                            {
                                                var daysepleave = new Leave();
                                                daysepleave.Start_leave = datecount;
                                                daysepleave.End_leave = datecount;
                                                daysepleave.leave_type = leaf.leave_type;
                                                if ((slnonindcounthalfextemp % 1) == 0)
                                                {
                                                    daysepleave.half = false;
                                                }
                                                else
                                                {
                                                    daysepleave.half = true;
                                                    slnonindcounthalfextemp -= 0.5;
                                                }

                                                sickleaveindup.Add(daysepleave);
                                            }
                                        }
                                        else
                                        {
                                            if (sickleaveindhp.Exists(x => x.Start_leave == leaf.Start_leave))
                                            {
                                                var daysepleave = new Leave();
                                                daysepleave.Start_leave = datecount;
                                                daysepleave.End_leave = datecount;
                                                daysepleave.leave_type = leaf.leave_type;
                                                if ((slnonindcounthalfex % 1) == 0)
                                                {
                                                    daysepleave.half = false;
                                                }
                                                else
                                                {
                                                    daysepleave.half = true;
                                                    slnonindcounthalfex -= 0.5;
                                                }

                                                sickleaveindhp.Add(daysepleave);
                                            }
                                        }
                                    }
                                }

                                foreach (var leaf in maternityleavelist1)
                                {
                                    mlcount++;
                                    mlcounthalfex++;
                                    if (!maternityleave.Exists(x => x.Id == leaf.Id))
                                    {
                                        maternityleave.Add(leaf);
                                        if (leaf.half)
                                        {
                                            mlcount -= 0.5;
                                            mlcounthalfex -= 0.5;
                                        }
                                    }

                                    if (mlcount > 45)
                                    {
                                        var mlcounttemp = slindcount - 45;
                                        var mlcounthalfextemp = mlcounthalfex - 45;
                                        if (mlcounttemp > 15)
                                        {
                                            if (maternityleaveup.Exists(x => x.Start_leave == leaf.Start_leave))
                                            {
                                                var daysepleave = new Leave();
                                                daysepleave.Start_leave = datecount;
                                                daysepleave.End_leave = datecount;
                                                daysepleave.leave_type = leaf.leave_type;
                                                if ((mlcounthalfextemp % 1) == 0)
                                                {
                                                    daysepleave.half = false;
                                                }
                                                else
                                                {
                                                    daysepleave.half = true;
                                                    mlcounthalfextemp -= 0.5;
                                                }

                                                maternityleaveup.Add(daysepleave);
                                            }
                                        }
                                        else
                                        {
                                            if (maternityleavehp.Exists(x => x.Start_leave == leaf.Start_leave))
                                            {
                                                var daysepleave = new Leave();
                                                daysepleave.Start_leave = datecount;
                                                daysepleave.End_leave = datecount;
                                                daysepleave.leave_type = leaf.leave_type;
                                                if ((mlcounthalfex % 1) == 0)
                                                {
                                                    daysepleave.half = false;
                                                }
                                                else
                                                {
                                                    daysepleave.half = true;
                                                    mlcounthalfex -= 0.5;
                                                }

                                                maternityleavehp.Add(daysepleave);
                                            }
                                        }
                                    }
                                }

                                datecount = datecount.AddDays(1);
                            } while (datecount < yearend);

                            var absd = 0;
                            var mlcounthp = 0d;
                            var mlcountup = 0d;
                            var slindcounthp = 0d;
                            var slindcountup = 0d;
                            var slnonindcounthp = 0d;
                            var slnonindcountup = 0d;
                            do
                            {
                                var leave1_1 = db.Leaves.Where(
                                    x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
                                        && x.Start_leave <= leavedate1
                                        && x.End_leave >= leavedate1).ToList();
                                var leave2_1 = db.Leaves.Where(
                                    x => x.Employee_id == masterFile.employee_id && x.Start_leave <= leavedate1
                                        && x.End_leave >= leavedate1).ToList();
                                var abslist1_1 = db.leave_absence.Where(
                                    x => x.Employee_id == masterFile.employee_id && x.fromd <= leavedate1
                                        && x.tod >= leavedate1).ToList();

                                if (maternityleavehp.Exists(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                                {
                                    var temp = maternityleavehp.Find(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                    mlcounthp++;
                                    if (temp.half)
                                    {
                                        mlcounthp -= 0.5;
                                    }
                                }

                                if (maternityleaveup.Exists(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                                {
                                    var temp = maternityleaveup.Find(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                    mlcountup++;
                                    if (temp.half)
                                    {
                                        mlcountup -= 0.5;
                                    }
                                }

                                if (sickleaveindhp.Exists(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                                {
                                    var temp = sickleaveindhp.Find(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                    slindcounthp++;
                                    if (temp.half)
                                    {
                                        slindcounthp -= 0.5;
                                    }
                                }

                                if (sickleaveindup.Exists(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                                {
                                    var temp = sickleaveindup.Find(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                    slindcountup++;
                                    if (temp.half)
                                    {
                                        slindcountup -= 0.5;
                                    }
                                }

                                if (sickleavenonindhp.Exists(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                                {
                                    var temp = sickleavenonindhp.Find(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                    slnonindcounthp++;
                                    if (temp.half)
                                    {
                                        slnonindcounthp -= 0.5;
                                    }
                                }

                                if (sickleavenonindup.Exists(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                                {
                                    var temp = sickleavenonindup.Find(x =>
                                        x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                                    slnonindcountup++;
                                    if (temp.half)
                                    {
                                        slnonindcountup -= 0.5;
                                    }
                                }

                                foreach (var leaf in leave1_1)
                                    if (!leave1.Exists(x => x.Id == leaf.Id))
                                        leave1.Add(leaf);
                                foreach (var leaf1 in abslist1_1)
                                    if (!leave1.Exists(x => x.Id == leaf1.Id))
                                        abslist1.Add(leaf1);
                                foreach (var leaf in leave2_1)
                                    if (!leave2.Exists(x => x.Id == leaf.Id))
                                        leave2.Add(leaf);

                                leavedate1 = leavedate1.AddDays(1);
                            } while (leavedate1 < leavedateend);

//                        var leave2 = db.Leaves.Where(
//                            x => x.Employee_id == masterFile.employee_id && x.Start_leave >= payr.forthemonth
//                                                                         && x.Start_leave <= endmo
//                                                                         && x.End_leave >= payr.forthemonth
//                                                                         && x.End_leave <= endmo).ToList();
                            var lowp = 0;
                            var datediff1 = new DateTime();
                            var datediff2 = new DateTime();
                            if (payr.forthemonth.Value.Month == 1)
                            {
                                datediff1 = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                            }
                            else
                            {
                                datediff1 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1,
                                    21);
                            }

                            datediff2 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month, 20);
                            var datediff = (datediff1 - datediff2).Days;
                            foreach (var leaf in leave1)
                            {
                                var dif = leaf.End_leave - leaf.Start_leave;
                                var date1 = leaf.Start_leave;
                                var date2 = leaf.End_leave;
                                while (date1 != date2)
                                {
                                    if (payr.forthemonth.Value.Month == 1)
                                    {
                                        if (date1 >= datediff1 && date1 < datediff2) lowp += 1;
                                    }
                                    else
                                    {
                                        if (date1 >= datediff1 && date1 < datediff2) lowp += 1;
                                    }

                                    date1 = date1.Value.AddDays(1);
                                }

                                var daysinm = DateTime.DaysInMonth(payr.forthemonth.Value.Year,
                                    payr.forthemonth.Value.Month);
                                if (lowp != datediff) lowp += 1;
                                //lowp += dif.Value.Days + 1;
                            }

                            foreach (var absence in abslist1)
                            {
                                var dif = absence.tod - absence.fromd;
                                var date1 = absence.fromd;
                                var date2 = absence.tod;
                                while (date1 != date2)
                                {
                                    if (payr.forthemonth.Value.Month == 1)
                                    {
                                        if (date1 >= datediff1 && date1 < datediff2) absd += 1;
                                    }
                                    else
                                    {
                                        if (date1 >= datediff1 && date1 < datediff2) absd += 1;
                                    }

                                    date1 = date1.Value.AddDays(1);
                                }

                                var daysinm = DateTime.DaysInMonth(payr.forthemonth.Value.Year,
                                    payr.forthemonth.Value.Month);
                                if (absd != datediff) absd += 1;
                            }

                            var lab1 = lab.Find(x => x.EMPNO == masterFile.employee_no);
                            var aqt = 0L;
                            var aqf = 0L;
                            var aqh = 0L;
                            if (lab1 == null) goto tos;
                            var attd1 = att.FindAll(x => x.EmpID == lab1.ID).ToList();
                            var attd = new List<Attendance>();
                            var attd1_1 = att.FindAll(x => x.EmpID == lab1.ID).ToList();
                            var attd_1 = new List<Attendance>();
                            foreach (var atq in attd1)
                            {
                                var attq1 = new Attendance();
                                if (attd.Exists(x => x.EmpID == atq.EmpID))
                                {
                                    attq1 = attd.First();
                                    attd.Remove(attq1);
                                    long.TryParse(attq1.C1, out var hrs1);
                                    long.TryParse(atq.C1, out var hrs2);
                                    var sum = hrs1 + hrs2;
                                    attq1.C1 = sum.ToString();
                                    long.TryParse(attq1.C2, out hrs1);
                                    long.TryParse(atq.C2, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C2 = sum.ToString();
                                    long.TryParse(attq1.C3, out hrs1);
                                    long.TryParse(atq.C3, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C3 = sum.ToString();
                                    long.TryParse(attq1.C4, out hrs1);
                                    long.TryParse(atq.C4, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C4 = sum.ToString();
                                    long.TryParse(attq1.C5, out hrs1);
                                    long.TryParse(atq.C5, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C5 = sum.ToString();
                                    long.TryParse(attq1.C6, out hrs1);
                                    long.TryParse(atq.C6, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C6 = sum.ToString();
                                    long.TryParse(attq1.C7, out hrs1);
                                    long.TryParse(atq.C7, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C7 = sum.ToString();
                                    long.TryParse(attq1.C8, out hrs1);
                                    long.TryParse(atq.C8, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C8 = sum.ToString();
                                    long.TryParse(attq1.C9, out hrs1);
                                    long.TryParse(atq.C9, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C9 = sum.ToString();
                                    long.TryParse(attq1.C10, out hrs1);
                                    long.TryParse(atq.C10, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C10 = sum.ToString();
                                    long.TryParse(attq1.C11, out hrs1);
                                    long.TryParse(atq.C11, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C11 = sum.ToString();
                                    long.TryParse(attq1.C12, out hrs1);
                                    long.TryParse(atq.C12, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C12 = sum.ToString();
                                    long.TryParse(attq1.C13, out hrs1);
                                    long.TryParse(atq.C13, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C13 = sum.ToString();
                                    long.TryParse(attq1.C14, out hrs1);
                                    long.TryParse(atq.C14, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C14 = sum.ToString();
                                    long.TryParse(attq1.C15, out hrs1);
                                    long.TryParse(atq.C15, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C15 = sum.ToString();
                                    long.TryParse(attq1.C16, out hrs1);
                                    long.TryParse(atq.C16, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C16 = sum.ToString();
                                    long.TryParse(attq1.C17, out hrs1);
                                    long.TryParse(atq.C17, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C17 = sum.ToString();
                                    long.TryParse(attq1.C18, out hrs1);
                                    long.TryParse(atq.C18, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C18 = sum.ToString();
                                    long.TryParse(attq1.C19, out hrs1);
                                    long.TryParse(atq.C19, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C19 = sum.ToString();
                                    long.TryParse(attq1.C20, out hrs1);
                                    long.TryParse(atq.C20, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C20 = sum.ToString();
                                    long.TryParse(attq1.C21, out hrs1);
                                    long.TryParse(atq.C21, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C21 = sum.ToString();
                                    long.TryParse(attq1.C22, out hrs1);
                                    long.TryParse(atq.C22, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C22 = sum.ToString();
                                    long.TryParse(attq1.C23, out hrs1);
                                    long.TryParse(atq.C23, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C23 = sum.ToString();
                                    long.TryParse(attq1.C24, out hrs1);
                                    long.TryParse(atq.C24, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C24 = sum.ToString();
                                    long.TryParse(attq1.C25, out hrs1);
                                    long.TryParse(atq.C25, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C25 = sum.ToString();
                                    long.TryParse(attq1.C26, out hrs1);
                                    long.TryParse(atq.C26, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C26 = sum.ToString();
                                    long.TryParse(attq1.C27, out hrs1);
                                    long.TryParse(atq.C27, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C27 = sum.ToString();
                                    long.TryParse(attq1.C28, out hrs1);
                                    long.TryParse(atq.C28, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C28 = sum.ToString();
                                    long.TryParse(attq1.C29, out hrs1);
                                    long.TryParse(atq.C29, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C29 = sum.ToString();
                                    long.TryParse(attq1.C30, out hrs1);
                                    long.TryParse(atq.C30, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C30 = sum.ToString();
                                    long.TryParse(attq1.C31, out hrs1);
                                    long.TryParse(atq.C31, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C31 = sum.ToString();
                                    attd.Add(attq1);
                                }
                                else
                                {
                                    attd.Add(atq);
                                }
                            }

                            foreach (var atq in attd1_1)
                            {
                                var attq1 = new Attendance();
                                if (attd_1.Exists(x => x.EmpID == atq.EmpID))
                                {
                                    attq1 = attd_1.First();
                                    attd_1.Remove(attq1);
                                    long.TryParse(attq1.C1, out var hrs1);
                                    long.TryParse(atq.C1, out var hrs2);
                                    var sum = hrs1 + hrs2;
                                    attq1.C1 = sum.ToString();
                                    long.TryParse(attq1.C2, out hrs1);
                                    long.TryParse(atq.C2, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C2 = sum.ToString();
                                    long.TryParse(attq1.C3, out hrs1);
                                    long.TryParse(atq.C3, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C3 = sum.ToString();
                                    long.TryParse(attq1.C4, out hrs1);
                                    long.TryParse(atq.C4, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C4 = sum.ToString();
                                    long.TryParse(attq1.C5, out hrs1);
                                    long.TryParse(atq.C5, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C5 = sum.ToString();
                                    long.TryParse(attq1.C6, out hrs1);
                                    long.TryParse(atq.C6, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C6 = sum.ToString();
                                    long.TryParse(attq1.C7, out hrs1);
                                    long.TryParse(atq.C7, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C7 = sum.ToString();
                                    long.TryParse(attq1.C8, out hrs1);
                                    long.TryParse(atq.C8, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C8 = sum.ToString();
                                    long.TryParse(attq1.C9, out hrs1);
                                    long.TryParse(atq.C9, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C9 = sum.ToString();
                                    long.TryParse(attq1.C10, out hrs1);
                                    long.TryParse(atq.C10, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C10 = sum.ToString();
                                    long.TryParse(attq1.C11, out hrs1);
                                    long.TryParse(atq.C11, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C11 = sum.ToString();
                                    long.TryParse(attq1.C12, out hrs1);
                                    long.TryParse(atq.C12, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C12 = sum.ToString();
                                    long.TryParse(attq1.C13, out hrs1);
                                    long.TryParse(atq.C13, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C13 = sum.ToString();
                                    long.TryParse(attq1.C14, out hrs1);
                                    long.TryParse(atq.C14, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C14 = sum.ToString();
                                    long.TryParse(attq1.C15, out hrs1);
                                    long.TryParse(atq.C15, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C15 = sum.ToString();
                                    long.TryParse(attq1.C16, out hrs1);
                                    long.TryParse(atq.C16, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C16 = sum.ToString();
                                    long.TryParse(attq1.C17, out hrs1);
                                    long.TryParse(atq.C17, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C17 = sum.ToString();
                                    long.TryParse(attq1.C18, out hrs1);
                                    long.TryParse(atq.C18, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C18 = sum.ToString();
                                    long.TryParse(attq1.C19, out hrs1);
                                    long.TryParse(atq.C19, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C19 = sum.ToString();
                                    long.TryParse(attq1.C20, out hrs1);
                                    long.TryParse(atq.C20, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C20 = sum.ToString();
                                    long.TryParse(attq1.C21, out hrs1);
                                    long.TryParse(atq.C21, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C21 = sum.ToString();
                                    long.TryParse(attq1.C22, out hrs1);
                                    long.TryParse(atq.C22, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C22 = sum.ToString();
                                    long.TryParse(attq1.C23, out hrs1);
                                    long.TryParse(atq.C23, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C23 = sum.ToString();
                                    long.TryParse(attq1.C24, out hrs1);
                                    long.TryParse(atq.C24, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C24 = sum.ToString();
                                    long.TryParse(attq1.C25, out hrs1);
                                    long.TryParse(atq.C25, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C25 = sum.ToString();
                                    long.TryParse(attq1.C26, out hrs1);
                                    long.TryParse(atq.C26, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C26 = sum.ToString();
                                    long.TryParse(attq1.C27, out hrs1);
                                    long.TryParse(atq.C27, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C27 = sum.ToString();
                                    long.TryParse(attq1.C28, out hrs1);
                                    long.TryParse(atq.C28, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C28 = sum.ToString();
                                    long.TryParse(attq1.C29, out hrs1);
                                    long.TryParse(atq.C29, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C29 = sum.ToString();
                                    long.TryParse(attq1.C30, out hrs1);
                                    long.TryParse(atq.C30, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C30 = sum.ToString();
                                    long.TryParse(attq1.C31, out hrs1);
                                    long.TryParse(atq.C31, out hrs2);
                                    sum = hrs1 + hrs2;
                                    attq1.C31 = sum.ToString();
                                    attd_1.Add(attq1);
                                }
                                else
                                {
                                    attd_1.Add(atq);
                                }
                            }

                            var attd_final = new List<Attendance>();
                            var attd_final1 = new List<Attendance>();
                            attd_final.AddRange(attd);
                            attd_final.AddRange(attd_1);
                            foreach (var aq1 in attd_final.OrderByDescending(x => x.MainTimeSheet.TMonth)
                                         .ThenBy(x => x.EmpID))
                            {
                                var empnam = new Attendance();
                                if (aq1.MainTimeSheet.TMonth.Month == 1)
                                {
                                    empnam = attd_final.Find(x =>
                                        x.EmpID == aq1.EmpID &&
                                        x.MainTimeSheet.TMonth.Month == 12);
                                }
                                else
                                {
                                    empnam = attd_final.Find(x =>
                                        x.EmpID == aq1.EmpID &&
                                        x.MainTimeSheet.TMonth.Month == aq1.MainTimeSheet.TMonth.Month - 1);
                                }

                                if (empnam != null)
                                {
                                    aq1.C21 = empnam.C21;
                                    aq1.C22 = empnam.C22;
                                    aq1.C23 = empnam.C23;
                                    aq1.C24 = empnam.C24;
                                    aq1.C25 = empnam.C25;
                                    aq1.C26 = empnam.C26;
                                    aq1.C27 = empnam.C27;
                                    aq1.C28 = empnam.C28;
                                    aq1.C29 = empnam.C29;
                                    aq1.C30 = empnam.C30;
                                    aq1.C31 = empnam.C31;
                                    if (!attd_final1.Exists(x => x.EmpID == aq1.EmpID))
                                    {
                                        attd_final1.Add(aq1);
                                    }
                                }
                            }

                            var newdate = new DateTime();
                            if (payr.forthemonth.Value.Month == 1)
                            {
                                newdate = new DateTime(payr.forthemonth.Value.Year - 1, 12, 1);
                            }
                            else
                            {
                                newdate = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1,
                                    1);
                            }

                            var datestart = new DateTime();
                            var dateend = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month, 20);
                            if (payr.forthemonth.Value.Month == 1)
                            {
                                datestart = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                            }
                            else
                            {
                                datestart = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1,
                                    21);
                            }

                            var hlistday = new List<int>();
                            var hlistday1 = GetAllholi(month.Value);
                            foreach (var i in hlistday1)
                            {
                                var dt1 = new DateTime(dateend.Year, dateend.Month, i);
                                if (datestart <= dt1 && dt1 <= dateend)
                                {
                                    hlistday.Add(i);
                                }
                            }

                            hlistday1 = GetAllholi(newdate);
                            foreach (var i in hlistday1)
                            {
                                var dt1 = new DateTime(datestart.Year, datestart.Month, i);
                                if (datestart <= dt1 && dt1 <= dateend)
                                {
                                    hlistday.Add(i);
                                }
                            }

                            foreach (var aq in attd_final1)
                            {
                                var fdaylist = new List<int>();
                                var fdaylist1 = GetAll(month.Value, aq.MainTimeSheet.Project);
                                foreach (var i in fdaylist1)
                                {
                                    var dt1 = new DateTime(dateend.Year, dateend.Month, i);
                                    if (datestart <= dt1 && dt1 <= dateend)
                                    {
                                        fdaylist.Add(i);
                                    }
                                }

                                fdaylist1 = GetAll(newdate, aq.MainTimeSheet.Project);
                                foreach (var i in fdaylist1)
                                {
                                    var dt1 = new DateTime(datestart.Year, datestart.Month, i);
                                    if (datestart <= dt1 && dt1 <= dateend)
                                    {
                                        fdaylist.Add(i);
                                    }
                                }

                                var x = 0L;
                                var x1 = 0L;
                                var leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                var y = 0L;
                                if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) &&
                                    !hlistday.Exists(f => f.Equals(1)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C1, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) &&
                                    !hlistday.Exists(f => f.Equals(2)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C2, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) &&
                                    !hlistday.Exists(f => f.Equals(3)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C3, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) &&
                                    !hlistday.Exists(f => f.Equals(4)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C4, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) &&
                                    !hlistday.Exists(f => f.Equals(5)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C5, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) &&
                                    !hlistday.Exists(f => f.Equals(6)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C6, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) &&
                                    !hlistday.Exists(f => f.Equals(7)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C7, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) &&
                                    !hlistday.Exists(f => f.Equals(8)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C8, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) &&
                                    !hlistday.Exists(f => f.Equals(9)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C9, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) &&
                                    !hlistday.Exists(f => f.Equals(10)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C10, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) &&
                                    !hlistday.Exists(f => f.Equals(11)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C11, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) &&
                                    !hlistday.Exists(f => f.Equals(12)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C12, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) &&
                                    !hlistday.Exists(f => f.Equals(13)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C13, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) &&
                                    !hlistday.Exists(f => f.Equals(14)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C14, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) &&
                                    !hlistday.Exists(f => f.Equals(15)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C15, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) &&
                                    !hlistday.Exists(f => f.Equals(16)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C16, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) &&
                                    !hlistday.Exists(f => f.Equals(17)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C17, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) &&
                                    !hlistday.Exists(f => f.Equals(18)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C18, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) &&
                                    !hlistday.Exists(f => f.Equals(19)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C19, out y);
                                    if (y > 8) x += y - 8;
                                }

                                if (leavedate.Month == 1)
                                {
                                    leavedate = new DateTime(month.Value.Year - 1, 12, 21);
                                }
                                else
                                {
                                    leavedate = new DateTime(month.Value.Year, month.Value.Month - 1, 21);
                                }

                                if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) &&
                                    !hlistday.Exists(f => f.Equals(20)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C20, out y);
                                    if (y > 8) x += y - 8;
                                }

                                if (leavedate.Month == 1)
                                {
                                    leavedate = new DateTime(month.Value.Year - 1, 12, 21);
                                }
                                else
                                {
                                    leavedate = new DateTime(month.Value.Year, month.Value.Month - 1, 21);
                                }


                                if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) &&
                                    !hlistday.Exists(f => f.Equals(21)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C21, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) &&
                                    !hlistday.Exists(f => f.Equals(22)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C22, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) &&
                                    !hlistday.Exists(f => f.Equals(23)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C23, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) &&
                                    !hlistday.Exists(f => f.Equals(24)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C24, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) &&
                                    !hlistday.Exists(f => f.Equals(25)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C25, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) &&
                                    !hlistday.Exists(f => f.Equals(26)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C26, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) &&
                                    !hlistday.Exists(f => f.Equals(27)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C27, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) &&
                                    !hlistday.Exists(f => f.Equals(28)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C28, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) &&
                                    !hlistday.Exists(f => f.Equals(29)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C29, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) &&
                                    !hlistday.Exists(f => f.Equals(30)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C30, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) &&
                                    !hlistday.Exists(f => f.Equals(31)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C31, out y);
                                    if (y > 8) x += y - 8;
                                }

                                leavedate = leavedate.AddDays(1);

                                leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                var y1 = 0L;
                                x1 = 0L;
                                if (aq.C1 != null && fdaylist.Exists(g => g.Equals(1)) &&
                                    !hlistday.Exists(f => f.Equals(1)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C1, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C2 != null && fdaylist.Exists(g => g.Equals(2)) &&
                                    !hlistday.Exists(f => f.Equals(2)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C2, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C3 != null && fdaylist.Exists(g => g.Equals(3)) &&
                                    !hlistday.Exists(f => f.Equals(3)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C3, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C4 != null && fdaylist.Exists(g => g.Equals(4)) &&
                                    !hlistday.Exists(f => f.Equals(4)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C4, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C5 != null && fdaylist.Exists(g => g.Equals(5)) &&
                                    !hlistday.Exists(f => f.Equals(5)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C5, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C6 != null && fdaylist.Exists(g => g.Equals(6)) &&
                                    !hlistday.Exists(f => f.Equals(6)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C6, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C7 != null && fdaylist.Exists(g => g.Equals(7)) &&
                                    !hlistday.Exists(f => f.Equals(7)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C7, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C8 != null && fdaylist.Exists(g => g.Equals(8)) &&
                                    !hlistday.Exists(f => f.Equals(8)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C8, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C9 != null && fdaylist.Exists(g => g.Equals(9)) &&
                                    !hlistday.Exists(f => f.Equals(9)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C9, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C10 != null && fdaylist.Exists(g => g.Equals(10)) &&
                                    !hlistday.Exists(f => f.Equals(10)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C10, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C11 != null && fdaylist.Exists(g => g.Equals(11)) &&
                                    !hlistday.Exists(f => f.Equals(11)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C11, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C12 != null && fdaylist.Exists(g => g.Equals(12)) &&
                                    !hlistday.Exists(f => f.Equals(12)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C12, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C13 != null && fdaylist.Exists(g => g.Equals(13)) &&
                                    !hlistday.Exists(f => f.Equals(13)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C13, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C14 != null && fdaylist.Exists(g => g.Equals(14)) &&
                                    !hlistday.Exists(f => f.Equals(14)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C14, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C15 != null && fdaylist.Exists(g => g.Equals(15)) &&
                                    !hlistday.Exists(f => f.Equals(15)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C15, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C16 != null && fdaylist.Exists(g => g.Equals(16)) &&
                                    !hlistday.Exists(f => f.Equals(16)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C16, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C17 != null && fdaylist.Exists(g => g.Equals(17)) &&
                                    !hlistday.Exists(f => f.Equals(17)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C17, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C18 != null && fdaylist.Exists(g => g.Equals(18)) &&
                                    !hlistday.Exists(f => f.Equals(18)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C18, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C19 != null && fdaylist.Exists(g => g.Equals(19)) &&
                                    !hlistday.Exists(f => f.Equals(19)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C19, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C20 != null && fdaylist.Exists(g => g.Equals(20)) &&
                                    !hlistday.Exists(f => f.Equals(20)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C20, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                if (leavedate.Month == 1)
                                {
                                    leavedate = new DateTime(month.Value.Year - 1, 12, 21);
                                }
                                else
                                {
                                    leavedate = new DateTime(month.Value.Year, month.Value.Month - 1, 21);
                                }


                                if (aq.C21 != null && fdaylist.Exists(g => g.Equals(21)) &&
                                    !hlistday.Exists(f => f.Equals(21)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C21, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C22 != null && fdaylist.Exists(g => g.Equals(22)) &&
                                    !hlistday.Exists(f => f.Equals(22)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C22, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C23 != null && fdaylist.Exists(g => g.Equals(23)) &&
                                    !hlistday.Exists(f => f.Equals(23)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C23, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C24 != null && fdaylist.Exists(g => g.Equals(24)) &&
                                    !hlistday.Exists(f => f.Equals(24)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C24, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C25 != null && fdaylist.Exists(g => g.Equals(25)) &&
                                    !hlistday.Exists(f => f.Equals(25)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C25, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C26 != null && fdaylist.Exists(g => g.Equals(26)) &&
                                    !hlistday.Exists(f => f.Equals(26)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C26, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C27 != null && fdaylist.Exists(g => g.Equals(27)) &&
                                    !hlistday.Exists(f => f.Equals(27)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C27, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C28 != null && fdaylist.Exists(g => g.Equals(28)) &&
                                    !hlistday.Exists(f => f.Equals(28)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C28, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C29 != null && fdaylist.Exists(g => g.Equals(29)) &&
                                    !hlistday.Exists(f => f.Equals(29)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C29, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C30 != null && fdaylist.Exists(g => g.Equals(30)) &&
                                    !hlistday.Exists(f => f.Equals(30)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C30, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C31 != null && fdaylist.Exists(g => g.Equals(31)) &&
                                    !hlistday.Exists(f => f.Equals(31)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C31, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = new DateTime(month.Value.Year, month.Value.Month, 1);
                                if (aq.FridayHours.HasValue) aqf += x1;
                                y1 = 0L;
                                x1 = 0L;
                                if (aq.C1 != null && !fdaylist.Exists(g => g.Equals(1)) &&
                                    hlistday.Exists(f => f.Equals(1)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C1, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C2 != null && !fdaylist.Exists(g => g.Equals(2)) &&
                                    hlistday.Exists(f => f.Equals(2)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C2, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C3 != null && !fdaylist.Exists(g => g.Equals(3)) &&
                                    hlistday.Exists(f => f.Equals(3)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C3, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C4 != null && !fdaylist.Exists(g => g.Equals(4)) &&
                                    hlistday.Exists(f => f.Equals(4)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C4, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C5 != null && !fdaylist.Exists(g => g.Equals(5)) &&
                                    hlistday.Exists(f => f.Equals(5)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C5, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C6 != null && !fdaylist.Exists(g => g.Equals(6)) &&
                                    hlistday.Exists(f => f.Equals(6)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C6, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C7 != null && !fdaylist.Exists(g => g.Equals(7)) &&
                                    hlistday.Exists(f => f.Equals(7)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C7, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C8 != null && !fdaylist.Exists(g => g.Equals(8)) &&
                                    hlistday.Exists(f => f.Equals(8)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C8, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C9 != null && !fdaylist.Exists(g => g.Equals(9)) &&
                                    hlistday.Exists(f => f.Equals(9)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C9, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C10 != null && !fdaylist.Exists(g => g.Equals(10)) &&
                                    hlistday.Exists(f => f.Equals(10)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C10, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C11 != null && !fdaylist.Exists(g => g.Equals(11)) &&
                                    hlistday.Exists(f => f.Equals(11)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C11, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C12 != null && !fdaylist.Exists(g => g.Equals(12)) &&
                                    hlistday.Exists(f => f.Equals(12)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C12, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C13 != null && !fdaylist.Exists(g => g.Equals(13)) &&
                                    hlistday.Exists(f => f.Equals(13)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C13, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C14 != null && !fdaylist.Exists(g => g.Equals(14)) &&
                                    hlistday.Exists(f => f.Equals(14)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C14, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C15 != null && !fdaylist.Exists(g => g.Equals(15)) &&
                                    hlistday.Exists(f => f.Equals(15)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C15, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C16 != null && !fdaylist.Exists(g => g.Equals(16)) &&
                                    hlistday.Exists(f => f.Equals(16)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C16, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C17 != null && !fdaylist.Exists(g => g.Equals(17)) &&
                                    hlistday.Exists(f => f.Equals(17)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C17, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C18 != null && !fdaylist.Exists(g => g.Equals(18)) &&
                                    hlistday.Exists(f => f.Equals(18)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C18, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C19 != null && !fdaylist.Exists(g => g.Equals(19)) &&
                                    hlistday.Exists(f => f.Equals(19)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C19, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C20 != null && !fdaylist.Exists(g => g.Equals(20)) &&
                                    hlistday.Exists(f => f.Equals(20)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C20, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                if (leavedate.Month == 1)
                                {
                                    leavedate = new DateTime(month.Value.Year - 1, 12, 21);
                                }
                                else
                                {
                                    leavedate = new DateTime(month.Value.Year, month.Value.Month - 1, 21);
                                }


                                if (aq.C21 != null && !fdaylist.Exists(g => g.Equals(21)) &&
                                    hlistday.Exists(f => f.Equals(21)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C21, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C22 != null && !fdaylist.Exists(g => g.Equals(22)) &&
                                    hlistday.Exists(f => f.Equals(22)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C22, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C23 != null && !fdaylist.Exists(g => g.Equals(23)) &&
                                    hlistday.Exists(f => f.Equals(23)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C23, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C24 != null && !fdaylist.Exists(g => g.Equals(24)) &&
                                    hlistday.Exists(f => f.Equals(24)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C24, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C25 != null && !fdaylist.Exists(g => g.Equals(25)) &&
                                    hlistday.Exists(f => f.Equals(25)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C25, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C26 != null && !fdaylist.Exists(g => g.Equals(26)) &&
                                    hlistday.Exists(f => f.Equals(26)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C26, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C27 != null && !fdaylist.Exists(g => g.Equals(27)) &&
                                    hlistday.Exists(f => f.Equals(27)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C27, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C28 != null && !fdaylist.Exists(g => g.Equals(28)) &&
                                    hlistday.Exists(f => f.Equals(28)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C28, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C29 != null && !fdaylist.Exists(g => g.Equals(29)) &&
                                    hlistday.Exists(f => f.Equals(29)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C29, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C30 != null && !fdaylist.Exists(g => g.Equals(30)) &&
                                    hlistday.Exists(f => f.Equals(30)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
                                {
                                    long.TryParse(aq.C30, out y1);

                                    {
                                        x1 += y1;
                                    }
                                }

                                leavedate = leavedate.AddDays(1);
                                if (aq.C31 != null && !fdaylist.Exists(g => g.Equals(31)) &&
                                    hlistday.Exists(f => f.Equals(31)) &&
                                    !leave2.Exists(z => z.Start_leave <= leavedate && z.End_leave >= leavedate) &&
                                    !leave2.Exists(z =>
                                        z.actual_return_date == null &&
                                        z.End_leave.Value.Month == payr.forthemonth.Value.Month)
                                   )
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
                            var ant = 0d;
                            if (payr.OTNight != null)
                            {
                                double.TryParse(Unprotect(payr.OTNight), out ant);
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
                                if (!payr.Leave.days.HasValue)
                                {
                                    payr.Leave.days = 0;
                                }

                                payr.Leave.days = lowp;
                            }

                            if (mlcounthp > 0 || mlcountup > 0)
                            {
                                payr.LWOP = maternityleave.OrderByDescending(x => x.Start_leave).First().Id;
                                payr.Leave = maternityleave.OrderByDescending(x => x.Start_leave).First();
                                if (!payr.Leave.days.HasValue)
                                {
                                    payr.Leave.days = 0;
                                }

                                if (mlcounthp != 0)
                                {
                                    var temp = 0.5 * (mlcounthp - (mlcounthp % 1)) + (mlcounthp % 1);

                                    payr.Leave.days += (int)temp;
                                    if ((temp % 1) != 0)
                                    {
                                        payr.Leave.half = true;
                                    }
                                    else
                                    {
                                        payr.Leave.half = false;
                                    }
                                }

                                if (mlcountup != 0)
                                {
                                    payr.Leave.days += (int)mlcountup;
                                    if ((mlcountup % 1) != 0)
                                    {
                                        payr.Leave.half = true;
                                    }
                                    else
                                    {
                                        payr.Leave.half = false;
                                    }
                                }
                            }

                            if (slindcountup > 0 || slindcounthp > 0)
                            {
                                payr.LWOP = sickleaveind.OrderByDescending(x => x.Start_leave).First().Id;
                                payr.Leave = sickleaveind.OrderByDescending(x => x.Start_leave).First();
                                if (!payr.Leave.days.HasValue)
                                {
                                    payr.Leave.days = 0;
                                }

                                if (slindcounthp != 0)
                                {
                                    var temp = 0.5 * (slindcounthp - (slindcounthp % 1)) + (slindcounthp % 1);

                                    payr.Leave.days += (int)temp;
                                    if ((temp % 1) != 0)
                                    {
                                        payr.Leave.half = true;
                                    }
                                    else
                                    {
                                        payr.Leave.half = false;
                                    }
                                }

                                if (slindcountup != 0)
                                {
                                    payr.Leave.days += (int)slindcountup;
                                    if ((slindcountup % 1) != 0)
                                    {
                                        payr.Leave.half = true;
                                    }
                                    else
                                    {
                                        payr.Leave.half = false;
                                    }
                                }
                            }

                            if (slnonindcountup > 0 || slnonindcounthp > 0)
                            {
                                payr.LWOP = sickleavenonind.OrderByDescending(x => x.Start_leave).First().Id;
                                payr.Leave = sickleavenonind.OrderByDescending(x => x.Start_leave).First();
                                if (!payr.Leave.days.HasValue)
                                {
                                    payr.Leave.days = 0;
                                }

                                if (slnonindcounthp != 0)
                                {
                                    var temp = 0.5 * (slnonindcounthp - (slnonindcounthp % 1)) + (slnonindcounthp % 1);

                                    payr.Leave.days += (int)temp;
                                    if ((temp % 1) != 0)
                                    {
                                        payr.Leave.half = true;
                                    }
                                    else
                                    {
                                        payr.Leave.half = false;
                                    }
                                }

                                if (mlcountup != 0)
                                {
                                    payr.Leave.days += (int)slnonindcountup;
                                    if ((slnonindcountup % 1) != 0)
                                    {
                                        payr.Leave.half = true;
                                    }
                                    else
                                    {
                                        payr.Leave.half = false;
                                    }
                                }
                            }

                            var bac = 0d;
                            double.TryParse(Unprotect(con.basic), out bac);
                            var basperh = bac * 12 / 360 / 8;
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
                                    if (date1.Value.Month == payr.forthemonth.Value.Month) al += 1;
                                    date1 = date1.Value.AddDays(1);
                                }

                                al += 1;
                            }

                            payr.TotalOT = (aqf * 1.5 * basperh + aqh * 2.5 * basperh + aqt * 1.25 * basperh + ant)
                                .ToString();
                            var sal = 0d;
                            double.TryParse(Unprotect(con.salary_details), out sal);
                            var labs = 0d;
                            var ldays = 0d;
                            if (payr.leave_absence != null) labs = payr.leave_absence.absence.Value;

                            if (payr.Leave != null)
                            {
                                ldays = payr.Leave.days.Value;
                                if (payr.Leave.half)
                                {
                                    ldays += 0.5;
                                }
                            }

                            var TLWOP = (labs + ldays) * (sal * 12 / 360);
                            var fad = 0d;

                            /*var fadamount = 0d;
                            if (payr.contract != null)
                            {
                                var confa = 0d;
                                double.TryParse(Unprotect(payr.contract.food_allowance), out confa);
                                if (confa == 0)
                                {
                                    fadamount = 0;
                                }
                                else
                                {
                                    var mindate = new DateTime();
                                    if (payr.forthemonth.Value.Month == 1)
                                    {
                                        mindate = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                                    }
                                    else
                                    {
                                        mindate = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1,
                                            21);
                                    }

                                    var moutdate = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month,
                                        20);
                                    var minoutlist = new List<Manpowerinoutform>();
                                    if (lab1 != null)
                                    {
                                        do
                                        {
                                            var minout = db1.Manpowerinoutforms
                                                .Where(x => x.check_in < mindate && x.EmpID == lab1.ID && x.camp != "Camp Musaffah").ToList();
                                            foreach (var inout in minout)
                                            {
                                                if (!minoutlist.Exists(
                                                    x => x.EmpID == inout.EmpID && x.Project == inout.Project))
                                                {
                                                    minoutlist.Add(inout);
                                                }
                                            }

                                            mindate = mindate.AddDays(1);
                                        } while (mindate <= moutdate);
                                    }

                                    if (payr.forthemonth.Value.Month == 1)
                                    {
                                        datediff1 = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                                    }
                                    else
                                    {
                                        datediff1 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1,
                                            21);
                                    }

                                    datediff2 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month, 20);
                                    datediff = (datediff2 - datediff1).Days + 1;
                                    var faddays = 0;
                                    var fadextrastop = 0;
                                    foreach (var inout in minoutlist)
                                    {
                                        var date1 = inout.check_in;
                                        var date2 = inout.check_out;
                                        if (date2 == null)
                                        {
                                            date2 = datediff2;
                                        }

                                        while (date1 != date2)
                                        {
                                            if (date1 >= datediff1 && date1 < datediff2) faddays += 1;
                                            date1 = date1.Value.AddDays(1);
                                        }

                                        if (faddays != datediff && fadextrastop > 0)
                                        {
                                            faddays += 1;
                                            fadextrastop = 0;
                                        }
                                    }

                                    var fadamountpday = 250d * 12d / 360d;
                                    //fadamount = fadamountpday * faddays;
                                    fadamount = fadamountpday * (datediff - faddays);
                                    if (fadamount > 250)
                                    {
                                        fadamount = 250;
                                    }
                                }
                            }
                            else
                            {
                                fadamount = 0;
                            }
                            payr.FoodAllow = Protect(fadamount.ToString());*/

                            var tac = 0d;
                            var arr = 0d;
                            var foo = 0d;
                            if (payr.TicketAllowance_ != null && IsBase64Encoded(payr.TicketAllowance_))
                                double.TryParse(Unprotect(payr.TicketAllowance_), out tac);
                            if (payr.Arrears != null && IsBase64Encoded(payr.Arrears))
                                double.TryParse(Unprotect(payr.Arrears), out arr);
                            if (payr.FoodAllow != null && IsBase64Encoded(payr.FoodAllow))
                                double.TryParse(Unprotect(payr.FoodAllow), out foo);
                            payr.totalpayable = (sal + tac + arr).ToString();
                            double.TryParse(payr.totalpayable, out var a);
                            double.TryParse(payr.TotalOT, out var b);
                            payr.TotalDedution = (TLWOP).ToString();
                            //payr.TotalDedution = (TLWOP + fadamount).ToString();
                            double.TryParse(payr.TotalDedution, out var c10);
                            if (labs + ldays >= DateTime.DaysInMonth(payr.forthemonth.Value.Year,
                                    payr.forthemonth.Value.Month))
                                payr.NetPay = 0.ToString();
                            else
                                payr.NetPay = (a + b - c10).ToString();
                            paylist.Add(payr);
                            Create(payr);
                        }
                    }

                var model12 = new paysavedlist();
                var savedlist1 = db.payrollsaveds.ToList();
                var savedlist = savedlist1
                    .FindAll(x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1)).ToList();
                if (savedlist.Count != 0)
                    model12 = new paysavedlist
                    {
                        Payrollsaved = savedlist.OrderBy(x => x.employee_no),
                        Payroll = paylist.OrderBy(x => x.master_file.employee_no)
                    };
                else
                    model12 = new paysavedlist
                    {
                        Payroll = paylist.OrderBy(x => x.master_file.employee_no),
                        Payrollsaved = savedlist.OrderBy(x => x.employee_no)
                    };
                return View(model12);
            }

            var model11 = new paysavedlist
            {
                Payroll = new List<payrole>(), Payrollsaved = new List<payrollsaved>()
            };
            return View(model11);
        }

        public ActionResult index1(DateTime? month, string save, string refresh)
        {
            if (month.HasValue)
            {
                var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed)
                    .ToList();
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

                var paylist = new List<payrole>();
                var lab = db1.LabourMasters.ToList();
                var mts = new List<MainTimeSheet>();
                var mts_1 = new List<MainTimeSheet>();
                var att_1 = new List<Attendance>();
                var att = new List<Attendance>();
                ViewBag.payday = month;
                var paylisteisting = db.payroles.ToList();
                mts = db1.MainTimeSheets.Where(
                        x => x.TMonth.Month == month.Value.Month && x.TMonth.Year == month.Value.Year
                                                                 && (x.ManPowerSupplier == 1 ||
                                                                     x.ManPowerSupplier == 8 ||
                                                                     x.ManPowerSupplier == 9))
                    .ToList();
                if (month.Value.Month == 1)
                {
                    mts_1 = db1.MainTimeSheets.Where(
                        x => x.TMonth.Month == 12 && x.TMonth.Year == (month.Value.Year - 1)
                                                  && (x.ManPowerSupplier == 1 ||
                                                      x.ManPowerSupplier == 8 ||
                                                      x.ManPowerSupplier == 9)).ToList();
                }
                else
                {
                    mts_1 = db1.MainTimeSheets.Where(
                        x => x.TMonth.Month == (month.Value.Month - 1) && x.TMonth.Year == (month.Value.Year)
                                                                       && (x.ManPowerSupplier == 1 ||
                                                                           x.ManPowerSupplier == 8 ||
                                                                           x.ManPowerSupplier == 9)).ToList();
                }

                var endmo = new DateTime(
                    month.Value.Year,
                    month.Value.Month,
                    DateTime.DaysInMonth(month.Value.Year, month.Value.Month));
            }

            return View();
        }

        public ActionResult payslip(int? Employee_id, DateTime? eddate)
        {
            ViewBag.Employee_id = Employee_id;
            ViewBag.eddate = eddate;
            var alist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no && x.status != "inactive"))
                    afinallist.Add(file);
            }

            var pay = db.payrollsaveds.ToList();
            afinallist = afinallist.FindAll(x => x.contracts.Count != 0);
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_no");
            var payslip = new payrollsaved();
            var model111 = new payslipmodel
            {
                contract = new contract(), master_file = new master_file(), paysaved = new payrollsaved()
            };
            if (eddate.HasValue && Employee_id.HasValue)
            {
                eddate = new DateTime(eddate.Value.Year, eddate.Value.Month, 1);
                var endmo = new DateTime(
                    eddate.Value.Year,
                    eddate.Value.Month,
                    DateTime.DaysInMonth(eddate.Value.Year, eddate.Value.Month));
                var empname10 = afinallist.Find(x => x.employee_id == Employee_id);
                payslip = pay.Find(x => x.employee_no == empname10.employee_no && x.forthemonth == eddate);
                if (payslip == null)
                {
                    ViewBag.eddate = null;
                    goto xe;
                }

                var leave1 = db.Leaves.Where(
                        x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "6"
                                                                              && x.Start_leave <= payslip.forthemonth
                                                                              && x.End_leave >= payslip.forthemonth)
                    .ToList();
                var leave2 = db.Leaves.Where(
                        x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "1"
                                                                              && x.Start_leave <= payslip.forthemonth
                                                                              && x.End_leave >= payslip.forthemonth)
                    .ToList();
                var leave3 = db.Leaves.Where(
                        x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "2"
                                                                              && x.Start_leave <= payslip.forthemonth
                                                                              && x.End_leave >= payslip.forthemonth)
                    .ToList();
                var leave4 = db.Leaves.Where(
                        x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "3"
                                                                              && x.Start_leave <= payslip.forthemonth
                                                                              && x.End_leave >= payslip.forthemonth)
                    .ToList();
                var leave5 = db.Leaves.Where(
                        x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "4"
                                                                              && x.Start_leave <= payslip.forthemonth
                                                                              && x.End_leave >= payslip.forthemonth)
                    .ToList();
                var leave6 = db.Leaves.Where(
                        x => x.master_file.employee_no == payslip.employee_no && x.leave_type == "5"
                                                                              && x.Start_leave <= payslip.forthemonth
                                                                              && x.End_leave >= payslip.forthemonth)
                    .ToList();
                var lowp = 0;
                foreach (var leaf in leave1)
                {
                    //var dif = leaf.End_leave - leaf.Start_leave;
                    //lowp += dif.Value.Days + 1;
                    var date1 = leaf.Start_leave;
                    var date2 = leaf.End_leave;
                    while (date1 != date2)
                    {
                        if (date1.Value.Month == payslip.forthemonth.Value.Month) lowp += 1;
                        date1 = date1.Value.AddDays(1);
                    }

                    var daysinm = DateTime.DaysInMonth(payslip.forthemonth.Value.Year,
                        payslip.forthemonth.Value.Month);
                    if (lowp != daysinm) lowp += 1;
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
                        if (date1.Value.Month == payslip.forthemonth.Value.Month) al += 1;
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
                        if (date1.Value.Month == payslip.forthemonth.Value.Month) sl += 1;
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
                        if (date1.Value.Month == payslip.forthemonth.Value.Month) com += 1;
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
                        if (date1.Value.Month == payslip.forthemonth.Value.Month) mat += 1;
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
                        if (date1.Value.Month == payslip.forthemonth.Value.Month) haj += 1;
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
                    var empname1 = afinallist.Find(x => x.employee_no == payslip.employee_no);
                    var asf = empname1.date_joined;
                    var leaves = db.Leaves.Include(l => l.master_file).OrderByDescending(x => x.Id).Where(
                        x => x.Employee_id == Employee_id && x.Start_leave >= asf);
                    var tempdate = new DateTime(2020, 12, 31);
                    var times = tempdate - asf;
                    var period = times.Value.TotalDays + 1;
                    var ump = leaves.ToList();
                    foreach (var leaf in ump)
                    {
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


                        if (leaf.leave_type == "1")
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

                    netperiod = period - unpaid;
                    accrued = Math.Round(netperiod * 30 / 360);
                    lbal = accrued - avalied;
                    ViewBag.lbal = lbal;
                }
                ViewBag.al = al;
                ViewBag.sl = sl;
                ViewBag.com = com;
                ViewBag.mat = mat;
                ViewBag.haj = haj;
                ViewBag.holi = GetAllholi(eddate.Value).Count;

                var empname = afinallist.Find(x => x.employee_no == payslip.employee_no);
                var abslist1 = db.leave_absence.Where(
                    x => x.month.Value.Month == eddate.Value.Month && x.month.Value.Year == eddate.Value.Year
                                                                   && x.Employee_id == empname.employee_id).ToList();
                var absd = 0;
                foreach (var leaf in abslist1)
                {
                    var dif = leaf.tod - leaf.fromd;
                    absd += dif.Value.Days + 1;
                }

                var con = db.contracts.OrderByDescending(x => x.date_changed).ToList();
                var conemp = con.Find(x => x.employee_no == empname.employee_id);
                model111.master_file = empname;
                model111.paysaved = payslip;
                model111.contract = conemp;
            }

            xe: ;
            return View(model111);
        }

        public void DownloadExcel(DateTime? month)
        {
            List<payrole> passexel;
            if (month != null)
                passexel = db.payroles.Where(x => x.forthemonth.Value.Month == month.Value.Month).ToList();
            else
            {
                goto end;
            }

            //else passexel = db.payroles.ToList();
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("payroll");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "basic";
            Sheet.Cells["D1"].Value = "gross";
            Sheet.Cells["E1"].Value = "ticket allowance";
            Sheet.Cells["F1"].Value = "arrears";
            Sheet.Cells["G1"].Value = "total payable";
            Sheet.Cells["H1"].Value = "OTRegular(hrs)";
            Sheet.Cells["I1"].Value = "OTRegular(amt)";
            Sheet.Cells["J1"].Value = "OTFriday(hrs)";
            Sheet.Cells["K1"].Value = "OTFriday(amt)";
            Sheet.Cells["L1"].Value = "HolidayOT(hrs)";
            Sheet.Cells["M1"].Value = "HolidayOT(amt)";
            Sheet.Cells["N1"].Value = "OTNight";
            Sheet.Cells["O1"].Value = "TotalOT";
            Sheet.Cells["P1"].Value = "cashAdvances";
            Sheet.Cells["Q1"].Value = "HouseAllow";
            Sheet.Cells["R1"].Value = "FoodAllow";
            Sheet.Cells["S1"].Value = "Timekeeping";
            Sheet.Cells["T1"].Value = "Communication";
            Sheet.Cells["U1"].Value = "TrafficFines";
            Sheet.Cells["V1"].Value = "Absent";
            Sheet.Cells["W1"].Value = "LWOP";
            Sheet.Cells["X1"].Value = "Total LWOP";
            Sheet.Cells["Y1"].Value = "others";
            Sheet.Cells["Z1"].Value = "TotalDedution";
            Sheet.Cells["AA1"].Value = "NetPay";
            Sheet.Cells["AB1"].Value = "remarks";
            var row = 2;
            foreach (var item in passexel)
            {
                double.TryParse(Unprotect(item.HolidayOT), out var b1);
                var bas = Unprotect(item.contract.basic);
                double.TryParse(bas, out var bas1);
                var basperh1 = ((bas1 * 12) / 360) / 8;
                var bdays = b1;
                b1 = b1 * 2.5 * basperh1;
                double.TryParse(Unprotect(item.OTFriday), out var c1);
                var cdays1 = c1;
                c1 = c1 * 1.5 * basperh1;
                double.TryParse(Unprotect(item.OTRegular), out var a1);
                var adays1 = a1;
                a1 = a1 * 1.25 * basperh1;
                double.TryParse(Unprotect(item.contract.salary_details), out var sal);
                var labs = 0d;
                var ldays = 0d;
                if (item.leave_absence?.absence != null)
                    labs = item.leave_absence.absence.Value;

                if (item.Leave?.days != null)
                    ldays = item.Leave.days.Value;

                var TLWOP = (labs + ldays) * (sal * 12 / 360);
                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = Unprotect(item.contract.basic);
                Sheet.Cells[string.Format("D{0}", row)].Value = Unprotect(item.contract.salary_details);
                Sheet.Cells[string.Format("E{0}", row)].Value = Unprotect(item.contract.ticket_allowance);
                Sheet.Cells[string.Format("F{0}", row)].Value = Unprotect(item.contract.arrears);
                Sheet.Cells[string.Format("G{0}", row)].Value = Unprotect(item.totalpayable);
                Sheet.Cells[string.Format("H{0}", row)].Value = Unprotect(item.OTRegular);
                Sheet.Cells[string.Format("I{0}", row)].Value = a1;
                Sheet.Cells[string.Format("J{0}", row)].Value = Unprotect(item.OTFriday);
                Sheet.Cells[string.Format("K{0}", row)].Value = c1;
                Sheet.Cells[string.Format("L{0}", row)].Value = Unprotect(item.HolidayOT);
                Sheet.Cells[string.Format("M{0}", row)].Value = b1;
                if (item.cashAdvances != null)
                {
                    Sheet.Cells[string.Format("N{0}", row)].Value = Unprotect(item.OTNight);
                }

                Sheet.Cells[string.Format("O{0}", row)].Value = Unprotect(item.TotalOT);
                if (item.cashAdvances != null)
                {
                    Sheet.Cells[string.Format("P{0}", row)].Value = Unprotect(item.cashAdvances);
                }
                else
                {
                    Sheet.Cells[string.Format("P{0}", row)].Value = 0;
                }

                if (item.HouseAllow != null)
                {
                    Sheet.Cells[string.Format("Q{0}", row)].Value = Unprotect(item.HouseAllow);
                }
                else
                {
                    Sheet.Cells[string.Format("Q{0}", row)].Value = 0;
                }

                if (item.FoodAllow != null)
                {
                    Sheet.Cells[string.Format("R{0}", row)].Value = Unprotect(item.FoodAllow);
                }
                else
                {
                    Sheet.Cells[string.Format("R{0}", row)].Value = 0;
                }

                if (item.Timekeeping != null)
                {
                    Sheet.Cells[string.Format("S{0}", row)].Value = Unprotect(item.Timekeeping);
                }
                else
                {
                    Sheet.Cells[string.Format("S{0}", row)].Value = 0;
                }

                if (item.Communication != null)
                {
                    Sheet.Cells[string.Format("T{0}", row)].Value = Unprotect(item.Communication);
                }
                else
                {
                    Sheet.Cells[string.Format("T{0}", row)].Value = 0;
                }

                if (item.TrafficFines != null)
                {
                    Sheet.Cells[string.Format("U{0}", row)].Value = Unprotect(item.TrafficFines);
                }
                else
                {
                    Sheet.Cells[string.Format("U{0}", row)].Value = 0;
                }

                Sheet.Cells[string.Format("V{0}", row)].Value =
                    item.leave_absence != null ? item.leave_absence.absence : 0;
                Sheet.Cells[string.Format("W{0}", row)].Value = item.Leave != null ? item.Leave.days : 0;
                Sheet.Cells[string.Format("X{0}", row)].Value = TLWOP;
                if (item.others != null)
                {
                    Sheet.Cells[string.Format("Y{0}", row)].Value = Unprotect(item.others);
                }
                else
                {
                    Sheet.Cells[string.Format("Y{0}", row)].Value = 0;
                }

                Sheet.Cells[string.Format("Z{0}", row)].Value = Unprotect(item.TotalDedution);
                Sheet.Cells[string.Format("AA{0}", row)].Value = Unprotect(item.NetPay);
                Sheet.Cells[string.Format("AB{0}", row)].Value = item.remarks;
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename = payroll.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
            end: ;
        }

        public ActionResult wpsprint(DateTime? month, string company)
        {
            ViewBag.month1 = month;
            var wpslist = new List<Wpsmodel>();
            // if (!company.IsNullOrWhiteSpace())
            // {
            //     ViewBag.company = company;
            // }
            // else
            // {
            //     return View(wpslist);
            // }
            if (month.HasValue)
            {
                var payrolllist = db.payrollsaveds.ToList();
                var labourcardlist = db.labour_card.ToList();
                var masterfilelist = db.master_file.ToList();
                var banklist = db.bank_details.ToList();
                var passportlist = db.passports.ToList();
                var emidlist = db.emirates_id.ToList();
                var parollformonth = payrolllist.FindAll(x =>
                    x.forthemonth.Value.Year == month.Value.Year && x.forthemonth.Value.Month == month.Value.Month);
                var i = 0;
                foreach (var pa in parollformonth)
                {
                    var new_wps = new Wpsmodel();
                    var mf = masterfilelist.FindAll(x => x.employee_no == pa.employee_no)
                        .OrderByDescending(x => x.date_changed).ToList();
                    var lc = labourcardlist.FindAll(x => x.emp_no == mf.First().employee_id)
                        .OrderByDescending(x => x.date_changed).ToList();
                    var bd = banklist.FindAll(x => x.employee_no == mf.First().employee_id)
                        .OrderByDescending(x => x.Employee_Id).ToList();
                    var emid = emidlist.FindAll(x => x.employee_no == mf.First().employee_id)
                        .OrderByDescending(x => x.employee_no).ToList();
                    var pass = passportlist.FindAll(x => x.employee_no == mf.First().employee_id)
                        .OrderByDescending(x => x.employee_no).ToList();
                    i++;
                    new_wps.srno = i;
                    if (bd.Count > 0)
                    {
                        new_wps.BankDetails = bd.First();
                        new_wps.BankDetailsid = bd.First().Employee_Id;
                    }

                    if (mf.Count > 0)
                    {
                        new_wps.MasterFile = mf.First();
                        new_wps.MasterFileid = mf.First().employee_id;
                    }

                    if (emid.Count > 0)
                    {
                        new_wps.passportoremiratesid = emid.First().eid_no.ToString();
                    }
                    else
                    {
                        if (pass.Count > 0)
                        {
                            new_wps.passportoremiratesid = pass.First().passport_no;
                        }
                    }

                    if (lc.Count > 0)
                    {
                        new_wps.LabourCard = lc.First();
                        new_wps.LabourCardid = lc.First().employee_id;
                        if (new_wps.LabourCard.establishment != null && !new_wps.LabourCard.personal_no.HasValue)
                        {
                            new_wps.LabourCard.establishment += " NON WPS";
                        }
                        else if (new_wps.LabourCard.establishment != null && new_wps.LabourCard.personal_no.HasValue)
                        {
                            new_wps.LabourCard.establishment = new_wps.LabourCard.establishment;
                        }
                        else
                        {
                            new_wps.LabourCard = null;
                        }
                    }
                    else
                    {
                        new_wps.LabourCard = null;
                    }

                    new_wps.Payrollsaved = pa;
                    new_wps.Payrollsavedid = pa.Id;
                    double result = 0d;
                    double.TryParse(Unprotect(pa.NetPay), out result);
                    new_wps.panet = 0d;
                    new_wps.panet = result;

                    wpslist.Add(new_wps);
                }

                return View(wpslist.OrderBy(x => x.panet));
            }

            return View(wpslist);
        }

        public ActionResult wpssub(DateTime? month)
        {
            ViewBag.month1 = month;
            var wpslist = new List<Wpsmodel>();
            if (month.HasValue)
            {
                var payrolllist = db.payrollsaveds.ToList();
                var labourcardlist = db.labour_card.ToList();
                var masterfilelist = db.master_file.ToList();
                var banklist = db.bank_details.ToList();
                var conlist = db.contracts.ToList();
                var parollformonth = payrolllist.FindAll(x =>
                    x.forthemonth.Value.Year == month.Value.Year && x.forthemonth.Value.Month == month.Value.Month);
                var i = 0;
                foreach (var pa in parollformonth)
                {
                    var new_wps = new Wpsmodel();
                    var mf = masterfilelist.FindAll(x => x.employee_no == pa.employee_no)
                        .OrderByDescending(x => x.date_changed).ToList();
                    var lc = labourcardlist.FindAll(x => x.emp_no == mf.First().employee_id)
                        .OrderByDescending(x => x.date_changed).ToList();
                    var bd = banklist.FindAll(x => x.employee_no == mf.First().employee_id)
                        .OrderByDescending(x => x.Employee_Id).ToList();
                    var co = conlist.FindAll(x => x.employee_no == mf.First().employee_id)
                        .OrderByDescending(x => x.employee_id).ToList();
                    i++;
                    new_wps.srno = i;
                    if (bd.Count > 0)
                    {
                        new_wps.BankDetails = bd.First();
                        new_wps.BankDetailsid = bd.First().Employee_Id;
                    }

                    if (mf.Count > 0)
                    {
                        new_wps.MasterFile = mf.First();
                        new_wps.MasterFileid = mf.First().employee_id;
                    }

                    if (co.Count > 0)
                    {
                        new_wps.Contract = co.First();
                        new_wps.contractid = co.First().employee_id;
                    }

                    if (lc.Count > 0)
                    {
                        new_wps.LabourCard = lc.First();
                        new_wps.LabourCardid = lc.First().employee_id;
                    }

                    new_wps.Payrollsaved = pa;
                    new_wps.Payrollsavedid = pa.Id;
                    double result = 0d;
                    double.TryParse(Unprotect(pa.NetPay), out result);
                    new_wps.panet = 0d;
                    new_wps.panet = result;
                    wpslist.Add(new_wps);
                }

                return View(wpslist.OrderBy(x => x.panet));
            }

            return View(wpslist);
        }

        public ActionResult reconsilation(DateTime? month)
        {
            var payrolllist = db.payrollsaveds.ToList();
            var reconlist = new List<Reconsilationmodel>();
            if (month != null)
            {
                var cmgross = 0d;
                var pmgross = 0d;
                var month1 = payrolllist.FindAll(x =>
                    x.forthemonth.Value.Month == month.Value.Month && x.forthemonth.Value.Year == month.Value.Year);
                foreach (var cm in month1)
                {
                    double.TryParse(Unprotect(cm.Gross), out var cg);
                    cmgross += cg;
                }

                month1 = payrolllist.FindAll(x =>
                    x.forthemonth.Value.Month == month.Value.Month && x.forthemonth.Value.Year == month.Value.Year &&
                    (x.remarks != null || x.ded_add != null));
                var month2 = new List<payrollsaved>();
                if (month.Value.Month == 1)
                {
                    month2 = payrolllist.FindAll(x =>
                        x.forthemonth.Value.Month == (12) &&
                        x.forthemonth.Value.Year == (month.Value.Year - 1));
                }
                else
                {
                    month2 = payrolllist.FindAll(x =>
                        x.forthemonth.Value.Month == (month.Value.Month - 1) &&
                        x.forthemonth.Value.Year == (month.Value.Year));
                }

                if (month2 == null)
                {
                    goto a;
                }

                foreach (var cm in month2)
                {
                    double.TryParse(Unprotect(cm.Gross), out var cg);
                    pmgross += cg;
                }

                ViewBag.cmgross = cmgross;
                ViewBag.pmgross = pmgross;
                ViewBag.diffgross = Math.Abs(cmgross - pmgross);
                ViewBag.cmdate = month1.First().forthemonth.Value.ToString("MMMM yyyy");
                ViewBag.pmdate = month2.First().forthemonth.Value.ToString("MMMM yyyy");
                if (month1 == null)
                {
                    goto a;
                }

                foreach (var payrollsaved in month1)
                {
                    var recon = new Reconsilationmodel();
                    var paynew = month2.Find(x => x.employee_no == payrollsaved.employee_no);
                    recon.ded_add = payrollsaved.ded_add;
                    recon.employee_id = payrollsaved.employee_no;
                    recon.employee_name = payrollsaved.employee_name;
                    recon.remarks = payrollsaved.remarks;
                    double.TryParse(Unprotect(payrollsaved.Gross), out var ab);
                    recon.gross = Math.Abs(ab);
                    double.TryParse(Unprotect(payrollsaved.amount), out var ab2);
                    recon.amount = ab2;
                    reconlist.Add(recon);
                }
            }

            a: ;
            return View(reconlist);
        }

        public ActionResult DownloadExcelwps(DateTime? month)
        {
            var wpslist = new List<Wpsmodel>();
            if (month.HasValue)
            {
                var payrolllist = db.payrollsaveds.ToList();
                var labourcardlist = db.labour_card.ToList();
                var masterfilelist = db.master_file.ToList();
                var banklist = db.bank_details.ToList();
                var parollformonth = payrolllist.FindAll(x =>
                    x.forthemonth.Value.Year == month.Value.Year && x.forthemonth.Value.Month == month.Value.Month);
                var i = 0;
                foreach (var pa in parollformonth)
                {
                    var new_wps = new Wpsmodel();
                    var mf = masterfilelist.FindAll(x => x.employee_no == pa.employee_no)
                        .OrderByDescending(x => x.date_changed).ToList();
                    var lc = labourcardlist.FindAll(x => x.emp_no == mf.First().employee_id)
                        .OrderByDescending(x => x.date_changed).ToList();
                    var bd = banklist.FindAll(x => x.employee_no == mf.First().employee_id)
                        .OrderByDescending(x => x.Employee_Id).ToList();
                    i++;
                    new_wps.srno = i;
                    if (bd.Count > 0)
                    {
                        new_wps.BankDetails = bd.First();
                        new_wps.BankDetailsid = bd.First().Employee_Id;
                    }

                    if (mf.Count > 0)
                    {
                        new_wps.MasterFile = mf.First();
                        new_wps.MasterFileid = mf.First().employee_id;
                    }


                    if (lc.Count > 0)
                    {
                        new_wps.LabourCard = lc.First();
                        new_wps.LabourCardid = lc.First().employee_id;
                    }

                    new_wps.Payrollsaved = pa;
                    new_wps.Payrollsavedid = pa.Id;
                    double result = 0d;
                    double.TryParse(Unprotect(pa.NetPay), out result);
                    new_wps.panet = 0d;
                    new_wps.panet = result;

                    wpslist.Add(new_wps);
                }

                var Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("wps".ToUpper());
                var row = 1;
                var molnolist = new List<string>() { "549959", "575203", "663119", "1115891", null };
                foreach (var mol in molnolist)
                {
                    var srno = 1;
                    var total = 0d;
                    var total1 = 0d;
                    var total2 = 0d;
                    Sheet.Cells[string.Format("E{0}", row)].Value = "comoany name :- citiscape".ToUpper();
                    row++;
                    Sheet.Cells[string.Format("E{0}", row)].Value = ("mol id no :- " + mol).ToUpper();
                    row++;
                    Sheet.Cells[string.Format("E{0}", row)].Value =
                        ("payroll for the month of :- " + month.Value.ToString("M")).ToUpper();
                    row++;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Srno".ToUpper();
                    Sheet.Cells[string.Format("B{0}", row)].Value = "name".ToUpper();
                    Sheet.Cells[string.Format("C{0}", row)].Value = "work permit no".ToUpper();
                    Sheet.Cells[string.Format("D{0}", row)].Value = "personal no".ToUpper();
                    Sheet.Cells[string.Format("E{0}", row)].Value = "bank name".ToUpper();
                    Sheet.Cells[string.Format("F{0}", row)].Value = "IBAN".ToUpper();
                    Sheet.Cells[string.Format("G{0}", row)].Value = "no of days absent".ToUpper();
                    Sheet.Cells[string.Format("H{0}", row)].Value = "fixed portion".ToUpper();
                    Sheet.Cells[string.Format("I{0}", row)].Value = "variable portion".ToUpper();
                    Sheet.Cells[string.Format("J{0}", row)].Value = "total payment".ToUpper();
                    row++;
                    var molwps = wpslist.FindAll(x => x.Payrollsaved.establishment == mol).OrderBy(x => x.panet)
                        .ToList();
                    foreach (var item in molwps)
                    {
                        double a = 0, b = 0, c = 0, d = 0, e = 0;
                        var sum = a + b + c;
                        if (item.Payrollsaved != null)
                        {
                            if (item.Payrollsaved.Gross != null)
                            {
                                double.TryParse(payrolesController.Unprotect(item.Payrollsaved.Gross), out e);
                            }

                            if (item.Payrollsaved.NetPay != null)
                            {
                                double.TryParse(payrolesController.Unprotect(item.Payrollsaved.NetPay), out d);
                            }

                            if (item.Payrollsaved.TotalOT != null)
                            {
                                double.TryParse(payrolesController.Unprotect(item.Payrollsaved.TotalOT), out a);
                            }

                            if (item.Payrollsaved.Arrears != null)
                            {
                                double.TryParse(payrolesController.Unprotect(item.Payrollsaved.Arrears), out b);
                            }

                            if (item.Payrollsaved.TicketAllowance_ != null)
                            {
                                double.TryParse(payrolesController.Unprotect(item.Payrollsaved.TicketAllowance_),
                                    out c);
                            }
                        }

                        var temptotal = total2 + d;
                        if (temptotal > 490000)
                        {
                            Sheet.Cells[string.Format("G{0}", row)].Value = "total";
                            Sheet.Cells[string.Format("H{0}", row)].Value = total;
                            Sheet.Cells[string.Format("I{0}", row)].Value = total1;
                            Sheet.Cells[string.Format("J{0}", row)].Value = total2;
                            row += 5;
                            Sheet.Cells[string.Format("E{0}", row)].Value = "comoany name :- citiscape".ToUpper();
                            row++;
                            Sheet.Cells[string.Format("E{0}", row)].Value = ("mol id no :- " + mol).ToUpper();
                            row++;
                            Sheet.Cells[string.Format("E{0}", row)].Value =
                                ("payroll for the month of :- " + month.Value.ToString("M")).ToUpper();
                            row++;
                            Sheet.Cells[string.Format("A{0}", row)].Value = "Srno".ToUpper();
                            Sheet.Cells[string.Format("B{0}", row)].Value = "name".ToUpper();
                            Sheet.Cells[string.Format("C{0}", row)].Value = "work permit no".ToUpper();
                            Sheet.Cells[string.Format("D{0}", row)].Value = "personal no".ToUpper();
                            Sheet.Cells[string.Format("E{0}", row)].Value = "bank name".ToUpper();
                            Sheet.Cells[string.Format("F{0}", row)].Value = "IBAN".ToUpper();
                            Sheet.Cells[string.Format("G{0}", row)].Value = "no of days absent".ToUpper();
                            Sheet.Cells[string.Format("H{0}", row)].Value = "fixed portion".ToUpper();
                            Sheet.Cells[string.Format("I{0}", row)].Value = "variable portion".ToUpper();
                            Sheet.Cells[string.Format("J{0}", row)].Value = "total payment".ToUpper();
                            row++;
                            total2 = 0;
                        }

                        total += e;
                        total1 += sum;
                        total2 += d;
                        Sheet.Cells[string.Format("A{0}", row)].Value = srno;
                        if (item.MasterFile != null)
                        {
                            if (item.MasterFile.employee_name != null)
                            {
                                Sheet.Cells[string.Format("B{0}", row)].Value = item.MasterFile.employee_name;
                            }
                        }

                        if (item.LabourCard != null)
                        {
                            if (item.LabourCard.work_permit_no != null)
                            {
                                Sheet.Cells[string.Format("C{0}", row)].Value = item.LabourCard.work_permit_no;
                            }

                            if (item.LabourCard.personal_no != null)
                            {
                                Sheet.Cells[string.Format("D{0}", row)].Value = item.LabourCard.personal_no;
                            }
                        }

                        if (item.BankDetails != null)
                        {
                            if (item.BankDetails.bank_name != null)
                            {
                                Sheet.Cells[string.Format("E{0}", row)].Value = item.BankDetails.bank_name;
                            }

                            if (item.BankDetails.IBAN != null)
                            {
                                Sheet.Cells[string.Format("F{0}", row)].Value = item.BankDetails.IBAN;
                            }
                        }

                        if (item.Payrollsaved != null)
                        {
                            if (item.Payrollsaved.Absents != null)
                            {
                                Sheet.Cells[string.Format("G{0}", row)].Value = item.Payrollsaved.Absents;
                            }
                        }

                        Sheet.Cells[string.Format("H{0}", row)].Value = e;
                        Sheet.Cells[string.Format("I{0}", row)].Value = sum;
                        Sheet.Cells[string.Format("J{0}", row)].Value = d;
                        row++;
                        srno++;
                    }

                    Sheet.Cells[string.Format("G{0}", row)].Value = "total";
                    Sheet.Cells[string.Format("H{0}", row)].Value = total;
                    Sheet.Cells[string.Format("I{0}", row)].Value = total1;
                    Sheet.Cells[string.Format("J{0}", row)].Value = total2;
                    row += 5;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                this.Response.Clear();
                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("content-disposition", "filename =wps.xlsx");
                this.Response.BinaryWrite(Ep.GetAsByteArray());
                this.Response.End();
                return RedirectToAction("wpsprint", month);
            }


            return RedirectToAction("wpsprint", month);
        }

        public ActionResult payroll(DateTime? month, string save, string refresh)
        {
            var paylist = new List<payrole>();
            var conlist = db.contracts.OrderByDescending(x => x.date_changed).ToList();
            if (month == null)
            {
                goto end;
            }

            ViewBag.payday = month;
            var payrolllist = db.payroles.ToList();
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
                    if ((file.last_working_day.HasValue && file.last_working_day > month) ||
                        (file.status != "inactive" && !file.last_working_day.HasValue))
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
            // var temp31 =afinallist[0];
            // var temp11 =afinallist[1];
            // var temp21 =afinallist[2];
            // afinallist.Remove(temp31);
            // afinallist.Remove(temp11);
            // afinallist.Remove(temp21);

            var startdate = new DateTime();
            if (month.Value.Month == 1)
            {
                startdate = new DateTime(month.Value.Year - 1, 12, 21);
            }
            else
            {
                startdate = new DateTime(month.Value.Year, month.Value.Month - 1, 21);
            }

            var enddate = new DateTime(month.Value.Year, month.Value.Month, 20);
            var access_datalist = db1.access_datenew.Where(x =>
                x.entrydate >= startdate &&
                x.entrydate <= enddate).ToList();
            var monthnew = new DateTime(month.Value.Year, month.Value.Month, 1);
            var tempdate = startdate;

            foreach (var masterFile in afinallist)
            {
                var payr = new payrole();
                var newtemp = new bool();
                if (payrolllist.Exists(x => x.forthemonth == monthnew
                                            && x.employee_no == masterFile.employee_id))
                {
                    payr = payrolllist.Find(
                        x => x.forthemonth == monthnew
                             && x.employee_no == masterFile.employee_id);
                    newtemp = false;
                }
                else
                {
                    payr = new payrole();
                    payr.save = false;
                    payr.master_file = masterFile;
                    payr.employee_no = masterFile.employee_id;
                    payr.forthemonth = new DateTime(month.Value.Year, month.Value.Month, 1);
                    var conid = conlist.Find(x => x.employee_no == payr.employee_no);
                    payr.Rstate = "R";
                    if (conid == null)
                    {
                        goto funend;
                    }

                    payr.con_id = conid.employee_id;
                    payr.contract = conid;
                    payr.totalpayable = "0";
                    payr.OTRegular = "0";
                    payr.OTFriday = "0";
                    payr.OTNight = "0";
                    payr.HolidayOT = "0";
                    payr.Fot = "0";
                    payr.TotalOT = "0";
                    payr.cashAdvances = "0";
                    payr.HouseAllow = "0";
                    payr.FoodAllow = "0";
                    payr.Timekeeping = "0";
                    payr.Communication = "0";
                    payr.TrafficFines = "0";
                    payr.TotalDedution = "0";
                    payr.NetPay = "0";
                    payr.remarks = "0";
                    payr.TicketAllowance_ = "0";
                    payr.Arrears = "0";
                    payr.TransportationAllowance_ = "0";
                    payr.others = "0";
                    payr.amount = "0";
                    payr.ded_add = "0";
                    newtemp = true;
                }

                if (payr.save) goto sav;
                if (!refresh.IsNullOrWhiteSpace())
                {
                    payr.Rstate = "C";
                }

                if (payr.Rstate == "R" && !newtemp)
                {
                    goto R;
                }

                var leavedate1 = new DateTime();
                if (payr.forthemonth.Value.Month == 1)
                {
                    leavedate1 = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                }
                else
                {
                    leavedate1 = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1,
                        21);
                }

                var leavedateend = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month,
                    20);
                var leave1 = new List<Leave>();
                var leave2 = new List<Leave>();
                var abslist1 = new List<leave_absence>();
                var currentyear = DateTime.Today.Year;
                var sickleaveind = new List<Leave>();
                var sickleavenonind = new List<Leave>();
                var sickleaveindhp = new List<Leave>();
                var sickleavenonindhp = new List<Leave>();
                var sickleaveindup = new List<Leave>();
                var sickleavenonindup = new List<Leave>();
                var maternityleave = new List<Leave>();
                var maternityleavehp = new List<Leave>();
                var maternityleaveup = new List<Leave>();
                var yearstart = new DateTime(currentyear, 1, 1);
                var yearend = new DateTime(currentyear, 12, 31);
                var sickleaveindlist = new List<Leave>();
                var sickleavenonindlist = new List<Leave>();
                var datecount = yearstart;
                var slindcount = 0d;
                var slindcounthalfex = 0d;
                var slnonindcount = 0d;
                var slnonindcounthalfex = 0d;
                var mlcount = 0d;
                var mlcounthalfex = 0d;
                do
                {
                    var sickleaveindlist1 = db.Leaves.Where(
                        x => x.Employee_id == masterFile.employee_id && x.leave_type == "7"
                                                                     && x.Start_leave <= datecount
                                                                     && x.End_leave >= datecount).ToList();
                    var sickleavenonindlist1 = db.Leaves.Where(
                        x => x.Employee_id == masterFile.employee_id && x.leave_type == "2"
                                                                     && x.Start_leave <= datecount
                                                                     && x.End_leave >= datecount).ToList();
                    var maternityleavelist1 = db.Leaves.Where(
                        x => x.Employee_id == masterFile.employee_id && x.leave_type == "4"
                                                                     && x.Start_leave <= datecount
                                                                     && x.End_leave >= datecount).ToList();
                    foreach (var leaf in sickleaveindlist1)
                    {
                        slindcount++;
                        slindcounthalfex++;
                        if (!sickleaveind.Exists(x => x.Id == leaf.Id))
                        {
                            sickleaveind.Add(leaf);
                            if (leaf.half)
                            {
                                slindcount -= 0.5;
                                slindcounthalfex -= 0.5;
                            }
                        }

                        if (slindcount > 180)
                        {
                            var slindcounttemp = slindcount - 180;
                            var slindcounthalfextemp = slindcounthalfex - 180;
                            if (slindcounttemp > 180)
                            {
                                if (sickleaveindup.Exists(x => x.Start_leave == leaf.Start_leave))
                                {
                                    var daysepleave = new Leave();
                                    daysepleave.Start_leave = datecount;
                                    daysepleave.End_leave = datecount;
                                    daysepleave.leave_type = leaf.leave_type;
                                    if ((slindcounthalfextemp % 1) == 0)
                                    {
                                        daysepleave.half = false;
                                    }
                                    else
                                    {
                                        daysepleave.half = true;
                                        slindcounthalfextemp -= 0.5;
                                    }

                                    sickleaveindup.Add(daysepleave);
                                }
                            }
                            else
                            {
                                if (sickleaveindhp.Exists(x => x.Start_leave == leaf.Start_leave))
                                {
                                    var daysepleave = new Leave();
                                    daysepleave.Start_leave = datecount;
                                    daysepleave.End_leave = datecount;
                                    daysepleave.leave_type = leaf.leave_type;
                                    if ((slindcounthalfex % 1) == 0)
                                    {
                                        daysepleave.half = false;
                                    }
                                    else
                                    {
                                        daysepleave.half = true;
                                        slindcounthalfex -= 0.5;
                                    }

                                    sickleaveindhp.Add(daysepleave);
                                }
                            }
                        }
                    }

                    foreach (var leaf in sickleavenonindlist1)
                    {
                        slnonindcount++;
                        slnonindcounthalfex++;
                        if (!sickleavenonind.Exists(x => x.Id == leaf.Id))
                        {
                            sickleavenonind.Add(leaf);
                            if (leaf.half)
                            {
                                slnonindcount -= 0.5;
                                slnonindcounthalfex -= 0.5;
                            }
                        }

                        if (slnonindcount > 15)
                        {
                            var slnonindcounttemp = slindcount - 15;
                            var slnonindcounthalfextemp = slnonindcounthalfex - 15;
                            if (slnonindcounttemp > 30)
                            {
                                if (sickleaveindup.Exists(x => x.Start_leave == leaf.Start_leave))
                                {
                                    var daysepleave = new Leave();
                                    daysepleave.Start_leave = datecount;
                                    daysepleave.End_leave = datecount;
                                    daysepleave.leave_type = leaf.leave_type;
                                    if ((slnonindcounthalfextemp % 1) == 0)
                                    {
                                        daysepleave.half = false;
                                    }
                                    else
                                    {
                                        daysepleave.half = true;
                                        slnonindcounthalfextemp -= 0.5;
                                    }

                                    sickleaveindup.Add(daysepleave);
                                }
                            }
                            else
                            {
                                if (sickleaveindhp.Exists(x => x.Start_leave == leaf.Start_leave))
                                {
                                    var daysepleave = new Leave();
                                    daysepleave.Start_leave = datecount;
                                    daysepleave.End_leave = datecount;
                                    daysepleave.leave_type = leaf.leave_type;
                                    if ((slnonindcounthalfex % 1) == 0)
                                    {
                                        daysepleave.half = false;
                                    }
                                    else
                                    {
                                        daysepleave.half = true;
                                        slnonindcounthalfex -= 0.5;
                                    }

                                    sickleaveindhp.Add(daysepleave);
                                }
                            }
                        }
                    }

                    foreach (var leaf in maternityleavelist1)
                    {
                        mlcount++;
                        mlcounthalfex++;
                        if (!maternityleave.Exists(x => x.Id == leaf.Id))
                        {
                            maternityleave.Add(leaf);
                            if (leaf.half)
                            {
                                mlcount -= 0.5;
                                mlcounthalfex -= 0.5;
                            }
                        }

                        if (mlcount > 45)
                        {
                            var mlcounttemp = mlcount - 45;
                            var mlcounthalfextemp = mlcounthalfex - 45;
                            if (mlcounttemp > 15)
                            {
                                if (maternityleaveup.Exists(x => x.Start_leave == leaf.Start_leave))
                                {
                                    var daysepleave = new Leave();
                                    daysepleave.Start_leave = datecount;
                                    daysepleave.End_leave = datecount;
                                    daysepleave.leave_type = leaf.leave_type;
                                    if ((mlcounthalfextemp % 1) == 0)
                                    {
                                        daysepleave.half = false;
                                    }
                                    else
                                    {
                                        daysepleave.half = true;
                                        mlcounthalfextemp -= 0.5;
                                    }

                                    maternityleaveup.Add(daysepleave);
                                }
                            }
                            else
                            {
                                if (maternityleavehp.Exists(x => x.Start_leave == leaf.Start_leave))
                                {
                                    var daysepleave = new Leave();
                                    daysepleave.Start_leave = datecount;
                                    daysepleave.End_leave = datecount;
                                    daysepleave.leave_type = leaf.leave_type;
                                    if ((mlcounthalfex % 1) == 0)
                                    {
                                        daysepleave.half = false;
                                    }
                                    else
                                    {
                                        daysepleave.half = true;
                                        mlcounthalfex -= 0.5;
                                    }

                                    maternityleavehp.Add(daysepleave);
                                }
                            }
                        }
                    }

                    datecount = datecount.AddDays(1);
                } while (datecount < yearend);

                var absd = 0;
                var mlcounthp = 0d;
                var mlcountup = 0d;
                var slindcounthp = 0d;
                var slindcountup = 0d;
                var slnonindcounthp = 0d;
                var slnonindcountup = 0d;
                var datestart = new DateTime();
                var dateend = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month, 20);
                if (payr.forthemonth.Value.Month == 1)
                {
                    datestart = new DateTime(payr.forthemonth.Value.Year - 1, 12, 21);
                }
                else
                {
                    datestart = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1, 21);
                }

                var aqt = 0L;
                var aqf = 0L;
                var aqh = 0L;
                if (masterFile == null) goto tos1;
                var attd1 = access_datalist.FindAll(x => x.emp_no == masterFile.employee_no).OrderBy(x => x.entrydate)
                    .ToList();
                var attd2 = access_datalist.FindAll(x => x.emp_no == masterFile.employee_no).OrderBy(x => x.entrydate)
                    .ToList();
                var atd = new List<access_datenew>();
                foreach (var atq in attd1)
                {
                    var sameday = attd1.FindAll(x => x.entrydate == atq.entrydate && x.emp_no == atq.emp_no);
                    if (sameday.Count() > 1)
                    {
                        sameday.Remove(atq);

                        if (sameday != null && sameday.Count() > 0)
                        {
                            var temp = attd2.Find(x => x != null && x.Id == atq.Id);

                            if (temp != null)
                            {
                                foreach (var atq1 in sameday)
                                {
                                    if (atq1.project_id != atq.project_id && atq1.project_id.HasValue)
                                    {
                                        if (long.TryParse(atq1.hours, out long tempvar))
                                        {
                                            var temphrp = 0l;
                                            long.TryParse(temp.hours, out temphrp);
                                            temphrp += tempvar;
                                            temp.hours = temphrp.ToString();
                                        }
                                        else
                                        {
                                            temp.hours = atq1.hours;
                                        }
                                    }

                                    attd2.Remove(atq1);
                                }

                                if (attd2.Exists(x => x != null && x.Id == atq.Id))
                                {
                                    atd.Add(temp);
                                }
                            }
                        }
                    }
                    else
                    {
                        atd.Add(atq);
                    }
                }

                var abslisttimesheettemp = atd.FindAll(x => x.hours == "A");
                var abslisttimesheet = new List<leave_absence>();
                var mastervar = db.master_file.OrderByDescending(x => x.date_changed).ToList();
                foreach (var file in abslisttimesheettemp)
                {
                    var monthstart = new DateTime(month.Value.Year, month.Value.Month, 1);
                    if (!abslisttimesheet.Exists(x =>
                            x.master_file.employee_no == file.emp_no && x.fromd <= file.entrydate &&
                            x.tod >= file.entrydate))
                    {
                        var absvar = new leave_absence();
                        var emp = mastervar.Find(x => x.employee_no == file.emp_no);
                        if (emp != null)
                        {
                            var date2 = file.entrydate.Value.AddDays(-1);
                            if (abslisttimesheet.Exists(x =>
                                    x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2))
                            {
                                var abs1 = abslisttimesheet.Find(x =>
                                    x.Employee_id == emp.employee_id && x.fromd <= date2 && x.tod >= date2);
                                abs1.tod = file.entrydate;
                                abs1.absence = (abs1.tod - abs1.fromd).Value.Days + 1;
                                this.db.Entry(abs1).State = EntityState.Modified;
                                this.db.SaveChanges();
                                ;
                            }
                            else
                            {
                                absvar.Employee_id = emp.employee_id;
                                absvar.fromd = file.entrydate;
                                absvar.tod = file.entrydate;
                                absvar.absence = 1;
                                absvar.month = monthstart;
                                this.db.leave_absence.Add(absvar);
                                this.db.SaveChanges();
                            }
                        }
                    }
                }


                var newdate = new DateTime();
                if (payr.forthemonth.Value.Month == 1)
                {
                    newdate = new DateTime(payr.forthemonth.Value.Year - 1, 12, 1);
                }
                else
                {
                    newdate = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month - 1, 1);
                }

                var hlistday = new List<int>();
                var hlistday1 = GetAllholi(month.Value);
                foreach (var i in hlistday1)
                {
                    var dt1 = new DateTime(dateend.Year, dateend.Month, i);
                    if (datestart <= dt1 && dt1 <= dateend)
                    {
                        hlistday.Add(i);
                    }
                }

                hlistday1 = GetAllholi(newdate);
                foreach (var i in hlistday1)
                {
                    var dt1 = new DateTime(datestart.Year, datestart.Month, i);
                    if (datestart <= dt1 && dt1 <= dateend)
                    {
                        hlistday.Add(i);
                    }
                }

                foreach (var atq in atd)
                {
                    var fdaylist = new List<int>();
                    long.TryParse(atq.project_id.ToString(), out long tempatq);
                    var fdaylist1 = GetAll(month.Value, tempatq);
                    foreach (var i in fdaylist1)
                    {
                        var dt1 = new DateTime(dateend.Year, dateend.Month, i);
                        if (datestart <= dt1 && dt1 <= dateend)
                        {
                            fdaylist.Add(i);
                        }
                    }

                    long.TryParse(atq.project_id.ToString(), out long tempatq1);
                    fdaylist1 = GetAll(newdate, tempatq1);
                    foreach (var i in fdaylist1)
                    {
                        var dt1 = new DateTime(datestart.Year, datestart.Month, i);
                        if (datestart <= dt1 && dt1 <= dateend)
                        {
                            fdaylist.Add(i);
                        }
                    }

                    var y = 0l;
                    long.TryParse(atq.hours, out y);
                    if (!leave2.Exists(z => z.Start_leave <= atq.entrydate && z.End_leave >= atq.entrydate))
                    {
                        if (atq.entrydate.HasValue && !fdaylist.Exists(x =>
                                x.Equals(atq.entrydate.Value.Day)) &&
                            !hlistday.Exists(q => q.Equals(atq.entrydate.Value.Day)))
                        {
                            if (y > 8) aqt += y - 8;
                        }
                        else if (atq.entrydate.HasValue && fdaylist.Exists(x =>
                                     x.Equals(atq.entrydate.Value.Day)) &&
                                 !hlistday.Exists(q => q.Equals(atq.entrydate.Value.Day)))
                        {
                            if (y != 0)
                            {
                                aqf += 1;
                            }
                        }
                        else if (atq.entrydate.HasValue && !fdaylist.Exists(x =>
                                     x.Equals(atq.entrydate.Value.Day)) &&
                                 hlistday.Exists(q => q.Equals(atq.entrydate.Value.Day)))
                        {
                            if (y != 0)
                            {
                                aqh += 1;
                            }
                        }
                        else if (atq.entrydate.HasValue && fdaylist.Exists(x =>
                                     x.Equals(atq.entrydate.Value.Day)) &&
                                 hlistday.Exists(q => q.Equals(atq.entrydate.Value.Day)))
                        {
                            if (y != 0)
                            {
                                aqh += 1;
                            }
                        }
                    }
                }

                tos1: ;

                do
                {
                    var leave1_1 = db.Leaves.Where(
                        x => x.Employee_id == masterFile.employee_id && x.leave_type == "6"
                                                                     && x.Start_leave <= leavedate1
                                                                     && x.End_leave >= leavedate1).ToList();
                    var leave2_1 = db.Leaves.Where(
                        x => x.Employee_id == masterFile.employee_id && x.Start_leave <= leavedate1
                                                                     && x.End_leave >= leavedate1).ToList();
                    var abslist1_1 = db.leave_absence.Where(
                        x => x.Employee_id == masterFile.employee_id && x.fromd <= leavedate1
                                                                     && x.tod >= leavedate1).ToList();

                    if (maternityleavehp.Exists(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                    {
                        var temp = maternityleavehp.Find(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                        mlcounthp++;
                        if (temp.half)
                        {
                            mlcounthp -= 0.5;
                        }
                    }

                    if (maternityleaveup.Exists(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                    {
                        var temp = maternityleaveup.Find(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                        mlcountup++;
                        if (temp.half)
                        {
                            mlcountup -= 0.5;
                        }
                    }

                    if (sickleaveindhp.Exists(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                    {
                        var temp = sickleaveindhp.Find(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                        slindcounthp++;
                        if (temp.half)
                        {
                            slindcounthp -= 0.5;
                        }
                    }

                    if (sickleaveindup.Exists(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                    {
                        var temp = sickleaveindup.Find(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                        slindcountup++;
                        if (temp.half)
                        {
                            slindcountup -= 0.5;
                        }
                    }

                    if (sickleavenonindhp.Exists(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                    {
                        var temp = sickleavenonindhp.Find(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                        slnonindcounthp++;
                        if (temp.half)
                        {
                            slnonindcounthp -= 0.5;
                        }
                    }

                    if (sickleavenonindup.Exists(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1))
                    {
                        var temp = sickleavenonindup.Find(x =>
                            x.Start_leave <= leavedate1 && x.End_leave >= leavedate1);
                        slnonindcountup++;
                        if (temp.half)
                        {
                            slnonindcountup -= 0.5;
                        }
                    }

                    foreach (var leaf in leave1_1)
                        if (!leave1.Exists(x => x.Id == leaf.Id))
                            leave1.Add(leaf);
                    foreach (var leaf1 in abslist1_1)
                        if (!leave1.Exists(x => x.Id == leaf1.Id))
                            abslist1.Add(leaf1);
                    foreach (var leaf in leave2_1)
                        if (!leave2.Exists(x => x.Id == leaf.Id))
                            leave2.Add(leaf);

                    leavedate1 = leavedate1.AddDays(1);
                } while (leavedate1 < leavedateend);

                var lowp = 0;
                var datediff = (enddate - startdate).Days + 1;
                foreach (var leaf in leave1)
                {
                    var dif = leaf.End_leave - leaf.Start_leave;
                    var date1 = leaf.Start_leave;
                    var date2 = leaf.End_leave;
                    while (date1 != date2)
                    {
                        if (date1 >= startdate && date1 < enddate) lowp += 1;
                        date1 = date1.Value.AddDays(1);
                    }

                    // var daysinm = DateTime.DaysInMonth(payr.forthemonth.Value.Year,
                    //   payr.forthemonth.Value.Month);
                    //if (lowp != datediff) lowp += 1;
                    //lowp += dif.Value.Days + 1;
                }


                foreach (var absence in abslist1)
                {
                    var dif = absence.tod - absence.fromd;
                    var date1 = absence.fromd;
                    var date2 = absence.tod;
                    while (date1 != date2)
                    {
                        if (date1 >= date1 && date1 < date2) absd += 1;
                        date1 = date1.Value.AddDays(1);
                    }
                    // var daysinm = DateTime.DaysInMonth(payr.forthemonth.Value.Year,
                    //     payr.forthemonth.Value.Month);
                    // if (absd != diff) absd += 1;
                }


                if (leave1.Count != 0)
                {
                    payr.LWOP = leave1.OrderByDescending(x => x.Start_leave).First().Id;
                    payr.Leave = leave1.OrderByDescending(x => x.Start_leave).First();
                    payr.Leave.days = lowp;
                }

                payr.OTRegular = aqt.ToString();
                payr.OTFriday = aqf.ToString();
                payr.HolidayOT = aqh.ToString();
                var ant = 0d;
                var m = 0d;
                if (payr.OTNight != null && IsBase64Encoded(payr.OTNight))
                {
                    double.TryParse(Unprotect(payr.OTNight), out ant);
                }

                var comrat = 0d;
                var totded = 0d;
                if (payr.cashAdvances != null && IsBase64Encoded(payr.cashAdvances))
                {
                    double.TryParse(Unprotect(payr.cashAdvances), out comrat);
                    totded += comrat;
                }

                if (payr.HouseAllow != null && IsBase64Encoded(payr.HouseAllow))
                {
                    double.TryParse(Unprotect(payr.HouseAllow), out comrat);
                    totded += comrat;
                }

                if (payr.TrafficFines != null && IsBase64Encoded(payr.TrafficFines))
                {
                    double.TryParse(Unprotect(payr.TrafficFines), out comrat);
                    totded += comrat;
                }

                if (payr.TransportationAllowance_ != null && IsBase64Encoded(payr.TransportationAllowance_))
                {
                    double.TryParse(Unprotect(payr.TransportationAllowance_), out comrat);
                    totded += comrat;
                }

                if (payr.Timekeeping != null && IsBase64Encoded(payr.Timekeeping))
                {
                    double.TryParse(Unprotect(payr.Timekeeping), out comrat);
                    totded += comrat;
                }

                if (payr.Communication != null && IsBase64Encoded(payr.Communication))
                {
                    double.TryParse(Unprotect(payr.Communication), out comrat);
                    totded += comrat;
                }

                if (payr.amount != null && IsBase64Encoded(payr.amount))
                {
                    double.TryParse(Unprotect(payr.others), out comrat);
                    totded += comrat;
                }

                if (payr.amount != null && IsBase64Encoded(payr.amount))
                {
                    double.TryParse(Unprotect(payr.amount), out comrat);
                    totded += comrat;
                }

                if (payr.leave_absence != null) payr.leave_absence.absence = absd;

                if (payr.Leave != null)
                {
                    payr.Leave.days = lowp;
                }

                var gross1 = 0d;

                double.TryParse(Unprotect(payr.contract.salary_details), out gross1);
                var TLWOP1 = (absd + lowp) * (gross1 * 12 / 360);
                totded += TLWOP1;

                payr.TotalDedution = totded.ToString();
                var con = conlist.Find(c1 => c1.employee_no == masterFile.employee_id);
                if (con == null)
                {
                    goto funend;
                }

                var bac = 0d;
                double.TryParse(Unprotect(con.basic), out bac);
                var basperh = bac * 12 / 360 / 8;
                var basperd = bac * 12 / 360;
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
                        if (date1.Value.Month == payr.forthemonth.Value.Month) al += 1;
                        date1 = date1.Value.AddDays(1);
                    }

                    al += 1;
                }

                // payr.TotalOT =
                //     (aqf * 1.5 * basperh + aqh * 2.5 * basperh + aqt * 1.25 * basperh + ant)
                //     .ToString();
                payr.TotalOT =
                    (aqf * 0.5 * basperd + aqh * 0.5 * basperd + aqt * 1.25 * basperh + ant)
                    .ToString();
                var sal = 0d;
                double.TryParse(Unprotect(con.salary_details), out sal);
                var tac = 0d;
                var arr = 0d;
                var foo = 0d;
                if (payr.TicketAllowance_ != null && IsBase64Encoded(payr.TicketAllowance_))
                    double.TryParse(Unprotect(payr.TicketAllowance_), out tac);

                if (payr.Arrears != null && IsBase64Encoded(payr.Arrears))
                    double.TryParse(Unprotect(payr.Arrears), out arr);
                if (payr.FoodAllow != null && IsBase64Encoded(payr.FoodAllow))
                    double.TryParse(Unprotect(payr.FoodAllow), out foo);
                payr.totalpayable = (sal + tac + arr + foo).ToString();
                double.TryParse(payr.totalpayable, out var a);
                double.TryParse(payr.TotalOT, out var b);
                double.TryParse(payr.TotalDedution, out var c);
                var endday = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month,
                    DateTime.DaysInMonth(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month));
                var stday = new DateTime(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month, 1);
                var d = 0d;
                if (payr.amount != null && IsBase64Encoded(payr.amount))
                    double.TryParse(Unprotect(payr.amount), out d);
                if (absd + lowp >=
                    DateTime.DaysInMonth(payr.forthemonth.Value.Year, payr.forthemonth.Value.Month))
                    payr.NetPay = 0.ToString();
                else
                    payr.NetPay = (a + b - c - d).ToString();

                if (save.IsNullOrWhiteSpace())
                    payr.save = false;
                else
                    payr.save = true;
                if (payr.Rstate == "C")
                {
                    payr.Rstate = "R";
                }

                if (newtemp)
                {
                    Create(payr);
                }
                else
                {
                    Edit(payr, "edit");
                }

                R: ;
                if (payr.Rstate == "R")
                {
                    paylist.Add(payr);
                }

                sav: ;
                if (!save.IsNullOrWhiteSpace())
                {
                    var paysavedlist = db.payrollsaveds.ToList();
                    if (paysavedlist.Exists(x =>
                            x.forthemonth == payr.forthemonth && x.employee_no == payr.master_file.employee_no))
                        goto save_end;
                    var paysave = new payrollsaved();
                    if (payr.master_file != null)
                    {
                        paysave.employee_no = payr.master_file.employee_no;
                        paysave.employee_name = payr.master_file.employee_name;
                        if (payr.master_file.labour_card.Count > 0)
                        {
                            paysave.establishment = payr.master_file.labour_card.Last().establishment;
                        }
                    }

                    var a1 = 0d;
                    var b1 = 0d;
                    var c1 = 0d;
                    var d1 = 0d;
                    var e1 = 0d;
                    var f1 = 0d;
                    if (payr.contract != null)
                    {
                        if (payr.HolidayOT != null)
                        {
                            double.TryParse(Unprotect(payr.HolidayOT), out b1);
                        }

                        if (payr.contract != null)
                        {
                            var bas = "0";
                            bas = Unprotect(payr.contract.basic);
                            double.TryParse(bas, out var bas1);
                            var basperh1 = bas1 * 12 / 360 / 8;
                            var basperd1 = bas1 * 12 / 360;
                            var bdays = b1;
                            b1 = b1 * 0.5 * basperd1;
                            if (payr.OTFriday != null)
                            {
                                double.TryParse(Unprotect(payr.OTFriday), out c1);
                            }

                            var cdays1 = c1;
                            c1 = c1 * 0.5 * basperd1;
                            if (payr.HolidayOT != null)
                            {
                                double.TryParse(Unprotect(payr.OTRegular), out a1);
                            }

                            var adays1 = a1;
                            a1 = a1 * 1.25 * basperh1;
                        }

                        if (payr.contract.basic != null)
                            paysave.Basic = payr.contract.basic;
                        if (payr.contract.housing_allowance != null)
                            paysave.CHouseAllow = payr.contract.housing_allowance;
                        if (payr.contract.transportation_allowance != null)
                            paysave.CTransportationAllowance = payr.contract.transportation_allowance;
                        if (payr.contract.food_allowance != null)
                            paysave.CFoodAllow = payr.contract.food_allowance;
                        if (payr.amount != null)
                            paysave.amount = payr.amount;
                        if (payr.ded_add != null)
                            paysave.ded_add = payr.ded_add;
                        if (payr.contract.salary_details != null)
                        {
                            paysave.Gross = payr.contract.salary_details;
                            double.TryParse(Unprotect(payr.contract.salary_details), out d1);
                            double.TryParse(Unprotect(payr.TotalOT), out e1);
                            paysave.Grosstotal = Protect((d1 + e1).ToString());
                        }
                    }

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
                    paysave.TotalOT = payr.TotalOT;
                    paysave.cashAdvances = payr.cashAdvances;
                    paysave.HouseAllow = payr.HouseAllow;
                    paysave.TransportationAllowance_ = payr.TransportationAllowance_;
                    paysave.FoodAllow = payr.FoodAllow;
                    paysave.Timekeeping = payr.Timekeeping;
                    paysave.Communication = payr.Communication;
                    paysave.TrafficFines = payr.TrafficFines;
                    if (payr.leave_absence != null)
                        paysave.Absents = (int?)payr.leave_absence.absence;
                    else
                        paysave.Absents = 0;

                    if (payr.Leave != null)
                        paysave.LWOP = payr.Leave.days;
                    else
                        paysave.LWOP = 0;

                    if (payr.amount != null)
                        paysave.amount = payr.amount;
                    var gross = 0d;
                    double.TryParse(Unprotect(payr.contract.salary_details), out gross);
                    var TLWOP = (paysave.Absents + paysave.LWOP) * (gross * 12 / 360);
                    paysave.TotalLWOP = TLWOP.ToString();
                    paysave.others = payr.others;
                    paysave.TotalDedution = payr.TotalDedution;
                    paysave.NetPay = payr.NetPay;
                    paysave.remarks = payr.remarks;
                    paysave.forthemonth = payr.forthemonth;
                    payr.save = true;
                    db.Entry(payr).State = EntityState.Modified;
                    db.SaveChanges();
                    db.payrollsaveds.Add(paysave);
                    db.SaveChanges();
                    save_end: ;
                }

                funend: ;
            }

            var model12 = new paysavedlist();
            var savedlist1 = db.payrollsaveds.ToList();
            var savedlist = savedlist1
                .FindAll(x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1)).ToList();
            if (savedlist.Count != 0)
                model12 = new paysavedlist
                {
                    Payrollsaved = savedlist.OrderBy(x => x.employee_no),
                    Payroll = paylist.OrderBy(x => x.master_file.employee_no)
                };
            else
                model12 = new paysavedlist
                {
                    Payroll = paylist.OrderBy(x => x.master_file.employee_no),
                    Payrollsaved = savedlist.OrderBy(x => x.employee_no)
                };
            return View(model12);
            end: ;
            var model11 = new paysavedlist
            {
                Payroll = new List<payrole>(),
                Payrollsaved = new List<payrollsaved>()
            };
            return View(model11);
        }

        public ActionResult payrollReport(DateTime? month, string company)
        {
            if (!company.IsNullOrWhiteSpace())
            {
                ViewBag.company = company;
            }
            else
            {
                ViewBag.company = "new";
                var model11 = new paysavedlist
                {
                    Payroll = new List<payrole>(),
                    Payrollsaved = new List<payrollsaved>()
                };
                return View(model11);
            }

            if (!month.HasValue)
            {
                var model11 = new paysavedlist
                {
                    Payroll = new List<payrole>(),
                    Payrollsaved = new List<payrollsaved>()
                };
                return View(model11);
            }

            ViewBag.month = month.Value.ToString("MMMM yyyy");

            var con = db.contracts.ToList();
            var bank = db.bank_details.ToList();
            var paylist = db.payroles.ToList()
                .FindAll(x => x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1));
            var model12 = new paysavedlist();
            var savedlist1 = db.payrollsaveds.ToList();
            var labor = db.labour_card.ToList();
            foreach (var payrole in paylist)
            {
                var temp = labor.Find(x => x.emp_no == payrole.employee_no);
                var temp2 = bank.Find(x => x.employee_no == payrole.employee_no);
                if (temp != null)
                {
                    payrole.establishment = temp.establishment;
                    if (temp2 == null)
                    {
                        payrole.establishment = "CHEQUE" + " " + temp.establishment;
                    }
                }
                else
                {
                    payrole.establishment = "NON WPS";
                }
            }

            if (company == "citiscape")
            {
                var savedlist = savedlist1
                    .FindAll(x =>
                        x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1) &&
                        con.Exists(y => y.company == null || y.company == "citiscape")).ToList();
                if (savedlist.Count != 0)
                {
                    model12 = new paysavedlist
                    {
                        Payrollsaved = savedlist.OrderBy(y => y.establishment).ThenBy(x => x.employee_no),
                        Payroll = paylist.OrderBy(x => x.establishment).ThenBy(x => x.master_file.employee_no)
                    };
                }
                else
                {
                    model12 = new paysavedlist
                    {
                        Payroll = paylist.OrderBy(x => x.master_file.employee_no).ThenBy(x => x.establishment),
                        Payrollsaved = new List<payrollsaved>()
                    };
                }

                return View(model12);
            }
            else if (company == "grove")
            {
                var savedlist = savedlist1
                    .FindAll(x =>
                        x.forthemonth == new DateTime(month.Value.Year, month.Value.Month, 1) &&
                        con.Exists(y => y.company != null && y.company == "grove")).ToList();
                if (savedlist.Count != 0)
                    model12 = new paysavedlist
                    {
                        Payrollsaved = savedlist.OrderBy(y => y.establishment).ThenBy(x => x.employee_no),
                        Payroll = paylist.OrderBy(x => x.establishment).ThenBy(x => x.master_file.employee_no)
                    };
                else
                    model12 = new paysavedlist
                    {
                        Payroll = paylist.OrderBy(x => x.master_file.employee_no),
                        Payrollsaved = new List<payrollsaved>()
                    };
                return View(model12);
            }
            else
            {
                var model11 = new paysavedlist
                {
                    Payroll = new List<payrole>(),
                    Payrollsaved = new List<payrollsaved>()
                };
                return View(model11);
            }
        }

        public ActionResult cal_absence(DateTime? payrollmonth)
        {
            if (!payrollmonth.HasValue)
            {
                return View(new List<calab>());
            }

            var startdate = new DateTime(payrollmonth.Value.Year, payrollmonth.Value.Month - 1, 21);
            if (payrollmonth.Value.Month == 1)
            {
                startdate = new DateTime(payrollmonth.Value.Year - 1, 12, 21);
            }

            var enddate = new DateTime(payrollmonth.Value.Year, payrollmonth.Value.Month, 20);

            var alist = this.db.master_file
                .Where(e => e.last_working_day == null)
                .OrderBy(e => e.employee_no)
                .ThenByDescending(x => x.date_changed)
                .ToList();

            var afinallist = alist
                .GroupBy(x => x.employee_no)
                .Select(g => g.First())
                .Where(file => file.employee_no != 0 && file.employee_no != 1 && file.employee_no != 100001)
                .ToList().FindAll(x=>x.contracts.Count > 0 && (x.contracts.First().departmant_project != "Procurement" ));
            afinallist.RemoveAll(x => x.contracts.First().designation.Contains("Procurement"));

            var HObioatt = db.hiks
                .Where(x => x.date >= startdate && x.date <= enddate)
                .ToList();
            var leaveList = db.Leaves
                .Where(x => x.Start_leave <= enddate && x.End_leave >= startdate)
                .ToList();
            var holidayList = db1.Holidays.Where(x => x.Date >= startdate && x.Date <= enddate).ToList();

            var proatt = dbbio.iclock_transaction
                .Where(x => x.punch_time >= startdate && x.punch_time <= enddate)
                .ToList();

            var abslist = new List<absencelist>();
            var calabslist = new List<calab>();

            foreach (var file in afinallist)
            {
                var tempdate = startdate;
                var tempcalabs = new calab
                {
                    Employee_id = file.employee_id,
                    absdays = 0,
                    absmonth = new DateTime(payrollmonth.Value.Year,
                        payrollmonth.Value.Month,
                        1),
                    master_file = file
                };

                var tranferlist = db.transferlists.Where(x=>x.Employee_id == file.employee_id && x.reason == "approved").OrderByDescending(x=>x.datemodifief).ToList();
                while (tempdate <= enddate)
                {
                    var tempHOattlist = HObioatt
                        .Where(x => x.ID == file.employee_no.ToString() && x.date == tempdate.Date)
                        .ToList();

                    var tempproattlist = proatt
                        .Where(x => x.emp_code == file.employee_no.ToString() && x.punch_time.Date == tempdate)
                        .ToList();

                    if (!tempHOattlist.Any() && !tempproattlist.Any())
                    {
                        if (!tranferlist.Any())
                        {
                            if (tempdate.DayOfWeek != DayOfWeek.Saturday && tempdate.DayOfWeek != DayOfWeek.Sunday &&
                                !leaveList.Exists(x =>
                                    x.End_leave >= tempdate && x.Start_leave <= tempdate &&
                                    x.Employee_id == file.employee_id) &&
                                !holidayList.Exists(x => x.Date >= tempdate && x.Date <= tempdate))
                            {
                                abslist.Add(new absencelist
                                {
                                    emp_id = file.employee_id,
                                    abs_date = tempdate
                                });
                                tempcalabs.absdays++;
                            }
                        }
                        else
                        {
                            if (tranferlist.Count() > 1)
                            {
                                var proweekend = new List<HRweekend>();
                                foreach (var transferlist in tranferlist)
                                {
                                    proweekend.AddRange( db.HRweekends.Where(x => x.pro_id == transferlist.npro_id).ToList());
                                }
                                
                                var tempproj = new List<HRweekend>();

                                foreach (var weekend in proweekend)
                                {
                                    if (!tempproj.Exists(x=>x.dweek.DayOfWeek == weekend.dweek.DayOfWeek))
                                    {
                                        tempproj.Add(weekend);
                                    }
                                }

                                if (!tempproj.Exists(x=>x.dweek.DayOfWeek == tempdate.DayOfWeek) &&
                                    !leaveList.Exists(x =>
                                        x.End_leave >= tempdate && x.Start_leave <= tempdate &&
                                        x.Employee_id == file.employee_id) &&
                                    !holidayList.Exists(x => x.Date >= tempdate && x.Date <= tempdate))
                                {
                                    abslist.Add(new absencelist
                                    {
                                        emp_id = file.employee_id,
                                        abs_date = tempdate
                                    });
                                    tempcalabs.absdays++;
                                }

                            }
                            else     
                            {
                                var proweekend = db.HRweekends.Where(x => x.pro_id == tranferlist[0].npro_id).ToList();
                                if (!proweekend.Exists(x => x.dweek.DayOfWeek == tempdate.DayOfWeek) &&
                                    !leaveList.Exists(x =>
                                        x.End_leave >= tempdate && x.Start_leave <= tempdate &&
                                        x.Employee_id == file.employee_id) &&
                                    !holidayList.Exists(x => x.Date >= tempdate && x.Date <= tempdate))
                                {
                                    abslist.Add(new absencelist
                                    {
                                        emp_id = file.employee_id,
                                        abs_date = tempdate
                                    });
                                    tempcalabs.absdays++;
                                }
                            }
                        }

                    }

                    tempdate = tempdate.AddDays(1);
                }

                calabslist.Add(tempcalabs);
            }

            /*
            foreach (var calab in calabslist)
            {
                var savetest = db.calabs.ToList();
                if (savetest.Exists(x => x.absmonth == calab.absmonth && x.Employee_id == calab.Employee_id))
                {
                    var tempsavevar = savetest.Find(x =>
                        x.absmonth == calab.absmonth && x.Employee_id == calab.Employee_id);
                    if (tempsavevar != null)
                    {
                        tempsavevar.absdays = calab.absdays;
                        db.Entry(tempsavevar).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.calabs.Add(calab);
                    db.SaveChanges();
                }
            }

            foreach (var absencelist in abslist)
            {
                var savetest = db.absencelists.ToList();
                if (savetest.Exists(x => x.abs_date == absencelist.abs_date && x.emp_id == absencelist.emp_id))
                {
                    var tempsavevar = savetest.Find(x =>
                        x.abs_date == absencelist.abs_date && x.emp_id == absencelist.emp_id);
                    if (tempsavevar != null)
                    {
                        tempsavevar.abs_date = absencelist.abs_date;
                        db.Entry(tempsavevar).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.absencelists.Add(absencelist);
                    db.SaveChanges();
                }
            }*/

            return View(calabslist);
        }

        public ActionResult cal_lwop(DateTime? payrollmonth)
        {
            var callowplist = new List<cal_lwop>();
            if (payrollmonth.HasValue)
            {
                var startdate = new DateTime(payrollmonth.Value.Year, payrollmonth.Value.Month - 1, 21);
                if (payrollmonth.Value.Month == 1)
                {
                    startdate = new DateTime(payrollmonth.Value.Year - 1, 12, 21);
                }

                var enddate = new DateTime(payrollmonth.Value.Year, payrollmonth.Value.Month, 20);

                var leave_list = db.Leaves.Where(x =>
                    (x.End_leave >= startdate && x.Start_leave <= enddate && x.End_leave >= startdate && x.leave_type == "6") ||
                    (x.actual_return_date == null && x.Return_leave <= enddate)).ToList();


                var alist = this.db.master_file

                    .Where(e => e.last_working_day == null)
                    .OrderBy(e => e.employee_no)
                    .ThenByDescending(x => x.date_changed)
                    .ToList();

                var afinallist = alist
                    .GroupBy(x => x.employee_no)
                    .Select(g => g.First())
                    .Where(file => file.employee_no != 0 && file.employee_no != 1 && file.employee_no != 100001 /*&& file.employee_no == 5411*/)
                    .ToList();
                foreach (var file in afinallist)
                {
                    var HObioatt = db.hiks
                        .Where(x => x.ID == file.employee_no.ToString())
                        .ToList();
                    var proatt = dbbio.iclock_transaction
                        .Where(x => x.emp_code == file.employee_no.ToString())
                        .ToList();
                    var tempdate = startdate;
                    var leave_listemp = leave_list.FindAll(x => x.Employee_id == file.employee_id).ToList();
                    if (leave_listemp.Exists(x => !x.actual_return_date.HasValue))
                    {
                        var templeavelist = leave_listemp.FindAll(x => !x.actual_return_date.HasValue).ToList();
                        foreach (var leaf in templeavelist)
                        {
                            if (HObioatt.Exists(x => x.date == leaf.Return_leave) || proatt.Exists(x =>  x.punch_time.Date == leaf.Return_leave))
                            {
                                leave_listemp.Remove(leaf);
                                leaf.actual_return_date = leaf.Return_leave;
                                leave_listemp.Add(leaf);
                            }
                        }
                    }
                    var emplowp = new cal_lwop
                    {
                        Employee_id = file.employee_id,
                        lwop_days = 0,
                        lwop_month = new DateTime(payrollmonth.Value.Year,
                            payrollmonth.Value.Month,
                            1),
                        master_file = file
                    };
                    while (tempdate <= enddate )
                    {
                        if (leave_listemp.Exists(x=>x.leave_type == "6" && x.End_leave >= tempdate && x.Start_leave <= tempdate))
                        {
                            emplowp.lwop_days++;
                        }
                        else if (leave_listemp.Exists(x => !x.actual_return_date.HasValue))
                        {
                            emplowp.lwop_days++;
                        }

                        tempdate = tempdate.AddDays(1);
                    }
                    callowplist.Add(emplowp);

                }

                /*foreach (var callw in callowplist)
                {
                    var savetest = db.cal_lwop.ToList();
                    if (savetest.Exists(x => x.lwop_month == callw.lwop_month && x.Employee_id == callw.Employee_id))
                    {
                        var tempsavevar = savetest.Find(x =>
                            x.lwop_month == callw.lwop_month && x.Employee_id == callw.Employee_id);
                        if (tempsavevar != null)
                        {
                            tempsavevar.lwop_days = callw.lwop_days;
                            db.Entry(tempsavevar).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        db.cal_lwop.Add(callw);
                        db.SaveChanges();
                    }
                }*/
            }

            return View(callowplist.Where(x=>x.lwop_days > 0).ToList());
        }
        
        public ActionResult calovertime(DateTime? payrollmonth)
        {
            var calotlist = new List<cal_ot>();
            if (!payrollmonth.HasValue)
            {
                return View(calotlist);
            }

            var startdate = new DateTime(payrollmonth.Value.Year, payrollmonth.Value.Month - 1, 21);
            if (payrollmonth.Value.Month == 1)
            {
                startdate = new DateTime(payrollmonth.Value.Year - 1, 12, 21);
            }

            var enddate = new DateTime(payrollmonth.Value.Year, payrollmonth.Value.Month, 20);
            var HObioatt = db.hiks
                .Where(x => x.date >= startdate && x.date <= enddate)
                .ToList();
            var projectatt = dbbio.iclock_transaction.Where(x => x.punch_time >= startdate && x.punch_time <= enddate).ToList();
            var alist = this.db.master_file
                .Where(e => e.last_working_day == null)
                .OrderBy(e => e.employee_no)
                .ThenByDescending(x => x.date_changed)
                .ToList();
            var afinallist = alist
                .GroupBy(x => x.employee_no)
                .Select(g => g.First())
                .Where(x => x.employee_no != 0 && x.employee_no != 1 && x.employee_no != 100001)
                .ToList();
            var opapped = db.HRotapps.Where(x => x.otdate <= enddate && x.otdate >= startdate && x.status == "approved").ToList();
            var holidaylist = db1.Holidays.Where(x => x.Date <= enddate && x.Date >= startdate).ToList();
            var leaveList = db.Leaves
                .Where(x => x.Start_leave <= enddate && x.End_leave >= startdate)
                .ToList();

            var emplist = afinallist.FindAll(x => projectatt.Exists(y => y.emp_code == x.employee_no.ToString()) && opapped.Exists(y=>y.Employee_id == x.employee_id));
            foreach (var file in emplist)
            {
                var tranferlist = db.transferlists.Where(x => x.Employee_id == file.employee_id && x.reason == "approved").OrderByDescending(x => x.datemodifief).ToList();
                var calot = new cal_ot
                {
                    Employee_id = file.employee_id,
                    ROT = 0,
                    HOT = 0,
                    WOT = 0,
                    OT_month = payrollmonth.Value,
                    master_file = file
                };
                var tempdate = startdate;
                while (tempdate <= enddate)
                {
                    var otday = new HRotapp();
                    if (!opapped.Exists(x =>
                            x.Employee_id == file.employee_no && x.otdate == tempdate && x.status == "approved"))
                    {
                        goto nextday;
                    }
                    else
                    {
                        otday = opapped.Find(x =>
                            x.Employee_id == file.employee_no && x.otdate == tempdate && x.status == "approved");
                    }

                    var otweekday = db.HRweekends
                        .Where(x => x.pro_id == otday.project_id && x.dweek.DayOfWeek == tempdate.DayOfWeek)
                        .OrderByDescending(y => y.wdate).ToList();
                    var weekendcondition = new bool();
                    if (otweekday.Count() > 0)
                    {
                        var tempweeklist = new List<HRweekend>();
                        foreach (var week in otweekday)
                        {
                            if(!tempweeklist.Exists(x => x.pro_id == week.pro_id));
                            {
                                tempweeklist.Add(week);
                            }
                        }

                        if (tempweeklist.Exists(x=>x.dweek.DayOfWeek == tempdate.DayOfWeek))
                        {
                            weekendcondition = true;
                        }
                        else
                        {
                            weekendcondition=false;
                        }

                    }
                    else
                    {
                        weekendcondition = (tempdate.DayOfWeek != DayOfWeek.Saturday &&
                                            tempdate.DayOfWeek != DayOfWeek.Sunday);
                    }

                    if (!leaveList.Exists(x => x.Start_leave <= tempdate && x.End_leave >= tempdate))
                    {
                        if (holidaylist.Exists(x => x.Date == tempdate))
                        {
                            calot.HOT++;
                        }
                        else if (weekendcondition)
                        {
                            calot.WOT++;
                        }
                        else
                        {
                            var projectattday = projectatt.FindAll(x => x.punch_time.Date == tempdate)
                                .OrderBy(x => x.punch_time).ToList();
                            var starttime = projectattday.First().punch_time;
                            var endtime = projectattday.Last().punch_time;
                            var totaltime = endtime - startdate;
                            var nightOvertimeHours = 0d;
                            var  regularOvertimeHours = 0d;
                            if (totaltime.TotalHours > 8)
                            {
                                var otr = totaltime.TotalHours - 8;
                                var Otstarttime = starttime.AddHours(8);
                                var nightStartTime = new TimeSpan(22, 0, 0); 
                                var nightEndTime = new TimeSpan(4, 0, 0);
                                if ((Otstarttime.TimeOfDay >= nightStartTime && Otstarttime.TimeOfDay < new TimeSpan(24, 0, 0)) ||
                                    (endtime.TimeOfDay >= new TimeSpan(0, 0, 0) && endtime.TimeOfDay < nightEndTime))
                                {
                                    if (Otstarttime.TimeOfDay >= nightStartTime)
                                    {
                                        nightOvertimeHours += (TimeSpan.FromHours(24) - Otstarttime.TimeOfDay).TotalHours;
                                    }

                                    if (endtime.TimeOfDay < nightEndTime)
                                    {
                                        nightOvertimeHours += endtime.TimeOfDay.TotalHours;
                                    }
                                    
                                    regularOvertimeHours = otr - nightOvertimeHours;
                                }
                                else
                                {
                                    regularOvertimeHours = otr;
                                }
                                calot.ROT = regularOvertimeHours;
                                //calot.NOT = nightOvertimeHours;
                            }
                        }

                    }

                    nextday: ;
                    tempdate = tempdate.AddDays(1);
                }

                calotlist.Add(calot);
            }

            /*
            foreach (var ot in calotlist)
            {
                    var savetest = db.cal_ot.ToList();
                    if (savetest.Exists(x => x.OT_month == ot.OT_month && x.Employee_id == ot.Employee_id))
                    {
                        var tempsavevar = savetest.Find(x =>
                            x.OT_month == ot.OT_month && x.Employee_id == ot.Employee_id);
                        if (tempsavevar != null)
                        {
                            tempsavevar.HOT = ot.HOT;
                            tempsavevar.ROT = ot.ROT;
                            tempsavevar.WOT = ot.WOT;
                            tempsavevar.NOT = ot.NOT;
                            db.Entry(tempsavevar).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        db.cal_ot.Add(ot);
                        db.SaveChanges();
                    }
            }
            */

            return View(calotlist);
        }
        public ActionResult payroll_index(DateTime? payrollmonth /*string message*/)
        {

            return View();
        }

        [HttpPost]
        public ActionResult calnewPayroll(DateTime? payrollmonth)
        {
            var paylist = new List<payrole>();
            if (!payrollmonth.HasValue)
            {
                var calot = db.cal_ot.Where(x => x.OT_month == payrollmonth.Value).OrderBy(x => x.Employee_id).ToList();
                var calabs = db.calabs.Where(x => x.absmonth == payrollmonth.Value).OrderBy(x => x.Employee_id).ToList();
                var callwop = db.cal_lwop.Where(x => x.lwop_month == payrollmonth.Value).OrderBy(x=>x.Employee_id).ToList();
                var contract = db.contracts.OrderByDescending(x=>x.date_changed).ThenBy(x=>x.employee_no).ToList();
                var alist = this.db.master_file
                    .Where(e => e.last_working_day == null)
                    .OrderBy(e => e.employee_no)
                    .ThenByDescending(x => x.date_changed)
                    .ToList();
                var afinallist = alist
                    .GroupBy(x => x.employee_no)
                    .Select(g => g.First())
                    .Where(x => x.employee_no != 0 && x.employee_no != 1 && x.employee_no != 100001)
                    .ToList();
                payrollmonth = new DateTime(payrollmonth.Value.Year, payrollmonth.Value.Month, 1);
                var payrolllist = db.payrolls.Where(x => x.forthemonth == payrollmonth.Value).ToList();
                foreach (var masterFile in afinallist)
                {
                    var conemp = contract.Find(x => x.employee_id == masterFile.employee_id);
                    var calotemp = calot.Find(x => x.Employee_id == masterFile.employee_id);
                    var calabsemp = calabs.Find(x => x.Employee_id == masterFile.employee_id);
                    var callwopemp = callwop.Find(x => x.Employee_id == masterFile.employee_id);
                    var pay = new payroll
                    {
                        employee_no = masterFile.employee_id, forthemonth = payrollmonth,
                        con_id = conemp.employee_id,
                        TicketAllowance_ = "0",
                        Arrears = "0",
                        FoodAllow = "0",
                        totalpayable = "0",
                        OTRegular = "0",
                        OTRegularAmt = "0",
                        OTFriday = "0",
                        OTFridayAmt = "0",
                        OTNight = "0",
                        OTNightAmt = "0",
                        HolidayOT = "0",
                        HolidayOTAmt = "0",
                        cashAdvances = "0",
                        HouseAllow = "0",
                        TransportationAllowance_ = "0",
                        Timekeeping = "0",
                        Communication = "0",
                        TrafficFines = "0",
                        Absents = "0",
                        AbsentsAmt = "0",
                        LWOP = "0",
                        LWOPAmt = "0",
                        TotalLWOP = "0",
                        others = "0",
                        Pension = "0",
                        TotalDedution = "0",
                        NetPay = "0",
                        remarks = null,
                        extra = null,
                        extra1 = null,
                        extra2 = null,
                        contract = conemp,
                        master_file = masterFile
                    };
                    if (payrolllist.Count() > 0 && payrolllist.Exists(x => x.employee_no == masterFile.employee_id))
                    {
                        pay = payrolllist.Find(x => x.employee_no == masterFile.employee_id);
                    }

                    var bas = "0";
                    bas = Unprotect(pay.contract.basic);
                    double.TryParse(bas, out var bas1);
                    var gros = "0";
                    gros = Unprotect(pay.contract.basic);
                    double.TryParse(gros, out var gros1);

                    var temp = 0d;
                    if (calotemp != null)
                    {
                        var tot = 0d;
                        pay.OTRegular = calotemp.ROT.ToString();
                        temp = bas1 / 8 * 1.25 * calotemp.ROT;
                        tot += temp;
                        pay.OTRegular = temp.ToString();
                        pay.OTNight = calotemp.NOT.ToString();
                        temp = bas1 / 8 * 1.5 * calotemp.NOT;
                        tot += temp;
                        pay.OTNightAmt = temp.ToString();
                        pay.OTFriday = calotemp.WOT.ToString();
                        double.TryParse(calotemp.WOT.ToString(), out var result);
                        temp = ((gros1 / 30d) * calotemp.WOT) + ((bas1 / 2d) * result);
                        tot += temp;
                        pay.OTFriday = temp.ToString();
                        pay.HolidayOT = calotemp.HOT.ToString();
                        double.TryParse(calotemp.HOT.ToString(), out var result1);
                        temp = ((gros1 / 30d) * calotemp.HOT) + ((bas1 / 2d) * result1);
                        tot += temp;
                        pay.HolidayOTAmt = temp.ToString();
                        pay.extra = tot.ToString(); //used extra for total overtime amount

                    }

                    if (calabsemp != null)
                    {
                        pay.Absents = calabsemp.absdays.ToString();
                        pay.AbsentsAmt = calabsemp.absdays.ToString();//need to put calculations 
                        
                    }

                    if (callwopemp != null)
                    {
                        pay.LWOP = callwopemp.lwop_days.ToString();
                        pay.LWOPAmt = callwopemp.lwop_days.ToString();//need to put calculations 

                    }

                    temp = 0d;
                    var tp = 0d;
                    double.TryParse(pay.contract.salary_details, out temp);
                    tp += temp;
                    double.TryParse(pay.TicketAllowance_, out temp);// we'll change to be automated
                    tp += temp;
                    double.TryParse(pay.FoodAllow, out temp);// we'll change to be automated
                    tp += temp;
                    double.TryParse(pay.Arrears, out temp);
                    tp += temp;
                    pay.totalpayable = temp.ToString();

                }
            }

            return RedirectToAction("payroll_index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}