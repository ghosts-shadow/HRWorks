using System.Web.Security;

namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;

    using HRworks.Models;

    public class liquidationsController : Controller
    {
        private readonly HREntities db = new HREntities();

        // GET: liquidations/Create
        [Authorize(Roles = "liquidation")]
        public ActionResult Create(long? refr, DateTime? date1 , long? liq)
        {
            var refrlist = this.db.liquidation_ref.ToList();
            if (refr.HasValue && date1.HasValue && liq.HasValue)
            {
                this.ViewBag.refr = refr;
                this.ViewBag.date = date1.Value.ToShortDateString();
                this.ViewBag.liq = liq;
            }else
            {
                this.ViewBag.refr = refrlist.Last().refr;
                this.ViewBag.date = refrlist.Last().date.Value.ToShortDateString();
                this.ViewBag.liq = refrlist.Last().liq;
            }
            this.ViewBag.employee_no = new SelectList(
                this.db.master_file.OrderBy(e => e.employee_no),
                "employee_id",
                "employee_no");
            ViewBag.nameofgov = new List<SelectListItem>()
            {
                new SelectListItem() {Value = "FEDERAL AUTHORITY OF IDENTITY AND CITIZENSHIP", Text = "FEDERAL AUTHORITY OF IDENTITY AND CITIZENSHIP"},
                new SelectListItem() {Value = "MINISTRY OF HUMAN RESOURCES & EMIRATISATION", Text = "MINISTRY OF HUMAN RESOURCES & EMIRATISATION"},
                new SelectListItem() {Value = "DEPARTMENT OF ECONOMIC ACTIVITIES", Text = "DEPARTMENT OF ECONOMIC ACTIVITIES"},
                new SelectListItem() {Value = "MUNICIPAL DEPARTMENT", Text = "MUNICIPAL DEPARTMENT"},
                new SelectListItem() {Value = "ABU DHABI POST", Text = "ABU DHABI POST"},
                new SelectListItem() {Value = "TASHEEL", Text = "TASHEEL"},
                new SelectListItem() {Value = "TAWJEEH", Text = "TAWJEEH"},
                new SelectListItem() {Value = "JUDICIAL DEPARTMENT", Text = "JUDICIAL DEPARTMENT"},
                new SelectListItem() {Value = "MINISTRY OF INTERIOR", Text = "MINISTRY OF INTERIOR"},
                new SelectListItem() {Value = "TRAFFIC DEPARTMENT", Text = "TRAFFIC DEPARTMENT"},
                new SelectListItem() {Value = "ANSARI EXCHANGE", Text = "ANSARI EXCHANGE"}
            };
            ViewBag.expenses = new List<SelectListItem>()
            {
                new SelectListItem() { Value="ARAMEX ABUDHABI", Text= "ARAMEX ABUDHABI" },
                new SelectListItem() { Value="MOL EMPLOYEES LIST", Text= "MOL EMPLOYEES LIST" },
                new SelectListItem() { Value="BANK", Text= "BANK" },
                new SelectListItem() { Value="CHARGE CARD", Text= "CHARGE CARD" },
                new SelectListItem() { Value="CANCEL LABOUR CARD", Text= "CANCEL LABOUR CARD" },
                new SelectListItem() { Value="CANCEL RESIDENCE", Text= "CANCEL RESIDENCE" },
                new SelectListItem() { Value="CHANGE STATUS", Text= "CHANGE STATUS" },
                new SelectListItem() { Value="COMPANY BOXES", Text= "COMPANY BOXES" },
                new SelectListItem() { Value="COMPANY FINES", Text= "COMPANY FINES" },
                new SelectListItem() { Value="COMPANY LICENSE RENEWAL", Text= "COMPANY LICENSE RENEWAL" },
                new SelectListItem() { Value="DEPOSIT REFUND", Text= "DEPOSIT REFUND" },
                new SelectListItem() { Value="FINAL EXIT TICKET", Text= "FINAL EXIT TICKET" },
                new SelectListItem() { Value="FINES & RENEW EMIRATES ID", Text= "FINES & RENEW EMIRATES ID" },
                new SelectListItem() { Value="ISSUE  RESIDENCE", Text= "ISSUE  RESIDENCE" },
                new SelectListItem() { Value="ISSUE NEW VISA", Text= "ISSUE NEW VISA" },
                new SelectListItem() { Value="TYPING CANCELLATION OF JOB OFFELETTER", Text= "TYPING CANCELLATION OF JOB OFFELETTER" },
                new SelectListItem() { Value="TYPING JOB OFFER", Text= "TYPING JOB OFFER" },
                new SelectListItem() { Value="FEES JUDICIAL DEPARTMENT", Text= "FEES JUDICIAL DEPARTMENT" },
                new SelectListItem() { Value="LEAVING - ISSUE", Text= "LEAVING - ISSUE" },
                new SelectListItem() { Value="MODIFY CONTRACT", Text= "MODIFY CONTRACT" },
                new SelectListItem() { Value="MODIFY ELECTRONIC WORK PERMIT", Text= "MODIFY ELECTRONIC WORK PERMIT" },
                new SelectListItem() { Value="NEW VISA PENALTY", Text= "NEW VISA PENALTY" },
                new SelectListItem() { Value="OUTSIDE THE COUNTRY CANCELLATION", Text= "OUTSIDE THE COUNTRY CANCELLATION" },
                new SelectListItem() { Value="PAY FINE RESIDENCE", Text= "PAY FINE RESIDENCE" },
                new SelectListItem() { Value="PRE APPROVAL FOR WORK PERMIT", Text= "PRE APPROVAL FOR WORK PERMIT" },
                new SelectListItem() { Value="PRO-CARD CANCEL", Text= "PRO-CARD CANCEL" },
                new SelectListItem() { Value="ABSCONDING - ELECTRONIC", Text= "ABSCONDING - ELECTRONIC" },
                new SelectListItem() { Value="REPORT ESCAPE", Text= "REPORT ESCAPE" },
                new SelectListItem() { Value="APPROVAL FOR WORK PERMIT FINES", Text= "APPROVAL FOR WORK PERMIT FINES" },
                new SelectListItem() { Value="TRADE LICENSE UPDATE", Text= "TRADE LICENSE UPDATE" },
                new SelectListItem() { Value="MEDICAL EXAMINATION ", Text= "MEDICAL EXAMINATION " },
                new SelectListItem() { Value="AMENDING AN EMPLOYMENT CONTRACT", Text= "AMENDING AN EMPLOYMENT CONTRACT" },
                new SelectListItem() { Value="VISA CANCELLATION VISA- WORK", Text= "VISA CANCELLATION VISA- WORK" },
                new SelectListItem() { Value="UPDATE WORKPERMIT INFORMATION", Text= "UPDATE WORKPERMIT INFORMATION" },
                new SelectListItem() { Value="UPDATE IMMIGRATION FILE", Text= "UPDATE IMMIGRATION FILE" },
                new SelectListItem() { Value="TYPING ELECTRONIC PRE APPROVAL FOR WORK", Text= "TYPING ELECTRONIC PRE APPROVAL FOR WORK" },
                new SelectListItem() { Value="TYPING APPROVAL FOR WORK PERMIT", Text= "TYPING APPROVAL FOR WORK PERMIT" },
                new SelectListItem() { Value="TRANSACTION FUND", Text= "TRANSACTION FUND" },
                new SelectListItem() { Value="SUBMIT RENEW LABOUR CARD", Text= "SUBMIT RENEW LABOUR CARD" },
                new SelectListItem() { Value="SUBMIT NEW WORK PERMIT", Text= "SUBMIT NEW WORK PERMIT" },
                new SelectListItem() { Value="SUBMIT ELECTRONIC WORK PERMIT", Text= "SUBMIT ELECTRONIC WORK PERMIT" },
                new SelectListItem() { Value="RESIDENCY-FROM NEW PASSPORT", Text= "RESIDENCY-FROM NEW PASSPORT" },
                new SelectListItem() { Value="RESIDENCES - PAY NEW FINE", Text= "RESIDENCES - PAY NEW FINE" },
                new SelectListItem() { Value="REPORTS - RESIDENCE DETAILS", Text= "REPORTS - RESIDENCE DETAILS" },
                new SelectListItem() { Value="RENEWAL OF TRADE LICENSE – ABUDHABI", Text= "RENEWAL OF TRADE LICENSE – ABUDHABI" },
                new SelectListItem() { Value="RENEWAL OF TRADE LICENSE – AL AIN", Text= "RENEWAL OF TRADE LICENSE – AL AIN" },
                new SelectListItem() { Value="RENEWAL OF NATIONAL OR GCC ELECTRONIC WORK", Text= "RENEWAL OF NATIONAL OR GCC ELECTRONIC WORK" },
                new SelectListItem() { Value="RENEWAL OF AL AIN PO BOX", Text= "RENEWAL OF AL AIN PO BOX" },
                new SelectListItem() { Value="RENEWAL OF ABUDHABI PO BOX", Text= "RENEWAL OF ABUDHABI PO BOX" },
                new SelectListItem() { Value="RENEW WORK PERMIT", Text= "RENEW WORK PERMIT" },
                new SelectListItem() { Value="RENEW SYSTEM", Text= "RENEW SYSTEM" },
                new SelectListItem() { Value="VISA RENEWAL", Text= "VISA RENEWAL" },
                new SelectListItem() { Value="NOQODI-ANSARI EXCHANGE", Text= "NOQODI-ANSARI EXCHANGE" },
                new SelectListItem() { Value="RENEW RESIDENCE", Text= "RENEW RESIDENC" },
                new SelectListItem() { Value="RENEW ESTABLISHMENTS", Text= "RENEW ESTABLISHMENTS" },
                new SelectListItem() { Value="RENEW ELECTRONIC WORK PERMIT", Text= "RENEW ELECTRONIC WORK PERMIT" }
            };
            var eel = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var liste = new List<master_file>();
            foreach (var file in eel)
                if (!liste.Exists(x => x.employee_no == file.employee_no))
                    liste.Add(file);

            this.ViewBag.eee = this.db.master_file.Select(x => x.employee_no).ToList();
            this.ViewBag.ee = new SelectList(liste, "employee_id", "employee_no");
            return this.View();
        }

        // POST: liquidations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "liquidation")]
        public ActionResult Create(liquilist liquilist)
        {
            ViewBag.nameofgov = new List<SelectListItem>()
            {
                new SelectListItem() {Value = "FEDERAL AUTHORITY OF IDENTITY AND CITIZENSHIP", Text = "FEDERAL AUTHORITY OF IDENTITY AND CITIZENSHIP"},
                new SelectListItem() {Value = "MINISTRY OF HUMAN RESOURCES & EMIRATISATION", Text = "MINISTRY OF HUMAN RESOURCES & EMIRATISATION"},
                new SelectListItem() {Value = "DEPARTMENT OF ECONOMIC ACTIVITIES", Text = "DEPARTMENT OF ECONOMIC ACTIVITIES"},
                new SelectListItem() {Value = "MUNICIPAL DEPARTMENT", Text = "MUNICIPAL DEPARTMENT"},
                new SelectListItem() {Value = "ABU DHABI POST", Text = "ABU DHABI POST"},
                new SelectListItem() {Value = "TASHEEL", Text = "TASHEEL"},
                new SelectListItem() {Value = "TAWJEEH", Text = "TAWJEEH"},
                new SelectListItem() {Value = "JUDICIAL DEPARTMENT", Text = "JUDICIAL DEPARTMENT"},
                new SelectListItem() {Value = "MINISTRY OF INTERIOR", Text = "MINISTRY OF INTERIOR"},
                new SelectListItem() {Value = "TRAFFIC DEPARTMENT", Text = "TRAFFIC DEPARTMENT"},
                new SelectListItem() {Value = "ANSARI EXCHANGE", Text = "ANSARI EXCHANGE"}
            };
            ViewBag.expenses = new List<SelectListItem>()
            {
                new SelectListItem() { Value="ARAMEX ABUDHABI", Text= "ARAMEX ABUDHABI" },
                new SelectListItem() { Value="MOL EMPLOYEES LIST", Text= "MOL EMPLOYEES LIST" },
                new SelectListItem() { Value="BANK", Text= "BANK" },
                new SelectListItem() { Value="CHARGE CARD", Text= "CHARGE CARD" },
                new SelectListItem() { Value="CANCEL LABOUR CARD", Text= "CANCEL LABOUR CARD" },
                new SelectListItem() { Value="CANCEL RESIDENCE", Text= "CANCEL RESIDENCE" },
                new SelectListItem() { Value="CHANGE STATUS", Text= "CHANGE STATUS" },
                new SelectListItem() { Value="COMPANY BOXES", Text= "COMPANY BOXES" },
                new SelectListItem() { Value="COMPANY FINES", Text= "COMPANY FINES" },
                new SelectListItem() { Value="COMPANY LICENSE RENEWAL", Text= "COMPANY LICENSE RENEWAL" },
                new SelectListItem() { Value="DEPOSIT REFUND", Text= "DEPOSIT REFUND" },
                new SelectListItem() { Value="FINAL EXIT TICKET", Text= "FINAL EXIT TICKET" },
                new SelectListItem() { Value="FINES & RENEW EMIRATES ID", Text= "FINES & RENEW EMIRATES ID" },
                new SelectListItem() { Value="ISSUE  RESIDENCE", Text= "ISSUE  RESIDENCE" },
                new SelectListItem() { Value="ISSUE NEW VISA", Text= "ISSUE NEW VISA" },
                new SelectListItem() { Value="TYPING CANCELLATION OF JOB OFFELETTER", Text= "TYPING CANCELLATION OF JOB OFFELETTER" },
                new SelectListItem() { Value="TYPING JOB OFFER", Text= "TYPING JOB OFFER" },
                new SelectListItem() { Value="FEES JUDICIAL DEPARTMENT", Text= "FEES JUDICIAL DEPARTMENT" },
                new SelectListItem() { Value="LEAVING - ISSUE", Text= "LEAVING - ISSUE" },
                new SelectListItem() { Value="MODIFY CONTRACT", Text= "MODIFY CONTRACT" },
                new SelectListItem() { Value="MODIFY ELECTRONIC WORK PERMIT", Text= "MODIFY ELECTRONIC WORK PERMIT" },
                new SelectListItem() { Value="NEW VISA PENALTY", Text= "NEW VISA PENALTY" },
                new SelectListItem() { Value="OUTSIDE THE COUNTRY CANCELLATION", Text= "OUTSIDE THE COUNTRY CANCELLATION" },
                new SelectListItem() { Value="PAY FINE RESIDENCE", Text= "PAY FINE RESIDENCE" },
                new SelectListItem() { Value="PRE APPROVAL FOR WORK PERMIT", Text= "PRE APPROVAL FOR WORK PERMIT" },
                new SelectListItem() { Value="PRO-CARD CANCEL", Text= "PRO-CARD CANCEL" },
                new SelectListItem() { Value="ABSCONDING - ELECTRONIC", Text= "ABSCONDING - ELECTRONIC" },
                new SelectListItem() { Value="REPORT ESCAPE", Text= "REPORT ESCAPE" },
                new SelectListItem() { Value="APPROVAL FOR WORK PERMIT FINES", Text= "APPROVAL FOR WORK PERMIT FINES" },
                new SelectListItem() { Value="TRADE LICENSE UPDATE", Text= "TRADE LICENSE UPDATE" },
                new SelectListItem() { Value="MEDICAL EXAMINATION ", Text= "MEDICAL EXAMINATION " },
                new SelectListItem() { Value="AMENDING AN EMPLOYMENT CONTRACT", Text= "AMENDING AN EMPLOYMENT CONTRACT" },
                new SelectListItem() { Value="VISA CANCELLATION VISA- WORK", Text= "VISA CANCELLATION VISA- WORK" },
                new SelectListItem() { Value="UPDATE WORKPERMIT INFORMATION", Text= "UPDATE WORKPERMIT INFORMATION" },
                new SelectListItem() { Value="UPDATE IMMIGRATION FILE", Text= "UPDATE IMMIGRATION FILE" },
                new SelectListItem() { Value="TYPING ELECTRONIC PRE APPROVAL FOR WORK", Text= "TYPING ELECTRONIC PRE APPROVAL FOR WORK" },
                new SelectListItem() { Value="TYPING APPROVAL FOR WORK PERMIT", Text= "TYPING APPROVAL FOR WORK PERMIT" },
                new SelectListItem() { Value="TRANSACTION FUND", Text= "TRANSACTION FUND" },
                new SelectListItem() { Value="SUBMIT RENEW LABOUR CARD", Text= "SUBMIT RENEW LABOUR CARD" },
                new SelectListItem() { Value="SUBMIT NEW WORK PERMIT", Text= "SUBMIT NEW WORK PERMIT" },
                new SelectListItem() { Value="SUBMIT ELECTRONIC WORK PERMIT", Text= "SUBMIT ELECTRONIC WORK PERMIT" },
                new SelectListItem() { Value="RESIDENCY-FROM NEW PASSPORT", Text= "RESIDENCY-FROM NEW PASSPORT" },
                new SelectListItem() { Value="RESIDENCES - PAY NEW FINE", Text= "RESIDENCES - PAY NEW FINE" },
                new SelectListItem() { Value="REPORTS - RESIDENCE DETAILS", Text= "REPORTS - RESIDENCE DETAILS" },
                new SelectListItem() { Value="RENEWAL OF TRADE LICENSE – ABUDHABI", Text= "RENEWAL OF TRADE LICENSE – ABUDHABI" },
                new SelectListItem() { Value="RENEWAL OF TRADE LICENSE – AL AIN", Text= "RENEWAL OF TRADE LICENSE – AL AIN" },
                new SelectListItem() { Value="RENEWAL OF NATIONAL OR GCC ELECTRONIC WORK", Text= "RENEWAL OF NATIONAL OR GCC ELECTRONIC WORK" },
                new SelectListItem() { Value="RENEWAL OF AL AIN PO BOX", Text= "RENEWAL OF AL AIN PO BOX" },
                new SelectListItem() { Value="RENEWAL OF ABUDHABI PO BOX", Text= "RENEWAL OF ABUDHABI PO BOX" },
                new SelectListItem() { Value="RENEW WORK PERMIT", Text= "RENEW WORK PERMIT" },
                new SelectListItem() { Value="RENEW SYSTEM", Text= "RENEW SYSTEM" },
                new SelectListItem() { Value="VISA RENEWAL", Text= "VISA RENEWAL" },
                new SelectListItem() { Value="NOQODI-ANSARI EXCHANGE", Text= "NOQODI-ANSARI EXCHANGE" },
                new SelectListItem() { Value="RENEW RESIDENCE", Text= "RENEW RESIDENC" },
                new SelectListItem() { Value="RENEW ESTABLISHMENTS", Text= "RENEW ESTABLISHMENTS" },
                new SelectListItem() { Value="RENEW ELECTRONIC WORK PERMIT", Text= "RENEW ELECTRONIC WORK PERMIT" }
            };
            var la = new liqiapproval();
            var errorlist = new List<string>();
            var refrlist = this.db.liquidation_ref.ToList();
            this.ViewBag.employee_no = new SelectList(
                this.db.master_file.OrderBy(e => e.employee_no),
                "employee_id",
                "employee_no");
            var eel = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var liste = new List<master_file>();
            foreach (var file in eel)
                if (!liste.Exists(x => x.employee_no == file.employee_no))
                    liste.Add(file);

            this.ViewBag.ee = new SelectList(liste, "employee_id", "employee_no");
            if (this.ModelState.IsValid)
            {
                foreach (var liqui in liquilist.Liquidations1)
                {
                    var lilist = this.db.liquidations.ToList();
                    if (liqui.bill_no != null && liqui.employee_no != null)
                    {
                        if (!lilist.Exists(x=>x.bill_no == liqui.bill_no && x.nameofgov == liqui.nameofgov))
                        {
                        liqui.refr = refrlist.Last().Id;
                        liqui.changed_by = User.Identity.Name;;
                        liqui.date_changed = DateTime.Now;
                        this.db.liquidations.Add(liqui);
                        this.db.SaveChanges();
                        var liquilist2 = this.db.liquidations.ToList();
                        la.sumited_by = User.Identity.Name;
                        la.date = refrlist.Last().date;
                        la.refre = refrlist.Last().refr;
                        la.pid = liquilist2.Last().Id;
                        la.status = "submitted";
                        this.db.liqiapprovals.Add(la);
                        this.db.SaveChanges();
                        }
                    else
                    {
                        if (errorlist.Count == 0)
                        { 
                            var errorstring = "the following bill no exists ";
                            errorlist.Add(errorstring);
                        }

                        var errorstring1 = " bill no:- "+liqui.bill_no+" exists for the issure " + liqui.nameofgov;
                        errorlist.Add(errorstring1);
                    }
                    }
                }

                if (errorlist.Count !=0)
                {
                    goto error1;
                }
                return this.RedirectToAction("Create", "liquidation_ref");
            }
            error1: ;
            ViewBag.error = errorlist;
            this.ViewBag.refr = refrlist.Last().refr;
            this.ViewBag.date = refrlist.Last().date.Value.ToShortDateString();
            return this.View(liquilist);
        }

        public ActionResult index(int? empno)
        {
            var lii = this.db.liquidations.ToList().OrderBy(x => x.master_file.employee_no);
            var lalist = this.db.liqiapprovals.ToList();
            List<liquidation> lii2 = new List<liquidation>();
            var eel = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var liste = new List<master_file>();
            foreach (var file in eel)
                if (!liste.Exists(x => x.employee_no == file.employee_no))
                    liste.Add(file);
            this.ViewBag.empno = new SelectList(liste, "employee_id", "employee_no");
            
            foreach (var lii1 in lii)
            {
                if (lalist.Exists(x => x.pid == lii1.Id))
                {
                    var il1 = lalist.Find(x => x.pid == lii1.Id);
                    if (il1.status == "submitted")
                    {
                        lii2.Add(lii1);
                    }
                    else if (il1.status.Contains("rejected"))
                    {
                        lii1.discription =  lii1.discription + " (" + il1.status + ")";
                        lii2.Add(lii1);
                    }
                    else
                    {
                        lii1.discription =  lii1.discription + " (" + il1.status + ")";
                        lii2.Add(lii1);
                    }
                }
            }
            if (empno != null)
            {
                var lii3 = lii2.FindAll(x=>x.employee_no == empno);
                return this.View(lii3.OrderByDescending(x=>x.liquidation_ref.refr));
            }
            else
            {
                return this.View(lii2.OrderByDescending(x => x.liquidation_ref.refr));
            }
        }

        [Authorize(Roles = "super_admin,admin")]
        public ActionResult subindex(DateTime? from ,DateTime? to)
        {
            var lii = this.db.liquidations.ToList().OrderBy(x => x.master_file.employee_no);
            var lalist = this.db.liqiapprovals.ToList();
            List<liquidation> lii2 = new List<liquidation>();
            var eel = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var liste = new List<master_file>();
            foreach (var file in eel)
                if (!liste.Exists(x => x.employee_no == file.employee_no))
                    liste.Add(file);
            this.ViewBag.empno = new SelectList(liste, "employee_id", "employee_no");
            
            foreach (var lii1 in lii)
            {
                if (lalist.Exists(x => x.pid == lii1.Id))
                {
                    var il1 = lalist.Find(x => x.pid == lii1.Id);
                    if (il1.status == "submitted")
                    {
                        lii2.Add(lii1);
                    }
                    else if (il1.status.Contains("rejected"))
                    {
                        lii1.discription =  lii1.discription + " (" + il1.status + ")";
                        lii2.Add(lii1);
                    }
                    else
                    {
                        lii1.discription =  lii1.discription + " (" + il1.status + ")";
                        lii2.Add(lii1);
                    }
                }
            }
            var sumlq = new List<liquisum>();
            List<liquidation> zzzz = new List<liquidation>();
            if (from.HasValue && to.HasValue)
            {
                 zzzz = lii2.FindAll(x => x.invoice_date >= from && x.invoice_date <= to);
            }
            else if(from.HasValue && !to.HasValue)
            {
                var enddate = new DateTime(from.Value.Year,from.Value.Month,DateTime.DaysInMonth(from.Value.Year,from.Value.Month));
                var startdate = new DateTime(from.Value.Year,from.Value.Month,1);
                zzzz = lii2.FindAll(x => x.invoice_date >= startdate && x.invoice_date <= enddate);
            }
            foreach (var lq in zzzz)
            {
                var wtf = new liquisum();
                wtf.ldate = lq.invoice_date;
                wtf.description = lq.expenses;
                wtf.quantity = 1;
                if (lq.invoice_amount != null) wtf.amount = (float)lq.invoice_amount.Value;
                if (!sumlq.Exists(x=> x.description == wtf.description))
                {
                    sumlq.Add(wtf);
                }
                else
                {
                    var aq = sumlq.Find(x => x.description == wtf.description);
                    sumlq.Remove(aq);
                    aq.amount = aq.amount + wtf.amount;
                    aq.quantity = aq.quantity + wtf.quantity;
                    sumlq.Add(aq);

                }
            }

            return this.View(sumlq.OrderBy(x => x.ldate));
        }

        public ActionResult print(DateTime? pdate, int? prefr , int? preli)
        {
            var printlist1 = new List<liquidation>();
            var printlist = new List<liquidation>();
            var printlist2 = new List<liquidation>();
            if (pdate.HasValue && prefr.HasValue)
            {
                var liqireflist = this.db.liquidation_ref.Where(x => x.date == pdate && x.refr == prefr && x.liq == preli).ToList();
                var liqilist = this.db.liquidations.ToList();
                foreach (var liid in liqireflist)
                {
                    printlist1 = this.db.liquidations.Where(x => x.refr == liid.Id).ToList();
                    this.ViewBag.prefr = prefr;
                    this.ViewBag.preli = preli;
                    this.ViewBag.pdate = pdate.Value;
                }
                decimal ttinsum = 0;

                foreach (var pl in printlist1)
                {
                    if (pl.invoice_amount.HasValue) ttinsum += pl.invoice_amount.Value;
                    pl.discription = "EMP#" + pl.master_file.employee_no;
                    if (!printlist2.Exists(x => x.expenses == pl.expenses))
                        printlist2.Add(pl);
                }
                    this.ViewBag.ttinsum = ttinsum;

                /*foreach (var pl1 in printlist2)
                {
                    decimal insum = 0;
                    decimal vsum = 0;
                    decimal tinsum = 0;
                    var pll = printlist1.FindAll(x => x.expenses == pl1.expenses);
                    foreach (var liq in pll)
                    {
                        if (liq.invoice.HasValue) insum = insum + liq.invoice.Value;
                        if (liq.VAT.HasValue) vsum = vsum + liq.VAT.Value;
                        if (liq.invoice_amount.HasValue) tinsum = tinsum + liq.invoice_amount.Value;
                    }

                    ttinsum += tinsum;
                    var count = pll.Count();
                    pl1.discription = $"{pl1.expenses} for {count} employees";
                    pl1.invoice_date = null;
                    pl1.VAT = vsum;
                    pl1.invoice = insum;
                    pl1.invoice_amount = tinsum;
                }*/
            }

            return this.View(printlist1);
        }

        [Authorize(Roles = "Manager")]
        public ActionResult liquiapprove(DateTime? pdate, int? prefr)
        {
            var lii =new List<liquidation>();
            List<liquidation> lii2 =new List<liquidation>();
            if (pdate.HasValue && prefr.HasValue)
            {
                var lalist = this.db.liqiapprovals.ToList();
                var lli = this.db.liquidations.Where(x => x.liquidation_ref.date == pdate.Value).OrderBy(x => x.master_file.employee_no);
                lii = lli.ToList();
                
                foreach (var lii1 in lii)
                {
                    if (lalist.Exists(x=>x.pid==lii1.Id))
                    {
                        var il1 = lalist.Find(x => x.pid == lii1.Id);
                        if (il1.status!="approved")
                        {
                            lii2.Add(lii1);
                        }
                    }
                }
                ViewBag.pdate = pdate.Value.ToShortDateString();
                ViewBag.pregr = prefr;
            }
            return this.View(lii2);
        }

        [Authorize(Roles = "Manager")]
        public ActionResult approveliqui(int? pid)
        {
            var lalist = this.db.liqiapprovals.ToList();
            var la = lalist.Find(x => x.pid == pid);
            if (la.pid != 0 || la != null)
            {
                la.approved_by = User.Identity.Name;
                la.status = "approved";
                db.Entry(la).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("liquiapprove");
        }

        [Authorize(Roles = "Manager")]
        public ActionResult rejectliqui(int? pid, string remark)
        {

            var lalist = this.db.liqiapprovals.ToList();
            var la = lalist.Find(x => x.pid == pid);
            if (la.pid != 0 || la != null)
            { 
                la.status = "rejected due to :" + remark;
            db.Entry(la).State = EntityState.Modified;
            db.SaveChanges();
            }
            return RedirectToAction("liquiapprove");
        }

        // GET: liquidations1/Edit/5
        //[Authorize(Roles = "super_admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            liquidation liquidation = db.liquidations.Find(id);
            if (liquidation == null)
            {
                return HttpNotFound();
            }

            ViewBag.employee_no = new SelectList(
                db.master_file,
                "employee_id",
                "employee_name",
                liquidation.employee_no);
            ViewBag.nameofgov = new List<SelectListItem>()
            {
                new SelectListItem() {Value = "FEDERAL AUTHORITY OF IDENTITY AND CITIZENSHIP", Text = "FEDERAL AUTHORITY OF IDENTITY AND CITIZENSHIP"},
                new SelectListItem() {Value = "MINISTRY OF HUMAN RESOURCES & EMIRATISATION", Text = "MINISTRY OF HUMAN RESOURCES & EMIRATISATION"},
                new SelectListItem() {Value = "DEPARTMENT OF ECONOMIC ACTIVITIES", Text = "DEPARTMENT OF ECONOMIC ACTIVITIES"},
                new SelectListItem() {Value = "MUNICIPAL DEPARTMENT", Text = "MUNICIPAL DEPARTMENT"},
                new SelectListItem() {Value = "ABU DHABI POST", Text = "ABU DHABI POST"},
                new SelectListItem() {Value = "TASHEEL", Text = "TASHEEL"},
                new SelectListItem() {Value = "TAWJEEH", Text = "TAWJEEH"},
                new SelectListItem() {Value = "JUDICIAL DEPARTMENT", Text = "JUDICIAL DEPARTMENT"},
                new SelectListItem() {Value = "MINISTRY OF INTERIOR", Text = "MINISTRY OF INTERIOR"},
                new SelectListItem() {Value = "TRAFFIC DEPARTMENT", Text = "TRAFFIC DEPARTMENT"},
                new SelectListItem() {Value = "ANSARI EXCHANGE", Text = "ANSARI EXCHANGE"}
            };
            ViewBag.expenses = new List<SelectListItem>()
            {
                new SelectListItem() { Value="ARAMEX ABUDHABI", Text= "ARAMEX ABUDHABI" },
                new SelectListItem() { Value="MOL EMPLOYEES LIST", Text= "MOL EMPLOYEES LIST" },
                new SelectListItem() { Value="BANK", Text= "BANK" },
                new SelectListItem() { Value="CHARGE CARD", Text= "CHARGE CARD" },
                new SelectListItem() { Value="CANCEL LABOUR CARD", Text= "CANCEL LABOUR CARD" },
                new SelectListItem() { Value="CANCEL RESIDENCE", Text= "CANCEL RESIDENCE" },
                new SelectListItem() { Value="CHANGE STATUS", Text= "CHANGE STATUS" },
                new SelectListItem() { Value="COMPANY BOXES", Text= "COMPANY BOXES" },
                new SelectListItem() { Value="COMPANY FINES", Text= "COMPANY FINES" },
                new SelectListItem() { Value="COMPANY LICENSE RENEWAL", Text= "COMPANY LICENSE RENEWAL" },
                new SelectListItem() { Value="DEPOSIT REFUND", Text= "DEPOSIT REFUND" },
                new SelectListItem() { Value="FINAL EXIT TICKET", Text= "FINAL EXIT TICKET" },
                new SelectListItem() { Value="FINES & RENEW EMIRATES ID", Text= "FINES & RENEW EMIRATES ID" },
                new SelectListItem() { Value="ISSUE  RESIDENCE", Text= "ISSUE  RESIDENCE" },
                new SelectListItem() { Value="ISSUE NEW VISA", Text= "ISSUE NEW VISA" },
                new SelectListItem() { Value="TYPING CANCELLATION OF JOB OFFELETTER", Text= "TYPING CANCELLATION OF JOB OFFELETTER" },
                new SelectListItem() { Value="TYPING JOB OFFER", Text= "TYPING JOB OFFER" },
                new SelectListItem() { Value="FEES JUDICIAL DEPARTMENT", Text= "FEES JUDICIAL DEPARTMENT" },
                new SelectListItem() { Value="LEAVING - ISSUE", Text= "LEAVING - ISSUE" },
                new SelectListItem() { Value="MODIFY CONTRACT", Text= "MODIFY CONTRACT" },
                new SelectListItem() { Value="MODIFY ELECTRONIC WORK PERMIT", Text= "MODIFY ELECTRONIC WORK PERMIT" },
                new SelectListItem() { Value="NEW VISA PENALTY", Text= "NEW VISA PENALTY" },
                new SelectListItem() { Value="OUTSIDE THE COUNTRY CANCELLATION", Text= "OUTSIDE THE COUNTRY CANCELLATION" },
                new SelectListItem() { Value="PAY FINE RESIDENCE", Text= "PAY FINE RESIDENCE" },
                new SelectListItem() { Value="PRE APPROVAL FOR WORK PERMIT", Text= "PRE APPROVAL FOR WORK PERMIT" },
                new SelectListItem() { Value="PRO-CARD CANCEL", Text= "PRO-CARD CANCEL" },
                new SelectListItem() { Value="ABSCONDING - ELECTRONIC", Text= "ABSCONDING - ELECTRONIC" },
                new SelectListItem() { Value="REPORT ESCAPE", Text= "REPORT ESCAPE" },
                new SelectListItem() { Value="APPROVAL FOR WORK PERMIT FINES", Text= "APPROVAL FOR WORK PERMIT FINES" },
                new SelectListItem() { Value="TRADE LICENSE UPDATE", Text= "TRADE LICENSE UPDATE" },
                new SelectListItem() { Value="MEDICAL EXAMINATION ", Text= "MEDICAL EXAMINATION " },
                new SelectListItem() { Value="AMENDING AN EMPLOYMENT CONTRACT", Text= "AMENDING AN EMPLOYMENT CONTRACT" },
                new SelectListItem() { Value="VISA CANCELLATION VISA- WORK", Text= "VISA CANCELLATION VISA- WORK" },
                new SelectListItem() { Value="UPDATE WORKPERMIT INFORMATION", Text= "UPDATE WORKPERMIT INFORMATION" },
                new SelectListItem() { Value="UPDATE IMMIGRATION FILE", Text= "UPDATE IMMIGRATION FILE" },
                new SelectListItem() { Value="TYPING ELECTRONIC PRE APPROVAL FOR WORK", Text= "TYPING ELECTRONIC PRE APPROVAL FOR WORK" },
                new SelectListItem() { Value="TYPING APPROVAL FOR WORK PERMIT", Text= "TYPING APPROVAL FOR WORK PERMIT" },
                new SelectListItem() { Value="TRANSACTION FUND", Text= "TRANSACTION FUND" },
                new SelectListItem() { Value="SUBMIT RENEW LABOUR CARD", Text= "SUBMIT RENEW LABOUR CARD" },
                new SelectListItem() { Value="SUBMIT NEW WORK PERMIT", Text= "SUBMIT NEW WORK PERMIT" },
                new SelectListItem() { Value="SUBMIT ELECTRONIC WORK PERMIT", Text= "SUBMIT ELECTRONIC WORK PERMIT" },
                new SelectListItem() { Value="RESIDENCY-FROM NEW PASSPORT", Text= "RESIDENCY-FROM NEW PASSPORT" },
                new SelectListItem() { Value="RESIDENCES - PAY NEW FINE", Text= "RESIDENCES - PAY NEW FINE" },
                new SelectListItem() { Value="REPORTS - RESIDENCE DETAILS", Text= "REPORTS - RESIDENCE DETAILS" },
                new SelectListItem() { Value="RENEWAL OF TRADE LICENSE – ABUDHABI", Text= "RENEWAL OF TRADE LICENSE – ABUDHABI" },
                new SelectListItem() { Value="RENEWAL OF TRADE LICENSE – AL AIN", Text= "RENEWAL OF TRADE LICENSE – AL AIN" },
                new SelectListItem() { Value="RENEWAL OF NATIONAL OR GCC ELECTRONIC WORK", Text= "RENEWAL OF NATIONAL OR GCC ELECTRONIC WORK" },
                new SelectListItem() { Value="RENEWAL OF AL AIN PO BOX", Text= "RENEWAL OF AL AIN PO BOX" },
                new SelectListItem() { Value="RENEWAL OF ABUDHABI PO BOX", Text= "RENEWAL OF ABUDHABI PO BOX" },
                new SelectListItem() { Value="RENEW WORK PERMIT", Text= "RENEW WORK PERMIT" },
                new SelectListItem() { Value="RENEW SYSTEM", Text= "RENEW SYSTEM" },
                new SelectListItem() { Value="VISA RENEWAL", Text= "VISA RENEWAL" },
                new SelectListItem() { Value="NOQODI-ANSARI EXCHANGE", Text= "NOQODI-ANSARI EXCHANGE" },
                new SelectListItem() { Value="RENEW RESIDENCE", Text= "RENEW RESIDENC" },
                new SelectListItem() { Value="RENEW ESTABLISHMENTS", Text= "RENEW ESTABLISHMENTS" },
                new SelectListItem() { Value="RENEW ELECTRONIC WORK PERMIT", Text= "RENEW ELECTRONIC WORK PERMIT" }
            };
            ViewBag.refr = new SelectList(db.liquidation_ref, "Id", "Id", liquidation.refr);
            return View(liquidation);
        }
        // POST: liquidations1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "super_admin")]
        public ActionResult Edit(
            liquidation liquidation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liquidation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.employee_no = new SelectList(
                db.master_file,
                "employee_id",
                "employee_name",
                liquidation.employee_no);
            ViewBag.nameofgov = new List<SelectListItem>()
            {
                new SelectListItem() {Value = "FEDERAL AUTHORITY OF IDENTITY AND CITIZENSHIP", Text = "FEDERAL AUTHORITY OF IDENTITY AND CITIZENSHIP"},
                new SelectListItem() {Value = "MINISTRY OF HUMAN RESOURCES & EMIRATISATION", Text = "MINISTRY OF HUMAN RESOURCES & EMIRATISATION"},
                new SelectListItem() {Value = "DEPARTMENT OF ECONOMIC ACTIVITIES", Text = "DEPARTMENT OF ECONOMIC ACTIVITIES"},
                new SelectListItem() {Value = "MUNICIPAL DEPARTMENT", Text = "MUNICIPAL DEPARTMENT"},
                new SelectListItem() {Value = "ABU DHABI POST", Text = "ABU DHABI POST"},
                new SelectListItem() {Value = "TASHEEL", Text = "TASHEEL"},
                new SelectListItem() {Value = "TAWJEEH", Text = "TAWJEEH"},
                new SelectListItem() {Value = "JUDICIAL DEPARTMENT", Text = "JUDICIAL DEPARTMENT"},
                new SelectListItem() {Value = "MINISTRY OF INTERIOR", Text = "MINISTRY OF INTERIOR"},
                new SelectListItem() {Value = "TRAFFIC DEPARTMENT", Text = "TRAFFIC DEPARTMENT"},
                new SelectListItem() {Value = "ANSARI EXCHANGE", Text = "ANSARI EXCHANGE"}
            };
            ViewBag.expenses = new List<SelectListItem>()
            {
                new SelectListItem() { Value="ARAMEX ABUDHABI", Text= "ARAMEX ABUDHABI" },
                new SelectListItem() { Value="MOL EMPLOYEES LIST", Text= "MOL EMPLOYEES LIST" },
                new SelectListItem() { Value="BANK", Text= "BANK" },
                new SelectListItem() { Value="CHARGE CARD", Text= "CHARGE CARD" },
                new SelectListItem() { Value="CANCEL LABOUR CARD", Text= "CANCEL LABOUR CARD" },
                new SelectListItem() { Value="CANCEL RESIDENCE", Text= "CANCEL RESIDENCE" },
                new SelectListItem() { Value="CHANGE STATUS", Text= "CHANGE STATUS" },
                new SelectListItem() { Value="COMPANY BOXES", Text= "COMPANY BOXES" },
                new SelectListItem() { Value="COMPANY FINES", Text= "COMPANY FINES" },
                new SelectListItem() { Value="COMPANY LICENSE RENEWAL", Text= "COMPANY LICENSE RENEWAL" },
                new SelectListItem() { Value="DEPOSIT REFUND", Text= "DEPOSIT REFUND" },
                new SelectListItem() { Value="FINAL EXIT TICKET", Text= "FINAL EXIT TICKET" },
                new SelectListItem() { Value="FINES & RENEW EMIRATES ID", Text= "FINES & RENEW EMIRATES ID" },
                new SelectListItem() { Value="ISSUE  RESIDENCE", Text= "ISSUE  RESIDENCE" },
                new SelectListItem() { Value="ISSUE NEW VISA", Text= "ISSUE NEW VISA" },
                new SelectListItem() { Value="TYPING CANCELLATION OF JOB OFFELETTER", Text= "TYPING CANCELLATION OF JOB OFFELETTER" },
                new SelectListItem() { Value="TYPING JOB OFFER", Text= "TYPING JOB OFFER" },
                new SelectListItem() { Value="FEES JUDICIAL DEPARTMENT", Text= "FEES JUDICIAL DEPARTMENT" },
                new SelectListItem() { Value="LEAVING - ISSUE", Text= "LEAVING - ISSUE" },
                new SelectListItem() { Value="MODIFY CONTRACT", Text= "MODIFY CONTRACT" },
                new SelectListItem() { Value="MODIFY ELECTRONIC WORK PERMIT", Text= "MODIFY ELECTRONIC WORK PERMIT" },
                new SelectListItem() { Value="NEW VISA PENALTY", Text= "NEW VISA PENALTY" },
                new SelectListItem() { Value="OUTSIDE THE COUNTRY CANCELLATION", Text= "OUTSIDE THE COUNTRY CANCELLATION" },
                new SelectListItem() { Value="PAY FINE RESIDENCE", Text= "PAY FINE RESIDENCE" },
                new SelectListItem() { Value="PRE APPROVAL FOR WORK PERMIT", Text= "PRE APPROVAL FOR WORK PERMIT" },
                new SelectListItem() { Value="PRO-CARD CANCEL", Text= "PRO-CARD CANCEL" },
                new SelectListItem() { Value="ABSCONDING - ELECTRONIC", Text= "ABSCONDING - ELECTRONIC" },
                new SelectListItem() { Value="REPORT ESCAPE", Text= "REPORT ESCAPE" },
                new SelectListItem() { Value="APPROVAL FOR WORK PERMIT FINES", Text= "APPROVAL FOR WORK PERMIT FINES" },
                new SelectListItem() { Value="TRADE LICENSE UPDATE", Text= "TRADE LICENSE UPDATE" },
                new SelectListItem() { Value="MEDICAL EXAMINATION ", Text= "MEDICAL EXAMINATION " },
                new SelectListItem() { Value="AMENDING AN EMPLOYMENT CONTRACT", Text= "AMENDING AN EMPLOYMENT CONTRACT" },
                new SelectListItem() { Value="VISA CANCELLATION VISA- WORK", Text= "VISA CANCELLATION VISA- WORK" },
                new SelectListItem() { Value="UPDATE WORKPERMIT INFORMATION", Text= "UPDATE WORKPERMIT INFORMATION" },
                new SelectListItem() { Value="UPDATE IMMIGRATION FILE", Text= "UPDATE IMMIGRATION FILE" },
                new SelectListItem() { Value="TYPING ELECTRONIC PRE APPROVAL FOR WORK", Text= "TYPING ELECTRONIC PRE APPROVAL FOR WORK" },
                new SelectListItem() { Value="TYPING APPROVAL FOR WORK PERMIT", Text= "TYPING APPROVAL FOR WORK PERMIT" },
                new SelectListItem() { Value="TRANSACTION FUND", Text= "TRANSACTION FUND" },
                new SelectListItem() { Value="SUBMIT RENEW LABOUR CARD", Text= "SUBMIT RENEW LABOUR CARD" },
                new SelectListItem() { Value="SUBMIT NEW WORK PERMIT", Text= "SUBMIT NEW WORK PERMIT" },
                new SelectListItem() { Value="SUBMIT ELECTRONIC WORK PERMIT", Text= "SUBMIT ELECTRONIC WORK PERMIT" },
                new SelectListItem() { Value="RESIDENCY-FROM NEW PASSPORT", Text= "RESIDENCY-FROM NEW PASSPORT" },
                new SelectListItem() { Value="RESIDENCES - PAY NEW FINE", Text= "RESIDENCES - PAY NEW FINE" },
                new SelectListItem() { Value="REPORTS - RESIDENCE DETAILS", Text= "REPORTS - RESIDENCE DETAILS" },
                new SelectListItem() { Value="RENEWAL OF TRADE LICENSE – ABUDHABI", Text= "RENEWAL OF TRADE LICENSE – ABUDHABI" },
                new SelectListItem() { Value="RENEWAL OF TRADE LICENSE – AL AIN", Text= "RENEWAL OF TRADE LICENSE – AL AIN" },
                new SelectListItem() { Value="RENEWAL OF NATIONAL OR GCC ELECTRONIC WORK", Text= "RENEWAL OF NATIONAL OR GCC ELECTRONIC WORK" },
                new SelectListItem() { Value="RENEWAL OF AL AIN PO BOX", Text= "RENEWAL OF AL AIN PO BOX" },
                new SelectListItem() { Value="RENEWAL OF ABUDHABI PO BOX", Text= "RENEWAL OF ABUDHABI PO BOX" },
                new SelectListItem() { Value="RENEW WORK PERMIT", Text= "RENEW WORK PERMIT" },
                new SelectListItem() { Value="RENEW SYSTEM", Text= "RENEW SYSTEM" },
                new SelectListItem() { Value="VISA RENEWAL", Text= "VISA RENEWAL" },
                new SelectListItem() { Value="NOQODI-ANSARI EXCHANGE", Text= "NOQODI-ANSARI EXCHANGE" },
                new SelectListItem() { Value="RENEW RESIDENCE", Text= "RENEW RESIDENC" },
                new SelectListItem() { Value="RENEW ESTABLISHMENTS", Text= "RENEW ESTABLISHMENTS" },
                new SelectListItem() { Value="RENEW ELECTRONIC WORK PERMIT", Text= "RENEW ELECTRONIC WORK PERMIT" }
            };
            ViewBag.refr = new SelectList(db.liquidation_ref, "Id", "Id", liquidation.refr);
            return View(liquidation);
        }

        // GET: liquidations1/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            liquidation liquidation = db.liquidations.Find(id);
            if (liquidation == null)
            {
                return HttpNotFound();
            }

            return View(liquidation);
        }

        // POST: liquidations1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            liquidation liquidation = db.liquidations.Find(id);
            db.liquidations.Remove(liquidation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }

    }

}