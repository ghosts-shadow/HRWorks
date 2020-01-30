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
    public class liquidation_refController : Controller
    {
        private HREntities db = new HREntities();

        public ActionResult Create()
        {
            return View();
        }

        // POST: liquidation_ref/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,date,refr")] liquidation_ref liquidation_ref)
        {
            if (ModelState.IsValid)
            {
                db.liquidation_ref.Add(liquidation_ref);
                db.SaveChanges();
                return RedirectToAction("Create","liquidations");
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
