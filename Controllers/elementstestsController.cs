using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Office.Interop.Word;
using MimeKit;

namespace HRworks.Controllers
{
    public class elementstestsController : Controller
    {
        // GET: elementstests
        public ActionResult emailtest()
        {

            var email = "hrteam@citiscapegroup.com";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hrworks", "leave@citiscapegroup.com"));
            message.To.Add((new MailboxAddress("HR", email)));
            message.Subject = "attendance adjustment approvals";
            message.Body = new TextPart("plain")
            {
                Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that  the request for attendance adjustment by the employee  () has been submitted but does not have a record in employee relations table" + "\n\n\n" +
                       "Thanks Best Regards, "
            };
            return View();
        }
    }
}