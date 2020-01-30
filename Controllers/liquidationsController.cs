namespace HRworks.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    using HRworks.Models;

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
            return this.View();
        }

        public ActionResult index()
        {
            var lii = this.db.liquidations.ToList();
            return this.View(lii);
        }

        // POST: liquidations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(liquilist liquilist)
        {
            if (this.ModelState.IsValid)
            {
                    var refrlist = this.db.liquidation_ref.ToList();
                foreach (var liqui in liquilist.Liquidations1)
                {
                    liqui.refr = refrlist.Last().Id;
                    this.db.liquidations.Add(liqui);
                    this.db.SaveChanges();
                }
                    return this.RedirectToAction("Create", "liquidation_ref");
            }
            return this.View(liquilist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}