using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using System.Security.Cryptography;
using System.Text;

namespace HRworks.Controllers
{
    using System.Security.Cryptography;
    using System.Text;

    using Microsoft.AspNet.Identity;
    [Authorize(Roles = "payrole,super_admin")]
    public class payroleloginsController : Controller
    {
        private HREntities db = new HREntities();
        // GET: payrolelogins/Create
        public ActionResult Create()
        {
            var uchecklist = this.db.payrolelogins.ToList();
            if (!uchecklist.Exists(x => x.pro_username == User.Identity.GetUserName()))
            {
                ViewBag.newuser = "new";
            }
            else
            {
                ViewBag.newuser = "old";
            }
            ViewBag.error = "";
            return View();
        }

        // POST: payrolelogins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,pro_username,pro_password")] payrolelogin payrolelogin)
        {
            ViewBag.error = "";
            if (ModelState.IsValid && payrolelogin.pro_password != null)
            {
                if (db.payrolelogins.Count() > 0  )
                {
                    payrolelogin.pro_username = User.Identity.GetUserName();
                    var prlist = this.db.payrolelogins.ToList();
                    byte[] tmpSource;
                    byte[] tmpHash;
                    var sSourceData = payrolelogin.pro_password;
                    //Create a byte array from source data.
                    tmpSource = ASCIIEncoding.ASCII.GetBytes(sSourceData);
                    tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
                    payrolelogin.pro_password = ByteArrayToString(tmpHash);
                    if (prlist.Exists(x=>x.pro_username == payrolelogin.pro_username))
                    {
                        var prpass = (prlist.Find(x => x.pro_username == payrolelogin.pro_username));
                        if (prpass.pro_password == payrolelogin.pro_password)
                        {
                            return RedirectToAction("Index","payroles");
                        }
                        else
                        {
                            ViewBag.error = "incorrect";
                            return this.View(payrolelogin);
                        }
                    }
                    else
                    { 
                        payrolelogin.pro_username = User.Identity.GetUserName();
                        db.payrolelogins.Add(payrolelogin);
                        db.SaveChanges();
                        return RedirectToAction("Index", "payroles");
                    }
                }
                else
                {
                    byte[] tmpSource;
                    byte[] tmpHash;
                    var sSourceData = payrolelogin.pro_password;
                    //Create a byte array from source data.
                    tmpSource = ASCIIEncoding.ASCII.GetBytes(sSourceData);
                    tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
                    payrolelogin.pro_password = ByteArrayToString(tmpHash);
                    payrolelogin.pro_username = User.Identity.GetUserName();
                    db.payrolelogins.Add(payrolelogin);
                    db.SaveChanges();
                    return RedirectToAction("Index", "payroles");
                }
            }

            return View(payrolelogin);
        }

        static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }

            return sOutput.ToString();
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
