using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HRworks.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace HRworks.Controllers
{
    public class rolesettingController : Controller
    {
        public ActionResult settingrole(string username,string rolename)
        {
            //Roles.AddUserToRole();
            var context = new ApplicationDbContext();
            var allUsers = context.Users.ToList();
            var allRoles = context.Roles.ToList();
            ViewBag.roles = new SelectList(allRoles,"Name","Name");
            ViewBag.users = new SelectList(allUsers,"id","UserName");
            if (!username.IsNullOrWhiteSpace() && !rolename.IsNullOrWhiteSpace())
            {
                var userman = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
                userman.AddToRole(username, rolename);
            }

            return View();
        }
    }
}