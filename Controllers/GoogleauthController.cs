using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace HRworks.Controllers
{
    public class GoogleauthController : Controller
    {
        // GET: Googleauth
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TwoFactorAuthenticate()
        {

            ViewBag.Status = true;
            string googleAuthKey = WebConfigurationManager.AppSettings["GoogleAuthKey"];
            string UserUniqueKey = (User.Identity.Name + googleAuthKey);
            Session["UserUniqueKey"] = UserUniqueKey;
            var token = Request["CodeDigit"];
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            var setupInfo = TwoFacAuth.GenerateSetupCode("http://hrworks.ddns.net:6333/citiworks", User.Identity.Name, ConvertSecretToBytes(UserUniqueKey, false), 300);
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            ViewBag.SetupCode = setupInfo.ManualEntryKey;
            bool isValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, token, false);
            if (isValid)
            {
                HttpCookie TwoFCookie = new HttpCookie("TwoFCookie");
                string UserCode = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(UserUniqueKey)));

                Session["IsValidTwoFactorAuthentication"] = true;
                return RedirectToAction("Index", "contracts");
            }

            var Message = "Google Two Factor PIN is expired or wrong";
            ViewBag.Message = Message;
           // return RedirectToAction("Create", "contractlogins",new {gmes= Message });
           return View();
        }
        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
           secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);

    }
}