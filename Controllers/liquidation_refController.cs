using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;

namespace HRworks.Controllers
{
    [Authorize]
    public class liquidation_refController : Controller
    {
        private HREntities db = new HREntities();
        [Authorize(Roles = "liquidation,super_admin")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: liquidation_ref/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "liquidation,super_admin")]
        public ActionResult Create([Bind(Include = "Id,date,refr,liq")] liquidation_ref liquidation_ref)
        {
            if (ModelState.IsValid)
            {
                long? a = 0l;
                DateTime? ba = DateTime.Now;
                long? c = 0l, refid;

                if (this.db.liquidation_ref.ToList().Exists(x => x.date == liquidation_ref.date && x.refr == liquidation_ref.refr && x.liq == liquidation_ref.liq))
                {
                    var aaaa = this.db.liquidation_ref.ToList().Find(x => x.date == liquidation_ref.date && x.refr == liquidation_ref.refr
                                                                                                         && x.liq
                                                                                                         == liquidation_ref
                                                                                                             .liq);
                    a = aaaa.refr;
                    ba = aaaa.date;
                    c = aaaa.liq;
                    liquidation_ref = aaaa;
                    this.db.Entry(liquidation_ref).State = EntityState.Modified;
                    this.db.SaveChanges();
                    aaaa = this.db.liquidation_ref.ToList().Find(x => x.date == liquidation_ref.date && x.refr == liquidation_ref.refr
                        && x.liq
                        == liquidation_ref
                            .liq);
                    refid = aaaa.Id;
                }
                else
                {
                    a = liquidation_ref.refr;
                    ba = liquidation_ref.date;
                    c = liquidation_ref.liq;
                    db.liquidation_ref.Add(liquidation_ref);
                    db.SaveChanges();
                    var aaaa = this.db.liquidation_ref.ToList().Find(x => x.date == liquidation_ref.date && x.refr == liquidation_ref.refr
                        && x.liq
                        == liquidation_ref
                            .liq);
                    refid = aaaa.Id;
                }
                return RedirectToAction("Create", "liquidations",new {refr = refid});
            }

            return View(liquidation_ref);
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
