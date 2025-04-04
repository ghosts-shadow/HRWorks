using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using PagedList;
using HRworks.Models;
using Microsoft.Ajax.Utilities;

namespace HRworks.Controllers
{
    using System.Web.Routing;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    [Authorize]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null || filterContext.HttpContext.Request.Url.Host
                != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new {controller = "Home", action = "Logout", area = "Main"}));
            }
        }
    }
    [Authorize]
    public class master_fileController : Controller
    {
        private HREntities db = new HREntities();

        // GET: master_file
        public ActionResult statusupdate()
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            foreach (var file in afinallist)
            {
                if (file.labour_card.Count() != 0 && file.visas.Count() != 0)
                {
                    file.status = "active";
                    var leavelist = db.Leaves.ToList();
                    var leaveflist = leavelist.FindAll(x =>
                        x.actual_return_date == null && x.Employee_id == file.employee_id);
                    foreach (var leaf in leaveflist)
                    {
                        if (leaf.Start_leave <= DateTime.Now.Date && DateTime.Now.Date <= leaf.End_leave)
                        {
                            file.status = "on leave";
                        }
                    }
                }
                else
                {
                    file.status = "in process";
                }

                if (file.last_working_day != null)
                {
                    file.status = "inactive";
                }

                this.db.Entry(file).State = EntityState.Modified;
                this.db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public List<master_file> emplist()
        {

            var prealist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList().GroupBy(x => x.employee_no).Select(s => s.First());
            var alist = prealist
                .Where(e => e.last_working_day == null)
                .ToList();

            var afinallist = alist
                .GroupBy(x => x.employee_no)
                .Select(g => g.First())
                .Where(file => file.employee_no != 0 && file.employee_no != 1 && file.employee_no != 100001)
                .ToList();

            return afinallist;
        }

        public void DownloadExcel(string search)
        {
            List<master_file> passexel;

            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            if (search != null)
            {
                int idk;
                if (int.TryParse(search, out idk))
                {
                    passexel = afinallist.FindAll(x => x.employee_no.Equals(idk)).ToList();
                }
                else
                {
                    passexel = afinallist.FindAll(x => x.employee_name.Contains(search)).ToList();
                }

            }
            else
            {
                passexel = afinallist;
            }

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("employee details");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "nationality";
            Sheet.Cells["D1"].Value = "dob";
            Sheet.Cells["E1"].Value = "date_joined";
            Sheet.Cells["F1"].Value = "last_working_day";
            Sheet.Cells["G1"].Value = "gender";
            Sheet.Cells["H1"].Value = "EID no";
            Sheet.Cells["I1"].Value = "EID expiry";
            Sheet.Cells["J1"].Value = "IBAN";
            Sheet.Cells["K1"].Value = "bank name";
            Sheet.Cells["L1"].Value = "Card no";
            Sheet.Cells["M1"].Value = "Age";
            Sheet.Cells["N1"].Value = "Dependency";
            Sheet.Cells["O1"].Value = "Marital Status";
            Sheet.Cells["P1"].Value = "Annual Primium";
            Sheet.Cells["Q1"].Value = "Deletion Date";
            Sheet.Cells["R1"].Value = "Invoice No";
            Sheet.Cells["S1"].Value = "credit amount";
            Sheet.Cells["T1"].Value = "company_code";
            Sheet.Cells["U1"].Value = "Passport No";
            Sheet.Cells["V1"].Value = "Passport Expiry";
            Sheet.Cells["W1"].Value = "Passport Issue Date";
            Sheet.Cells["X1"].Value = "Passport Return Date";
            Sheet.Cells["Y1"].Value = "Passport Remarks";
            Sheet.Cells["Z1"].Value = "status";
            Sheet.Cells["AA1"].Value = "uid no";
            Sheet.Cells["AB1"].Value = "file no";
            Sheet.Cells["AC1"].Value = "place of issue";
            Sheet.Cells["AD1"].Value = "accompanied by";
            Sheet.Cells["AE1"].Value = "rv expiry";
            Sheet.Cells["AF1"].Value = "rv issue";
            Sheet.Cells["AG1"].Value = "proff as per visa";
            Sheet.Cells["AH1"].Value = "work_permit_no";
            Sheet.Cells["AI1"].Value = "personal_no";
            Sheet.Cells["AJ1"].Value = "proffession";
            Sheet.Cells["AK1"].Value = "lc_expiry";
            Sheet.Cells["AL1"].Value = "establishment";
            Sheet.Cells["AM1"].Value = "status";
            Sheet.Cells["AN1"].Value = "changed by";
            Sheet.Cells["AO1"].Value = "date changed";
            Sheet.Cells["AP1"].Value = "img";
            Sheet.Cells["AQ1"].Value = "Departmant/Project";
            Sheet.Cells["AR1"].Value = "Designation";
            int row = 2;
            foreach (var item in passexel.OrderBy(x=>x.employee_no))
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.nationality;
                if (item.dob != null)
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.dob.Value.ToString("dd MMM yyyy");
                if (item.date_joined != null)
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.date_joined.Value.ToString("dd MMM yyyy");
                if (item.last_working_day != null)
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.last_working_day.Value.ToString("dd MMM yyyy");
                Sheet.Cells[string.Format("G{0}", row)].Value = item.gender;
                if (item.emirates_id.Count != 0)
                {
                    var em = item.emirates_id.OrderByDescending(x => x.date_changed).First();
                    Sheet.Cells[string.Format("H{0}", row)].Value = em.eid_no.ToString("D");
                    Sheet.Cells[string.Format("I{0}", row)].Value = em.eid_expiry.ToString("dd MMM yyyy");
                }

                if (item.bank_details.Count != 0)
                {
                    var bk = item.bank_details.OrderByDescending(x => x.Employee_Id).First();
                    Sheet.Cells[string.Format("J{0}", row)].Value = bk.IBAN;
                    Sheet.Cells[string.Format("K{0}", row)].Value = bk.bank_name;
                }

                if (item.insurances.Count != 0)
                {
                    var inc = item.insurances.OrderByDescending(x => x.employee_id).First();
                    if (inc.card_no != null)
                        Sheet.Cells[string.Format("L{0}", row)].Value = inc.card_no.Value.ToString("D");
                    Sheet.Cells[string.Format("M{0}", row)].Value = inc.age;
                    Sheet.Cells[string.Format("N{0}", row)].Value = inc.dependency;
                    Sheet.Cells[string.Format("O{0}", row)].Value = inc.marital_status;
                    Sheet.Cells[string.Format("P{0}", row)].Value = inc.annual_primium;
                    if (inc.deletion_date != null)
                        Sheet.Cells[string.Format("Q{0}", row)].Value =
                            inc.deletion_date.Value.ToString("dd MMM yyyy");
                    if (inc.invoice_no != null)
                        Sheet.Cells[string.Format("R{0}", row)].Value = inc.invoice_no.Value.ToString("D");
                    if (inc.credit_amt != null)
                        Sheet.Cells[string.Format("S{0}", row)].Value = inc.credit_amt.Value.ToString("F");
                }

                if (item.passports.Count != 0)
                {
                    var pass = item.passports.OrderByDescending(x => x.employee_id).First();
                    Sheet.Cells[string.Format("T{0}", row)].Value = pass.company_code;
                    Sheet.Cells[string.Format("U{0}", row)].Value = pass.passport_no;
                    if (pass.passport_expiry != null)
                        Sheet.Cells[string.Format("V{0}", row)].Value =
                            pass.passport_expiry.Value.ToString("dd MMM yyyy");
                    if (pass.passport_issue_date != null)
                        Sheet.Cells[string.Format("W{0}", row)].Value =
                            pass.passport_issue_date.Value.ToString("dd MMM yyyy");
                    if (pass.passport_return_date != null)
                        Sheet.Cells[string.Format("X{0}", row)].Value =
                            pass.passport_return_date.Value.ToString("dd MMM yyyy");
                    Sheet.Cells[string.Format("Y{0}", row)].Value = pass.passport_remarks;
                    Sheet.Cells[string.Format("Z{0}", row)].Value = pass.status;
                }

                if (item.visas.Count != 0)
                {
                    var VI = item.visas.OrderByDescending(x => x.employee_id).First();

                    if (VI.uid_no != null)
                        Sheet.Cells[string.Format("AA{0}", row)].Value = VI.uid_no.Value.ToString("D");
                    if (VI.file_no != null)
                        Sheet.Cells[string.Format("AB{0}", row)].Value = VI.file_no.Value.ToString("D");
                    Sheet.Cells[string.Format("AC{0}", row)].Value = VI.place_of_issue;
                    Sheet.Cells[string.Format("AD{0}", row)].Value = VI.accompanied_by;
                    if (VI.rv_expiry != null)
                        Sheet.Cells[string.Format("AE{0}", row)].Value = VI.rv_expiry.Value.ToString("dd MMM yyyy");
                    if (VI.rv_issue != null)
                        Sheet.Cells[string.Format("AF{0}", row)].Value = VI.rv_issue.Value.ToString("dd MMM yyyy");
                    Sheet.Cells[string.Format("AG{0}", row)].Value = VI.proff_as_per_visa;
                }

                if (item.labour_card.Count != 0)
                {
                    var la = item.labour_card.OrderByDescending(x => x.employee_id).First();
                    if (la.work_permit_no != null)
                        Sheet.Cells[string.Format("AH{0}", row)].Value = la.work_permit_no.Value.ToString("D");
                    if (la.personal_no != null)
                        Sheet.Cells[string.Format("AI{0}", row)].Value = la.personal_no.Value.ToString("D");
                    Sheet.Cells[string.Format("AJ{0}", row)].Value = la.proffession;
                    if (la.lc_expiry != null)
                        Sheet.Cells[string.Format("AK{0}", row)].Value = la.lc_expiry.Value.ToString("dd MMM yyyy");
                    Sheet.Cells[string.Format("AL{0}", row)].Value = la.establishment;
                }

                Sheet.Cells[string.Format("AM{0}", row)].Value = item.status;
                Sheet.Cells[string.Format("AN{0}", row)].Value = item.changed_by;
                if (item.date_changed != null)
                    Sheet.Cells[string.Format("AO{0}", row)].Value =
                        item.date_changed.Value.ToString("dd MMM yyyy");
                Sheet.Cells[string.Format("AP{0}", row)].Value = item.img;
                if (item.contracts.Count != 0)
                {
                    var con = item.contracts.OrderByDescending(x => x.employee_id).First();
                    Sheet.Cells[string.Format("AQ{0}", row)].Value = con.departmant_project;
                    Sheet.Cells[string.Format("AR{0}", row)].Value = con.designation;
                }
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename =EMPLOYEE_DETAILS.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        public ActionResult Index(string search, int? page, int? pagesize, int? idsearch)
        {
            //            return View(db.master_file.ToList());
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize = 100;

            if (pagesize != 0)
            {
                int a = 100;
                if (pagesize != null)
                {
                    if (pagesize != 0)
                    {
                        a = (int) pagesize;
                    }
                }

                defaSize = a;
            }

            ViewBag.pagesize = defaSize;
            IPagedList<master_file> passlist = null;
            var ab = db.master_file.OrderBy(p => p.employee_no).ThenBy(x => x.date_changed).ToList();
            var lists = new List<master_file>();
            int j = 0;
            passlist = db.master_file.OrderBy(x => x.employee_id).ToPagedList(pageIndex, defaSize);
            int i;
            if (ab.Count != 0)
            {
                for (i = 0; i < ab.Count; i++)
                {
                    if (++j != ab.Count())
                    {
                        if (ab[i].employee_no == ab[j].employee_no)
                        {
                            continue;
                        }
                        else
                        {
                            lists.Add(ab[i]);
                        }
                    }
                }

                if (ab.Count != 1)
                {
                    if (ab[--j] != ab[i = i - 2])
                    {
                        lists.Add(ab[j]);
                    }
                }

                if (lists.Count == 0)
                {
                    i -= 1;
                    lists.Add(ab[i]);
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                lists.RemoveRange(0, lists.Count);
                j = 0;
                int idk;
                if (int.TryParse(search, out idk))
                {
                    ab = db.master_file
                        .Where(x => x.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.employee_no).ThenBy(x => x.date_changed).ToList();

                    int emiidNum = 0;
                    int searchNum = 0;
                    ab.AddRange(db.master_file
                        .AsEnumerable() // Moves data processing to memory
                        .Where(x => !x.emiid.IsNullOrWhiteSpace() &&
                                    int.TryParse(x.emiid.Substring(2), out emiidNum) &&
                                    int.TryParse(search, out searchNum) &&
                                    emiidNum == searchNum)
                        .OrderBy(x => x.employee_no)
                        .ThenBy(x => x.date_changed)
                        .ToList());
                }
                else
                {
                    if (search.ToLower().Contains("g-"))
                    {
                        int emiidNum = 0;
                        int searchNum = 0;
                        ab = db.master_file
                            .AsEnumerable() // Moves data processing to memory
                            .Where(x =>!x.emiid.IsNullOrWhiteSpace()&&
                                int.TryParse(x.emiid.Substring(2), out  emiidNum) &&
                                int.TryParse(search.Substring(2), out  searchNum) &&
                                emiidNum == searchNum)
                            .OrderBy(x => x.employee_no)
                            .ThenBy(x => x.date_changed)
                            .ToList();
                    }
                    else
                    {
                        ab = db.master_file
                            .Where(x => x.employee_name.ToLower().Contains(search) /*.Contains(search) /*.StartsWith(search)*/)
                            .OrderBy(x => x.employee_no).ThenBy(x => x.date_changed).ToList();
                    }
                }

                if (ab.Count != 0)
                {
                    for (i = 0; i < ab.Count; i++)
                    {
                        if (++j != ab.Count())
                        {
                            if (ab[i].employee_no == ab[j].employee_no)
                            {
                                continue;
                            }
                            else
                            {
                                lists.Add(ab[i]);
                            }
                        }
                    }

                    if (ab.Count != 1)
                    {
                        if (ab[--j] != ab[i = i - 2])
                        {
                            lists.Add(ab[j]);
                        }
                    }

                    if (lists.Count == 0)
                    {
                        i -= 1;

                        lists.Add(ab[i]);
                    }
                }

                return View(lists.ToPagedList(page ?? 1, defaSize));
//
//                return View(.ToPagedList(page ?? 1, defaSize));
            }

            return View(lists.ToPagedList(page ?? 1, defaSize));
        }


        // GET: master_file/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            master_file master_file = db.master_file.Find(id);
            if (master_file == null)
            {
                return HttpNotFound();
            }

            return View(master_file);
        }

        // GET: master_file/Create
        [Authorize(Roles = "super_admin,admin,payrole")]
        public ActionResult Create()
        {
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");

            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Citiscape", Value = "1" },
                new ListItem { Text = "Grove", Value = "2" }
            };
            this.ViewBag.company = new SelectList(listItems, "Value", "Text");
            return View();
        }

        // POST: master_file/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin")]
        public ActionResult Create([Bind(Include =
                "employee_no,employee_name,nationality,dob,date_joined,last_working_day,gender,IBAN,account_no,bank_name,img,id,emiid,company")]
            master_file master_file, HttpPostedFileBase fileBase)
        {
            //            if (ModelState.IsValid)
            //            {
            //                db.master_file.Add(master_file);
            //                db.SaveChanges();
            //                return RedirectToAction("Index");
            //            }
            string serverfile;
            if (fileBase != null)
            {
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/masterfile/" + fileexe);
                var empno = master_file.emiid;
                if (master_file.company == "2")
                {
                    empno = "G-"+ master_file.emiid;
                    
                }
                serverfile =
                    "D:/HR/img/masterfile/" + empno; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/masterfile/" + empno + "/" + empno +
                                 "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/masterfile/" + empno + "/" +
                                                            empno + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }

            if (ModelState.IsValid)
            {
                var current = DateTime.Now;
                var img = new master_file();
                int.TryParse(master_file.emiid, out var emno);
                if (master_file.company == "2")
                {
                    emno += 77700000;
                    img.emiid = $"G-{emno:D4}";
                }
                else
                {
                    img.emiid = emno.ToString();
                }
                    img.employee_no = emno;

                img.employee_name = master_file.employee_name;
                img.nationality = master_file.nationality;
                img.dob = master_file.dob;
                img.date_joined = master_file.date_joined;
                img.last_working_day = master_file.last_working_day;
                img.gender = master_file.gender;
                img.changed_by = User.Identity.Name;
                img.date_changed = current;
                img.status = "in process";
                img.img = serverfile;
                db.master_file.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Citiscape", Value = "1" },
                new ListItem { Text = "Grove", Value = "2" }
            };
            this.ViewBag.company = new SelectList(listItems, "Value", "Text");

            return View(master_file);
        }

        // GET: master_file/Edit/5
        [Authorize(Roles = "super_admin,admin,payrole")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            master_file master_file = db.master_file.Find(id);
            if (master_file == null)
            {
                return HttpNotFound();
            }

            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Citiscape", Value = "1" },
                new ListItem { Text = "Grove", Value = "2" }
            };
            this.ViewBag.company = new SelectList(listItems, "Value", "Text");
            if (!master_file.emiid.IsNullOrWhiteSpace() && master_file.emiid.Contains("G-"))
            {

                master_file.company = "Grove";
            }
            else
            {
                master_file.company = "Citiscape";
            }
            return View(master_file);
        }

        // POST: master_file/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,payrole")]
        public ActionResult Edit([Bind(Include =
                "employee_id,employee_no,employee_name,nationality,dob,date_joined,last_working_day,gender,IBAN,account_no,bank_name,img,id,status,emiid,company")]
            master_file master_file, HttpPostedFileBase fileBase)
        {
            var imglist = this.db.master_file.ToList();
            string serverfile;
            var img = imglist.Find(x => x.employee_id == master_file.employee_id);
            if (fileBase != null)
            {
                var empno = master_file.emiid;
                if (master_file.company == "2")
                {
                    empno = "G-" + master_file.emiid;

                }
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/masterfile/" + fileexe);
                serverfile =
                    "D:/HR/img/masterfile/" + empno; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/masterfile/" + empno + "/" + empno +
                                 "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/masterfile/" + empno + "/" +
                                                            empno + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                if (img.img != null)
                {
                    serverfile = img.img;
                }
                else
                {
                    serverfile = null;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    /*var img = new master_file();
    //                img.emirates_id = master_file.emirates_id;
    //                img.employee_no = master_file.employee_no;
    //                img.employee_name = master_file.employee_name;
    //                img.nationality = master_file.nationality;
    //                img.dob = master_file.dob;
    //                img.date_joined = master_file.date_joined;
    //                img.last_working_day = master_file.last_working_day;
    //                img.gender = master_file.gender;
                    master_file.img = serverfile;
                    master_file.img = serverfile;
                    master_file.changed_by = User.Identity.Name;
                    master_file.date_changed = DateTime.Now;
                    db.master_file.Add(master_file);*/
                    var current = DateTime.Now;
                    int.TryParse(master_file.emiid, out var empno);
                    if (master_file.company.ToLower() == "grove")
                    {
                        img.emiid = $"G-{empno:D4}";
                        empno = empno + 77700000;
                        if (master_file.employee_no.ToString().Contains("777"))
                        {
                            img.employee_no = empno;
                        }
                    }
                    else
                    {
                        img.employee_no = empno;
                    }

                    img.employee_name = master_file.employee_name;
                    img.nationality = master_file.nationality;
                    img.dob = master_file.dob;
                    img.date_joined = master_file.date_joined;
                    img.last_working_day = master_file.last_working_day;
                    img.gender = master_file.gender;
                    img.changed_by = User.Identity.Name;
                    img.date_changed = current;
                    if (!master_file.status.IsNullOrWhiteSpace())
                    {
                        img.status = master_file.status;
                    }
                    else
                    {
                        if (img.labour_card.Count != 0 || img.labour_card.Count != 0)
                        {
                            img.status = "active";
                        }
                    }

                    img.img = serverfile;

                    this.db.Entry(img).State = EntityState.Modified;
                    this.db.SaveChanges();
                    /*
                    db.master_file.Add(img);
                    db.SaveChanges();*/
                    return RedirectToAction("Index");
                } /*
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    return Content(e.ToString());
                    throw;
                }*/
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine(
                            "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                ve.PropertyName,
                                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                ve.ErrorMessage);
                        }
                    }

                    return Content(e.ToString());
                    throw;
                }
            }

            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no = new SelectList(db.emirates_id, "id", "id", master_file.employee_no);
            var listItems = new List<ListItem>
            {
                new ListItem { Text = "Citiscape", Value = "1" },
                new ListItem { Text = "Grove", Value = "2" }
            };
            this.ViewBag.company = new SelectList(listItems, "Value", "Text");
            return View(master_file);
        }

        // GET: master_file/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            master_file master_file = db.master_file.Find(id);
            if (master_file == null)
            {
                return HttpNotFound();
            }

            return View(master_file);
        }

        // POST: master_file/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            master_file master_file = db.master_file.Find(id);
            db.master_file.Remove(master_file);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult report(DateTime? from, DateTime? to, string status)
        {
            var afinallist = new List<master_file>();
            var afinallist2 = new List<master_file>();
            var alist = db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                    afinallist.Add(file);
            }

            if (from.HasValue && to.HasValue)
            {
                ViewBag.from = from.Value.ToString("d");
                ViewBag.to = to.Value.ToString("d");
                ;

                if (status == "inactive")
                {
                    foreach (var file in afinallist)
                    {
                        if (file.status == "inactive" && file.last_working_day > from && file.last_working_day < to)
                        {
                            afinallist2.Add(file);
                        }
                    }
                }

                if (status == "joining")
                {
                    foreach (var file in afinallist)
                    {
                        if (file.date_joined > from && file.date_joined < to)
                        {
                            afinallist2.Add(file);
                        }
                    }
                }

                return View(afinallist2);
            }

            return View(afinallist);
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