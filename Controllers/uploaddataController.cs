using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRworks.Models;
using OfficeOpenXml;

namespace HRworks.Controllers
{
    [Authorize]
    public class uploaddataController : Controller
    {
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
                            var pro = new  insurance();
                            foreach (DataRow dr in dt.Rows)
                            {
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
    }
}