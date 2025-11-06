using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using Microsoft.Ajax.Utilities;
using MimeKit;
using MailKit.Net.Smtp;

namespace HRworks.Controllers
{

    [Authorize]
    public class Att_adjController : Controller
    {
        private HREntities db = new HREntities();
        private biometrics_DBEntities db1 = new biometrics_DBEntities();

        // GET: Att_adj
        public ActionResult Index()
        {
            var empuser = db.usernames
                .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);
            
            var att_adj = db.Att_adj.Include(a => a.master_file).Where(x=>x.master_file.employee_no == empuser.master_file.employee_no);
            return View(att_adj.ToList());
        }

        public ActionResult empattindex(DateTime? empatdatefrom, DateTime? empatdateto)
        {
            var empuser = db.usernames
                .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);
            var empint = 7770000;
            var finallist = new List<hik>();
            if (empatdatefrom.HasValue)
            {
                var empstring = empuser.master_file.employee_no.ToString();
                if (empstring.Contains("7770"))
                {
                    var sub = empstring.Substring(4, 4);
                    int.TryParse(sub, out int result);
                    empint += result;

                }

                var HOatt = db.hiks
                    .Where(x => x.ID == empuser.master_file.employee_no.ToString() || x.ID == empint.ToString())
                    .ToList();
                var projectatt = db1.iclock_transaction.Where(x =>
                        x.emp_code == empuser.master_file.employee_no.ToString() || x.emp_code == empint.ToString())
                    .ToList();
                if (HOatt.Count > 0)
                {
                    foreach (var hik in HOatt)
                    {
                        hik.ID = empuser.master_file.emiid;
                        hik.Person = empuser.master_file.employee_name;
                        finallist.Add(hik);
                    }
                }

                if (projectatt.Count > 0)
                {
                    foreach (var tratt in projectatt)
                    {
                        var protoho = new hik();
                        protoho.ID = empuser.master_file.emiid;
                        protoho.datetime = tratt.punch_time;
                        if (tratt.punch_state == "0")
                        {
                            protoho.Status = "check in";
                        }
                        else
                        {
                            protoho.Status = "check out";
                        }
                        finallist.Add(protoho);
                    }
                }
                

                finallist = finallist.FindAll(x => x.date.HasValue && x.date.Value.Date >= empatdatefrom.Value.Date);
                if (empatdateto.HasValue)
                {
                    finallist = finallist.FindAll(x => x.date.Value.Date <= empatdateto.Value.Date).OrderBy(x=>x.datetime).ToList();
                }

                return View(finallist.OrderBy(x => {
                    // Try to parse numeric part
                    if (int.TryParse(x.ID, out var num))
                        return (0, num); // group 0 = plain numbers
                    else if (x.ID.StartsWith("G-") && int.TryParse(x.ID.Substring(2), out var gnum))
                        return (1, gnum); // group 1 = G-numbers
                    else
                        return (2, int.MaxValue); // group 2 = anything else
                }).ToList());
            }

