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
    using System.Drawing;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.Ajax.Utilities;
    using Microsoft.AspNet.Identity;
    using QRCoder;

    [NoDirectAccess]
    [Authorize(Roles = "super_admin,employee_con,payrole")]
    public class contractloginsController : Controller
    {
        private HREntities db = new HREntities();
        [Authorize(Roles = "super_admin")]
        public ActionResult conacces() 
        {
            //var conalist = db.usernames.Where(x=>x.AspNetUser.AspNetRoles.Where(y=>y.Name == "employee_con" || y.Name == "payrole").Count()>0).ToList();
            var conalist = db.AspNetUsers.Where(u => u.AspNetRoles.Where(y => y.Name == "employee_con" || y.Name == "payrole").Count() > 0).ToList();
            var conalelist = db.contractlogins.ToList();
            var templist = new List<AspNetUser>();
            foreach (var item in conalist)
            {
                if (!conalelist.Exists(x=>x.pro_username == item.UserName))
                {
                    templist.Add(item);                    
                }
            }
            ViewBag.pro_username = new SelectList(templist, "UserName", "UserName");
            var con = Request["pro_username"];
            if (con != null)
            {
                var convar = new contractlogin();
                convar.pro_username = con;
                db.contractlogins.Add(convar);
                db.SaveChanges();
                return RedirectToAction("Index", "contracts");
            }
            return View();
        }

        // GET: contractlogins/Create
        public ActionResult Create()
        {
            var uchecklist = this.db.contractlogins.ToList();
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
        // POST: contractlogins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,pro_username,pro_password")] contractlogin contractlogin)
        {
            ViewBag.error = "";
            if (ModelState.IsValid && contractlogin.pro_password != null)
            {
                if (db.contractlogins.Count() > 0)
                {
                    contractlogin.pro_username = User.Identity.GetUserName();
                    var prlist = this.db.contractlogins.ToList();
                    byte[] tmpSource;
                    byte[] tmpHash;
                    var sSourceData = contractlogin.pro_password;
                    //Create a byte array from source data.
                    tmpSource = ASCIIEncoding.ASCII.GetBytes(sSourceData);
                    tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
                    contractlogin.pro_password = ByteArrayToString(tmpHash);
                    if (prlist.Exists(x => x.pro_username == contractlogin.pro_username))
                    {
                        var prpass = (prlist.Find(x => x.pro_username == contractlogin.pro_username));
                        if (prpass.pro_password != null)
                        {
                            if (prpass.pro_password == contractlogin.pro_password)
                            {
                                return RedirectToAction("TwoFactorAuthenticate", "Googleauth", new { id = prpass.Id });
                            }
                            else
                            {
                                ViewBag.error = "incorrect";
                                return this.View(contractlogin);
                            }
                        }
                        else 
                        {
                            prpass.pro_password = contractlogin.pro_password;
                            db.Entry(prpass).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("TwoFactorAuthenticate", "Googleauth", new { id = prpass.Id });
                        }
                    }
                    /*else
                    {
                        contractlogin.pro_username = User.Identity.GetUserName();
                        db.contractlogins.Add(contractlogin);
                        db.SaveChanges();
                        return RedirectToAction("Index", "contracts");
                    }*/
                }/*
                else
                {
                    byte[] tmpSource;
                    byte[] tmpHash;
                    var sSourceData = contractlogin.pro_password;
                    //Create a byte array from source data.
                    tmpSource = ASCIIEncoding.ASCII.GetBytes(sSourceData);
                    tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
                    contractlogin.pro_password = ByteArrayToString(tmpHash);
                    contractlogin.pro_username = User.Identity.GetUserName();
                    db.contractlogins.Add(contractlogin);
                    db.SaveChanges();
                    return RedirectToAction("Index", "contracts");
                }*/
            }
            return View(contractlogin);
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
