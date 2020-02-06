namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    using HRworks.Models;

    using Microsoft.Ajax.Utilities;

    public class liquidationsController : Controller
    {
        private readonly HREntities db = new HREntities();

        // GET: liquidations/Create
        public ActionResult Create()
        {
            var refrlist=this.db.liquidation_ref.ToList();
            ViewBag.refr = refrlist.Last().refr;
            ViewBag.date = refrlist.Last().date.Value.ToShortDateString();
            ViewBag.employee_no = new SelectList(db.master_file.OrderBy(e => e.employee_no),"employee_id","employee_no");
            var eel=this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var liste = new List<master_file>();
            foreach (var file in eel)
            {
                if (!liste.Exists(x=>x.employee_no == file.employee_no))
                {
                    liste.Add(file);
                }
            }

            ViewBag.eee = this.db.master_file.Select(x=>x.employee_no).ToList();
            ViewBag.ee = new SelectList(liste, "employee_id", "employee_no");
            return this.View();
        }

        public ActionResult index()
        {
            var lii = this.db.liquidations.ToList().OrderBy(x => x.master_file.employee_no);
            return this.View(lii);
        }

        public ActionResult print(DateTime? pdate,int? prefr)
        {
            var printlist1=new List<liquidation>();
                var printlist= new List<liquidation>();
                var printlist2= new List<liquidation>();
            if (pdate.HasValue && prefr.HasValue)
            {
                var liqireflist = this.db.liquidation_ref.Where(x => x.date == pdate && x.refr == prefr).ToList();
                var liqilist = this.db.liquidations.ToList();
                foreach (var liid in liqireflist)
                {
                    printlist1 = this.db.liquidations.Where(x => x.refr == liid.Id).ToList();
                    ViewBag.refr = prefr;
                    ViewBag.pdate = pdate.Value.ToShortDateString();
                }
                foreach (var pl in printlist1)
                {
                    if (!printlist2.Exists(x => x.expenses == pl.expenses))
                    {
                        printlist2.Add(pl);
                    }
                }

                decimal ttinsum = 0;
                foreach (var pl1 in printlist2)
                {
                    decimal insum = 0;
                    decimal vsum = 0;
                    decimal tinsum = 0;
                    var pll = printlist1.FindAll(x => x.expenses == pl1.expenses);
                    foreach (var liq in pll)
                    {
                        if (liq.invoice.HasValue)
                        {
                            insum = insum + liq.invoice.Value;
                        }
                        if (liq.VAT.HasValue)
                        {
                            vsum = vsum + liq.VAT.Value;
                        }
                        if (liq.invoice_amount.HasValue)
                        {
                            tinsum = tinsum + liq.invoice_amount.Value;
                        }
                    }
                    ttinsum += tinsum;
                    ViewBag.ttinsum = ttinsum;
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

        // POST: liquidations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(liquilist liquilist)
        {
            var refrlist = this.db.liquidation_ref.ToList();
            ViewBag.employee_no = new SelectList(
                db.master_file.OrderBy(e => e.employee_no),
                "employee_id",
                "employee_no");
            var eel = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var liste = new List<master_file>();
            foreach (var file in eel)
            {
                if (!liste.Exists(x => x.employee_no == file.employee_no))
                {
                    liste.Add(file);
                }
            }

            ViewBag.ee = new SelectList(liste, "employee_id", "employee_no");
            if (this.ModelState.IsValid)
            {
                foreach (var liqui in liquilist.Liquidations1)
                {
                    var lilist = db.liquidations.ToList();
                    if (liqui.bill_no != null && liqui.employee_no != null)
                    {
                                liqui.refr = refrlist.Last().Id;
                                this.db.liquidations.Add(liqui);
                                this.db.SaveChanges();
                    }
                }
                
                return this.RedirectToAction("Create", "liquidation_ref");
            }
            ViewBag.refr = refrlist.Last().refr;
            ViewBag.date = refrlist.Last().date.Value.ToShortDateString();
            return this.View(liquilist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}