            return View(finallist);
        }

        //[Authorize(Roles = "HOD,employee,Manager")]
        public ActionResult EmpIndex() 
        {

            var emprel = db.emprels.ToList();
            var empuser = db.usernames
                .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);

            var att_adj = new List<Att_adj>();
            if (empuser == null)
            {
                return View(att_adj);
            }
            att_adj = db.Att_adj.Include(a => a.master_file).Where(x=>x.emp_ID == empuser.employee_no).OrderByDescending(x=>x.date_added).ToList();
            return View(att_adj);
        }

        // GET: Att_adj/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Att_adj att_adj = db.Att_adj.Find(id);
            if (att_adj == null)
            {
                return HttpNotFound();
            }
            return View(att_adj);
        }

        // GET: Att_adj/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Att_adj/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,emp_ID,which_date,late_in,early_out,reason,ap1,ap2,date_added,date_modified")] Att_adj att_adj)
        {
            if (ModelState.IsValid)
            {
                var emprel = db.emprels.ToList();
                var empuser = db.usernames
                    .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);
                att_adj.emp_ID = empuser.employee_no.Value;
                att_adj.master_file = empuser.master_file;
                att_adj.date_added = DateTime.Now;
                att_adj.date_modified = DateTime.Now;
                db.Att_adj.Add(att_adj);
                db.SaveChanges();
                var sendmailtrid = db.Att_adj.ToList().Last();
               // SendMail("", "submitted", sendmailtrid.Id);
                return RedirectToAction("Index");
            }
            
            return View(att_adj);
        }

        // GET: Att_adj/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Att_adj att_adj = db.Att_adj.Find(id);
            if (att_adj == null)
            {
                return HttpNotFound();
            }
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name", att_adj.emp_ID);
            return View(att_adj);
        }

        // POST: Att_adj/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,emp_ID,which_date,late_in,early_out,reason,ap1,ap2,date_added,date_modified")] Att_adj att_adj)
        {
            if (ModelState.IsValid)
            {
                db.Entry(att_adj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.emp_ID = new SelectList(db.master_file, "employee_id", "employee_name", att_adj.emp_ID);
            return View(att_adj);
        }

        // GET: Att_adj/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Att_adj att_adj = db.Att_adj.Find(id);
            if (att_adj == null)
            {
                return HttpNotFound();
            }
            return View(att_adj);
        }

        // POST: Att_adj/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Att_adj att_adj = db.Att_adj.Find(id);
            db.Att_adj.Remove(att_adj);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public void SendMail(string msg, string action, int elsid)
        {
            
            var empadj = db.Att_adj.ToList().Find(x => x.Id == elsid);
            var message = new MimeMessage();
            var emprellist = db.emprels.ToList();
            var emprel = emprellist.Find(x => x.Employee_id == empadj.emp_ID);
            var userlist = db.AspNetUsers.ToList();
            var usernamelist = db.usernames.ToList();
            var contractlist = db.contracts.OrderByDescending(x => x.date_changed).ToList();
            var desig = "";
            if (contractlist.Exists(x => x.employee_no == empadj.emp_ID))
            {
                var temp = contractlist.Find(x => x.employee_no == empadj.emp_ID);
                if (!temp.designation.IsNullOrWhiteSpace())
                {
                    desig = temp.designation;
                }
            }
            var emplusersname = usernamelist.Find(x => x.employee_no == emprel.Employee_id);
            message.From.Add(new MailboxAddress("Hrworks", "leave@citiscapegroup.com"));

            if (emprel == null)
            {
                var email = "hrteam@citiscapegroup.com";

                message.To.Add((new MailboxAddress("HR", email)));
                message.Subject = "time adjustment";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for time adjustment  by the employee  (" +
                           emplusersname.master_file.emiid + ") " +
                           emplusersname.master_file.employee_name + "-" + desig + " has been submitted but does not have a record in emp relations table" + "\n\n\n" +
                           "Thanks Best Regards, "
                };
            }

            if (action.Equals("submitted"))
            {
                var nextusersname = usernamelist.Find(x => x.employee_no == emprel.line_man);
                var nextuser = userlist.Find(x => x.Id == nextusersname.aspnet_uid);
                message.To.Add((new MailboxAddress(nextusersname.full_name, nextuser.Email)));
                message.Subject = "time adjustment";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that the request for time adjustment  by the employee   (" +
                           emplusersname.master_file.emiid + ") " +
                           emplusersname.full_name + "-" + desig + " has been submitted for your approval" + "\n\n\n" +
                           "http://ess.citiscapegroup.com" + "\n\n\n" +
                           "Thanks Best Regards, "
                };
            }
            

            if (action.Contains("approved"))
            {
                if (action == "approved by line manager")
                {
                    var previoususersname = usernamelist.Find(x => x.employee_no == emprel.line_man);
                    var nextusersname = usernamelist.Find(x => x.employee_no == emprel.HOD);
                    var nextuser = userlist.Find(x => x.Id == nextusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(nextusersname.full_name, nextuser.Email)));
                    message.Subject = "time adjustment";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for time adjustment  by the employee  (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been approved by line manager " +
                               previoususersname.master_file.employee_name + " and forwarded for your approval" +
                               "\n\n\n" +
                               "http://ess.citiscapegroup.com" + "\n\n\n" +
                               "Thanks Best Regards, "
                    };
                }
                else if (action == "approved")
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Cc.Add((new MailboxAddress("Yahya Rashid", "yrashid@citiscapegroup.com")));
                    message.Subject = "time adjustment";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for time adjustment  by the employee  (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been approved" + "\n\n\n" +
                               "http://ess.citiscapegroup.com" + "\n\n\n" +
                               "Thanks Best Regards, "
                    };
                }
            }

            if (action.Contains("rejected"))

            {
                if (action.Contains("rejected by line manager"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "time adjustment";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for time adjustment  by the employee (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been rejected by line manager for " +
                               msg + "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }

                if (action.Contains("rejected by HOD"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "time adjustment";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for the request for time adjustment  by the employee   (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been rejected by HOD for " + msg +
                               "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }
                
            }

            if (message.To.Count != 0)
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("outlook.office365.com", 587, false);
                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("leave@citiscapegroup.com", "Tak98020");
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
        end:;
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
