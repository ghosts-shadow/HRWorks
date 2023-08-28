using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using Microsoft.Ajax.Utilities;
using MimeKit;
using MailKit.Net.Smtp;
using OfficeOpenXml;

namespace HRworks.Controllers
{
    [Authorize]
    public class detailsinarabicsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: detailsinarabics
        public ActionResult Index()
        {
            var detailsinarabics = db.detailsinarabics.Include(d => d.master_file);
            return View(detailsinarabics.ToList());
        }

        // GET: detailsinarabics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detailsinarabic detailsinarabic = db.detailsinarabics.Find(id);
            if (detailsinarabic == null)
            {
                return HttpNotFound();
            }
            return View(detailsinarabic);
        }

        // GET: detailsinarabics/Create
        public ActionResult Create()
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_name");
            return View();
        }

        // POST: detailsinarabics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,employee_id,ARname,ARposition,ARnationality")] detailsinarabic detailsinarabic)
        {
            if (ModelState.IsValid)
            {
                db.detailsinarabics.Add(detailsinarabic);
                db.SaveChanges();
                var lastcerid = db.certificatesavingtest_.ToList().Last();
                //SendMail(lastcerid.Id);
                return RedirectToAction("Index");
            }

            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_name", detailsinarabic.employee_id);
            return View(detailsinarabic);
        }

        // GET: detailsinarabics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detailsinarabic detailsinarabic = db.detailsinarabics.Find(id);
            if (detailsinarabic == null)
            {
                return HttpNotFound();
            }
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_name", detailsinarabic.employee_id);
            return View(detailsinarabic);
        }

        // POST: detailsinarabics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,employee_id,ARname,ARposition,ARnationality")] detailsinarabic detailsinarabic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detailsinarabic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }
            ViewBag.employee_id = new SelectList(afinallist, "employee_id", "employee_name", detailsinarabic.employee_id);
            return View(detailsinarabic);
        }

        // GET: detailsinarabics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detailsinarabic detailsinarabic = db.detailsinarabics.Find(id);
            if (detailsinarabic == null)
            {
                return HttpNotFound();
            }
            return View(detailsinarabic);
        }

        // POST: detailsinarabics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            detailsinarabic detailsinarabic = db.detailsinarabics.Find(id);
            db.detailsinarabics.Remove(detailsinarabic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult subcertificatereq(int? certificatetype, string destination, DateTime? from, DateTime? to,string embassy,string certificate_of)
        {
            
            ViewBag.submmites = "";
            var testdlink = new Dictionary<string, string>();
            {
                testdlink.Add("Consulate of The Islamic Republic Of Afghanistan", "Afghanistan");
                testdlink.Add("Embassy of The Islamic Republic of Afghanistan", "Afghanistan");
                testdlink.Add("Embassy Of The Republic Of Albania", "Albania");
                testdlink.Add("Consulate General Of The People's Democratic Republic of Algeria", "Algeria");
                testdlink.Add("Embassy Of The Democratic People's Republic Of Algeria", "Algeria");
                testdlink.Add("Consulate General Of The Republic Of Angola", "Angola");
                testdlink.Add("Embassy of the Republic of Angola", "Angola");
                testdlink.Add("Embassy Of Antigua and Barbuda", "Antigua and Barbuda");
                testdlink.Add("Embassy of The Republic of Argentina", "Argentina");
                testdlink.Add("Embassy of The Republic of Armenia", "Armenia");
                testdlink.Add("Consulate General of Australia", "Australia");
                testdlink.Add("Embassy of Australia", "Australia");
                testdlink.Add("Embassy of the Republic of Austria", "Austria");
                testdlink.Add("Consulate General of the Republic of Azerbaijan", "Azerbaijan");
                testdlink.Add("Embassy of The Republic of Azerbaijan", "Azerbaijan");
                testdlink.Add("Consulate General of The Kingdom of Bahrain", "Bahrain");
                testdlink.Add("Embassy of The Kingdom of Bahrain", "Bahrain");
                testdlink.Add("Consulate General of The People's Republic of Bangladesh", "Bangladesh");
                testdlink.Add("Embassy of People's Republic of Bangladesh", "Bangladesh");
                testdlink.Add("Consulate General of the Republic of Belarus", "Belarus");
                testdlink.Add("Embassy of The Republic of Belarus", "Belarus");
                testdlink.Add("Embassy of the Kingdom of Belgium", "Belgium");
                testdlink.Add("Consulate General of the Republic of Benin", "Benin");
                testdlink.Add("Embassy of The Republic of Benin", "Benin");
                testdlink.Add("Royal Embassy of Bhutan", "Bhutan");
                testdlink.Add("Embassy Of Bosnia and Herzegovina", "Bosnia and Herzegovina");
                testdlink.Add("Embassy of The Federal Republic of Brazil", "Brazil");
                testdlink.Add("PDD Test", "Brazil");
                testdlink.Add("Embassy of Brunei Darussalam", "Brunei");
                testdlink.Add("Consulate General of the Republic of Bulgaria", "Bulgaria");
                testdlink.Add("Embassy of BurkinaFaso", "BurkinaFaso");
                testdlink.Add("Consulate General of the Republic of Burundi", "Burundi");
                testdlink.Add("Embassy Of The Republic Of Burundi", "Burundi");
                testdlink.Add("Royal Embassy of Cambodia", "Cambodia");
                testdlink.Add("Embassy of the Republic of Cameroon", "Cameroon");
                testdlink.Add("Consulate General of Canada", "Canada");
                testdlink.Add("Embassy of Canada", "Canada");
                testdlink.Add("Consulate General of the Republic of Chad", "Chad");
                testdlink.Add("Embassy of The Republic of Chile", "Chile");
                testdlink.Add("Consulate General of The People's Republic of China", "China");
                testdlink.Add("Embassy Of People's Republic Of China", "China");
                testdlink.Add("Embassy of the Republic of Colombia", "Colombia");
                testdlink.Add("Consulate General of The Republic of The Comoros", "Comoros");
                testdlink.Add("Embassy of the Republic of Union of Comoros", "Comoros");
                testdlink.Add("Embassy of the Democratic Republic of the Congo", "Congo");
                testdlink.Add("Embassy of the Republic of Costa Rica", "Costa Rica");
                testdlink.Add("Embassy of The Republic of Croatia", "Croatia");
                testdlink.Add("Embassy of The Republic of Cuba", "cuba");
                testdlink.Add("Embassy of the Republic of Cyprus", "Cyprus");
                testdlink.Add("Embassy of The Czech Republic", "Czech");
                testdlink.Add("Royal Danish Consulate General", "Denmark");
                testdlink.Add("Royal Danish Embassy", "Denmark");
                testdlink.Add("Embassy Of The Republic Of Djibouti", "Djibouti");
                testdlink.Add("Embassy of the Dominican Republic", "Dominican");
                testdlink.Add("Embassy of Republic of Ecuador", "Ecuador");
                testdlink.Add("Counslate General of the Arab Republic of Egypt", "Egypt");
                testdlink.Add("Embassy Of The Arab Republic Of Egypt", "Egypt");
                testdlink.Add("Consulate of Republic of Equatorial Guinea", "Equatorial Guinea");
                testdlink.Add("Consulate General of the State of Eritrea", "Eritrea");
                testdlink.Add("Embassy of The State of Eritrea", "Eritrea");
                testdlink.Add("Embassy of Estonia", "Estonia");
                testdlink.Add("Embassy of the kingdom of Eswatini", "Eswatini");
                testdlink.Add("Embassy of the Federal Democratic Republic of Ethiopia", "Ethiopia");
                testdlink.Add("The Consulate General of The Federal Democratic Republic of Ethiopia", "Ethiopia");
                testdlink.Add("Embassy of the Republic of Fiji", "Fiji");
                testdlink.Add("Embassy of the Republic of Finland", "Finland");
                testdlink.Add("Consulate General of The Republic of France", "France");
                testdlink.Add("Embassy of The Republic of France", "France");
                testdlink.Add("Embassy of The Republic of Gabon", "Gabon");
                testdlink.Add("Embassy of The Republic Of Gambia", "Gambia");
                testdlink.Add("Embassy of The Federal Republic of Germany", "Germany");
                testdlink.Add("The Consulate General of The Federal Republic of Germany", "Germany");
                testdlink.Add("Consulate General of the Republic of Ghana", "Ghana");
                testdlink.Add("Embassy Of The Republic Of Ghana", "Ghana");
                testdlink.Add("Consulate General of Grenada", "Grenada");
                testdlink.Add("Embassy of the Republic of Guatemala", "Guatemala");
                testdlink.Add("Embassy of The Republic of Guinea", "Guinea");
                testdlink.Add("Embassy of the Republic of Guinea Bissau", "Guinea Bissau");
                testdlink.Add("Embassy of the Hellenic Republic", "Hellenic");
                testdlink.Add("Embassy of The Republic of Honduras", "Honduras");
                testdlink.Add("Embassy Of Hungary", "Hungary");
                testdlink.Add("Consulate General of India", "India");
                testdlink.Add("Embassy of India", "India");
                testdlink.Add("Embassy of The Republic of Indonesia", "Indonesia");
                testdlink.Add("The Consulate General of The Republic of Indonesia", "Indonesia");
                testdlink.Add("Consulate General of Islamic Republic of Iran", "Iran");
                testdlink.Add("Embassy of The Islamic Republic of Iran", "Iran");
                testdlink.Add("Consulate General of The Republic Of Iraq", "Iraq");
                testdlink.Add("Embassy of The Republic of Iraq", "Iraq");
                testdlink.Add("Embassy Of Ireland", "Ireland");
                testdlink.Add("Consulate General of the State of Israel", "Israel");
                testdlink.Add("Embassy of the State of Israel", "Israel");
                testdlink.Add("Embassy of The Republic of Italy", "Italy");
                testdlink.Add("The Consulate General of the Republic of Italy", "Italy");
                testdlink.Add("Embassy of the Republic of Ivory Coast", "Ivory Coast");
                testdlink.Add("Consulate General Of Japan", "Japan");
                testdlink.Add("Embassy Of Japan", "Japan");
                testdlink.Add("Consulate General of The Hashemite Kingdom of Jordan", "Jordan");
                testdlink.Add("Embassy of The Hashemite Kingdom of Jordan", "Jordan");
                testdlink.Add("Consulate General of The Republic of Kazakhstan", "Kazakhstan");
                testdlink.Add("Embassy of The Republic of Kazakhstan", "Kazakhstan");
                testdlink.Add("Embassy of The Republic of Kenya", "Kenya");
                testdlink.Add("The Consulate General of The Republic of Kenya", "Kenya");
                testdlink.Add("Consulate General of the Republic of Korea", "Korea");
                testdlink.Add("Embassy of The Republic of Korea", "Korea");
                testdlink.Add("Embassy of the Republic of Kosovo", "Kosovo");
                testdlink.Add("Consulate General of The State of Kuwait", "Kuwait");
                testdlink.Add("Embassy of Kuwait", "Kuwait");
                testdlink.Add("Consulate General of The Republic of Kyrgystan", "Kyrgystan");
                testdlink.Add("Embassy of The Republic of Kyrgystan", "Kyrgystan");
                testdlink.Add("Embassy of Lao People's Democratic Republic", "Laos");
                testdlink.Add("Embassy of Latvia", "Latvia");
                testdlink.Add("Embassy Of The Republic Of Lebanon", "Lebanon");
                testdlink.Add("The Consulate General of The Republic of Lebanon", "Lebanon");
                testdlink.Add("Embassy of The Kingdom of Lesotho", "Lesotho");
                testdlink.Add("Consulate General of Republic of Liberia", "Liberia");
                testdlink.Add("Consulate General Of The State Of Libya", "Libya");
                testdlink.Add("Embassy of Libya", "Libya");
                testdlink.Add("Embassy of the Republic of Lithuania", "Lithuania");
                testdlink.Add("Embassy of the Grand Duchy of Luxembourg", "Luxembourg");
                testdlink.Add("Embassy of the Republic of North Macedonia", "Macedonia");
                testdlink.Add("Embassy of The Republic of Malawi", "Malawi");
                testdlink.Add("The Consulate General of the Republic of Malawi", "Malawi");
                testdlink.Add("Consulate General Of Malaysia", "Malaysia");
                testdlink.Add("Embassy of Malaysia", "Malaysia");
                testdlink.Add("Embassy of Republic of Maldives", "Maldives");
                testdlink.Add("Embassy of The Republic Mali", "Mali");
                testdlink.Add("Embassy of The Republic of Malta", "Malta");
                testdlink.Add("The Consulate General Of Malta", "Malta");
                testdlink.Add("Consulate General of The Republic of Mauritius", "Mauritius");
                testdlink.Add("Embassy of The United States of Mexico", "Mexico");
                testdlink.Add("Embassy of the Republic of Moldova", "Moldova");
                testdlink.Add("Embassy of Mongolia", "Mongolia");
                testdlink.Add("Montenegro Embassy", "Montenegro");
                testdlink.Add("Consulate General of The Kingdom of Morocco", "Morocco");
                testdlink.Add("Embassy of The Kingdom of Morocco", "Morocco");
                testdlink.Add("Consulate General Of The Republic Of Mozambique", "Mozambique");
                testdlink.Add("Embassy Of The Republic Of Mozambique", "Mozambique");
                testdlink.Add("Embassy of The Islamic Republic of Muritania", "Muritania");
                testdlink.Add("Embassy of Nepal", "Nepal");
                testdlink.Add("Consulate General of The Kingdom of Netherlands", "Netherlands");
                testdlink.Add("Embassy of the Kingdom of the Netherlands", "Netherlands");
                testdlink.Add("Consulate General Of New Zealand", "New Zealand");
                testdlink.Add("New Zealand Embassy", "New Zealand");
                testdlink.Add("Embassy of Republic of Nicaragua", "Nicaragua");
                testdlink.Add("Consulate General of The Republic of Niger", "Niger");
                testdlink.Add("Embassy of The Republic of Niger", "Niger");
                testdlink.Add("Consulate General of The Federal Republic of Nigeria", "Nigeria");
                testdlink.Add("Embassy of The Federal Republic of Nigeria", "Nigeria");
                testdlink.Add("Embassy of the Kingdom Of Norway", "Norway");
                testdlink.Add("Royal Norwegian Consulate", "Norway");
                testdlink.Add("Embassy of The Sultanate of Oman", "Oman");
                testdlink.Add("Embassy of The Islamic Republic of Pakistan", "Pakistan");
                testdlink.Add("The Consulate General Of The Islamic Republic Of Pakistan", "Pakistan");
                testdlink.Add("Consulate General of the State of Palestine", "Palestine");
                testdlink.Add("Embassy of The State of Palestine", "Palestine");
                testdlink.Add("Consulate General of the Republic of Panama", "Panama");
                testdlink.Add("Embassy of the Republic of Panama", "Panama");
                testdlink.Add("Embassy of the Republic of Paraguay", "Paraguay");
                testdlink.Add("Consulate General of Peru", "Peru");
                testdlink.Add("Consulate of The Republic of Philippine", "Philippine");
                testdlink.Add("Embassy of The Republic of Philippine", "Philippine");
                testdlink.Add("Embassy Of The Republic of Poland", "Poland");
                testdlink.Add("Embassy of the Republic of Portugal", "Portugal");
                testdlink.Add("Portuguese Trade Center", "Portugal");
                testdlink.Add("Consulate General of Romania", "Romania");
                testdlink.Add("Embassy Of Romania", "Romania");
                testdlink.Add("Consulate General Of The Russian Federation", "Russia");
                testdlink.Add("Embassy of The Russian Federation", "Russia");
                testdlink.Add("Republic Of Rwanda", "Rwanda");
                testdlink.Add("Consulate General of St. Kitts and Nevis", "Saint Kitts and Nevis");
                testdlink.Add("Consulate General Of The Kingdom of Saudi Arabia", "Saudi Arabia");
                testdlink.Add("Embassy of The Kingdom of Saudi Arabia", "Saudi Arabia");
                testdlink.Add("Embassy of The Republic of Senegal", "Senegal");
                testdlink.Add("Embassy Of The Republic Of Serbia", "Serbia");
                testdlink.Add("Embassy of the Republic of Seychelles", "Seychelles");
                testdlink.Add("Embassy of Sierra Leone", "Sierra Leone");
                testdlink.Add("Consulate General of The Republic of Singapore", "Singapore");
                testdlink.Add("Embassy of the Republic of Singapore", "Singapore");
                testdlink.Add("Embassy of the Slovak Republic", "Slovakia");
                testdlink.Add("Embassy Of The Republic Of Slovenia", "Slovenia");
                testdlink.Add("Consulate General Of The Federal Republic Of Somalia", "Somalia");
                testdlink.Add("Embassy of The Federal Republic of Somalia", "Somalia");
                testdlink.Add("Embassy of The Republic of South Africa", "South Africa");
                testdlink.Add("The General Consulate of The Republic of South Africa", "South Africa");
                testdlink.Add("Embassy of The Republic of South Sudan", "South Sudan");
                testdlink.Add("Embassy of The Kingdom of Spain", "Spain");
                testdlink.Add("Consulate Of The Democratic Socialist Republic Of Sri Lanka", "Sri Lanka");
                testdlink.Add("Embassy of Of The Democratic Socialist Republic Of Sri Lanka", "Sri Lanka");
                testdlink.Add("Consulate General of Arab Republic of Sudan", "Sudan");
                testdlink.Add("Embassy of The Republic of Sudan", "Sudan");
                testdlink.Add("Embassy of The Kingdom of Sweden", "Sweden");
                testdlink.Add("Consulate General of Switzerland", "Switzerland");
                testdlink.Add("Embassy of Switzerland", "Switzerland");
                testdlink.Add("Consulate General of Arab Republic of Syria", "Syria");
                testdlink.Add("Embassy of the Arab Republic of Syria", "Syria");
                testdlink.Add("Consulate General Of The Republic Of Tajikistan", "Tajikistan");
                testdlink.Add("Embassy of The Republic of Tajikistan", "Tajikistan");
                testdlink.Add("Consulate of The United Republic of Tanzania", "Tanzania");
                testdlink.Add("Embassy of The United Republic of Tanzania", "Tanzania");
                testdlink.Add("Embassy Of The Kingdom of Thailand", "Thailand");
                testdlink.Add("Royal Thai Consulate-General", "Thailand");
                testdlink.Add("Embassy of The Kingdom of Tonga", "Tonga");
                testdlink.Add("Embassy Of The Republic Of Tunisia", "Tunisia");
                testdlink.Add("The Consulate General of The Republic of Tunisia", "Tunisia");
                testdlink.Add("Consulate of the Republic of Türkiye", "Türkiye");
                testdlink.Add("Embassy Of The Republic Of Türkiye", "Türkiye");
                testdlink.Add("Consulate General of Turkmenistan", "Turkmenistan");
                testdlink.Add("Embassy of Turkmenistan", "Turkmenistan");
                testdlink.Add("Embassy of Tuvalu", "Tuvalu");
                testdlink.Add("Embassy of The Republic of Uganda", "Uganda");
                testdlink.Add("Embassy of Ukraine", "Ukraine");
                testdlink.Add("British Embassy", "United Kingdom");
                testdlink.Add("The Consulate of United Kingdom", "United Kingdom");
                testdlink.Add("Consulate General Of The United States Of America", "United States Of America");
                testdlink.Add("Embassy Of The United States Of America", "United States Of America");
                testdlink.Add("Embassy of The Republic of Uruguay", "Uruguay");
                testdlink.Add("Consulate General Of The Republic Of Uzbekistan", "Uzbekistan");
                testdlink.Add("Embassy of the Republic of Uzbekistan", "Uzbekistan");
                testdlink.Add("Consulate General of The Republic of Vanuatu", "Vanuatu");
                testdlink.Add("Embassy of The Bolivarian Republic of Venezuela", "Venezuela");
                testdlink.Add("Embassy of the Socialist Republic of Vietnam", "Vietnam");
                testdlink.Add("Consulate General Of The Republic Of Yemen", "Yemen");
                testdlink.Add("Embassy of the republic of Yemen", "Yemen");
                testdlink.Add("Embassy of The Republic of Zambia", "Zambia");
                testdlink.Add("Republic of Zimbabwe", "Zimbabwe");
            }
            var tempstring = "";

            if (certificate_of.IsNullOrWhiteSpace())
            {
                goto end;
            }
            else
            {
                ViewBag.continueprogram = true;
                ViewBag.certificate_of = certificate_of;
            }

            if (certificatetype != null)
            {
                var userempnolists = db.usernames
                    .Where(x => x.employee_no != null && x.AspNetUser.UserName == User.Identity.Name).ToList();
                var empusers = userempnolists.Find(x => x.AspNetUser.UserName == User.Identity.Name);
                var leafcon = new LeavesController();
                var empjd = empusers.master_file;
                var certificatereqsave = new certificatesavingtest_();
                certificatereqsave.employee_id = empjd.employee_id;
                certificatereqsave.certificate_type = certificatetype.Value;
                if (!embassy.IsNullOrWhiteSpace())
                {
                    testdlink.TryGetValue(embassy,out tempstring);
                    certificatereqsave.destination = embassy+","+tempstring+","+from+","+to;
                }
                else
                {
                    if (destination.IsNullOrWhiteSpace())
                    {
                        destination = " ";
                    }
                    certificatereqsave.destination = destination;
                }

                certificatereqsave.status = "new EMP";
                certificatereqsave.submited_by = User.Identity.Name;
                certificatereqsave.approved_by = "";
                certificatereqsave.modifieddate_by = DateTime.Now;
                certificatereqsave.submition_date = DateTime.Today;
                certificatereqsave.cs_gr = certificate_of;
                certificatereqsave.submition_date = DateTime.Today;
                if (certificate_of == "GR_certificates")
                {
                    certificatesavinggrove certificatesavingG = new certificatesavinggrove();
                    var certogr = new certificatesavingtest_Controller();
                    certificatesavingG = certogr.concertificatesavinggrove(certificatereqsave);
                    var grcrtlist = db.certificatesavinggroves.ToList();
                    if (!grcrtlist.Exists(x=>x.employee_id == certificatesavingG.employee_id && x.certificate_type == certificatesavingG.certificate_type && x.submition_date == certificatesavingG.submition_date))
                    {
                        db.certificatesavinggroves.Add(certificatesavingG);
                        db.SaveChanges();
                    }
                    
                }
                else
                {

                    var grcrtlist = db.certificatesavingtest_.ToList();
                    if (!grcrtlist.Exists(x => x.employee_id == certificatereqsave.employee_id && x.certificate_type == certificatereqsave.certificate_type && x.submition_date == certificatereqsave.submition_date))
                    {
                        db.certificatesavingtest_.Add(certificatereqsave);
                        db.SaveChanges();
                    }
                }
                ViewBag.submmites = "certificate request has been successfully submitted";
            }
            List<string> CountryList = new List<string>();
            CultureInfo[] CInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo CInfo in CInfoList)
            {
                RegionInfo R = new RegionInfo(CInfo.LCID);
                if (!(CountryList.Contains(R.EnglishName)))
                {
                    CountryList.Add(R.EnglishName);
                }
            }

            CountryList.Sort();
            ViewBag.CountryList = CountryList;
            ViewBag.certificatetype = new SelectList(db.certificatetypes.Where(x => x.certificsatefor == "Employee"), "Id", "certificate_name_");
            end: ;
            return View();
        }


        public void SendMail(int id)
        {
            var message = new MimeMessage();
            var desig = "";
            var cstvar = db.certificatesavingtest_.ToList().Find(x=>x.Id == id);
            var usernamelist = db.usernames.ToList();
            var contractlist = db.contracts.OrderByDescending(x => x.date_changed).ToList();
            message.From.Add(new MailboxAddress("Hrworks", "leave@citiscapegroup.com"));
            var emplusersname = usernamelist.Find(x => x.employee_no == cstvar.master_file.employee_id);
            if (contractlist.Exists(x => x.employee_no == cstvar.master_file.employee_id))
            {
                var temp = contractlist.Find(x => x.employee_no == cstvar.master_file.employee_id);
                if (!temp.designation.IsNullOrWhiteSpace())
                {
                    desig = temp.designation;
                }
            }
            message.To.Add((new MailboxAddress("Yahya Rashid", "yrashid@citiscapegroup.com")));
            message.Subject = "certificate request approval";
            message.Body = new TextPart("plain")
            {
                Text = @"Dear Sir/ma'am," + "\n\n" + "Please note that certificate request for   (" +
                cstvar.master_file.employee_no + ") " +
                emplusersname.full_name + "-" + desig + " has been submitted for your approval" + "\n\n\n" +
                "http://hrworks.ddns.net:6333/citiworks/certificatesavingtest_/hrapproval" + "\n\n\n" +
                "Thanks Best Regards, "
            };
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
        }

        [ActionName("Importexcel")]
        [HttpPost]
        public ActionResult Importexcel()
        {
            if (this.Request.Files["FileUpload1"].ContentLength > 0)
            {
                var extension = Path.GetExtension(this.Request.Files["FileUpload1"].FileName).ToLower();
                string query = null;
                var connString = string.Empty;

                string[] validFileTypes = { ".csv" };

                var path1 = string.Format(
                    "{0}/{1}",
                    this.Server.MapPath("~/Content/Uploads"),
                    this.Request.Files["FileUpload1"].FileName);
                if (!Directory.Exists(path1)) Directory.CreateDirectory(this.Server.MapPath("~/Content/Uploads"));
                var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed)
                    .ToList();
                var afinallist = new List<master_file>();
                var erlist = new List<emprel>();
                foreach (var file in alist)
                {
                    if (afinallist.Count == 0) afinallist.Add(file);

                    if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
                }

                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1)) System.IO.File.Delete(path1);

                    this.Request.Files["FileUpload1"].SaveAs(path1);
                    if (extension == ".csv")
                    {
                        var dt = Utility.ConvertCSVtoDataTable(path1);
                        this.ViewBag.Data = dt;
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                var pro = new detailsinarabic();
                                var dublicatecheck = this.db.detailsinarabics.ToList();
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (column.ColumnName == "employee no")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var epid = afinallist.Find(x => x.employee_no == idm);
                                            if (epid == null ) goto e;
                                            if (epid.employee_no == 0) goto e;
                                            pro.employee_id = epid.employee_id;
                                        }
                                    }
                                    
                                    if (column.ColumnName == "NameArabic")
                                    {
                                        pro.ARname = dr[column].ToString();
                                    } 

                                    if (column.ColumnName == "PositionArabic")
                                    {
                                        pro.ARposition = dr[column].ToString();
                                    }

                                    if (column.ColumnName == "NationalityArabic")
                                    {
                                        pro.ARnationality = dr[column].ToString();
                                    }
                                }

                                if (pro.employee_id == 0) goto e;
                                if (!dublicatecheck.Exists(x =>x.employee_id == pro.employee_id))
                                {
                                    this.db.detailsinarabics.Add(pro);
                                    this.db.SaveChanges();
                                }
                            e: ;
                            }
                        }
                    }
                }
            }
            else
            {
                this.ViewBag.Error = "Please Upload Files in .csv format";
            }

            return this.View();
        }

        public ActionResult ImportExcel()
        {
            return this.View();
        }

        public void DownloadExcel()
        {
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("CONTRACT");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "NameArabic";
            Sheet.Cells["C1"].Value = "PositionArabic";
            Sheet.Cells["D1"].Value = "NationalityArabic";
            var row = 2;
            Sheet.Cells[string.Format("A{0}", row)].Value = 5386;
            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename = detailsinarabic_template.xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
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
