using Google.Authenticator;
using HRworks.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace HRworks.Controllers
{
    [NoDirectAccess]
    [Authorize(Roles = "super_admin,payrole,employee_con")]
    public class GoogleauthController : Controller
    {
        private HREntities db = new HREntities();
        // GET: Googleauth
        public ActionResult Index()
        {
            return View();
        }


        private string GenerateQRCode(string link)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(10);

            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] qrCodeBytes = ms.ToArray();
            string qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);

            var qrcodeimg = "data:image/png;base64," + qrCodeBase64;

            return qrcodeimg;
        }
        public string PlayStoreQRCode()
        {
            string playStoreLink = "https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2";
            return GenerateQRCode(playStoreLink);
        }

        public string AppStoreQRCode()
        {
            string appStoreLink = "https://apps.apple.com/us/app/google-authenticator/id388497605";
            return GenerateQRCode(appStoreLink);
        }

        public ActionResult TwoFactorAuthenticate(int id)
        {
            string googleAuthKey = WebConfigurationManager.AppSettings["GoogleAuthKey"];
            string UserUniqueKey = (User.Identity.Name + googleAuthKey);
            Session["UserUniqueKey"] = UserUniqueKey;
            var token = Request["CodeDigit"];
            var conlogid = this.db.contractlogins.ToList().Find(x=>x.Id == id);
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();

            if (conlogid != null) 
            {
                if (conlogid.Gauth || token != null)
                {
                    int timeWindowInSeconds = 300; // 5 minutes window
                    bool isValid = false; 
                    for (int i = -1; i <= 1; i++)
                    {
                        TimeSpan currentTime = DateTime.Now.AddSeconds(i * timeWindowInSeconds) - DateTime.Now;
                        bool codeIsValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, token, currentTime, false); 
                        if (codeIsValid)
                        {
                            isValid = true;
                            break;
                        }
                    }
                    if (isValid)
                    {
                        //HttpCookie TwoFCookie = new HttpCookie("TwoFCookie");
                        string UserCode = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(UserUniqueKey)));

                        Session["IsValidTwoFactorAuthentication"] = true;
                        Session["isactive"] = true;
                        conlogid.Gauth = true;
                        db.Entry(conlogid).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index", "contracts");
                    }
                    if (conlogid.Gauth)
                    {
                        if (token != null) { 
                        var Message = "Google Two Factor PIN is expired or wrong";
                        ViewBag.Message = Message;}
                        Session["isactive"] = true;
                        Session["IsValidTwoFactorAuthentication"] = false;
                    }
                    else
                    {
                        var Message = "Google Two Factor PIN is not activated";
                        ViewBag.Message = Message;
                        Session["isactive"] = false;
                        Session["IsValidTwoFactorAuthentication"] = false;
                        var setupInfo = TwoFacAuth.GenerateSetupCode("Citiscape HRM", User.Identity.Name, ConvertSecretToBytes(UserUniqueKey, false), 300);
                        ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                        ViewBag.QRCodeImagean = PlayStoreQRCode();
                        ViewBag.QRCodeImageio = AppStoreQRCode();
                        ViewBag.SetupCode = setupInfo.ManualEntryKey;
                    }

                }
                else
                {
                    Session["isactive"] = false;
                    Session["IsValidTwoFactorAuthentication"] = false;
                    var setupInfo = TwoFacAuth.GenerateSetupCode("Citiscape HRM", User.Identity.Name, ConvertSecretToBytes(UserUniqueKey, false), 300);
                    ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                    ViewBag.QRCodeImagean = PlayStoreQRCode();
                    ViewBag.QRCodeImageio = AppStoreQRCode();
                    ViewBag.SetupCode = setupInfo.ManualEntryKey;
                }
            }

           // return RedirectToAction("Create", "contractlogins",new {gmes= Message });
           return View();
        }
        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
           secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);

    }
}