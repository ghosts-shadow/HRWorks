using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using HRworks.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.IO;
using System.Data;
using MailKit.Net.Smtp;
using Microsoft.Office.Interop.Word;
using Microsoft.Ajax.Utilities;
using MimeKit;

namespace HRworks.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly HREntities db = new HREntities();
        private  RoleManager<IdentityRole> _roleManager => HttpContext.GetOwinContext().Get<RoleManager<IdentityRole>>();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [Authorize(Roles = "super_admin,registration,registration_HR")]
        public ActionResult Register()
        {
            return View();
        }
        

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,registration,registration_HR")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                HREntities db = new HREntities();
                var mancon = new master_fileController();
                var masteremp = mancon.emplist();
                var emp = new master_file();
                if (model.UserName.ToUpper().Contains("G-"))
                {
                    emp = masteremp.Find(x => x.emiid == model.UserName);
                }
                else
                {
                    int.TryParse(model.UserName, out var empno);
                    emp = masteremp.Find(x => x.employee_no == empno);
                }
                if (!masteremp.Exists(x=>x.employee_no == emp.employee_no))
                {
                    ModelState.AddModelError("EMPNO","invalid emp no");
                    goto fail;
                }
                HREntities df=new HREntities();
                username un=new username();
                string[] userrole = model.UserRole.Split(',');
                if (!userrole.Contains("employee"))
                {
                    var username = "";
                    var temp = model.Email.Split('@');
                    username = temp[0];
                    model.UserName = username;
                }
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                var i = 1;
                while(!result.Succeeded)
                {
                    model.UserName = model.UserName.Insert(model.UserName.Length, i.ToString());
                    user.UserName = model.UserName;
                    result = await UserManager.CreateAsync(user, model.Password);
                    i++;
                } 
                if (result.Succeeded)
                {
                    un.full_name = emp.employee_name;
                    un.aspnet_uid = user.Id; 
                    un.employee_no = emp.employee_id;
                    model.EMPNO = emp.employee_no;
                    model.full_name = emp.employee_name;
                    await this.UserManager.AddToRolesAsync(user.Id, userrole);
                    df.usernames.Add(un);
                    df.SaveChanges();
                    SendMail(model);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            fail: ;
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public async Task<string> RegisterUserAsync(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return "Invalid model state";

            var error = "";
            using (var db = new HREntities())
            {
                var masteremp = new master_fileController().emplist();
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var un = new username
                    {
                        full_name = model.full_name,
                        aspnet_uid = user.Id
                    };

                    var emp = masteremp.Find(x => x.employee_no == model.EMPNO);
                    if (emp != null)
                        un.employee_no = emp.employee_id;

                    var userRoles = model.UserRole.Split(',');
                    await UserManager.AddToRolesAsync(user.Id, userRoles);

                    db.usernames.Add(un);
                    db.SaveChanges();

                    SendMail(model);
                }
                else
                {
                    error = result.Errors.FirstOrDefault();
                }
            }

            return error;
        }
        public void SendMail(RegisterViewModel empreg)
        {
            if (string.IsNullOrWhiteSpace(empreg.Email) || !IsValidEmail(empreg.Email))
                return;

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("HR Leave System", "leave@citiscapegroup.com"));
            message.To.Add(new MailboxAddress(empreg.full_name, empreg.Email));
            message.Subject = "Leave system";

            message.Body = new TextPart("plain")
            {
                Text = $@"Dear {empreg.full_name},

Please find below credentials to access the HR system as per the request:

Username: {empreg.UserName}
Password: Qazwsx1!

HR Leave system Link: http://csmain.ddns.net:6333/citiworks

Note: Password can be changed through the portal. Please save it for reference.

Thank You And Best Regards"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("outlook.office365.com", 587, false);
                client.Authenticate("leave@citiscapegroup.com", "Tak98020");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }



        public ActionResult Importlistregister()
        {
            return View();
        }

        [ActionName("Importlistregister")]
        [HttpPost]
        public async Task<ActionResult> ImportlistregisterPost()
        {
            var uploadedFile = Request.Files["FileUpload1"];
            if (uploadedFile == null || uploadedFile.ContentLength <= 0)
            {
                ViewBag.Error = "Please upload a CSV file.";
                return View();
            }

            var extension = Path.GetExtension(uploadedFile.FileName).ToLower();
            var validFileTypes = new[] { ".csv" };

            if (!validFileTypes.Contains(extension))
            {
                ViewBag.Error = "Only CSV files are supported.";
                return View();
            }

            var uploadPath = Server.MapPath("~/Content/Uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fullPath = Path.Combine(uploadPath, Path.GetFileName(uploadedFile.FileName));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            uploadedFile.SaveAs(fullPath);

            var dt = Utility.ConvertCSVtoDataTable(fullPath);
            var errorList = new List<string>();
            var employeeList = new master_fileController().emplist();

            foreach (DataRow row in dt.Rows)
            {
                var model = new RegisterViewModel();
                bool skipRow = false;

                if (row["Emp No"] == null || string.IsNullOrWhiteSpace(row["Emp No"].ToString()))
                {
                    errorList.Add("Missing Employee Number");
                    continue;
                }

                var empNoValue = row["Emp No"].ToString();
                master_file emp = null;

                if (empNoValue.ToUpper().Contains("G-"))
                    emp = employeeList.Find(x => x.emiid == empNoValue);
                else if (int.TryParse(empNoValue, out var empNoParsed))
                    emp = employeeList.Find(x => x.employee_no == empNoParsed);

                if (emp == null)
                {
                    errorList.Add($"Employee not found for: {empNoValue}");
                    continue;
                }

                model.EMPNO = emp.employee_no;
                model.UserName = emp.emiid;
                model.full_name = emp.employee_name;

                if (row.Table.Columns.Contains("Assigned Email"))
                {
                    model.Email = row["Assigned Email"].ToString();
                    model.Password = "Qazwsx1!";
                    model.ConfirmPassword = "Qazwsx1!";
                    model.UserRole = "employee";
                }
                else
                {
                    errorList.Add($"Missing Assigned Email for: {emp.employee_name}");
                    continue;
                }

                var registrationError = await RegisterUserAsync(model);
                if (!string.IsNullOrWhiteSpace(registrationError))
                    errorList.Add($"{model.UserName}: {registrationError}");
            }

            ViewBag.Data = dt;
            ViewBag.Errorlist = errorList;

            return View("Importlistregister");
        }

        public async Task<ActionResult> RoleEdit(string id)
        {
            var user = UserManager.Users.ToList().Find(x=>x.Id == id);
            if (user == null)
            {
                goto fail;
            }
            HREntities df = new HREntities();
            var usernamelist = df.usernames.ToList();
            var roleedittemp = new RoleEdit();
            roleedittemp.UserId = user.Id;
            roleedittemp.Username = user.UserName;
            var empname = usernamelist.Find(x => x.aspnet_uid == user.Id).master_file.employee_name;
            roleedittemp.Name = empname;
            if (user.Roles.Count == 0)
            {
                goto fail;
            }
            var userrolelist = await UserManager.GetRolesAsync(id);
            foreach (var userroles in userrolelist)
            {
                if (roleedittemp.userroles == null)
                {
                    roleedittemp.userroles = new List<userroles>();
                    var userroletemp = new userroles();
                    userroletemp.rolename = userroles;
                    roleedittemp.userroles.Add(userroletemp);
                }
                else
                {
                    var userroletemp = new userroles();
                    userroletemp.rolename = userroles;
                    roleedittemp.userroles.Add(userroletemp);
                }
            }

            return View(roleedittemp);
            fail: ;
            return RedirectToAction("Index", "usernames");
        }
        [Authorize(Roles = "super_admin")]
        public async Task<ActionResult> ManageRoles(string userId)
        {
            var user = UserManager.Users.ToList().Find(x => x.Id == userId);
            var allRoles = db.AspNetRoles.Where(x=>!x.Name.Contains("admin")).ToList();
            var userRoles = await UserManager.GetRolesAsync(user.Id);

            var model = new ManageRolesViewModel
            {
                UserId = userId,
                Email = user.UserName,
                Roles = allRoles.Select(r => new RoleSelection
                {
                    RoleName = r.Name,
                    IsSelected = userRoles.Contains(r.Name)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "super_admin")]
        public async Task<ActionResult> ManageRoles(ManageRolesViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.UserId);
            var userRoles = await UserManager.GetRolesAsync(user.Id);

            foreach (var role in model.Roles)
            {
                if (role.IsSelected && !userRoles.Contains(role.RoleName))
                    await UserManager.AddToRoleAsync(user.Id, role.RoleName);

                if (!role.IsSelected && userRoles.Contains(role.RoleName))
                    await UserManager.RemoveFromRoleAsync(user.Id, role.RoleName);
            }

            return RedirectToAction("Index", "usernames"); 
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null /*|| !(await UserManager.IsEmailConfirmedAsync(user.Id))*/)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }
        
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}