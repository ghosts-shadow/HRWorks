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
    public class insurancesController : Controller
    {
        private HREntities db = new HREntities();

        // GET: insurances
        public void DownloadExcel(string search)
        {
            List<insurance> passexel;
            if (search != null)
            {
                passexel = db.insurances.Where(x => x.master_file.employee_name.StartsWith(search)).ToList();
            }
            else
            {
                passexel = db.insurances.Include(p => p.master_file).ToList();
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("INSURANCE");
            Sheet.Cells["A1"].Value = "employee no";
            Sheet.Cells["B1"].Value = "employee name";
            Sheet.Cells["C1"].Value = "card_no";
            Sheet.Cells["D1"].Value = "dob";
            Sheet.Cells["E1"].Value = "age";
            Sheet.Cells["F1"].Value = "gender";
            Sheet.Cells["G1"].Value = "dependency";
            Sheet.Cells["H1"].Value = "marital_status";
            Sheet.Cells["I1"].Value = "nationality";
            Sheet.Cells["J1"].Value = "eid_no";
            Sheet.Cells["K1"].Value = "uid_no";
            Sheet.Cells["L1"].Value = "pasport_no";
            Sheet.Cells["M1"].Value = "emitae_visa_issue";
            Sheet.Cells["N1"].Value = "annual_primium";
            Sheet.Cells["O1"].Value = "deletion_date";
            Sheet.Cells["P1"].Value = "invoice_no";
            Sheet.Cells["Q1"].Value = "credit_amt";
            Sheet.Cells["R1"].Value = "imgpath";
            Sheet.Cells["S1"].Value = "changed_by";
            Sheet.Cells["T1"].Value = "date_changed";
            int row = 2;
            foreach (var item in passexel)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.employee_no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.master_file.employee_name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.card_no;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.dob;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.age;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.gender;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.dependency;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.marital_status;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.nationality;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.eid_no;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.uid_no;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.pasport_no;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.emitae_visa_issue;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.annual_primium;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.deletion_date;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.invoice_no;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.credit_amt;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.imgpath;
                Sheet.Cells[string.Format("S{0}", row)].Value = item.changed_by;
                Sheet.Cells[string.Format("T{0}", row)].Value = item.date_changed.ToString();
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "filename =insurance.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult Index(string search, int? page, int? pagesize)
        {/*
            var insurances = db.insurances.Include(i => i.master_file);
            return View(insurances.ToList());*/
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
            ViewBag.psize = defaSize;
            IPagedList<insurance> passlist = null;
            passlist = db.insurances.OrderBy(x => x.employee_id).ToPagedList(pageIndex, defaSize);
            var ab = db.insurances.OrderBy(p => p.employee_no).ToList();
            var lists = new List<insurance>();
            int j = 0;
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
            if (search != null)
            {
                j = 0;
                ab = db.insurances.Where(x => x.master_file.employee_name.StartsWith(search)).ToList();
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
            }
            return View(lists);
        }

        // GET: insurances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            insurance insurance = db.insurances.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // GET: insurances/Create
        [Authorize(Roles = "admin,employee_INC")]
        public ActionResult Create()
        {
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View();
        }

        // POST: insurances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_INC")]
        public ActionResult Create([Bind(Include = "employee_id,employee_no,card_no,dob,age,gender,dependency,marital_status,nationality,eid_no,pasport_no,uid_no,emitae_visa_issue,annual_primium,deletion_date,invoice_no,credit_amt")] insurance insurance, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/insurance/" + fileexe);
                serverfile = "D:/HR/insurance/" + insurance.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile);
                int i = 0;
                do
                {
                    serverfile = "D:/HR/insurance/" + insurance.employee_no + "/" + insurance.employee_no + "_" + i +fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/insurance/" + insurance.employee_no + "/" + insurance.employee_no + "_" + i +fileexe));
                    fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new insurance();
                img.employee_no=insurance.employee_no;
                img.card_no = insurance.card_no;
                img.dob = insurance.master_file.dob;
                img.age = insurance.age;
                img.gender = insurance.master_file.gender;
                img.dependency = insurance.dependency;
                img.marital_status = insurance.marital_status;
                img.nationality = insurance.master_file.nationality;
                img.eid_no = insurance.eid_no;
                img.pasport_no = insurance.pasport_no;
                img.uid_no = insurance.uid_no;
                img.emitae_visa_issue = insurance.emitae_visa_issue;
                img.annual_primium = insurance.annual_primium;
                img.deletion_date = insurance.deletion_date;
                img.invoice_no = insurance.invoice_no;
                img.credit_amt = insurance.credit_amt;
                img.imgpath = serverfile;
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.insurances.Add(img);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(insurance);
        }

        // GET: insurances/Edit/5
        [Authorize(Roles = "admin,employee_INC")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            insurance insurance = db.insurances.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(insurance);
        }

        // POST: insurances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_INC")]
        public ActionResult Edit([Bind(Include = "id,employee_no,card_no,dob,age,gender,dependency,marital_status,nationality,eid_no,pasport_no,uid_no,emitae_visa_issue,annual_primium,deletion_date,invoice_no,credit_amt")] insurance insurance, HttpPostedFileBase fileBase)
        {
            string serverfile;
            if (fileBase != null)
            {
                var imgname = System.IO.Path.GetFileName(fileBase.FileName);
                var fileexe = System.IO.Path.GetExtension(fileBase.FileName);
                DirectoryInfo filepath = new DirectoryInfo("D:/HR/insurance/" + fileexe);
                serverfile = "D:/HR/insurance/" + insurance.employee_no;/*+ "/"+ passport.employee_no + fileexe;*/
                filepath = Directory.CreateDirectory(serverfile); int i = 0;
                do
                {
                    serverfile = "D:/HR/insurance/" + insurance.employee_no + "/" + insurance.employee_no + "_" + i + fileexe;
                    i++;
                } while (System.IO.File.Exists(serverfile = "D:/HR/insurance/" + insurance.employee_no + "/" + insurance.employee_no + "_" + i + fileexe));
                fileBase.SaveAs(serverfile);
            }
            else
            {
                serverfile = null;
            }
            if (ModelState.IsValid)
            {
                var img = new insurance();
                img.employee_no = insurance.employee_no;
                img.card_no = insurance.card_no;
                img.dob = insurance.master_file.dob;
                img.age = insurance.age;
                img.gender = insurance.master_file.gender;
                img.dependency = insurance.dependency;
                img.marital_status = insurance.marital_status;
                img.nationality = insurance.master_file.nationality;
                img.eid_no = insurance.eid_no;
                img.pasport_no = insurance.pasport_no;
                img.uid_no = insurance.uid_no;
                img.emitae_visa_issue = insurance.emitae_visa_issue;
                img.annual_primium = insurance.annual_primium;
                img.deletion_date = insurance.deletion_date;
                img.invoice_no = insurance.invoice_no;
                img.credit_amt = insurance.credit_amt;
                img.imgpath = serverfile;
                db.insurances.Add(img);
                img.changed_by = User.Identity.Name;
                img.date_changed = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.gender = new SelectList(db.Tables, "gender", "gender");
            ViewBag.employee_no = new SelectList(db.master_file, "employee_id", "employee_no");
            ViewBag.employee_no1 = new SelectList(db.master_file, "employee_id", "employee_name");
            return View(insurance);
        }

        // GET: insurances/Delete/5
        [Authorize(Roles = "admin,employee_INC")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            insurance insurance = db.insurances.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // POST: insurances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,employee_INC")]
        public ActionResult DeleteConfirmed(int id)
        {
            insurance insurance = db.insurances.Find(id);
            db.insurances.Remove(insurance);
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
