namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Web.Routing;
    using HRworks.Models;

    // public static class HtmlHelperExtensions
    // {
    //     public static MvcHtmlString decry(this HtmlHelper helper, byte[] text)
    //     {
    //         var test1 = new encriptiontest();
    //         Aes entri = new AesCryptoServiceProvider();
    //         var result = encriptiontestsController.DecryptStringFromBytes_Aes(text, entri.Key, entri.IV);
    //         return new MvcHtmlString(result);
    //     }
    // }
    [NoDirectAccess]
    public class encriptiontestsController : Controller
    {
        private readonly HREntities db = new HREntities();

        // GET: encriptiontests/Create
        public ActionResult Create()
        {
            return this.View();
        }

        

        // POST: encriptiontests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Id,name_,age_,phone_no")]
            decrymodel encriptiontest)
        {
            if (this.ModelState.IsValid)
            {
                var test1 = new encriptiontest(); 
                Aes entri = new AesCryptoServiceProvider();
                test1.Id = encriptiontest.Id;
                test1.name_ = Protect(encriptiontest.name_);
                test1.age_ = Protect(encriptiontest.age_);
                test1.phone_no = Protect(encriptiontest.phone_no);
                this.db.encriptiontests.Add(test1);
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            return this.View(encriptiontest);
        }

        // GET: encriptiontests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var encriptiontest = this.db.encriptiontests.Find(id);
            if (encriptiontest == null) return this.HttpNotFound();
            return this.View(encriptiontest);
        }

        private const string Purpose = "equalizer";
        public static string Protect(string unprotectedText)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedText);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }

        public static string Unprotect(string protectedText)
        {
            var protectedBytes = Convert.FromBase64String(protectedText);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
            var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
            return unprotectedText;
        }
        // POST: encriptiontests/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var encriptiontest = this.db.encriptiontests.Find(id);
            this.db.encriptiontests.Remove(encriptiontest);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }
        // GET: encriptiontests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var encriptiontest = this.db.encriptiontests.Find(id);
            if (encriptiontest == null) return this.HttpNotFound();
            return this.View(encriptiontest);
        }

        // GET: encriptiontests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var encriptiontest = this.db.encriptiontests.Find(id);
            if (encriptiontest == null) return this.HttpNotFound();
            return this.View(encriptiontest);
        }

        // POST: encriptiontests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "Id,name_,age_,phone_no")]
            encriptiontest encriptiontest)
        {
            if (this.ModelState.IsValid)
            {
                var test1 = new encriptiontest();
                Aes entri = new AesCryptoServiceProvider();
                test1.Id = encriptiontest.Id;
                test1.name_ = Protect(encriptiontest.name_);
                test1.age_ = Protect(encriptiontest.age_);
                test1.phone_no = Protect(encriptiontest.phone_no);
                this.db.encriptiontests.Add(test1);
                this.db.Entry(test1).State = EntityState.Modified;
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            return this.View(encriptiontest);
        }

        // GET: encriptiontests
        public ActionResult Index()
        {
            var decrylist = new List<decrymodel>();
            var encry = this.db.encriptiontests.ToList();
            Aes entri = new AesCryptoServiceProvider();
            foreach (var enc in encry)
            {
                var decry = new decrymodel();
                decry.Id = enc.Id;
                decry.age_ = Unprotect(enc.age_.ToString());
                decry.name_ = Unprotect(enc.name_.ToString());
                decry.phone_no = Unprotect(enc.phone_no.ToString());
                decrylist.Add(decry);
            } 

            return this.View(decrylist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null || filterContext.HttpContext.Request.Url.Host
                != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Home", action = "Logout", area = "Main" }));
            }
        }
    }
}