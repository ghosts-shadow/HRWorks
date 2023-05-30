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
            var masterFileEntries = db.master_file.ToList();

            var uniqueEntries = new List<master_file>();
            var duplicateEntries = new List<master_file>();

            foreach (var entry in masterFileEntries)
            {
                if (uniqueEntries.Any(e => e.employee_no == entry.employee_no))
                {
                    var existingEntry = uniqueEntries.First(e => e.employee_no == entry.employee_no);
                    if (entry.date_changed > existingEntry.date_changed)
                    {
                        uniqueEntries.Remove(existingEntry);
                        uniqueEntries.Add(entry);
                        duplicateEntries.Add(existingEntry);
                    }
                    else
                    {
                        duplicateEntries.Add(entry);
                    }
                }
                else
                {
                    uniqueEntries.Add(entry);
                }
            }
            
            foreach (var duplicateEntry in duplicateEntries)
            {
               // duplicateEntry. = uniqueEntries.First(e => e.employee_no == duplicateEntry.employee_no).connection;
            }
            
            //db.SaveChanges();
        }
    }
}