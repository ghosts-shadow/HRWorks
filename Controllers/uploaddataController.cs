using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HRworks.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.CodeAnalysis;
using OfficeOpenXml;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace HRworks.Controllers
{
    [Authorize]
    public class uploaddataController : Controller
    {
        private const string Purpose = "equalizer";

        public string Protect(string unprotectedText)
        {
            if (unprotectedText.IsNullOrWhiteSpace())
            {
                unprotectedText = "0";
            }

            var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedText);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }

        public string Unprotect(string protectedText)
        {
            try
            {
                var protectedBytes = Convert.FromBase64String(protectedText);
                var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
                var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
                return unprotectedText;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private readonly HREntities db = new HREntities();

        // GET: uploaddata
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Importincl()
        {
            return this.View();
        }

        [ActionName("Importincl")]
        [HttpPost]
        public ActionResult ImportIncl()
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
                            var leavecheck = this.db.contracts.ToList();
                            foreach (DataRow dr in dt.Rows)
                            {
                                var pro = new insurance();
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (dr[column] == null || dr[column].ToString() == " ") goto e;

                                    if (column.ColumnName == "Card no")
                                    {
                                        var dtt = dr[column].ToString();
                                        int.TryParse(dtt, out var a);
                                        pro.card_no = a;
                                    }

                                    if (column.ColumnName == "Dependency")
                                    {
                                        var dtt = dr[column].ToString();
                                        pro.dependency = dtt;
                                    }

                                    if (column.ColumnName == "Marital Status")
                                    {
                                        var dtt = dr[column].ToString();
                                        pro.marital_status = dtt;
                                    }

                                    if (column.ColumnName == "Annual Primium")
                                    {
                                        var dtt = dr[column].ToString();
                                        int.TryParse(dtt, out var a);
                                        pro.annual_primium = a;
                                    }

                                    if (column.ColumnName == "Invoice No")
                                    {
                                        var dtt = dr[column].ToString();
                                        int.TryParse(dtt, out var a);
                                        pro.invoice_no = a;
                                    }


                                    if (column.ColumnName == "Credit Amt")
                                    {
                                        var dtt = dr[column].ToString();
                                        int.TryParse(dtt, out var a);
                                        pro.credit_amt = a;
                                    }

                                    if (column.ColumnName == "Deletion Date")
                                    {
                                        var dtt = dr[column].ToString();
                                        DateTime.TryParse(dtt, out var a);
                                        pro.deletion_date = a;
                                    }


                                    if (column.ColumnName == "employee_no")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var epid = afinallist.Find(x => x.employee_no == idm);
                                            if (epid == null) goto e;
                                            pro.employee_no = epid.employee_id;
                                            var dob = epid.dob.Value;
                                            var age = 0;
                                            age = DateTime.Now.DayOfYear - dob.Year;
                                            if (DateTime.Now.DayOfYear < dob.DayOfYear)
                                                age = age - 1;
                                            pro.age = age;
                                        }
                                    }
                                }

                                this.db.insurances.Add(pro);
                                this.db.SaveChanges();
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


        public ActionResult Importbankdetails()
        {
            return this.View();
        }

        [ActionName("Importbankdetails")]
        [HttpPost]
        public ActionResult ImportbankDetails()
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
                                var pro = new bank_details();
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (dr[column] == null || dr[column].ToString() == " ") goto e;
                                    if (column.ColumnName == "IBAN")
                                    {
                                        var dtt = dr[column].ToString();
                                        pro.IBAN = dtt;
                                    }

                                    if (column.ColumnName == "Bank Name")
                                    {
                                        var dtt = dr[column].ToString();
                                        pro.bank_name = dtt;
                                    }

                                    if (column.ColumnName == "EMPNO")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var epid = afinallist.Find(x => x.employee_no == idm);
                                            if (epid == null) goto e;
                                            pro.employee_no = epid.employee_id;
                                            var dob = epid.dob.Value;
                                            var age = 0;
                                            age = DateTime.Now.Year - dob.Year;
                                            if (DateTime.Now.DayOfYear < dob.DayOfYear)
                                                age = age - 1;
                                        }
                                    }
                                }

                                var banklist = db.bank_details.ToList();
                                if (banklist.Exists(x => x.employee_no == pro.employee_no))
                                {
                                    var bankvar = banklist.Find(x => x.employee_no.Equals(pro.employee_no));
                                    bankvar.bank_name = pro.bank_name;
                                    bankvar.IBAN = pro.IBAN;
                                    db.Entry(bankvar).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    this.db.bank_details.Add(pro);
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


        public ActionResult Importconlcbdppeid()
        {
            return this.View();
        }

        [ActionName("Importconlcbdppeid")]
        [HttpPost]
        public ActionResult ImportConlcbdppeid()
        {
            var path1 = "";
            try
            {
                if (this.Request.Files["FileUpload1"].ContentLength > 0)
                {
                    var extension = Path.GetExtension(this.Request.Files["FileUpload1"].FileName).ToLower();
                    string query = null;
                    var connString = string.Empty;

                    string[] validFileTypes = { ".csv" };

                    path1 = string.Format(
                        "{0}/{1}",
                        this.Server.MapPath("~/Content/Uploads"),
                        this.Request.Files["FileUpload1"].FileName);
                    if (!Directory.Exists(path1)) Directory.CreateDirectory(this.Server.MapPath("~/Content/Uploads"));
                    var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed)
                        .ToList();
                    var afinallist = new List<master_file>();
                    var duplist = new List<master_file>();
                    foreach (var file in alist)
                    {
                        var temp = file.employee_no;
                        var temp2 = file.last_working_day;
                        var temp3 = file.status;
                        if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                        {
                            if ((file.last_working_day.HasValue) ||
                                (file.status != "inactive" && !file.last_working_day.HasValue))
                            {
                                if (!duplist.Exists(x => x.employee_no == file.employee_no))
                                {
                                    afinallist.Add(file);
                                }
                            }
                            else
                            {
                                duplist.Add(file);
                            }
                        }
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
                                    var probk = new bank_details();
                                    var prolc = new labour_card();
                                    var propp = new passport();
                                    var proeid = new emirates_id();
                                    var procon = new contract();
                                    foreach (DataColumn column in dt.Columns)
                                    {
                                        if (dr[column] == null || dr[column].ToString() == " ") goto e;
                                        //bank details
                                        {
                                            if (column.ColumnName == "IBAN")
                                            {
                                                var dtt = dr[column].ToString();
                                                probk.IBAN = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "Bank Name")
                                            {
                                                var dtt = dr[column].ToString();
                                                probk.bank_name = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "Account no")
                                            {
                                                var dtt = dr[column].ToString();
                                                long.TryParse(dtt, out long temp);
                                                probk.Account_no = temp;
                                                goto nextcol;
                                            }
                                        }
                                        //contract
                                        {
                                            if (column.ColumnName == "designation")
                                            {
                                                procon.designation = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "grade")
                                            {
                                                procon.grade = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "department/project")
                                            {
                                                procon.departmant_project = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "salary_details")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.salary_details = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "basic")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.basic = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "company")
                                            {
                                                procon.company = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "category")
                                            {
                                                procon.category = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "housing_allowance")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.housing_allowance = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "transportation_allowance")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.transportation_allowance = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "FOT")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.FOT = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "food_allowance")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.food_allowance = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "living_allowance")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.living_allowance = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "ticket_allowance")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.ticket_allowance = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "others")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.others = dtt;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "arrears")
                                            {
                                                var dtt = this.Protect(dr[column].ToString());
                                                procon.arrears = dtt;
                                                goto nextcol;
                                            }
                                        }
                                        //emid
                                        {
                                            if (column.ColumnName == "eid no")
                                            {
                                                var dtt = dr[column].ToString();
                                                long.TryParse(dtt, out var a);
                                                proeid.eid_no = a;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "eid expiry")
                                            {
                                                var dtt = dr[column].ToString();
                                                DateTime.TryParse(dtt, out var a);
                                                proeid.eid_expiry = a;
                                                goto nextcol;
                                            }
                                        }
                                        //passport
                                        {
                                            if (column.ColumnName == "company code")
                                            {
                                                propp.company_code = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "passport_no")
                                            {
                                                propp.passport_no = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "passport expiry")
                                            {
                                                var dtt = dr[column].ToString();
                                                DateTime.TryParse(dtt, out var a);
                                                propp.passport_expiry = a;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "Passport Issue Date")
                                            {
                                                var dtt = dr[column].ToString();
                                                DateTime.TryParse(dtt, out var a);
                                                propp.passport_issue_date = a;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "Passport Return Date")
                                            {
                                                var dtt = dr[column].ToString();
                                                DateTime.TryParse(dtt, out var a);
                                                propp.passport_return_date = a;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "Passport Remarks")
                                            {
                                                propp.passport_remarks = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "status")
                                            {
                                                propp.status = dr[column].ToString();
                                                goto nextcol;
                                            }
                                        }
                                        //labourcard
                                        {
                                            if (column.ColumnName == "work_permit_no")
                                            {
                                                var dtt = dr[column].ToString();
                                                long.TryParse(dtt, out var a);
                                                prolc.work_permit_no = a;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "personal_no")
                                            {
                                                var dtt = dr[column].ToString();
                                                long.TryParse(dtt, out var a);
                                                prolc.personal_no = a;
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "proffession")
                                            {
                                                prolc.proffession = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "establishment")
                                            {
                                                prolc.establishment = dr[column].ToString();
                                                goto nextcol;
                                            }

                                            if (column.ColumnName == "lc expiry")
                                            {
                                                var dtt = dr[column].ToString();
                                                DateTime.TryParse(dtt, out var a);
                                                prolc.lc_expiry = a;
                                                goto nextcol;
                                            }
                                        }
                                        if (column.ColumnName == "EMPNO")
                                        {
                                            int.TryParse(dr[column].ToString(), out var idm);
                                            if (idm != 0)
                                            {
                                                var epid = afinallist.Find(x => x.employee_no == idm);
                                                if (epid == null) goto e;
                                                probk.employee_no = epid.employee_id;
                                                prolc.emp_no = epid.employee_id;
                                                propp.employee_no = epid.employee_id;
                                                proeid.employee_no = epid.employee_id;
                                                procon.employee_no = epid.employee_id;
                                            }
                                            else goto e;
                                        }

                                        nextcol: ;
                                    }


                                    var banklist = db.bank_details.ToList();
                                    if (banklist.Exists(x => x.employee_no == probk.employee_no))
                                    {
                                        var bankvar = banklist.Find(x => x.employee_no.Equals(probk.employee_no));
                                        bankvar.bank_name = probk.bank_name;
                                        bankvar.IBAN = probk.IBAN;
                                        db.Entry(bankvar).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        if (probk != null)
                                        {
                                            this.db.bank_details.Add(probk);
                                            this.db.SaveChanges();
                                        }
                                    }

                                    var contractList = db.contracts.ToList();
                                    if (contractList.Exists(x => x.employee_no == probk.employee_no))
                                    {
                                        var convar = contractList.Find(x => x.employee_no.Equals(probk.employee_no));
                                        if (procon.con_id.HasValue && convar.con_id != procon.con_id)
                                        {
                                            convar.con_id = procon.con_id;
                                        }

                                        if (!procon.designation.IsNullOrWhiteSpace() &&
                                            convar.designation != procon.designation)
                                        {
                                            convar.designation = procon.designation;
                                        }

                                        if (!procon.grade.IsNullOrWhiteSpace() && convar.grade != procon.grade)
                                        {
                                            convar.grade = procon.grade;
                                        }

                                        if (!procon.departmant_project.IsNullOrWhiteSpace() &&
                                            convar.departmant_project != procon.departmant_project)
                                        {
                                            convar.departmant_project = procon.departmant_project;
                                        }

                                        if (!procon.company.IsNullOrWhiteSpace() &&
                                            convar.company != procon.designation)
                                        {
                                            convar.company = procon.company;
                                        }

                                        if (!procon.category.IsNullOrWhiteSpace() && convar.category != procon.category)
                                        {
                                            convar.category = procon.category;
                                        }

                                        if (!procon.salary_details.IsNullOrWhiteSpace() &&
                                            convar.salary_details != procon.salary_details)
                                        {
                                            convar.salary_details = procon.salary_details;
                                        }

                                        if (!procon.basic.IsNullOrWhiteSpace() && convar.basic != procon.basic)
                                        {
                                            convar.basic = procon.basic;
                                        }

                                        if (!procon.housing_allowance.IsNullOrWhiteSpace() &&
                                            convar.housing_allowance != procon.housing_allowance)
                                        {
                                            convar.housing_allowance = procon.housing_allowance;
                                        }

                                        if (!procon.transportation_allowance.IsNullOrWhiteSpace() &&
                                            convar.transportation_allowance != procon.transportation_allowance)
                                        {
                                            convar.transportation_allowance = procon.transportation_allowance;
                                        }

                                        if (!procon.FOT.IsNullOrWhiteSpace() && convar.FOT != procon.FOT)
                                        {
                                            convar.FOT = procon.FOT;
                                        }

                                        if (!procon.food_allowance.IsNullOrWhiteSpace() &&
                                            convar.food_allowance != procon.food_allowance)
                                        {
                                            convar.food_allowance = procon.food_allowance;
                                        }

                                        if (!procon.living_allowance.IsNullOrWhiteSpace() &&
                                            convar.living_allowance != procon.living_allowance)
                                        {
                                            convar.living_allowance = procon.living_allowance;
                                        }

                                        if (!procon.ticket_allowance.IsNullOrWhiteSpace() &&
                                            convar.ticket_allowance != procon.ticket_allowance)
                                        {
                                            convar.ticket_allowance = procon.ticket_allowance;
                                        }

                                        if (!procon.others.IsNullOrWhiteSpace() && convar.others != procon.others)
                                        {
                                            convar.others = procon.others;
                                        }

                                        if (!procon.arrears.IsNullOrWhiteSpace() && convar.arrears != procon.arrears)
                                        {
                                            convar.arrears = procon.arrears;
                                        }

                                        if (!procon.imgpath.IsNullOrWhiteSpace() && convar.imgpath != procon.imgpath)
                                        {
                                            convar.imgpath = procon.imgpath;
                                        }

                                        db.Entry(convar).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        if (procon != null)
                                        {
                                            this.db.contracts.Add(procon);
                                            this.db.SaveChanges();
                                        }
                                    }

                                    var emidlist = db.emirates_id.ToList();
                                    if (emidlist.Exists(x => x.employee_no == probk.employee_no))
                                    {
                                        var eidvar = emidlist.Find(x => x.employee_no.Equals(probk.employee_no));

                                        if (eidvar.eid_no != proeid.eid_no)
                                            eidvar.eid_no = proeid.eid_no;
                                        if (eidvar.eid_expiry != proeid.eid_expiry)
                                            eidvar.eid_expiry = proeid.eid_expiry;
                                        db.Entry(eidvar).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        if (proeid != null)
                                        {
                                            this.db.emirates_id.Add(proeid);
                                            this.db.SaveChanges();
                                        }
                                    }

                                    var pplist = db.passports.ToList();
                                    if (pplist.Exists(x => x.employee_no == probk.employee_no))
                                    {
                                        var ppvar = pplist.Find(x => x.employee_no.Equals(probk.employee_no));
                                        if (ppvar.company_code != propp.company_code)
                                            ppvar.company_code = propp.company_code;
                                        if (ppvar.passport_no != propp.passport_no)
                                            ppvar.passport_no = propp.passport_no;
                                        if (!propp.passport_expiry.HasValue &&
                                            ppvar.passport_expiry != propp.passport_expiry)
                                            ppvar.passport_expiry = propp.passport_expiry;
                                        db.Entry(ppvar).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        if (propp != null)
                                        {
                                            this.db.passports.Add(propp);
                                            this.db.SaveChanges();
                                        }
                                    }

                                    var lclist = db.labour_card.ToList();
                                    if (lclist.Exists(x => x.emp_no == prolc.emp_no))
                                    {
                                        var lcvar = lclist.Find(x => x.emp_no.Equals(probk.employee_no));
                                        if (!prolc.work_permit_no.HasValue && lcvar.imgpath != procon.imgpath)
                                            lcvar.work_permit_no = prolc.work_permit_no;
                                        if (!prolc.personal_no.HasValue && lcvar.personal_no != prolc.personal_no)
                                            lcvar.personal_no = prolc.personal_no;
                                        if (!prolc.proffession.IsNullOrWhiteSpace() &&
                                            lcvar.proffession != prolc.proffession)
                                            lcvar.proffession = prolc.proffession;
                                        if (!prolc.establishment.IsNullOrWhiteSpace() &&
                                            lcvar.establishment != prolc.establishment)
                                            lcvar.establishment = prolc.establishment;
                                        if (!prolc.lc_expiry.HasValue && lcvar.lc_expiry != prolc.lc_expiry)
                                            lcvar.lc_expiry = prolc.lc_expiry;
                                        db.Entry(lcvar).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        if (prolc != null)
                                        {
                                            this.db.labour_card.Add(prolc);
                                            this.db.SaveChanges();
                                        }
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
            }
            finally
            {
                if (System.IO.File.Exists(path1))
                {
                    System.IO.File.Delete(path1);
                }
            }

            return this.View();
        }


        public ActionResult Importlaborcard()
        {
            return this.View();
        }

        [ActionName("Importlaborcard")]
        [HttpPost]
        public ActionResult ImportLaborcard()
        {
            var path1 = "";
            try
            {
                if (this.Request.Files["FileUpload1"].ContentLength > 0)
                {
                    var extension = Path.GetExtension(this.Request.Files["FileUpload1"].FileName).ToLower();
                    string query = null;
                    var connString = string.Empty;

                    string[] validFileTypes = { ".csv" };

                    path1 = string.Format(
                        "{0}/{1}",
                        this.Server.MapPath("~/Content/Uploads"),
                        this.Request.Files["FileUpload1"].FileName);
                    if (!Directory.Exists(path1)) Directory.CreateDirectory(this.Server.MapPath("~/Content/Uploads"));
                    var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed)
                        .ToList();
                    var afinallist = new List<master_file>();
                    var duplist = new List<master_file>();
                    foreach (var file in alist)
                    {
                        var temp = file.employee_no;
                        var temp2 = file.last_working_day;
                        var temp3 = file.status;
                        if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                        {
                            if ((file.last_working_day.HasValue) ||
                                (file.status != "inactive" && !file.last_working_day.HasValue))
                            {
                                if (!duplist.Exists(x => x.employee_no == file.employee_no))
                                {
                                    afinallist.Add(file);
                                }
                            }
                            else
                            {
                                duplist.Add(file);
                            }
                        }
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
                                        var prolc = new labour_card();
                                    foreach (DataColumn column in dt.Columns)
                                    {
                                        if (dr[column] == null || dr[column].ToString() == " ")
                                        {
                                            goto e;
                                        }
                                        else if (column.ColumnName == "work_permit_no")
                                        {
                                            var dtt = dr[column].ToString();
                                            long.TryParse(dtt, out var a);
                                            prolc.work_permit_no = a;
                                        }

                                        else if (column.ColumnName == "personal_no")
                                        {
                                            var dtt = dr[column].ToString();
                                            long.TryParse(dtt, out var a);
                                            prolc.personal_no = a;
                                        }

                                        else if (column.ColumnName == "proffession")
                                        {
                                            prolc.proffession = dr[column].ToString();
                                        }

                                        else if (column.ColumnName == "establishment")
                                        {
                                            prolc.establishment = dr[column].ToString();
                                        }

                                        else if (column.ColumnName == "lc expiry")
                                        {
                                            var dtt = dr[column].ToString();
                                            DateTime.TryParse(dtt, out var a);
                                            prolc.lc_expiry = a;
                                        }

                                        else if (column.ColumnName == "EMPNO")
                                        {
                                            int.TryParse(dr[column].ToString(), out var idm);
                                            if (idm != 0)
                                            {
                                                var epid = afinallist.Find(x => x.employee_no == idm);
                                                if (epid == null) goto e;
                                                prolc.emp_no = epid.employee_id;
                                            }
                                            else goto e;
                                        }
                                    }

                                        var lclist = db.labour_card.ToList();
                                        if (lclist.Exists(x => x.emp_no == prolc.emp_no))
                                        {
                                            var lcvar = lclist.Find(x => x.emp_no.Equals(prolc.emp_no));
                                            if (prolc.work_permit_no.HasValue && lcvar.work_permit_no != prolc.work_permit_no)
                                                lcvar.work_permit_no = prolc.work_permit_no;
                                            if (prolc.personal_no.HasValue && lcvar.personal_no != prolc.personal_no)
                                                lcvar.personal_no = prolc.personal_no;
                                            if (!prolc.proffession.IsNullOrWhiteSpace() && lcvar.proffession != prolc.proffession)
                                                lcvar.proffession = prolc.proffession;
                                            if (!prolc.establishment.IsNullOrWhiteSpace() && lcvar.establishment != prolc.establishment)
                                                lcvar.establishment = prolc.establishment;
                                            if (prolc.lc_expiry.HasValue && lcvar.lc_expiry != prolc.lc_expiry)
                                                lcvar.lc_expiry = prolc.lc_expiry;
                                            db.Entry(lcvar).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            if (prolc != null)
                                            {
                                                this.db.labour_card.Add(prolc);
                                                this.db.SaveChanges();
                                            }
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
            }
            finally
            {
                if (System.IO.File.Exists(path1))
                {
                    System.IO.File.Delete(path1);
                }
            }

            return this.View();
        }

        public void Downloadincsample()
        {
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("inc");
            Sheet.Cells["A1"].Value = "Card no";
            Sheet.Cells["B1"].Value = "Dependency";
            Sheet.Cells["C1"].Value = "Marital Status";
            Sheet.Cells["D1"].Value = "Annual Primium";
            Sheet.Cells["E1"].Value = "Invoice No";
            Sheet.Cells["F1"].Value = "Credit Amt";
            Sheet.Cells["G1"].Value = "Deletion Date";
            Sheet.Cells["G2"].Value = "date format = : mm/dd/yyy";
            Sheet.Cells["H1"].Value = "employee_no";

            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename = incsample.csv");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
        }

        /*public void Downloadallsample()
        {
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("inc");
            Sheet.Cells["A1"].Value = "EMPNO";
            Sheet.Cells["B1"].Value = "IBAN";
            Sheet.Cells["C1"].Value = "Bank Name";
            Sheet.Cells["D1"].Value = "Account no";
            Sheet.Cells["E1"].Value = "designation";
            Sheet.Cells["F1"].Value = "grade";
            Sheet.Cells["G1"].Value = "department/project";
            Sheet.Cells["H1"].Value = "salary_details";
            Sheet.Cells["I1"].Value = "basic";
            Sheet.Cells["J1"].Value = "company";
            Sheet.Cells["K1"].Value = "category";
            Sheet.Cells["L1"].Value = "housing_allowance";
            Sheet.Cells["M1"].Value = "transportation_allowance";
            Sheet.Cells["N1"].Value = "FOT";
            Sheet.Cells["O1"].Value = "food_allowance";
            Sheet.Cells["P1"].Value = "living_allowance";
            Sheet.Cells["Q1"].Value = "ticket_allowance";
            Sheet.Cells["R1"].Value = "others";
            Sheet.Cells["S1"].Value = "arrears";
            Sheet.Cells["T1"].Value = "eid no";
            Sheet.Cells["U1"].Value = "eid expiry";
            Sheet.Cells["V1"].Value = "company code";
            Sheet.Cells["W1"].Value = "passport_no";
            Sheet.Cells["X1"].Value = "passport expiry";
            Sheet.Cells["Y1"].Value = "work_permit_no";
            Sheet.Cells["Z1"].Value = "personal_no";
            Sheet.Cells["AA1"].Value = "proffession";
            Sheet.Cells["AB1"].Value = "establishment";
            Sheet.Cells["AC1"].Value = "lc expiry";

            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename = incsample.csv");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
        }*/
        public void Downloadallsample()
        {
            StringBuilder csvData = new StringBuilder();
            csvData.AppendLine(
                "EMPNO,IBAN,Bank Name,Account no,designation,grade,department/project,salary_details,basic,company,category,housing_allowance,transportation_allowance,FOT,food_allowance,living_allowance,ticket_allowance,others,arrears,eid no,eid expiry,company code,passport_no,passport expiry,Passport Issue Date,Passport Return Date,Passport Remarks,status,work_permit_no,personal_no,proffession,establishment,lc expiry");
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("content-disposition", "attachment;filename=sample.csv");
            Response.Write(csvData.ToString());
            Response.End();
        }

        public ActionResult contractupdate()
        {
            return View();
        }

        [ActionName("contractupdate")]
        [HttpPost]
        public ActionResult contractUpdate()
        {
            var path1 = "";
            try
            {
                if (this.Request.Files["FileUpload1"].ContentLength > 0)
                {
                    var extension = Path.GetExtension(this.Request.Files["FileUpload1"].FileName).ToLower();
                    string query = null;
                    var connString = string.Empty;

                    string[] validFileTypes = { ".csv" };

                    path1 = string.Format(
                        "{0}/{1}",
                        this.Server.MapPath("~/Content/Uploads"),
                        this.Request.Files["FileUpload1"].FileName);
                    if (!Directory.Exists(path1)) Directory.CreateDirectory(this.Server.MapPath("~/Content/Uploads"));
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
                                var contractlist = db.contracts.ToList();
                                foreach (DataRow dr in dt.Rows)
                                {
                                    var contract = new contract();
                                    foreach (DataColumn column in dt.Columns)
                                    {
                                        if (dr[column] == null || dr[column].ToString() == " ") goto e;
                                        if (column.ColumnName == "EMPNO")
                                        {
                                            int.TryParse(dr[column].ToString(), out var idm);
                                            contract = contractlist.Find(x => x.master_file.employee_no == idm);
                                            if (contract == null)
                                            {
                                                goto e;
                                            }
                                        }else if (column.ColumnName == "grade")
                                        {
                                            contract.grade = dr[column].ToString();
                                        }


                                    }

                                    db.Entry(contract).State = EntityState.Modified;
                                    db.SaveChanges();
                                e:;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.ViewBag.Error = "Please Upload Files in .csv format";
                }
            }
            finally
            {
                if (System.IO.File.Exists(path1))
                {
                    System.IO.File.Delete(path1);
                }
            }

            return this.View();
        }

        public ActionResult Masterfileupload()
        {
                
            return View();
        }
        [HttpPost]
        [ActionName("Masterfileupload")]
        public ActionResult MasterfileuploaD()
        {

            var path1 = "";
            try
            {
                if (this.Request.Files["FileUpload1"].ContentLength > 0)
                {
                    var extension = Path.GetExtension(this.Request.Files["FileUpload1"].FileName).ToLower();
                    string query = null;
                    var connString = string.Empty;
                    string[] validFileTypes = { ".csv" };
                    path1 = string.Format(
                        "{0}/{1}",
                        this.Server.MapPath("~/Content/Uploads"),
                        this.Request.Files["FileUpload1"].FileName);
                    var masternotuploaded ="emp no not uploaded : ";
                    if (!Directory.Exists(path1)) Directory.CreateDirectory(this.Server.MapPath("~/Content/Uploads"));
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
                                    var masterfile = new master_file();
                                    foreach (DataColumn column in dt.Columns)
                                    {
                                        if (dr[column] == null || dr[column].ToString() == " ") goto e;
                                        if (column.ColumnName == "Citiscape Employee Number")
                                        {
                                            int.TryParse(dr[column].ToString(), out var idm);
                                            if (db.master_file.ToList().Exists(x => x.employee_no == idm))
                                            {
                                                masternotuploaded+=(dr[column].ToString() + " ");
                                                goto e;
                                            }

                                            masterfile.employee_no = idm;
                                        }
                                        else if (column.ColumnName == "Joining Date")
                                        {
                                            var dtt = dr[column].ToString();
                                            DateTime.TryParse(dtt, out var a);
                                            masterfile.date_joined = a;
                                        }
                                        else if (column.ColumnName == "EMPLOYEE NAME")
                                        {
                                            var dtt = dr[column].ToString();
                                            masterfile.employee_name = dtt;
                                        }
                                        else if (column.ColumnName == "Nationality")
                                        {
                                            var dtt = dr[column].ToString();
                                            masterfile.nationality = dtt;
                                        }
                                        else if (column.ColumnName == "DOB")
                                        {
                                            var dtt = dr[column].ToString();
                                            DateTime.TryParse(dtt, out var a);
                                            masterfile.dob = a;
                                        }


                                    }
                                    db.master_file.Add(masterfile);
                                    db.SaveChanges();
                                    e:;
                                }
                            }
                        }
                    }

                    if (masternotuploaded == "emp no not uploaded : ")
                    {
                        masternotuploaded = "";
                    }
                    ViewBag.masternotuploaded = masternotuploaded;
                }
                else
                {
                    this.ViewBag.Error = "Please Upload Files in .csv format";
                }
            }
            finally
            {
                if (System.IO.File.Exists(path1))
                {
                    System.IO.File.Delete(path1);
                }
            }

            return View();
        }
    }
}