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
            if (!empatdatefrom.HasValue)
            {
                empatdatefrom = DateTime.Now;
            }
            empatdatefrom = new DateTime(empatdatefrom.Value.Year, empatdatefrom.Value.Month, 1);
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
                {/*
                    foreach (var hik in HOatt)
                    {
                        hik.ID = empuser.master_file.emiid;
                        hik.Person = empuser.master_file.employee_name;
                        if (!finallist.Exists(x=>x.date == hik.date))
                        {
                            var templist = HOatt.FindAll(x=>x.date == hik.date);
                            finallist.Add(templist.First());
                            if (templist.Count>1)
                            {
                                finallist.Add(templist.Last());
                            }

                        }
                    }*/
                    foreach (var hik in HOatt)
                    {
                        hik.ID = empuser.master_file.emiid;
                        hik.Person = empuser.master_file.employee_name;
                    }

                    var groups = HOatt.OrderBy(x=>x.datetime)
                        .GroupBy(x =>
                        {
                            if (x.date != null) return x.date.Value.Date;
                            return default;
                        });

                    foreach (var g in groups)
                    {
                        var ordered = g.OrderBy(x => x.date).ToList();
                        var first = ordered.First();
                        finallist.Add(first);

                        if (ordered.Count > 1)
                        {
                            finallist.Add(ordered.Last());
                        }
                    }
                    
                }

                if (projectatt.Count > 0)
                {
                    foreach (var tratt in projectatt)
                    {
                        var protoho = new hik();
                        protoho.ID = empuser.master_file.emiid;
                        protoho.datetime = tratt.punch_time;
                        protoho.date = tratt.punch_time.Date;
                        protoho.time = tratt.punch_time.TimeOfDay;
                        protoho.Person = empuser.master_file.employee_name;
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
        //
        // // GET: Att_adj/Create
        // public ActionResult Create()
        // {
        //     return View();
        // }

        public ActionResult Create(DateTime? atjdate,TimeSpan? atjtime , bool? inout)
        {
            if (atjdate.HasValue && atjtime.HasValue && inout.HasValue)
            {
                ViewBag.which_date = atjdate.Value.Date.ToString("d");
                if (inout.Value)
                {
                    ViewBag.late_in = atjtime;
                }
                else
                {
                    ViewBag.early_out = atjtime;
                }
            }

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
                var emprellist = db.emprels.ToList();
                var empuser = db.usernames
                    .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);
                att_adj.emp_ID = empuser.employee_no.Value;
                att_adj.master_file = empuser.master_file;
                att_adj.date_added = DateTime.Now;
                att_adj.date_modified = DateTime.Now;
                if (emprellist.Exists(x => x.Employee_id == empuser.employee_no))
                {
                    var emprel = emprellist.Find(x => x.Employee_id == empuser.employee_no);
                    if (!emprel.HOD.HasValue)
                    {
                        att_adj.status = "pending HODs approval";
                    }
                    else
                    {
                        att_adj.status = "pending Line managers approval";
                    }
                    db.Att_adj.Add(att_adj);
                    db.SaveChanges();
                    var sendmailtrid = db.Att_adj.ToList().Last();
                    SendMail("", "submitted", sendmailtrid.Id);
                }
                else
                {
                    ViewBag.error = "no employee relations record found, please contact HR before Submitting again";
                    SendMailerror(empuser.employee_no.Value);
                    return View(att_adj);
                }

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
        [Authorize(Roles = "super_admin")]
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
        [Authorize(Roles = "super_admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Att_adj att_adj = db.Att_adj.Find(id);
            db.Att_adj.Remove(att_adj);
            db.SaveChanges();
            return RedirectToAction("HRviewAttadj");
        }
        public ActionResult att_adj_app()
        {
            var emprellist = db.emprels.ToList();
            var empuser = db.usernames
                .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);
            var attadjlist = new List<Att_adj>();
            if (empuser == null)
            {
                return View(attadjlist);
            }
            var logedinsrels = emprellist.FindAll(x => x.line_man == empuser.master_file.employee_id || x.HOD == empuser.master_file.employee_id);
            foreach (var emprel in logedinsrels)
            {
                if (emprel.line_man == empuser.master_file.employee_id)
                {
                    attadjlist.AddRange(db.Att_adj.ToList()
                        .FindAll(x => x.emp_ID == emprel.Employee_id && x.status == "pending Line managers approval"));
                    if (!emprel.HOD.HasValue)
                    {
                        attadjlist.AddRange(db.Att_adj.ToList()
                            .FindAll(x => x.emp_ID == emprel.Employee_id && x.status == "pending HODs approval"));
                    }

                }
                if (emprel.HOD.HasValue && emprel.HOD == empuser.master_file.employee_id)
                {
                    attadjlist.AddRange(db.Att_adj.ToList()
                        .FindAll(x => x.emp_ID == emprel.Employee_id && x.status == "pending HODs approval"));
                }

            }

            return View(attadjlist);
        }
        public ActionResult approve(int id,string actionvalue)
        {
            var empuser = db.usernames
                .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);
            var attadj = db.Att_adj.ToList().Find(x=>x.Id == id);
            if (attadj.status == "pending Line managers approval" && actionvalue.IsNullOrWhiteSpace())
            {
                attadj.ap1 = empuser.AspNetUser.UserName;
                attadj.status = "pending HODs approval";
                SendMail("", "approved line managers", id);
            }
            else if (attadj.status == "pending HODs approval" && actionvalue.IsNullOrWhiteSpace())
            {
                if (attadj.ap1.IsNullOrWhiteSpace())
                {
                    attadj.ap1 = empuser.AspNetUser.UserName;
                    attadj.status = "pending HR Approval";
                }
                else
                {
                    attadj.ap2 = empuser.AspNetUser.UserName;
                    attadj.status = "pending HR Approval";
                }
                SendMail("", "approved by HOD", id);
            }
            if (attadj.status == "pending HR Approval" && !actionvalue.IsNullOrWhiteSpace())
            {
                attadj.HR_ap = User.Identity.Name;
                attadj.status = "approved";
                SendMail("", "Approved", id);
            }
                attadj.date_modified = DateTime.Now;
            db.Entry(attadj).State = EntityState.Modified;
            db.SaveChanges();
            if (actionvalue == "HRapp")
            {
                return RedirectToAction("HRviewAttadj");
            }
            return RedirectToAction("att_adj_app");
        }

        public ActionResult reject(int id,string message)
        {
            var empuser = db.usernames
                .FirstOrDefault(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name);
            var attadj = db.Att_adj.ToList().Find(x => x.Id == id);
            if (attadj.status == "pending Line managers approval")
            {
                attadj.ap1 = empuser.AspNetUser.UserName;
                attadj.status = "rejected by line manager for:"+message;
                SendMail(message, "rejected by line manager", id);
            }
            else if (attadj.status == "pending HODs approval")
            {
                if (attadj.ap1.IsNullOrWhiteSpace())
                {
                    attadj.ap1 = empuser.AspNetUser.UserName;
                    attadj.status = "rejected by HOD for:"+message;
                    SendMail(message, "rejected by HOD", id);
                }
                else
                {
                    attadj.ap2 = empuser.AspNetUser.UserName;
                    attadj.status = "rejected by HOD for:"+message;
                    SendMail(message, "rejected by HOD", id);
                }
            }
            else if (attadj.status == "pending HR Approval")
            {
                attadj.ap2 = empuser.AspNetUser.UserName;
                attadj.status = "rejected by HR for:" + message;
                SendMail(message, "rejected by HR", id);
            }
            attadj.date_modified = DateTime.Now;
            db.Entry(attadj).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("att_adj_app"); 
        }

        public ActionResult HRviewAttadj(string empid, DateTime? empatdatefrom, DateTime? empatdateto)
        {
            var att_adj = db.Att_adj.Include(a => a.master_file).ToList();
            if (!empid.IsNullOrWhiteSpace())
            {
                att_adj = att_adj.FindAll(x => x.master_file.emiid.ToUpper() == empid.ToUpper());
            }

            if (empatdatefrom.HasValue)
            {
                att_adj = att_adj.FindAll(x => x.which_date >= empatdatefrom);
            }
            if (empatdateto.HasValue)
            {
                att_adj = att_adj.FindAll(x => x.which_date <= empatdateto);
            }

            var attadjHRapp = att_adj.Where(x => x.status == "pending HR Approval").OrderByDescending(x=>x.date_modified).ToList();
            var attadjnonHRapp = att_adj.Where(x => x.status != "pending HR Approval").OrderByDescending(x => x.date_modified).ToList();
            var finalattadj = new List<Att_adj>();
            finalattadj.AddRange(attadjHRapp);
            finalattadj.AddRange(attadjnonHRapp);

            return View(finalattadj);
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
                message.Subject = "attendance adjustment approvals";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for attendance adjustment by the employee  (" +
                           emplusersname.master_file.emiid + ") " +
                           emplusersname.master_file.employee_name + "-" + desig + " has been submitted but does not have a record in employee relations table" + "\n\n\n" +
                           "Thanks Best Regards, "
                };
            }

            if (action.Equals("submitted"))
            {
                var nextusersname = usernamelist.Find(x => x.employee_no == emprel.line_man);
                var nextuser = userlist.Find(x => x.Id == nextusersname.aspnet_uid);
                message.To.Add((new MailboxAddress(nextusersname.full_name, nextuser.Email)));
                message.Subject = "attendance adjustment approvals";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that the request for attendance adjustment  by the employee   (" +
                           emplusersname.master_file.emiid + ") " +
                           emplusersname.full_name + "-" + desig + " has been submitted for your approval" + "\n\n\n" +
                           "http://csmain.ddns.net:6333/citiworks" + "\n\n\n" +
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
                    message.Subject = "attendance adjustment approvals";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for attendance adjustment by the employee  (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been approved by line manager " +
                               previoususersname.master_file.employee_name + " and forwarded for your approval" +
                               "\n\n\n" +
                               "http://csmain.ddns.net:6333/citiworks" + "\n\n\n" +
                               "Thanks Best Regards, "
                    };
                }
                else if (action == "approved")
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));/*
                    message.Cc.Add((new MailboxAddress("Yahya Rashid", "yrashid@citiscapegroup.com")));*/
                    message.Subject = "attendance adjustment approvals";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for attendance adjustment  by the employee  (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been approved" + "\n\n\n" +
                               "http://csmain.ddns.net:6333/citiworks" + "\n\n\n" +
                               "Thanks Best Regards, "
                    };
                }
                else if (action == "approved by HOD")
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress("Yahya Rashid", "yrashid@citiscapegroup.com")));
                    message.Subject = "attendance adjustment approvals";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for attendance adjustment  by the employee  (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been approved BY HOD and forwarded for your approval" + "\n\n\n" +
                               "http://csmain.ddns.net:6333/citiworks" + "\n\n\n" +
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
                    message.Subject = "attendance adjustment approvals";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for attendance adjustment  by the employee (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been rejected by line manager for " +
                               msg + "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }

                if (action.Contains("rejected by HOD"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "attendance adjustment approvals";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for the request for attendance adjustment by the employee   (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been rejected by HOD for " + msg +
                               "\n\n\n" + "\n\n\n" + "Thanks Best Regards, "
                    };
                }
                if (action.Contains("rejected by HR"))
                {
                    var nextuser = userlist.Find(x => x.Id == emplusersname.aspnet_uid);
                    message.To.Add((new MailboxAddress(emplusersname.full_name, nextuser.Email)));
                    message.Subject = "attendance adjustment approvals";
                    message.Body = new TextPart("plain")
                    {
                        Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that leave application for the request for attendance adjustment by the employee   (" +
                               emplusersname.master_file.emiid + ") " +
                               emplusersname.full_name + "-" + desig + " has been rejected by HR for " + msg +
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

        public void SendMailerror(int elsid)
        {
            var message = new MimeMessage();
            var empadj = db.master_file.ToList().Find(x => x.employee_id == elsid);
            var emprellist = db.emprels.ToList();
            var usernamelist = db.usernames.ToList();
            var emprel = emprellist.Find(x => x.Employee_id == empadj.employee_id);
            var emplusersname = usernamelist.Find(x => x.employee_no == empadj.employee_id);
            var contractlist = db.contracts.OrderByDescending(x => x.date_changed).ToList();
            var desig = "";
            if (contractlist.Exists(x => x.employee_no == empadj.employee_id))
            {
                var temp = contractlist.Find(x => x.employee_no == empadj.employee_id);
                if (!temp.designation.IsNullOrWhiteSpace())
                {
                    desig = temp.designation;
                }
            }

            if (emprel == null)
            {
                var email = "hrteam@citiscapegroup.com";

                message.To.Add((new MailboxAddress("HR", email)));
                message.Subject = "attendance adjustment approvals";
                message.Body = new TextPart("plain")
                {
                    Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for attendance adjustment by the employee  (" +
                           emplusersname.master_file.emiid + ") " +
                           emplusersname.master_file.employee_name + "-" + desig + " can not be submitted as the employee does not have a record in employee relations table" + "\n\n\n" +
                           "Thanks Best Regards, "
                };
            }
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
