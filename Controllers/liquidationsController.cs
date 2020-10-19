namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    using HRworks.Models;

    public class liquidationsController : Controller
    {
        private readonly HREntities db = new HREntities();

        // GET: liquidations/Create
        [Authorize(Roles = "liquidation")]
        public ActionResult Create()
        {
            var refrlist = this.db.liquidation_ref.ToList();
            this.ViewBag.refr = refrlist.Last().refr;
            this.ViewBag.date = refrlist.Last().date.Value.ToShortDateString();
            this.ViewBag.employee_no = new SelectList(
                this.db.master_file.OrderBy(e => e.employee_no),
                "employee_id",
                "employee_no");
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
            var la = new liqiapproval();
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
                }

                return this.RedirectToAction("Create", "liquidation_ref");
            }

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
                return this.View(lii3.OrderBy(x=>x.invoice_date));
            }
            else
            {
                return this.View(lii2.OrderBy(x => x.invoice_date));
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

        public ActionResult print(DateTime? pdate, int? prefr)
        {
            var printlist1 = new List<liquidation>();
            var printlist = new List<liquidation>();
            var printlist2 = new List<liquidation>();
            if (pdate.HasValue && prefr.HasValue)
            {
                var liqireflist = this.db.liquidation_ref.Where(x => x.date == pdate && x.refr == prefr).ToList();
                var liqilist = this.db.liquidations.ToList();
                foreach (var liid in liqireflist)
                {
                    printlist1 = this.db.liquidations.Where(x => x.refr == liid.Id).ToList();
                    this.ViewBag.prefr = prefr;
                    this.ViewBag.pdate = pdate.Value.ToShortDateString();
                }

                foreach (var pl in printlist1)
                    if (!printlist2.Exists(x => x.expenses == pl.expenses))
                        printlist2.Add(pl);

                decimal ttinsum = 0;
                foreach (var pl1 in printlist2)
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
                    this.ViewBag.ttinsum = ttinsum;
                    var count = pll.Count();
                    pl1.discription = $"{pl1.expenses} for {count} employees";
                    pl1.invoice_date = null;
                    pl1.VAT = vsum;
                    pl1.invoice = insum;
                    pl1.invoice_amount = tinsum;
                }
            }

            return this.View(printlist2);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }

    }

}