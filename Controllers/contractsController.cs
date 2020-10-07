using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using PagedList;
using HRworks.Models;

namespace HRworks.Controllers
{
    using Microsoft.Ajax.Utilities;

    public class contractsController : Controller
    {
        private HREntities db = new HREntities();

        // GET: contracts
        public void DownloadExcel(string search)
        {
            List<contract> passexel;
            if (search != null)
            {
                passexel = db.contracts.Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).ToList();
            }
            else
            {
                passexel = db.contracts.Include(p=>p.master_file).ToList();
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("CONTRACT");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "designation";
            Sheet.Cells["D1"].Value = "grade";
            Sheet.Cells["E1"].Value = "departmant project";
            Sheet.Cells["F1"].Value = "salary_details";
            Sheet.Cells["G1"].Value = "basic ";
            Sheet.Cells["H1"].Value = "housing allowance";
            Sheet.Cells["I1"].Value = "IBAN";
            Sheet.Cells["J1"].Value = "account_no";
            Sheet.Cells["K1"].Value = "bank_name";
            Sheet.Cells["L1"].Value = "img";
            Sheet.Cells["Q1"].Value = "changed_by";
            Sheet.Cells["R1"].Value = "date_changed";
            int row = 2;
            foreach (var item in passexel)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.master_file.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.designation;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.grade;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.departmant_project;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.salary_details;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.basic;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.housing_allowance;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.transportation_allowance;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.FOT;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.food_allowance;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.living_allowance;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.transportation_allowance;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.others;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.arrears;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.imgpath;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.date_changed.ToString();
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename = CONTRACT.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult Index(string search, int? page, int? pagesize)
        {
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int defaSize = 10;
            if (pagesize != 0)
            {
                defaSize = (pagesize ?? 10);
            }

            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem() { Value="10", Text= "10" },
                new SelectListItem() { Value="15", Text= "15" },
                new SelectListItem() { Value="25", Text= "25" },
                new SelectListItem() { Value="50", Text= "50" },
                new SelectListItem() { Value="100", Text= "100" },
            }; 
            var ab = db.contracts.OrderBy(p => p.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
            var lists = new List<contract>();
            int j = 0;
            ViewBag.pagesize = defaSize;
            IPagedList<contract> passlist = null;
            int i;
            if (ab.Count != 0)
            {
                for (i = 0; i < ab.Count; i++)
                {
                    if (++j != ab.Count())
                    {
                        if (ab[i].employee_no != ab[j].employee_no)
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
            passlist = db.contracts.Include(p=>p.master_file).OrderBy(x => x.employee_id).ToPagedList(pageIndex, defaSize);
            if (!search.IsNullOrWhiteSpace())
            {
                
                lists.RemoveRange(0, lists.Count);
                j = 0;
                int idk;
                if (int.TryParse(search,out idk))
                {
                    ab = db.contracts.Where(x => x.master_file.employee_no.Equals(idk) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                }
                else
                {
                    ab = db.contracts.Where(x => x.master_file.employee_name.Contains(search) /*.Contains(search) /*.StartsWith(search)*/).OrderBy(x => x.master_file.employee_no).ThenBy(x => x.date_changed).ToList();
                }
                if (ab.Count != 0)
                {
                    for (i = 0; i < ab.Count; i++)
                    {
                        if (++j != ab.Count())
                        {
                            if (ab[i].employee_no != ab[j].employee_no)
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
            }
            
            return View(lists.ToPagedList(page ?? 1, defaSize));
        }

        // GET: contracts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contract contract = db.contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            return View(contract);
        }

        // GET: contracts/Create
        [Authorize(Roles = "admin,employee_con")]
        public ActionResult Create()
        {
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0)
                {
                    afinallist.Add(file);
                }

                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    afinallist.Add(file);
                }
            }

            ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(afinallist.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View();
        }

        // POST: contracts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_con")]
        public ActionResult Create([Bind(Include = "con_id,employee_no,designation,grade,departmant_project,salary_details,basic,housing_allowance,transportation_allowance,FOT,food_allowance,living_allowance,ticket_allowance,others,arrears,employee_id")] contract contract, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(contract.employee_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/contract/" + fileexe);
                serverfile = "D:/HR/img/contract/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/contract/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/contract/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            { 
                var img=new contract();
                img.con_id=contract.con_id;
                img.employee_no = contract.employee_no;
                img.designation = contract.designation;
                img.grade = contract.grade;
                img.departmant_project = contract.departmant_project;
                img.salary_details = contract.salary_details;
                img.basic = contract.basic;
                img.housing_allowance = contract.housing_allowance;
                img.transportation_allowance = contract.transportation_allowance;
                img.FOT = contract.FOT;
                img.food_allowance = contract.food_allowance;
                img.living_allowance = contract.living_allowance;
                img.ticket_allowance = contract.ticket_allowance;
                img.others = contract.others;
                img.arrears = contract.arrears;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.contracts.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist= new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count==0)
                {
                    afinallist.Add(file);
                }

                if (!afinallist.Exists(x=>x.employee_no==file.employee_no))
                {
                    afinallist.Add(file);
                }
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(afinallist.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View(contract);
        }

        // GET: contracts/Edit/5
        [Authorize(Roles = "admin,employee_con")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contract contract = db.contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }

            var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0)
                {
                    afinallist.Add(file);
                }

                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    afinallist.Add(file);
                }
            }
            ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no1 = new SelectList(afinallist.OrderBy(e => e.employee_name),"employee_id","employee_name");
            return View(contract);
        }

        // POST: contracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_con")]
        public ActionResult Edit([Bind(Include = "con_id,employee_no,designation,grade,departmant_project,salary_details,basic,housing_allowance,transportation_allowance,FOT,food_allowance,living_allowance,ticket_allowance,others,arrears,employee_id")] contract contract, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var a = db.master_file.Find(contract.employee_no);
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/img/contract/" + fileexe);
                serverfile = "D:/HR/img/contract/" + a.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/img/contract/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/img/contract/" + a.employee_no + "/" + a.employee_no + "_" + i + fileexe));

                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }

            if (ModelState.IsValid)
            {
                var img = new contract();
                img.con_id = contract.con_id;
                img.employee_no = contract.employee_no;
                img.designation = contract.designation;
                img.grade = contract.grade;
                img.departmant_project = contract.departmant_project;
                img.salary_details = contract.salary_details;
                img.basic = contract.basic;
                img.housing_allowance = contract.housing_allowance;
                img.transportation_allowance = contract.transportation_allowance;
                img.FOT = contract.FOT;
                img.food_allowance = contract.food_allowance;
                img.living_allowance = contract.living_allowance;
                img.ticket_allowance = contract.ticket_allowance;
                img.others = contract.others;
                img.arrears = contract.arrears;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.contracts.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var alist = db.master_file.OrderBy(e => e.employee_no).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0)
                {
                    afinallist.Add(file);
                }

                if (!afinallist.Exists(x => x.employee_no == file.employee_no))
                {
                    afinallist.Add(file);
                }
            }

            ViewBag.employee_no = new SelectList(afinallist, "employee_id", "employee_no");
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no1 = new SelectList(afinallist.OrderBy(e => e.employee_name), "employee_id", "employee_name");
            return View(contract);
        }

        // GET: contracts/Delete/5
        [Authorize(Roles = "admin,employee_con")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contract contract = db.contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            return View(contract);
        }

        // POST: contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_con")]
        public ActionResult DeleteConfirmed(int id)
        {
            contract contract = db.contracts.Find(id);
            db.contracts.Remove(contract);
            db.SaveChanges();
            return RedirectToAction("Index");
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
