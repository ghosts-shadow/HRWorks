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
    [Authorize(Roles ="super_admin,admin")]
    public class usernamesController : Controller
    {
        private HREntities db = new HREntities();

        // GET: usernames
        public ActionResult Index()
        {
            return View(db.usernames.Include(p=>p.AspNetUser).ToList());
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
