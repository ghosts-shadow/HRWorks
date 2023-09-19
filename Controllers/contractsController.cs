namespace HRworks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using HRworks.Models;

    using Microsoft.Ajax.Utilities;

    using OfficeOpenXml;

    using PagedList;

    [Authorize(Roles = "super_admin,payrole,employee_con")]
    public class contractsController : Controller
    {
        private const string Purpose = "equalizer";

        private readonly HREntities db = new HREntities();

        public bool seccheck()
        {
            if (Session["IsValidTwoFactorAuthentication"] != null)
            {
                if (!(bool)Session["IsValidTwoFactorAuthentication"])
                {
                    //RedirectToAction("Create", "contractlogins");
                    return false;
                }
            }
            else
            {
                //RedirectToAction("Create", "contractlogins");
                return false;
            }
            return true;
        }

        // GET: contracts/Create
        public ActionResult Create()
        {
            this.ViewBag.gender = new SelectList(this.db.Tables, "gender", "gender");
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            var listItems = new List<ListItem>
            {
                new ListItem {Text = "1", Value = "1"},
                new ListItem {Text = "2", Value = "2"},
                new ListItem {Text = "3", Value = "3"},
                new ListItem {Text = "4C", Value = "4C"},
                new ListItem {Text = "4B", Value = "4B"},
                new ListItem {Text = "4A", Value = "4A"},
                new ListItem {Text = "5B", Value = "5B"},
                new ListItem {Text = "5A", Value = "5A"},
                new ListItem {Text = "6B", Value = "6B"},
                new ListItem {Text = "6A ", Value = "6A"},
                new ListItem {Text = "7B", Value = "7B"},
                new ListItem {Text = "7A", Value = "7A"},
                new ListItem {Text = "8B", Value = "8B"},
                new ListItem {Text = "8A", Value = "8A"},
                new ListItem {Text = "9", Value = "9"}
            };
            this.ViewBag.gradelist = new SelectList(listItems, "Value", "Text");
            this.ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            this.ViewBag.employee_no1 = new SelectList(
                afinallist.OrderBy(e => e.employee_name),
                "employee_id",
                "employee_name");
            return this.View();
        }

        // POST: contracts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,payrole,employee_con")]
        public ActionResult Create(
            [Bind(
                Include =
                    "con_id,employee_no,designation,grade,departmant_project,salary_details,basic,housing_allowance,transportation_allowance,FOT,food_allowance,living_allowance,ticket_allowance,others,arrears,employee_id")]
            contract contract,
            HttpPostedFileBase fileBase)
        {
            string serverfile;
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "1", Value = "1"},
                new ListItem {Text = "2", Value = "2"},
                new ListItem {Text = "3", Value = "3"},
                new ListItem {Text = "4C", Value = "4C"},
                new ListItem {Text = "4B", Value = "4B"},
                new ListItem {Text = "4A", Value = "4A"},
                new ListItem {Text = "5B", Value = "5B"},
                new ListItem {Text = "5A", Value = "5A"},
                new ListItem {Text = "6B", Value = "6B"},
                new ListItem {Text = "6A ", Value = "6A"},
                new ListItem {Text = "7B", Value = "7B"},
                new ListItem {Text = "7A", Value = "7A"},
                new ListItem {Text = "8B", Value = "8B"},
                new ListItem {Text = "8A", Value = "8A"},
                new ListItem {Text = "9", Value = "9"}
            };
            this.ViewBag.gradelist = new SelectList(listItems, "Value", "Text");
            if (fileBase != null)
            {
                var a = this.db.master_file.Find(contract.employee_no);
                var imgname = Path.GetFileName(fileBase.FileName);
                var fileexe = Path.GetExtension(fileBase.FileName);
                var filepath = new DirectoryInfo("D:/HR/img/contract/" + fileexe);
                serverfile = "D:/HR/img/contract/" + a.employee_no; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                var i = 0;
                do
                {
                    serverfile = "D:/HR/img/contract/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                }
                while (System.IO.File.Exists(
                    serverfile = "D:/HR/img/contract/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }

            if (this.ModelState.IsValid)
            {
                var img = new contract();
                img.con_id = contract.con_id;
                img.employee_no = contract.employee_no;
                img.designation = contract.designation;
                img.grade = contract.grade;
                img.departmant_project = contract.departmant_project;
                img.salary_details = this.Protect(contract.salary_details);
                img.basic = this.Protect(contract.basic);
                img.housing_allowance = this.Protect(contract.housing_allowance);
                img.transportation_allowance = this.Protect(contract.transportation_allowance);
                img.FOT = this.Protect(contract.FOT);
                img.food_allowance = this.Protect(contract.food_allowance);
                img.living_allowance = this.Protect(contract.living_allowance);
                img.ticket_allowance = this.Protect(contract.ticket_allowance);
                img.others = this.Protect(contract.others);
                img.arrears = this.Protect(contract.arrears);
                img.imgpath = serverfile;
                img.changed_by = this.User.Identity.Name;
                img.date_changed = DateTime.Now;
                this.db.contracts.Add(img);
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.gender = new SelectList(this.db.Tables, "gender", "gender");
            this.ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            this.ViewBag.employee_no1 = new SelectList(
                afinallist.OrderBy(e => e.employee_name),
                "employee_id",
                "employee_name");
            return this.View(contract);
        }

        // GET: contracts/Delete/5
        [Authorize(Roles = "super_admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contract = this.db.contracts.Find(id);
            if (contract == null) return this.HttpNotFound();

            if (contract.living_allowance != null)
                contract.living_allowance = this.Unprotect(contract.living_allowance);

            if (contract.others != null) contract.others = this.Unprotect(contract.others);

            if (contract.food_allowance != null) contract.food_allowance = this.Unprotect(contract.food_allowance);

            if (contract.transportation_allowance != null)
                contract.transportation_allowance = this.Unprotect(contract.transportation_allowance);

            if (contract.ticket_allowance != null)
                contract.ticket_allowance = this.Unprotect(contract.ticket_allowance);

            if (contract.arrears != null) contract.arrears = this.Unprotect(contract.arrears);

            if (contract.housing_allowance != null)
                contract.housing_allowance = this.Unprotect(contract.housing_allowance);

            if (contract.basic != null) contract.basic = this.Unprotect(contract.basic);

            if (contract.salary_details != null) contract.salary_details = this.Unprotect(contract.salary_details);

            if (contract.FOT != null) contract.FOT = this.Unprotect(contract.FOT);
            return this.View(contract);
        }

        // POST: contracts/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var contract = this.db.contracts.Find(id);
            this.db.contracts.Remove(contract);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        // GET: contracts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contract = this.db.contracts.Find(id);
            if (contract == null) return this.HttpNotFound();

            if (contract.living_allowance != null)
                contract.living_allowance = this.Unprotect(contract.living_allowance);

            if (contract.others != null) contract.others = this.Unprotect(contract.others);

            if (contract.food_allowance != null) contract.food_allowance = this.Unprotect(contract.food_allowance);

            if (contract.transportation_allowance != null)
                contract.transportation_allowance = this.Unprotect(contract.transportation_allowance);

            if (contract.ticket_allowance != null)
                contract.ticket_allowance = this.Unprotect(contract.ticket_allowance);

            if (contract.arrears != null) contract.arrears = this.Unprotect(contract.arrears);

            if (contract.housing_allowance != null)
                contract.housing_allowance = this.Unprotect(contract.housing_allowance);

            if (contract.basic != null) contract.basic = this.Unprotect(contract.basic);

            if (contract.salary_details != null) contract.salary_details = this.Unprotect(contract.salary_details);

            if (contract.FOT != null) contract.FOT = this.Unprotect(contract.FOT);
            return this.View(contract);
        }

        // GET: contracts
        public void DownloadExcel(string search)
        {
            List<contract> passexel;
            if (search != null)
                passexel = this.db.contracts.Where(
                        x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/)
                    .ToList();
            else passexel = this.db.contracts.Include(p => p.master_file).ToList();
            var Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("CONTRACT");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "designation";
            Sheet.Cells["D1"].Value = "grade";
            Sheet.Cells["E1"].Value = "departmant project";
            Sheet.Cells["F1"].Value = "gross";
            Sheet.Cells["G1"].Value = "basic ";
            Sheet.Cells["H1"].Value = "housing allowance";
            Sheet.Cells["I1"].Value = "transportation allowance";
            Sheet.Cells["J1"].Value = "FOT";
            Sheet.Cells["K1"].Value = "food_allowance";
            Sheet.Cells["L1"].Value = "living_allowance";
            Sheet.Cells["M1"].Value = "others";
            Sheet.Cells["N1"].Value = "arrears";
            Sheet.Cells["O1"].Value = "img path";
            Sheet.Cells["P1"].Value = "changed by";
            Sheet.Cells["Q1"].Value = "date changed";
            var row = 2;
            foreach (var item in passexel)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.designation;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.grade;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.departmant_project;
                Sheet.Cells[string.Format("F{0}", row)].Value = this.Unprotect(item.salary_details);
                Sheet.Cells[string.Format("G{0}", row)].Value = this.Unprotect(item.basic);
                Sheet.Cells[string.Format("H{0}", row)].Value = this.Unprotect(item.housing_allowance);
                Sheet.Cells[string.Format("I{0}", row)].Value = this.Unprotect(item.transportation_allowance);
                Sheet.Cells[string.Format("J{0}", row)].Value = this.Unprotect(item.FOT);
                Sheet.Cells[string.Format("K{0}", row)].Value = this.Unprotect(item.food_allowance);
                Sheet.Cells[string.Format("L{0}", row)].Value = this.Unprotect(item.living_allowance);
                Sheet.Cells[string.Format("M{0}", row)].Value = this.Unprotect(item.others);
                Sheet.Cells[string.Format("N{0}", row)].Value = this.Unprotect(item.arrears);
                Sheet.Cells[string.Format("O{0}", row)].Value = item.imgpath;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.date_changed.ToString();
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            this.Response.Clear();
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("content-disposition", "filename = CONTRACT.xlsx");
            this.Response.BinaryWrite(Ep.GetAsByteArray());
            this.Response.End();
        }

        // GET: contracts/Edit/5
        [Authorize(Roles = "super_admin,admin,payrole,employee_con")]
        public ActionResult Edit(int? id)
        {
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "1", Value = "1"},
                new ListItem {Text = "2", Value = "2"},
                new ListItem {Text = "3", Value = "3"},
                new ListItem {Text = "4C", Value = "4C"},
                new ListItem {Text = "4B", Value = "4B"},
                new ListItem {Text = "4A", Value = "4A"},
                new ListItem {Text = "5B", Value = "5B"},
                new ListItem {Text = "5A", Value = "5A"},
                new ListItem {Text = "6B", Value = "6B"},
                new ListItem {Text = "6A ", Value = "6A"},
                new ListItem {Text = "7B", Value = "7B"},
                new ListItem {Text = "7A", Value = "7A"},
                new ListItem {Text = "8B", Value = "8B"},
                new ListItem {Text = "8A", Value = "8A"},
                new ListItem {Text = "9", Value = "9"}
            };
            this.ViewBag.gradelist = new SelectList(listItems, "Value", "Text");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contract = this.db.contracts.Find(id);
            if (contract == null) return this.HttpNotFound();

            if (contract.living_allowance != null)
                contract.living_allowance = this.Unprotect(contract.living_allowance);

            if (contract.others != null) contract.others = this.Unprotect(contract.others);

            if (contract.food_allowance != null) contract.food_allowance = this.Unprotect(contract.food_allowance);

            if (contract.transportation_allowance != null)
                contract.transportation_allowance = this.Unprotect(contract.transportation_allowance);

            if (contract.ticket_allowance != null)
                contract.ticket_allowance = this.Unprotect(contract.ticket_allowance);

            if (contract.arrears != null) contract.arrears = this.Unprotect(contract.arrears);

            if (contract.housing_allowance != null)
                contract.housing_allowance = this.Unprotect(contract.housing_allowance);

            if (contract.basic != null) contract.basic = this.Unprotect(contract.basic);

            if (contract.salary_details != null) contract.salary_details = this.Unprotect(contract.salary_details);

            if (contract.FOT != null) contract.FOT = this.Unprotect(contract.FOT);
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            this.ViewBag.gender = new SelectList(this.db.Tables, "gender", "gender");
            this.ViewBag.employee_no1 = new SelectList(
                afinallist.OrderBy(e => e.employee_name),
                "employee_id",
                "employee_name");
            return this.View(contract);
        }

        // POST: contracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super_admin,admin,payrole,employee_con")]
        public ActionResult Edit(
            contract contract,
            HttpPostedFileBase fileBase)
        {
            string serverfile = null;
            var listItems = new List<ListItem>
            {
                new ListItem {Text = "1", Value = "1"},
                new ListItem {Text = "2", Value = "2"},
                new ListItem {Text = "3", Value = "3"},
                new ListItem {Text = "4C", Value = "4C"},
                new ListItem {Text = "4B", Value = "4B"},
                new ListItem {Text = "4A", Value = "4A"},
                new ListItem {Text = "5B", Value = "5B"},
                new ListItem {Text = "5A", Value = "5A"},
                new ListItem {Text = "6B", Value = "6B"},
                new ListItem {Text = "6A ", Value = "6A"},
                new ListItem {Text = "7B", Value = "7B"},
                new ListItem {Text = "7A", Value = "7A"},
                new ListItem {Text = "8B", Value = "8B"},
                new ListItem {Text = "8A", Value = "8A"},
                new ListItem {Text = "9", Value = "9"}
            };
            this.ViewBag.gradelist = new SelectList(listItems, "Value", "Text");
            if (fileBase != null)
            {
                var a = this.db.master_file.Find(contract.employee_no);
                var imgname = Path.GetFileName(fileBase.FileName);
                var fileexe = Path.GetExtension(fileBase.FileName);
                var filepath = new DirectoryInfo("D:/HR/img/contract/" + fileexe);
                serverfile = "D:/HR/img/contract/" + a.employee_no; /*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                var i = 0;
                do
                {
                    serverfile = "D:/HR/img/contract/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                }
                while (System.IO.File.Exists(
                    serverfile = "D:/HR/img/contract/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }/*
            else
            {
                serverfile = null;
                var imglist = db.contracts.ToList().FindAll(x => x.employee_no == contract.employee_no)
                    .OrderByDescending(x => x.date_changed).ToList();
                var imgpath = imglist.FindAll(c => c.imgpath != null).OrderByDescending(x => x.date_changed).ToList();
                if (imgpath.Count != 0)
                {
                    var aserverfile = imgpath.First();
                    if (aserverfile != null)
                    {
                        serverfile = aserverfile.imgpath;
                    }
                }
                else
                {
                    serverfile = null;
                }
            }*/

            if (this.ModelState.IsValid)
            {
                var img = new contract();

                img.con_id = contract.con_id;
                img.employee_id = contract.employee_id;
                img.employee_no = contract.employee_no;
                img.designation = contract.designation;
                img.grade = contract.grade;
                img.departmant_project = contract.departmant_project;
                img.salary_details = this.Protect(contract.salary_details);
                img.basic = this.Protect(contract.basic);
                img.housing_allowance = this.Protect(contract.housing_allowance);
                img.transportation_allowance = this.Protect(contract.transportation_allowance);
                img.FOT = this.Protect(contract.FOT);
                img.food_allowance = this.Protect(contract.food_allowance);
                img.living_allowance = this.Protect(contract.living_allowance);
                img.ticket_allowance = this.Protect(contract.ticket_allowance);
                img.others = this.Protect(contract.others);
                img.arrears = this.Protect(contract.arrears);
                if (serverfile != null)
                {
                    img.imgpath = serverfile;
                }
                else
                {
                    img.imgpath = contract.imgpath;
                }
                img.changed_by = this.User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.Entry(img).State = EntityState.Modified;
                db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            var alist = this.db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            this.ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            this.ViewBag.gender = new SelectList(this.db.Tables, "gender", "gender");
            this.ViewBag.employee_no1 = new SelectList(
                afinallist.OrderBy(e => e.employee_name),
                "employee_id",
                "employee_name");
            return this.View(contract);
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
                            var pro = new contract();
                            foreach (DataRow dr in dt.Rows)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (dr[column] == null || dr[column].ToString() == " ") goto e;

                                    if (column.ColumnName == "designation") pro.designation = dr[column].ToString();
                                    if (column.ColumnName == "grade") pro.grade = dr[column].ToString();
                                    if (column.ColumnName == "department/project")
                                        pro.departmant_project = dr[column].ToString();
                                    if (column.ColumnName == "salary_details")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.salary_details = dtt;
                                    }

                                    if (column.ColumnName == "basic")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.basic = dtt;
                                    }

                                    if (column.ColumnName == "housing_allowance")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.housing_allowance = dtt;
                                    }

                                    if (column.ColumnName == "transportation_allowance")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.transportation_allowance = dtt;
                                    }

                                    if (column.ColumnName == "FOT")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.FOT = dtt;
                                    }

                                    if (column.ColumnName == "food_allowance")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.food_allowance = dtt;
                                    }

                                    if (column.ColumnName == "living_allowance")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.living_allowance = dtt;
                                    }

                                    if (column.ColumnName == "ticket_allowance")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.ticket_allowance = dtt;
                                    }

                                    if (column.ColumnName == "others")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.others = dtt;
                                    }

                                    if (column.ColumnName == "arrears")
                                    {
                                        var dtt = this.Protect(dr[column].ToString());
                                        pro.arrears = dtt;
                                    }

                                    if (column.ColumnName == "employee_no")
                                    {
                                        int.TryParse(dr[column].ToString(), out var idm);
                                        if (idm != 0)
                                        {
                                            var epid = afinallist.Find(x => x.employee_no == idm);
                                            if (epid == null) goto e;
                                            pro.employee_no = epid.employee_id;
                                        }
                                    }
                                }

                                this.db.contracts.Add(pro);
                                this.db.SaveChanges();
                                e: ;
                            }
                        }
                    }
                    else if (extension == ".xls" || extension == ".xlsx")
                    {
                        using (var package = new ExcelPackage(new FileInfo(path1)))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                            var dt = new DataTable();

                            for (int row = 1; row <= worksheet.Dimension.Rows; row++)
                            {
                                if (row == 1) 
                                {
                                    foreach (var cell in worksheet.Cells[row, 1, row, worksheet.Dimension.Columns])
                                    {
                                        dt.Columns.Add(cell.Text);
                                    }
                                }
                                else
                                {
                                    var newRow = dt.NewRow();
                                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                                    {
                                        newRow[col - 1] = worksheet.Cells[row, col].Text;
                                    }
                                    dt.Rows.Add(newRow);
                                }
                            }

                            this.ViewBag.Data = dt;
                            if (dt.Rows.Count > 0)
                            {
                                var leavecheck = this.db.contracts.ToList();
                                var pro = new contract();
                                foreach (DataRow dr in dt.Rows)
                                {
                                    foreach (DataColumn column in dt.Columns)
                                    {
                                        if (dr[column] == null || dr[column].ToString() == " ") goto e;

                                        if (column.ColumnName == "designation") pro.designation = dr[column].ToString();
                                        if (column.ColumnName == "grade") pro.grade = dr[column].ToString();
                                        if (column.ColumnName == "department/project")
                                            pro.departmant_project = dr[column].ToString();
                                        if (column.ColumnName == "salary_details")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.salary_details = dtt;
                                        }

                                        if (column.ColumnName == "basic")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.basic = dtt;
                                        }

                                        if (column.ColumnName == "housing_allowance")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.housing_allowance = dtt;
                                        }

                                        if (column.ColumnName == "transportation_allowance")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.transportation_allowance = dtt;
                                        }

                                        if (column.ColumnName == "FOT")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.FOT = dtt;
                                        }

                                        if (column.ColumnName == "food_allowance")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.food_allowance = dtt;
                                        }

                                        if (column.ColumnName == "living_allowance")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.living_allowance = dtt;
                                        }

                                        if (column.ColumnName == "ticket_allowance")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.ticket_allowance = dtt;
                                        }

                                        if (column.ColumnName == "others")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.others = dtt;
                                        }

                                        if (column.ColumnName == "arrears")
                                        {
                                            var dtt = this.Protect(dr[column].ToString());
                                            pro.arrears = dtt;
                                        }

                                        if (column.ColumnName == "employee_no")
                                        {
                                            int.TryParse(dr[column].ToString(), out var idm);
                                            if (idm != 0)
                                            {
                                                var epid = afinallist.Find(x => x.employee_no == idm);
                                                if (epid == null) goto e;
                                                pro.employee_no = epid.employee_id;
                                            }
                                        }
                                    }

                                    this.db.contracts.Add(pro);
                                    this.db.SaveChanges();
                                e:;
                                }
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

        public ActionResult Index(string search, int? page, int? pagesize)
        {
            if (!seccheck())
            {
                return RedirectToAction("Index", "Home");
            }
            var pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var defaSize = 10;
            if (pagesize != 0) defaSize = pagesize ?? 10;

            this.ViewBag.PageSize = new List<SelectListItem>
                                        {
                                            new SelectListItem { Value = "10", Text = "10" },
                                            new SelectListItem { Value = "15", Text = "15" },
                                            new SelectListItem { Value = "25", Text = "25" },
                                            new SelectListItem { Value = "50", Text = "50" },
                                            new SelectListItem { Value = "100", Text = "100" }
                                        };
            var ab = this.db.contracts.OrderBy(p => p.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
            var lists = new List<contract>();
            var j = 0;
            this.ViewBag.pagesize = defaSize;
            IPagedList<contract> passlist = null;
            int i;
            if (ab.Count != 0)
            {
                for (i = 0; i < ab.Count; i++)
                    if (++j != ab.Count())
                        if (ab[i].employee_no != ab[j].employee_no)
                            lists.Add(ab[i]);
                if (ab.Count != 1)
                    if (ab[--j] != ab[i = i - 2])
                        lists.Add(ab[j]);

                if (lists.Count == 0)
                {
                    i -= 1;
                    lists.Add(ab[i]);
                }
            }

            passlist = this.db.contracts.Include(p => p.master_file).OrderBy(x => x.employee_id)
                .ToPagedList(pageIndex, defaSize);
            if (!search.IsNullOrWhiteSpace())
            {
                lists.RemoveRange(0, lists.Count);
                j = 0;
                int idk;
                if (int.TryParse(search, out idk))
                    ab = this.db.contracts
                        .Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                else
                    ab = this.db.contracts
                        .Where(
                            x => x.master_file.employee_name
                                .Contains(search) /*.Contains(search) /*.StartsWith(search)*/)
                        .OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                if (ab.Count != 0)
                {
                    for (i = 0; i < ab.Count; i++)
                        if (++j != ab.Count())
                            if (ab[i].employee_no != ab[j].employee_no)
                                lists.Add(ab[i]);
                    if (ab.Count != 1)
                        if (ab[--j] != ab[i = i - 2])
                            lists.Add(ab[j]);

                    if (lists.Count == 0)
                    {
                        i -= 1;
                        lists.Add(ab[i]);
                    }
                }

                foreach (var contract in lists)
                {
                    if (contract.living_allowance != null)
                        contract.living_allowance = this.Unprotect(contract.living_allowance);
                    if (contract.others != null) contract.others = this.Unprotect(contract.others);
                    if (contract.food_allowance != null)
                        contract.food_allowance = this.Unprotect(contract.food_allowance);
                    if (contract.transportation_allowance != null)
                        contract.transportation_allowance = this.Unprotect(contract.transportation_allowance);
                    if (contract.ticket_allowance != null)
                        contract.ticket_allowance = this.Unprotect(contract.ticket_allowance);
                    if (contract.arrears != null) contract.arrears = this.Unprotect(contract.arrears);
                    if (contract.housing_allowance != null)
                        contract.housing_allowance = this.Unprotect(contract.housing_allowance);
                    if (contract.basic != null) contract.basic = this.Unprotect(contract.basic);
                    if (contract.salary_details != null)
                        contract.salary_details = this.Unprotect(contract.salary_details);
                    if (contract.FOT != null) contract.FOT = this.Unprotect(contract.FOT);
                }

                return this.View(lists.ToPagedList(page ?? 1, defaSize));
            }

            foreach (var contract in lists)
            {
                if (contract.living_allowance != null)
                    contract.living_allowance = this.Unprotect(contract.living_allowance);

                if (contract.others != null) contract.others = this.Unprotect(contract.others);

                if (contract.food_allowance != null) contract.food_allowance = this.Unprotect(contract.food_allowance);

                if (contract.transportation_allowance != null)
                    contract.transportation_allowance = this.Unprotect(contract.transportation_allowance);

                if (contract.ticket_allowance != null)
                    contract.ticket_allowance = this.Unprotect(contract.ticket_allowance);

                if (contract.arrears != null) contract.arrears = this.Unprotect(contract.arrears);

                if (contract.housing_allowance != null)
                    contract.housing_allowance = this.Unprotect(contract.housing_allowance);

                if (contract.basic != null) contract.basic = this.Unprotect(contract.basic);

                if (contract.salary_details != null) contract.salary_details = this.Unprotect(contract.salary_details);

                if (contract.FOT != null) contract.FOT = this.Unprotect(contract.FOT);
            }

            return this.View(lists.ToPagedList(page ?? 1, defaSize));
        }

        public string Protect(string unprotectedText)
        {
            if(unprotectedText.IsNullOrWhiteSpace())
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

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}