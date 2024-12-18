﻿using HRworks;

using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace HRworks
{
    using HRworks.Models;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
            this.createRolesandUsers();
        }

        private void createRolesandUsers()
        {
            var context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            // In Startup iam creating first Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists("admin"))
            {
                // first we create Admin role    
                var role = new IdentityRole();
                role.Name = "admin";
                roleManager.Create(role);

                // Here we create a Admin super user who will maintain the website                   
                var user = new ApplicationUser();
                user.UserName = "sdiniz";
                user.Email = "sdiniz";

                var userPWD = "Qazwsx1!";

                var chkUser = UserManager.Create(user, userPWD);

                // Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "admin");
                }
            }

            // creating Creating Manager role     
            if (!roleManager.RoleExists("manager"))
            {
                var role = new IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);
            }      
            if (!roleManager.RoleExists("super_admin"))
            {
                var role = new IdentityRole();
                role.Name = "super_admin";
                roleManager.Create(role);
            }     
            if (!roleManager.RoleExists("payrole"))
            {
                var role = new IdentityRole();
                role.Name = "payrole";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("liquidation"))
            {
                var role = new IdentityRole();
                role.Name = "liquidation";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("employee_EID"))
            {
                var role = new IdentityRole();
                role.Name = "employee_EID";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("employee_INC"))
            {
                var role = new IdentityRole();
                role.Name = "employee_INC";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("employee_PASS"))
            {
                var role = new IdentityRole();
                role.Name = "employee_PASS";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("employee_VLC"))
            {
                var role = new IdentityRole();
                role.Name = "employee_VLC";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("employee"))
            {
                var role = new IdentityRole();
                role.Name = "employee";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("HOD"))
            {
                var role = new IdentityRole();
                role.Name = "HOD";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("EXTHOD"))
            {
                var role = new IdentityRole();
                role.Name = "EXTHOD";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("registration"))
            {
                var role = new IdentityRole();
                role.Name = "registration";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("registration_HR"))
            {
                var role = new IdentityRole();
                role.Name = "registration_HR";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("slapproval"))
            {
                var role = new IdentityRole();
                role.Name = "slapproval";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("employee_rel"))
            {
                var role = new IdentityRole();
                role.Name = "employee_rel";
                roleManager.Create(role);
            }
        }


    }
}