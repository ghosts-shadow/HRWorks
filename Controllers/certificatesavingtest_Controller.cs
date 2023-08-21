using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HRworks.Models;
using iText;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using SautinSoft.Document;
using GemBox.Pdf;
using GemBox.Pdf.Content;
using Microsoft.Ajax.Utilities;
using Paragraph = iText.Layout.Element.Paragraph;
using PdfDocument = GemBox.Pdf.PdfDocument;
using PdfLoadOptions = SautinSoft.Document.PdfLoadOptions;
using MimeKit;
using MailKit.Net.Smtp;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace HRworks.Controllers
{
    [Authorize]
    public class certificatesavingtest_Controller : Controller
    {
        private HREntities db = new HREntities();
        private const string Purpose = "equalizer";

        // GET: certificatesavingtest_
        public ActionResult Index()
        {
            var certificatesavingtest_ = db.certificatesavingtest_.Include(c => c.master_file);
            return View(certificatesavingtest_.OrderByDescending(x=>x.Id).ToList());
        }

        public ActionResult hrapproval() 
        {
            var certificatesavingtest = db.certificatesavingtest_.Where(x=>x.status.Contains("new")).ToList();
            return View(certificatesavingtest);
        }
        
        public ActionResult Approve(int id) 
        {
            var cstvar = db.certificatesavingtest_.ToList().Find(x=>x.Id == id);
            cstvar.status = "approved";
            db.Entry(cstvar).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("hrapproval");
        }

        public ActionResult reject(int id,string message)
        {
            var cstvar = db.certificatesavingtest_.ToList().Find(x => x.Id == id);
            cstvar.status = "rejected for: "+ message; 
            db.Entry(cstvar).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("hrapproval");
        }

        public string Unprotect(string protectedText)
        {
            var protectedBytes = Convert.FromBase64String(protectedText);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
            var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
            return unprotectedText;
        }
        // GET: certificatesavingtest_/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                goto end;
            }
            certificatesavingtest_ certificatesavingtest_ = db.certificatesavingtest_.Find(id);
            if (certificatesavingtest_ == null)
            {
                goto end;
            }

            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            var masterfile = afinallist.Find(x => x.employee_id == certificatesavingtest_.employee_id);
            var passport = db.passports.ToList().OrderByDescending(x => x.date_changed).ToList()
                .Find(y => y.employee_id == masterfile.employee_id);
            var contract = db.contracts.ToList().OrderByDescending(x => x.date_changed).ToList()
                .Find(y => y.employee_no == masterfile.employee_id);
            var nameinarlist = db.detailsinarabics.ToList();
            var nameinar = nameinarlist.Find(x => x.employee_id == certificatesavingtest_.employee_id);
            if(nameinar == null)
            {
                nameinar.ARname = "";
                nameinar.ARnationality = "";
                nameinar.ARposition = "";
            }
            var banklist = db.bank_details.ToList();
            var bankvar = banklist.Find(x => x.employee_no == certificatesavingtest_.employee_id);

            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            if (certificatesavingtest_.certificate_type == 1)
            {
                string filepath = Server.MapPath("~/arabic certificates/Salary Certificate - Arabic - Male.pdf"); 
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Male.pdf");
                var csorgr = "CS-HRDCRT-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                     filepath = Server.MapPath("~/arabic certificates/Salary Certificate - Arabic - Male - Grove.pdf");
                     filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Male - Grove.pdf");
                    csorgr = "GR-HRDCRT-";
                }
                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {
                        
                            formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                            formattedText.TextAlignment = PdfTextAlignment.Right;
                            formattedText.Language = new PdfLanguage("Arabic");
                            formattedText.FontSize = 15.17;
                            formattedText.Append(nameinar.ARname);
                            document.Pages[0].Content.DrawText(formattedText,new PdfPoint(420,488));
                            formattedText.Clear();
                            formattedText.Append(nameinar.ARnationality);
                            document.Pages[0].Content.DrawText(formattedText,new PdfPoint(420,458));
                            formattedText.Clear();
                            formattedText.Append(nameinar.ARposition);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 398));

                    }

                    using (var formattedText = new PdfFormattedText())
                    {

                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        formattedText.Append(masterfile.date_joined.Value.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 338));
                        formattedText.Clear();
                        if (passport != null)
                        {
                            formattedText.Append(passport.passport_no);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 428));
                            formattedText.Clear();
                        }

                        if (contract != null)
                        {
                            formattedText.Append(Unprotect(contract.salary_details));
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 368));
                            formattedText.Clear();
                        }

                        formattedText.Append(certificatesavingtest_.destination);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(522, 633));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(114, 684));

                    }

                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = csorgr + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(65, 672));
                        formattedText.Clear();
                    }
                    document.Save(filepathout);
                    document.Close();
                }
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    if (certificatesavingtest_.cs_gr == "GR_certificates")
                    {
                        Response.AddHeader("Content-Disposition",
                            "attachment;filename=\"Salary Certificate - Arabic - Male - Grove.pdf\"");
                        Response.BinaryWrite(System.IO.File.ReadAllBytes(
                            Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Male - Grove.pdf")));
                    }
                    else
                    {
                        Response.AddHeader("Content-Disposition",
                            "attachment;filename=\"Salary Certificate - Arabic - Male.pdf\"");
                        Response.BinaryWrite(System.IO.File.ReadAllBytes(
                            Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Male.pdf")));
                    }

                    Response.Flush();
                    Response.End();
            }
            if (certificatesavingtest_.certificate_type == 2)
            {
                    string filepath = Server.MapPath("~/arabic certificates/Salary Certificate - Arabic - Female.pdf");
                    string filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Female.pdf");
                    var csorgr = "CS-HRDCRT-";
                    if (certificatesavingtest_.cs_gr == "GR_certificates")
                    {
                        filepath = Server.MapPath("~/arabic certificates/Salary Certificate - Arabic - Female.pdf");
                        filepathout =
                            Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Female.pdf");
                         csorgr = "GR-HRDCRT-";
                    }

                    ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {
                        
                            formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                            formattedText.TextAlignment = PdfTextAlignment.Right;
                            formattedText.Language = new PdfLanguage("Arabic");
                            formattedText.FontSize = 15.17;
                            formattedText.Append(nameinar.ARname);
                            document.Pages[0].Content.DrawText(formattedText,new PdfPoint(420,518));
                            formattedText.Clear();
                            formattedText.Append(nameinar.ARnationality);
                            document.Pages[0].Content.DrawText(formattedText,new PdfPoint(420,456));
                            formattedText.Clear();
                            formattedText.Append(nameinar.ARposition);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 422));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        formattedText.Append(masterfile.date_joined.Value.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 357));
                        formattedText.Clear();
                        if (passport != null)
                        {
                            formattedText.Append(passport.passport_no);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 485));
                            formattedText.Clear();
                        }

                        if (contract != null)
                        {
                        formattedText.Append(Unprotect(contract.salary_details));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 390));
                        formattedText.Clear();
                            
                        }
                        formattedText.Append(certificatesavingtest_.destination);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(515, 706));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(115, 706));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = csorgr + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(65, 695));
                        formattedText.Clear();
                    }
                    document.Save(filepathout);
                    document.Close();
                }
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    if (certificatesavingtest_.cs_gr == "GR_certificates")
                    {
                        Response.AddHeader("Content-Disposition",
                            "attachment;filename=\"Salary Certificate - Arabic - Female.pdf\"");
                        Response.BinaryWrite(System.IO.File.ReadAllBytes(
                            Server.MapPath(
                                "~/arabic certificates/tempstore/Salary Certificate - Arabic - Female.pdf")));
                    }
                    else
                    {
                        Response.AddHeader("Content-Disposition",
                            "attachment;filename=\"Salary Certificate - Arabic - Female.pdf\"");
                        Response.BinaryWrite(System.IO.File.ReadAllBytes(
                            Server.MapPath(
                                "~/arabic certificates/tempstore/Salary Certificate - Arabic - Female.pdf")));
                    }
                    Response.Flush();
                    Response.End();
            }
            if (certificatesavingtest_.certificate_type == 3)
            {
                    string filepath = Server.MapPath("~/arabic certificates/Employment Certificate - Arabic - Male.pdf");
                    string filepathout = Server.MapPath("~/arabic certificates/tempstore/Employment Certificate - Arabic - Male.pdf");
                var csorgr = "CS-HRDCRT-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                    {
                        filepath =
                            Server.MapPath("~/arabic certificates/Employment Certificate - Arabic - Male - Grove.pdf");
                        filepathout =
                            Server.MapPath(
                                "~/arabic certificates/tempstore/Employment Certificate - Arabic - Male - Grove.pdf");
                    csorgr = "GR-HRDCRT-";
                }

                    ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {
                        
                            formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                            formattedText.TextAlignment = PdfTextAlignment.Right;
                            formattedText.Language = new PdfLanguage("Arabic");
                            formattedText.FontSize = 15.17;
                            formattedText.Append(nameinar.ARname);
                            document.Pages[0].Content.DrawText(formattedText,new PdfPoint(420,513));
                            formattedText.Clear();
                            formattedText.Append(nameinar.ARnationality);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 487));
                            formattedText.Clear();
                            formattedText.Append(nameinar.ARposition);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 432));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        formattedText.Append(certificatesavingtest_.destination);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(465, 721));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(100, 721));
                        formattedText.Clear();
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText,new PdfPoint(420,461));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = csorgr + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(50, 704));
                        formattedText.Clear();
                    }
                    document.Save(filepathout);
                    document.Close();
                }
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"Employment Certificate - Arabic - Male - Grove.pdf\"");
                    Response.BinaryWrite(System.IO.File.ReadAllBytes(
                            Server.MapPath(
                                "~/arabic certificates/tempstore/Employment Certificate - Arabic - Male - Grove.pdf")));

                    }
                    else
                {
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"Employment Certificate - Arabic - Male.pdf\"");
                    Response.BinaryWrite(System.IO.File.ReadAllBytes(
                            Server.MapPath(
                                "~/arabic certificates/tempstore/Employment Certificate - Arabic - Male.pdf")));
                    }

                    Response.Flush();
                    Response.End();
            }
            if (certificatesavingtest_.certificate_type == 4)
            {
                    string filepath = Server.MapPath("~/arabic certificates/Employment Certificate - Arabic - female.pdf");
                    string filepathout = Server.MapPath("~/arabic certificates/tempstore/Employment Certificate - Arabic - Female.pdf");
                var csorgr = "CS-HRDCRT-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                    {
                        filepath = Server.MapPath("~/arabic certificates/Employment Certificate - Arabic - female - Grove.pdf");
                        filepathout = Server.MapPath( "~/arabic certificates/tempstore/Employment Certificate - Arabic - Female - Grove.pdf");
                    csorgr = "GR-HRDCRT-";
                }

                    ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {
                        
                            formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                            formattedText.TextAlignment = PdfTextAlignment.Right;
                            formattedText.Language = new PdfLanguage("Arabic");
                            formattedText.FontSize = 15.17;
                            formattedText.Append(nameinar.ARname);
                            document.Pages[0].Content.DrawText(formattedText,new PdfPoint(420,490));
                            formattedText.Clear();
                            formattedText.Append(nameinar.ARnationality);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 457));
                            formattedText.Clear();
                            formattedText.Append(nameinar.ARposition);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 386));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        formattedText.Append(certificatesavingtest_.destination);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(475, 657));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(100, 665));
                        formattedText.Clear();
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText,new PdfPoint(420,423));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = csorgr + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(50, 653));
                        formattedText.Clear();
                    }
                    document.Save(filepathout);
                    document.Close();
                }
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    if (certificatesavingtest_.cs_gr == "GR_certificates")
                    {
                        Response.AddHeader("Content-Disposition",
                            "attachment;filename=\"Employment Certificate - Arabic - female - Grove.pdf\"");
                        Response.BinaryWrite(System.IO.File.ReadAllBytes(
                            Server.MapPath(
                                "~/arabic certificates/tempstore/Employment Certificate - Arabic - female - Grove.pdf")));
                    }
                    else
                {
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"Employment Certificate - Arabic - female.pdf\"");
                    Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Employment Certificate - Arabic - female.pdf")));

                }
                    Response.Flush();
                    Response.End();
            }
            if (certificatesavingtest_.certificate_type == 5)
            {
                string filepath = Server.MapPath("~/arabic certificates/Salary Certificate.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate.pdf");
                var csorgr = "CS-HRDCRT-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Salary Certificate - Grove.pdf");
                    filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Grove.pdf");
                    csorgr = "GR-HRDCRT-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(152, 728));
                        formattedText.Clear();
                        var refstr = csorgr + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(150, 710));
                        formattedText.Clear();
                        formattedText.Append(certificatesavingtest_.destination);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(100, 675));
                        formattedText.Clear();
                        formattedText.Append(masterfile.employee_name);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(160, 528));
                        formattedText.Clear();
                        formattedText.Append(masterfile.nationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(160, 508));
                        formattedText.Clear();
                        if (passport != null)
                        {
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(160, 483));
                        formattedText.Clear();
                        }

                        if (contract != null)
                        {
                        formattedText.Append(contract.designation);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(160, 455));
                        formattedText.Clear();
                        formattedText.Append(Unprotect(contract.salary_details));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(192, 431));
                        formattedText.Clear();
                        }
                        formattedText.Append(masterfile.date_joined.Value.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(160, 410));

                    }
                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"Salary Certificate - Grove.pdf\"");
                    Response.BinaryWrite(
                        System.IO.File.ReadAllBytes(
                            Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Grove.pdf")));
                }
                else
                {
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Salary Certificate.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Salary Certificate.pdf")));
                }

                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 6)
            {
                string filepath = Server.MapPath("~/arabic certificates/Employment Certificate.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Employment Certificate.pdf");
                var csorgr = "CS-HRDCRT-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Employment Certificate - Grove.pdf");
                    filepathout = Server.MapPath("~/arabic certificates/tempstore/Employment Certificate - Grove.pdf");
                    csorgr = "GR-HRDCRT-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(175, 722));
                        formattedText.Clear();
                        var refstr = csorgr + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(175, 705));
                        formattedText.Clear();
                        formattedText.Append(masterfile.employee_name);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(172, 528));
                        formattedText.Clear();
                        formattedText.Append(masterfile.nationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(172, 508));
                        formattedText.Clear();
                        if (passport != null)
                        {
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(172, 485));
                        formattedText.Clear();
                            
                        }

                        if (contract != null)
                        {
                        formattedText.Append(contract.designation);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(172, 465));
                        formattedText.Clear();
                        }
                        formattedText.Append(masterfile.date_joined.Value.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(200, 445));

                    }
                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"Employment Certificate - Grove.pdf\"");
                    Response.BinaryWrite(System.IO.File.ReadAllBytes(
                        Server.MapPath("~/arabic certificates/tempstore/Employment Certificate - Grove.pdf")));
                }
                else
                {
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Employment Certificate.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Employment Certificate.pdf")));
                    
                }

                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 7)
            {
                string filepath = Server.MapPath("~/arabic certificates/Letter to Immigration - Male.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Letter to Immigration - Male.pdf");
                var csorgr = "CS-HRDLTR-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Letter to Immigration - Male - Grove .pdf");
                    filepathout = Server.MapPath("~/arabic certificates/tempstore/Letter to Immigration - Male - Grove .pdf"); 
                    csorgr = "GR-HRDLTR-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {

                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("Arabic");
                        formattedText.FontSize = 15.17;
                        formattedText.Append(nameinar.ARname);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 518));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARnationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 490));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARposition);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 434));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        if (passport != null)
                        {
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 464));
                       formattedText.Clear();
                            
                        }

                        if (contract != null)
                        {
                        formattedText.Append( Unprotect(contract.salary_details));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 404));
                        formattedText.Clear();
                            
                        }
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(107, 726));

                    }

                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(55, 715));
                        formattedText.Clear();
                    }

                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Letter to Immigration - Male.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 8)
            {
                string filepath = Server.MapPath("~/arabic certificates/Letter to Immigration - Female.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Letter to Immigration - Female.pdf");
                var csorgr = "CS-HRDLTR-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Letter to Immigration - Female - Grove.pdf");
                    filepathout =
                        Server.MapPath("~/arabic certificates/tempstore/Letter to Immigration - Female - Grove.pdf");
                    csorgr = "GR-HRDLTR-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {

                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("Arabic");
                        formattedText.FontSize = 15.17;
                        formattedText.Append(nameinar.ARname);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 472));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARnationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 447));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARposition);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 424));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        if (contract != null )
                        {
                        formattedText.Append( Unprotect(contract.salary_details));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 378));
                        formattedText.Clear();
                                    
                        }
                        if (passport != null )
                        {
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 400));
                       formattedText.Clear();
                                    
                        }
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(100, 701));

                    }

                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = csorgr + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(50, 692));
                        formattedText.Clear();
                    }

                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Letter to Immigration - Female.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 9)
            {
                string filepath = Server.MapPath("~/arabic certificates/Driving License - Light - Male.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Driving License - Light - Male.pdf");
                var csorgr = "CS-HRDLTR-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Driving License - Light - Male - Grove.pdf");
                    filepathout =
                        Server.MapPath("~/arabic certificates/tempstore/Driving License - Light - Male - Grove.pdf");
                    csorgr = "GR-HRDLTR-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {

                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("Arabic");
                        formattedText.FontSize = 15.17;
                        formattedText.Append(nameinar.ARname);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 531));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARnationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 507));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 481));
                       formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(120, 741));

                    }

                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(71, 729));
                        formattedText.Clear();
                    }

                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Driving License - Light - Male.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 10)
            {
                string filepath = Server.MapPath("~/arabic certificates/Driving License - Light - Female.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Driving License - Light - Female.pdf");
                var csorgr = "CS-HRDLTR-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Driving License - Light - Female - Grove.pdf");
                    filepathout =
                        Server.MapPath("~/arabic certificates/tempstore/Driving License - Light - Female - Grove.pdf");
                    csorgr = "GR-HRDLTR-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {

                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("Arabic");
                        formattedText.FontSize = 15.17;
                        formattedText.Append(nameinar.ARname);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 506));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARnationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 482));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 456));
                       formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(115, 731));

                    }

                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(67, 719));
                        formattedText.Clear();
                    }

                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Driving License - Light - Female.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 11)
            {
                string filepath = Server.MapPath("~/arabic certificates/Driving Licence - Heavy - Male.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Driving Licence - Heavy - Male.pdf");
                var csorgr = "CS-HRDLTR-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Driving Licence - Heavy - Male - Grove.pdf");
                    filepathout =
                        Server.MapPath("~/arabic certificates/tempstore/Driving Licence - Heavy - Male - Grove.pdf");
                    csorgr = "GR-HRDLTR-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {

                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("Arabic");
                        formattedText.FontSize = 15.17;
                        formattedText.Append(nameinar.ARname);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 480));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARnationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 456));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 436));
                       formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(105, 710));

                    }

                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(55, 700));
                        formattedText.Clear();
                    }

                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Driving Licence - Heavy - Male.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 13)
            {
                string filepath = Server.MapPath("~/arabic certificates/EXPERIENCE CERTIFICATE - english.pdf");
                string filepathout =
                    Server.MapPath("~/arabic certificates/tempstore/EXPERIENCE CERTIFICATE - english.pdf");
                var csorgr = "CS-HRDCRT-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/EXPERIENCE CERTIFICATE - english - Grove.pdf");
                    filepathout =
                        Server.MapPath("~/arabic certificates/tempstore/EXPERIENCE CERTIFICATE - english - Grove.pdf");
                    csorgr = "GR-HRDCRT-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 695));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 715));
                        formattedText.Clear();
                        formattedText.Append(masterfile.employee_name);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(170, 505));
                        formattedText.Clear();
                        formattedText.Append(masterfile.nationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(170, 480));
                        formattedText.Clear();
                        if (contract != null)
                        {
                        formattedText.Append(contract.designation);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(170, 453));
                        formattedText.Clear();
                            
                        }
                        formattedText.Append(masterfile.date_joined.Value.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(210, 426));
                        formattedText.Clear();
                        var enddatewof = certificatesavingtest_.destination;
                        var srtinglenth = enddatewof.Length;
                        var temp = 0;
                        if (srtinglenth > enddatewof.IndexOf("@"))
                        {
                            temp = enddatewof.IndexOf("@") + 1;
                            var endate = Convert.ToDateTime(enddatewof.Substring(temp));
                            formattedText.Append(endate.ToString("dd MMM yy"));
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(335, 426));
                            formattedText.Clear();
                        }
                    }

                    document.Save(filepathout);
                    document.Close();
                }

                {

                    /*
            DocumentCore dc = DocumentCore.Load(filepath, new PdfLoadOptions());
                ContentRange cr = dc.Content.Find("@date").First();
                if (cr != null)
                {
                    cr.Replace(DateTime.Today.ToString("dd MMM yy"));
                }
                cr = dc.Content.Find("@ref").First();
                if (cr != null)
                {
                    cr.Replace(certificatesavingtest_.Id.ToString("D"));
                }
                cr = dc.Content.Find("@name").First();
                if (cr != null)
                {
                    cr.Replace(masterfile.employee_name);
                }
                cr = dc.Content.Find("@nat").First();
                if (cr != null)
                {
                    cr.Replace(masterfile.nationality);
                }
                cr = dc.Content.Find("@pos").First();
                if (cr != null)
                {
                    cr.Replace(contract.designation);
                }
                cr = dc.Content.Find("@from").First();
                if (cr != null)
                {
                    cr.Replace(masterfile.date_joined.Value.ToString("dd MMM yy"));
                }
                cr = dc.Content.Find("@to").First();
                if (cr != null)
                {
                    var enddatewof = certificatesavingtest_.destination;
                    var srtinglenth = enddatewof.Length;
                    var temp = 0;
                    if (srtinglenth > enddatewof.IndexOf("@"))
                    {
                        temp = enddatewof.IndexOf("@")+1;
                        var endate = Convert.ToDateTime(enddatewof.Substring(temp));
                        cr.Replace(endate.ToString("dd MMM yy"));
                    }

                }
                dc.Save(filepathout);*/
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"EXPERIENCE CERTIFICATE.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 14)
            {
                string filepath = Server.MapPath("~/arabic certificates/Termination Notice.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Termination Notice.pdf");
                var csorgr = "CS-HRDLTR-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Termination Notice - Grove.pdf");
                    filepathout = Server.MapPath("~/arabic certificates/tempstore/Termination Notice - Grove.pdf");
                    csorgr = "GR-HRDLTR-";
                }

                using (var document = PdfDocument.Load(filepath))
                {
                    ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 658));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 638));
                        formattedText.Clear();
                        formattedText.Append(masterfile.employee_name);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 613));
                        formattedText.Clear();
                        if (contract != null)
                        {
                        formattedText.Append(contract.designation);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 587));
                        formattedText.Clear();
                        formattedText.Append(contract.departmant_project);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 562));
                        formattedText.Clear();
                            
                        }
                        var enddatewof = certificatesavingtest_.destination;
                        var srtinglenth = enddatewof.Length;
                        var temp = 0;
                        if (srtinglenth > enddatewof.IndexOf("@"))
                        {
                            temp = enddatewof.IndexOf("@") + 1;
                            var endate = Convert.ToDateTime(enddatewof.Substring(temp));
                            formattedText.Append(endate.ToString("dd MMM yy"));
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 433));
                            formattedText.Clear();
                        }

                        temp = 0;
                        if (srtinglenth > enddatewof.IndexOf("@"))
                        {
                            temp = enddatewof.IndexOf("@");
                            var reason = enddatewof.Remove(temp);
                            formattedText.Append(reason);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 460));
                            formattedText.Clear();
                        }
                    }

                    document.Save(filepathout);
                    document.Close();
                }

                {
                    /*DocumentCore dc = DocumentCore.Load(filepath, new PdfLoadOptions());
                    ContentRange cr = dc.Content.Find("@date").First();
                    if (cr != null)
                    {
                        cr.Replace(DateTime.Today.ToString("dd MMM yy"));
                    }

                    cr = dc.Content.Find("@name").First();
                    if (cr != null)
                    {
                        cr.Replace(masterfile.employee_name);
                    }

                    cr = dc.Content.Find("@pos").First();
                    if (cr != null)
                    {
                        cr.Replace(contract.designation);
                    }

                    cr = dc.Content.Find("@dep").First();
                    if (cr != null)
                    {
                        cr.Replace(contract.departmant_project);
                    }

                    cr = dc.Content.Find("@reason").First();
                    if (cr != null)
                    {
                        var enddatewof = certificatesavingtest_.destination;
                        var temp = enddatewof.IndexOf("@");
                        var reason = enddatewof.Remove(temp);
                        cr.Replace(reason);
                    }

                    cr = dc.Content.Find("@efdate").First();
                    if (cr != null)
                    {
                        var enddatewof = certificatesavingtest_.destination;
                        var temp = enddatewof.IndexOf("@") + 1;
                        var temp2 = enddatewof.Substring(temp);
                        var endate = Convert.ToDateTime(temp2);
                        cr.Replace(endate.ToString("dd MMM yy"));
                    }
                dc.Save(filepathout);*/
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Termination Notice.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 15)
            {
                string filepath = Server.MapPath("~/arabic certificates/Resignation Notice Acceptance.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Resignation Notice Acceptance.pdf");
                var csorgr = "CS-HRDLTR-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                     filepath = Server.MapPath("~/arabic certificates/Resignation Notice Acceptance - Grove.pdf");
                     filepathout =
                        Server.MapPath("~/arabic certificates/tempstore/Resignation Notice Acceptance - Grove.pdf");
                    csorgr = "GR-HRDLTR-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    var page = document.Pages[0];
                    double margins = 72;
                    double maxWidth = page.CropBox.Width - margins * 2;
                    double maxHeight = page.CropBox.Height - margins * 2;
                    double heightOffset = maxHeight + margins;
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Calibri");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 12.48;
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(130, 553));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(130, 535));
                        formattedText.Clear();
                        formattedText.Append(masterfile.employee_name);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(130, 489));
                        formattedText.Clear();
                        if (contract != null)
                        {
                            formattedText.Append(contract.designation);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(130, 472));
                            formattedText.Clear();
                        }

                        formattedText.Append("This letter is to acknowledge the receipt and acceptance of your letter of resignation, received on ");
                        formattedText.Append(certificatesavingtest_.submition_date.Value.ToString("dd MMM yy"));
                        formattedText.Append(" from the position of ");
                        if (contract != null)
                        {
                            formattedText.Append(contract.designation);
                        }
                        formattedText.Append(" and your last working day with the Company would be on ");
                        var enddatewof = certificatesavingtest_.destination;
                        var srtinglenth = enddatewof.Length;
                        var temp = 0;
                        if (srtinglenth > enddatewof.IndexOf("@"))
                        {
                            temp = enddatewof.IndexOf("@") + 1;
                            var endate = Convert.ToDateTime(enddatewof.Substring(temp));
                            formattedText.Append(endate.ToString("dd MMM yy"));
                        }
                        double y = 320;
                        int lineIndex = 0, charIndex = 0;
                        while (charIndex < formattedText.Length)
                        {
                            var line = formattedText.FormatLine(charIndex, maxWidth);
                            y += line.Height;

                            // If line cannot fit on the current page, write it on a new page.
                            bool lineCannotFit = y > maxHeight;
                            if (lineCannotFit)
                            {
                                page = document.Pages.Add();
                                y = line.Height;
                            }

                            page.Content.DrawText(line, new PdfPoint(margins, heightOffset - y));

                            ++lineIndex;
                            charIndex += line.Length;
                        }
                        formattedText.Clear();


                    }
                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Resignation Notice Acceptance.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 16)
            {
                string filepath = Server.MapPath("~/arabic certificates/Experience Certificate - Arabic Male.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Experience Certificate - Arabic Male.pdf");
                var csorgr = "CS-HRDCRT-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Experience Certificate - Arabic Male.pdf");
                    filepathout =
                        Server.MapPath("~/arabic certificates/tempstore/Experience Certificate - Arabic Male.pdf");
                    csorgr = "GR-HRDCRT-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {

                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("Arabic");
                        formattedText.FontSize = 15.17;
                        formattedText.Append(nameinar.ARname);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 482));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARnationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 452));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARposition);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 391));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        formattedText.Append(masterfile.date_joined.Value.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 360));
                        formattedText.Clear();
                        var enddatewof = certificatesavingtest_.destination;
                        var temp = enddatewof.IndexOf("@") + 1;
                        var temp2 = enddatewof.Substring(temp);
                        var endate = Convert.ToDateTime(temp2);
                        formattedText.Append(endate.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(315, 360));
                        formattedText.Clear();
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 425));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(150, 710));

                    }

                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(145, 686));
                        formattedText.Clear();
                    }

                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Experience Certificate - Arabic Male.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 17)
            {
                string filepath = Server.MapPath("~/arabic certificates/Experience Certificate - Arabic Female.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Experience Certificate - Arabic Female.pdf");
                var csorgr = "CS-HRDCRT-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath =
                        Server.MapPath("~/arabic certificates/Experience Certificate - Arabic Female - Grove.pdf");
                    filepathout =
                        Server.MapPath("~/arabic certificates/tempstore/Experience Certificate - Arabic Female - Grove.pdf");
                    csorgr = "GR-HRDCRT-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {

                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("Arabic");
                        formattedText.FontSize = 15.17;
                        formattedText.Append(nameinar.ARname);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 482));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARnationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 452));
                        formattedText.Clear();
                        formattedText.Append(nameinar.ARposition);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 391));

                    }
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Right;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        formattedText.Append(masterfile.date_joined.Value.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 360));
                        formattedText.Clear();
                        var enddatewof = certificatesavingtest_.destination;
                        var temp = enddatewof.IndexOf("@") + 1;
                        var temp2 = enddatewof.Substring(temp);
                        var endate = Convert.ToDateTime(temp2);
                        formattedText.Append(endate.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(315, 360));
                        formattedText.Clear();
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 425));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(150, 710));

                    }

                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(140, 686));
                        formattedText.Clear();
                    }

                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Experience Certificate - Arabic Female.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 18)
            {
                string filepath = Server.MapPath("~/arabic certificates/Visa request.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Visa request.pdf");
                var csorgr = "CS-HRDLTR-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Visa request - Grove.pdf");
                    csorgr = "GR-HRDLTR-";
                    filepathout = Server.MapPath("~/arabic certificates/tempstore/Visa request - Grove.pdf");
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    var page = document.Pages[0];
                    double margins = 72;
                    double maxWidth = page.CropBox.Width - margins * 2;
                    double maxHeight = page.CropBox.Height - margins * 2;
                    double heightOffset = maxHeight + margins;
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        formattedText.Clear();/*
                        var enddatewof = certificatesavingtest_.destination;
                        var temp = enddatewof.IndexOf("@") + 1;
                        var temp2 = enddatewof.Substring(temp);
                        var endate = Convert.ToDateTime(temp2);
                        formattedText.Append(endate.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(315, 360));
                        formattedText.Clear();*/
                        var namelist = certificatesavingtest_.destination.Split(',');
                        formattedText.Append(namelist[0]);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(73, 735));
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMMM yyyy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(100, 680));
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(100, 695));
                        formattedText.Clear();
                        formattedText.Append(masterfile.employee_name);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(150, 590));
                        formattedText.Clear();
                        formattedText.Append(masterfile.dob.Value.ToString("dd MMMM yyyy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(150, 570));
                        formattedText.Clear();
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(150, 550));
                        formattedText.Clear();
                        formattedText.Append(masterfile.nationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(150, 530));
                        formattedText.Clear();
                        formattedText.FontWeight = PdfFontWeight.Normal;
                        formattedText.Append("This is to certify that the above mentioned is a trusted member of this establishment, employed in the capacity of ");
                        if (contract != null)
                        {
                            formattedText.FontWeight = PdfFontWeight.Bold;
                            formattedText.Append(contract.designation);
                        }
                        formattedText.FontWeight = PdfFontWeight.Normal;
                        formattedText.Append(" since ");
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        formattedText.Append(masterfile.date_joined.Value.ToString("dd MMMM yyyy"));
                        formattedText.FontWeight = PdfFontWeight.Normal;
                        double y = 280;
                        int lineIndex = 0, charIndex = 0;
                        while (charIndex < formattedText.Length)
                        {
                            var line = formattedText.FormatLine(charIndex, maxWidth);
                            y += line.Height;

                            // If line cannot fit on the current page, write it on a new page.
                            bool lineCannotFit = y > maxHeight;
                            if (lineCannotFit)
                            {
                                page = document.Pages.Add();
                                y = line.Height;
                            }

                            page.Content.DrawText(line, new PdfPoint(margins, heightOffset - y));

                            ++lineIndex;
                            charIndex += line.Length;
                        }
                        formattedText.Clear();
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        formattedText.Append("Citiscape LLC"); 
                        formattedText.FontWeight = PdfFontWeight.Normal;
                        formattedText.Append(" has no objection for her/him to obtain a visit visa to ");
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        formattedText.Append(namelist[1]);
                        formattedText.FontWeight = PdfFontWeight.Normal;
                        formattedText.Append(" from ");
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        var tempdate = new DateTime();
                        DateTime.TryParse(namelist[2], out tempdate);
                        formattedText.Append(tempdate.ToString("dd MMMM yyyy"));
                        formattedText.FontWeight = PdfFontWeight.Normal;
                        formattedText.Append(" till ");
                        tempdate = new DateTime();
                        DateTime.TryParse(namelist[3], out tempdate);
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        formattedText.Append(tempdate.ToString("dd MMMM yyyy"));
                        formattedText.FontWeight = PdfFontWeight.Normal;
                        y = 350;
                        lineIndex = 0;
                        charIndex = 0;
                        while (charIndex < formattedText.Length)
                        {
                            var line = formattedText.FormatLine(charIndex, maxWidth);
                            y += line.Height;

                            // If line cannot fit on the current page, write it on a new page.
                            bool lineCannotFit = y > maxHeight;
                            if (lineCannotFit)
                            {
                                page = document.Pages.Add();
                                y = line.Height;
                            }

                            page.Content.DrawText(line, new PdfPoint(margins, heightOffset - y));

                            ++lineIndex;
                            charIndex += line.Length;
                        }

                    }
                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Visa request.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 19)
            {
                string filepath = Server.MapPath("~/arabic certificates/Salary transfer letter.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary transfer letter.pdf");
                var csorgr = "CS-HRDLTR-";
                if (certificatesavingtest_.cs_gr == "GR_certificates")
                {
                    filepath = Server.MapPath("~/arabic certificates/Salary transfer letter - Grove.pdf");
                    filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary transfer letter - Grove.pdf");
                    csorgr = "GR-HRDLTR-";
                }

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(111, 725));
                        formattedText.Clear();
                        var refstr = "CS-HRDLRT-" + certificatesavingtest_.Id.ToString("D") + "-" + DateTime.Now.ToString("yy");
                        formattedText.Append(refstr);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(142, 711));
                        formattedText.Clear();
                        formattedText.Append(certificatesavingtest_.destination);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(110, 690));
                        formattedText.Clear();
                        formattedText.Append(masterfile.employee_name);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(215, 525));
                        formattedText.Clear();
                        formattedText.Append(masterfile.nationality);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(215, 500));
                        formattedText.Clear();
                        if (contract != null)
                        {
                            formattedText.Append(contract.designation);
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(215, 475));
                            formattedText.Clear();
                            formattedText.Append(Unprotect(contract.salary_details));
                            document.Pages[0].Content.DrawText(formattedText, new PdfPoint(240, 440));
                            formattedText.Clear();
                        }
                        formattedText.Append(masterfile.date_joined.Value.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(215, 410));
                        formattedText.Clear();
                        formattedText.Append(bankvar.IBAN);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(215, 383));
                        formattedText.Clear();
                    }

                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Salary transfer letter.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 20)
            {
                string filepath = Server.MapPath("~/arabic certificates/reference no.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/reference no.pdf");

                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                using (var document = PdfDocument.Load(filepath))
                {
                    using (var formattedText = new PdfFormattedText())
                    {
                        formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                        formattedText.TextAlignment = PdfTextAlignment.Left;
                        formattedText.Language = new PdfLanguage("English");
                        formattedText.FontSize = 11;
                        formattedText.FontWeight = PdfFontWeight.Bold;
                        formattedText.Clear();
                        formattedText.Append(certificatesavingtest_.Id.ToString());
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(111, 745));
                        formattedText.Clear();
                    }

                    document.Save(filepathout);
                    document.Close();
                }
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"reference no.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            var cstvar = db.certificatesavingtest_.ToList().Find(x => x.Id == id);
            cstvar.status = "downloaded";
            db.Entry(cstvar).State = EntityState.Modified;
            db.SaveChanges();
        end:;

            return RedirectToAction("Index");
        }

        public ActionResult hrcertificatesubmmit( certificatesavingtest_ certificatesavingtest_,string certificate_of,DateTime? resignationsubdate)
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            if (certificate_of.IsNullOrWhiteSpace())
            {
                goto end;
            }
            else
            {
                ViewBag.continueprogram = true;
                ViewBag.certificate_of = certificate_of;
            }


            if (certificatesavingtest_.destination != null ||certificatesavingtest_.submition_date != null)
            {
                if (certificatesavingtest_.submition_date != null)
                {
                    if (certificatesavingtest_.destination == null)
                    {
                        certificatesavingtest_.destination = "";
                    }
                    if (resignationsubdate.HasValue) 
                    {
                        certificatesavingtest_.destination = resignationsubdate.Value.ToString("dd MMM yy"); 
                    }
                    certificatesavingtest_.destination = certificatesavingtest_.destination + "@"+
                                                       certificatesavingtest_.submition_date.Value.ToString("dd MMM yy");
                }
                if (certificatesavingtest_.certificate_type == 20)
                {
                    certificatesavingtest_.status = "approved";
                }
                else{ 
                certificatesavingtest_.status = "new HR";}
                certificatesavingtest_.submited_by = User.Identity.Name;
                certificatesavingtest_.modifieddate_by = DateTime.Now;
                certificatesavingtest_.submition_date = DateTime.Today;
                certificatesavingtest_.approved_by = "";
                certificatesavingtest_.cs_gr = certificate_of;
                db.certificatesavingtest_.Add(certificatesavingtest_);
                db.SaveChanges();
                var lastcerid = db.certificatesavingtest_.ToList().Last();
                SendMail(lastcerid.Id);
                return RedirectToAction("Index");
            }
            end: ;
            ViewBag.certificate_type = new SelectList(db.certificatetypes.Where(x => x.certificsatefor == "HR"), "Id",
                "certificate_name_");
            ViewBag.employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            return View();
        }


        public void SendMail(int id)
        {
            var message = new MimeMessage();
            var desig = "";
            var cstvar = db.certificatesavingtest_.ToList().Find(x => x.Id == id);
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
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            certificatesavingtest_ certificatesavingtest_ = db.certificatesavingtest_.Find(id);
            if (certificatesavingtest_ == null)
            {
                return HttpNotFound();
            }
            return View(certificatesavingtest_);
        }

        // POST: certificatesavingtest_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            certificatesavingtest_ certificatesavingtest_ = db.certificatesavingtest_.Find(id);
            db.certificatesavingtest_.Remove(certificatesavingtest_);
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
/*
// {

{
    
     if (certificatesavingtest_.certificate_type == 12)
    {
        string filepath = Server.MapPath("~/arabic certificates/Driving Licence - Heavy - female.pdf");
        string filepathout = Server.MapPath("~/arabic certificates/tempstore/Driving Licence - Heavy - female.pdf");
        if (certificatesavingtest_.cs_gr == "GR_certificates")
        {
             filepath = Server.MapPath("~/arabic certificates/Driving Licence - Heavy - female - Grove.pdf");
             filepathout =
                Server.MapPath("~/arabic certificates/tempstore/Driving Licence - Heavy - female - Grove.pdf");
        }

        ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        using (var document = PdfDocument.Load(filepath))
        {
            using (var formattedText = new PdfFormattedText())
            {

                formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                formattedText.TextAlignment = PdfTextAlignment.Right;
                formattedText.Language = new PdfLanguage("Arabic");
                formattedText.FontSize = 15.17;
                formattedText.Append(nameinar.ARname);
                document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 480));
                formattedText.Clear();
                formattedText.Append(nameinar.ARnationality);
                document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 456));

            }
            using (var formattedText = new PdfFormattedText())
            {
                formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                formattedText.TextAlignment = PdfTextAlignment.Right;
                formattedText.Language = new PdfLanguage("English");
                formattedText.FontSize = 11;
                formattedText.Clear();
                formattedText.Append(passport.passport_no);
                document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 436));
               formattedText.Clear();
                formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                document.Pages[0].Content.DrawText(formattedText, new PdfPoint(105, 710));

            }

            using (var formattedText = new PdfFormattedText())
            {
                formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
                formattedText.TextAlignment = PdfTextAlignment.Left;
                formattedText.Language = new PdfLanguage("English");
                formattedText.FontSize = 11;
                formattedText.Clear();
                formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                document.Pages[0].Content.DrawText(formattedText, new PdfPoint(115, 700));
                formattedText.Clear();
            }

            document.Save(filepathout);
            document.Close();
        }
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("Content-Disposition", "attachment;filename=\"Driving Licence - Heavy - female.pdf\"");
        Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
        Response.Flush();
        Response.End();
    }
    
}

//use for english pdfs
{
string filepath = Server.MapPath("~/arabic certificates/Salary Certificate - Arabic - Male.pdf");
string filepathut = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Male.pdf");
DocumentCore dc = DocumentCore.Load(filepath,new PdfLoadOptions());
ContentRange cr = dc.Content.Find("@").First();
if (cr != null)
{
    cr.Replace("this test is not working");
}
dc.Save(filepathut);
Response.Clear();
Response.ContentType = "application/pdf";
Response.AddHeader("Content-Disposition", "attachment;filename=\"Salary Certificate - Arabic - Male.pdf\"");
Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/Salary Certificate - Arabic - Male.pdf")));
Response.Flush();
Response.End();
// return View("cer1",certificatesavingtest_);
}

string filepath = Server.MapPath("~/arabic certificates/Salary Certificate - Arabic - Male.pdf");
string filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Male.pdf");
PdfDocument pfddoc = new PdfDocument(new PdfReader(filepath), new PdfWriter(filepathout));
pfddoc.RemovePage(2);
var newdoc = new Document(pfddoc);
var fontfamilies = new List<string>();
//fontfamilies.Add("Arial Typesetting");
fontfamilies.Add("Arial");
FontProvider provider = new FontProvider();
//Load in fonts in the directory into the provider
provider.AddDirectory(Server.MapPath("~/Content/fonts"));
provider.AddDirectory("c:/windows/fonts");
newdoc.ShowTextAligned(new Paragraph("serial no"), 130, 672, TextAlignment.LEFT);
newdoc.ShowTextAligned(new Paragraph(certificatesavingtest_.destination), 510, 632, TextAlignment.LEFT);
var ARname = nameinar.ARname; 
Encoding utf8 = Encoding.UTF8;
var nameAR = utf8.GetString(utf8.GetBytes(ARname));
var paraAR = new Paragraph(Bidi.BidiText(nameAR, 1).Text);
var test = Bidi.BidiText(nameAR, 10).Text;
FontSelectorStrategy strategy = provider.GetStrategy(ARname, fontfamilies);
strategy.NextGlyphs();
PdfFont arabicFont = strategy.GetCurrentFont(); 
paraAR.SetFont(arabicFont);
newdoc.ShowTextAligned((paraAR), 300, 100, TextAlignment.RIGHT);
newdoc.Close();
// }
using (var formattedText = new PdfFormattedText())
{

    formattedText.FontFamily = new PdfFontFamily("c:/windows/fonts", "Arial");
    formattedText.TextAlignment = PdfTextAlignment.Left;
    formattedText.Language = new PdfLanguage("English");
    formattedText.FontSize = 11;
    formattedText.Clear();
    formattedText.Append(masterfile.date_joined.Value.ToString("dd MMM yy"));
    document.Pages[0].Content.DrawText(formattedText, new PdfPoint(364, 338));
    formattedText.Clear();
    formattedText.Append(passport.passport_no);
    document.Pages[0].Content.DrawText(formattedText, new PdfPoint(365, 428));
    // formattedText.Clear();
    // formattedText.Append(Unprotect(contract.salary_details));
    // document.Pages[0].Content.DrawText(formattedText, new PdfPoint(364, 368));
    formattedText.Clear();
    formattedText.Append(certificatesavingtest_.destination);
    document.Pages[0].Content.DrawText(formattedText, new PdfPoint(500, 635));
    formattedText.Clear();
    formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
    document.Pages[0].Content.DrawText(formattedText, new PdfPoint(65, 685));

}
// GET: certificatesavingtest_/Create
public ActionResult Create()
{
    ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name");
    return View();
}

// POST: certificatesavingtest_/Create
// To protect from overposting attacks, enable the specific properties you want to bind to, for 
// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Create([Bind(Include = "Id,employee_id,certificate_type,destination,submition_date")] certificatesavingtest_ certificatesavingtest_)
{
    if (ModelState.IsValid)
    {
        db.certificatesavingtest_.Add(certificatesavingtest_);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", certificatesavingtest_.employee_id);
    return View(certificatesavingtest_);
}

// GET: certificatesavingtest_/Edit/5
public ActionResult Edit(int? id)
{
    if (id == null)
    {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }
    certificatesavingtest_ certificatesavingtest_ = db.certificatesavingtest_.Find(id);
    if (certificatesavingtest_ == null)
    {
        return HttpNotFound();
    }
    ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", certificatesavingtest_.employee_id);
    return View(certificatesavingtest_);
}

// POST: certificatesavingtest_/Edit/5
// To protect from overposting attacks, enable the specific properties you want to bind to, for 
// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit([Bind(Include = "Id,employee_id,certificate_type,destination,submition_date")] certificatesavingtest_ certificatesavingtest_)
{
    if (ModelState.IsValid)
    {
        db.Entry(certificatesavingtest_).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    ViewBag.employee_id = new SelectList(db.master_file, "employee_id", "employee_name", certificatesavingtest_.employee_id);
    return View(certificatesavingtest_);
}

// GET: certificatesavingtest_/Delete/5*/