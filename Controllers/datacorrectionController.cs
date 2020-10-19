using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRworks.Controllers
{
    using System.Data.Entity;

    using HRworks.Models;
    [Authorize(Users = "dinizsneden@gmail.com")]
    public class datacorrectionController : Controller
    {
        private HREntities db = new HREntities();

        // GET: datacorrection
        public void Index()
        {
            var con = this.db.contracts.ToList();
            var master = this.db.master_file.OrderBy(x=>x.employee_no).ThenByDescending(x=>x.date_changed).ToList();
            foreach (var id1 in con.OrderBy(x=>x.master_file.employee_no))
            {
                var cor = master.Find(x => x.employee_id == id1.employee_no);
                var mast = master.FindAll(x => x.employee_no == cor.employee_no).ToList();
                if (mast.Count() >=1)
                {
                    id1.employee_no = mast[0].employee_id;
                    db.Entry(id1).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
    }
}