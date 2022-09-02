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
using Paragraph = iText.Layout.Element.Paragraph;
using PdfDocument = GemBox.Pdf.PdfDocument;
using PdfLoadOptions = SautinSoft.Document.PdfLoadOptions;

namespace HRworks.Controllers
{
    public class certificatesavingtest_Controller : Controller
    {
        private HREntities db = new HREntities();
        private const string Purpose = "equalizer";

        // GET: certificatesavingtest_
        public ActionResult Index()
        {
            var certificatesavingtest_ = db.certificatesavingtest_.Include(c => c.master_file);
            return View(certificatesavingtest_.ToList());
        }


        public string Unprotect(string protectedText)
        {
            var protectedBytes = Convert.FromBase64String(protectedText);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
            var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
            return unprotectedText;
        }
        // GET: certificatesavingtest_/Details/5
        public void Details(int? id)
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

            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            if (certificatesavingtest_.certificate_type == 1)
            {
                    string filepath = Server.MapPath("~/arabic certificates/Salary Certificate - Arabic - Male.pdf");
                    string filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Male.pdf");
                {

                    /*
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
                newdoc.Close(); */
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
                            document.Pages.RemoveAt(1);

                    }
                    /*
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

                    }*/
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
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 428));
                        formattedText.Clear();
                        formattedText.Append(Unprotect(contract.salary_details));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 368));
                        formattedText.Clear();
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
                        formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(130, 672));
                        formattedText.Clear();
                    }
                    document.Save(filepathout);
                    document.Close();
                }
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"Salary Certificate - Arabic - Male.pdf\"");
                    Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Male.pdf")));
                    Response.Flush();
                    Response.End();
            }
            if (certificatesavingtest_.certificate_type == 2)
            {
                    string filepath = Server.MapPath("~/arabic certificates/Salary Certificate - Arabic - Female.pdf");
                    string filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Female.pdf");
              
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
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 485));
                        formattedText.Clear();
                        formattedText.Append(Unprotect(contract.salary_details));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 390));
                        formattedText.Clear();
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
                        formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(130, 695));
                        formattedText.Clear();
                    }
                    document.Save(filepathout);
                    document.Close();
                }
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"Salary Certificate - Arabic - Female.pdf\"");
                    Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Salary Certificate - Arabic - Female.pdf")));
                    Response.Flush();
                    Response.End();
            }
            if (certificatesavingtest_.certificate_type == 3)
            {
                    string filepath = Server.MapPath("~/arabic certificates/Employment Certificate - Arabic - Male.pdf");
                    string filepathout = Server.MapPath("~/arabic certificates/tempstore/Employment Certificate - Arabic - Male.pdf");
              
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
                    document.Save(filepathout);
                    document.Close();
                }
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"Employment Certificate - Arabic - Male.pdf\"");
                    Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Employment Certificate - Arabic - Male.pdf")));
                    Response.Flush();
                    Response.End();
            }
            if (certificatesavingtest_.certificate_type == 4)
            {
                    string filepath = Server.MapPath("~/arabic certificates/Employment Certificate - Arabic - female.pdf");
                    string filepathout = Server.MapPath("~/arabic certificates/tempstore/Employment Certificate - Arabic - Female.pdf");
              
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
                    document.Save(filepathout);
                    document.Close();
                }
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"Employment Certificate - Arabic - female.pdf\"");
                    Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Employment Certificate - Arabic - female.pdf")));
                    Response.Flush();
                    Response.End();
            }
            if (certificatesavingtest_.certificate_type == 5)
            {
                string filepath = Server.MapPath("~/arabic certificates/Salary Certificate.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Salary Certificate.pdf");
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
                cr = dc.Content.Find("@dest").First();
                if (cr != null)
                {
                    cr.Replace(certificatesavingtest_.destination);
                }
                cr = dc.Content.Find("@name").First();
                if (cr != null)
                {
                    cr.Replace(masterfile.employee_name);
                }
                cr = dc.Content.Find("@nationality").First();
                if (cr != null)
                {
                    cr.Replace(masterfile.nationality);
                }
                cr = dc.Content.Find("@pass").First();
                if (cr != null)
                {
                    cr.Replace(passport.passport_no);
                }
                cr = dc.Content.Find("@position").First();
                if (cr != null)
                {
                    cr.Replace(contract.designation);
                }
                cr = dc.Content.Find("@salary").First();
                if (cr != null)
                {
                    cr.Replace(Unprotect(contract.salary_details));
                }
                cr = dc.Content.Find("@jd").First();
                if (cr != null)
                {
                    cr.Replace(masterfile.date_joined.Value.ToString("dd MMM yy"));
                }
                dc.Save(filepathout);
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Salary Certificate.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Salary Certificate.pdf")));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 6)
            {
                string filepath = Server.MapPath("~/arabic certificates/Employment Certificate.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Employment Certificate.pdf");
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
                cr = dc.Content.Find("@nationality").First();
                if (cr != null)
                {
                    cr.Replace(masterfile.nationality);
                }
                cr = dc.Content.Find("@pass").First();
                if (cr != null)
                {
                    cr.Replace(passport.passport_no);
                }
                cr = dc.Content.Find("@position").First();
                if (cr != null)
                {
                    cr.Replace(contract.designation);
                }
                cr = dc.Content.Find("@Jd").First();
                if (cr != null)
                {
                    cr.Replace(masterfile.date_joined.Value.ToString("dd MMM yy"));
                }
                dc.Save(filepathout);
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Employment Certificate.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Employment Certificate.pdf")));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 7)
            {
                string filepath = Server.MapPath("~/arabic certificates/Letter to Immigration - Male.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Letter to Immigration - Male.pdf");

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
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 464));
                       formattedText.Clear();
                        formattedText.Append( Unprotect(contract.salary_details));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 404));
                        formattedText.Clear();
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
                        formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(120, 716));
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
                        formattedText.Append(passport.passport_no);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 400));
                       formattedText.Clear();
                        formattedText.Append( Unprotect(contract.salary_details));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(420, 378));
                        formattedText.Clear();
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
                        formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(120, 692));
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
                        formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(135, 729));
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
                        formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(135, 719));
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
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Driving Licence - Heavy - Male.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(filepathout));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 12)
            {
                string filepath = Server.MapPath("~/arabic certificates/Driving Licence - Heavy - female.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Driving Licence - Heavy - female.pdf");

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

            if (certificatesavingtest_.certificate_type == 13)
            {
                string filepath = Server.MapPath("~/arabic certificates/EXPERIENCE CERTIFICATE - english.pdf");
                string filepathout =
                    Server.MapPath("~/arabic certificates/tempstore/EXPERIENCE CERTIFICATE - english.pdf");

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
                        formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(229, 695));
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
                        formattedText.Append(contract.designation);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(170, 453));
                        formattedText.Clear();
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
                Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/EXPERIENCE CERTIFICATE - english.pdf")));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 14)
            {
                string filepath = Server.MapPath("~/arabic certificates/Termination Notice.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Termination Notice.pdf");

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
                        formattedText.Clear();
                        formattedText.Append(DateTime.Today.ToString("dd MMM yy"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 638));
                        formattedText.Clear();
                        formattedText.Append(masterfile.employee_name);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 613));
                        formattedText.Clear();
                        formattedText.Append(contract.designation);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 587));
                        formattedText.Clear();
                        formattedText.Append(contract.departmant_project);
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(155, 562));
                        formattedText.Clear();
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
                Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Termination Notice.pdf")));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 15)
            {
                string filepath = Server.MapPath("~/arabic certificates/Resignation Notice Acceptance.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Resignation Notice Acceptance.pdf");
                DocumentCore dc = DocumentCore.Load(filepath, new PdfLoadOptions());
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
                cr = dc.Content.Find("@lds").First();
                if (cr != null)
                {
                    var enddatewof = certificatesavingtest_.destination;
                    var temp = enddatewof.IndexOf("@") + 1;
                    var temp2 = enddatewof.Substring(temp);
                    var endate = Convert.ToDateTime(temp2);
                    cr.Replace(endate.ToString("dd MMM yy"));
                }
                dc.Save(filepathout);
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"Resignation Notice Acceptance.pdf\"");
                Response.BinaryWrite(System.IO.File.ReadAllBytes(Server.MapPath("~/arabic certificates/tempstore/Resignation Notice Acceptance.pdf")));
                Response.Flush();
                Response.End();
            }
            if (certificatesavingtest_.certificate_type == 16)
            {
                string filepath = Server.MapPath("~/arabic certificates/Experience Certificate - Arabic Male.pdf");
                string filepathout = Server.MapPath("~/arabic certificates/tempstore/Experience Certificate - Arabic Male.pdf");

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
                        formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(215, 685));
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
                        formattedText.Append(certificatesavingtest_.Id.ToString("D"));
                        document.Pages[0].Content.DrawText(formattedText, new PdfPoint(215, 685));
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
        end:;
        }

        public ActionResult hrcertificatesubmmit([Bind(Include = "Id,employee_id,certificate_type,destination,submition_date")] certificatesavingtest_ certificatesavingtest_)
        {
            var alist = this.db.master_file.OrderBy(e => e.employee_no).ThenByDescending(x => x.date_changed).ToList();
            var afinallist = new List<master_file>();
            foreach (var file in alist)
            {
                if (afinallist.Count == 0) afinallist.Add(file);

                if (!afinallist.Exists(x => x.employee_no == file.employee_no)) afinallist.Add(file);
            }

            if (certificatesavingtest_.destination != null ||certificatesavingtest_.submition_date != null)
            {
                if (certificatesavingtest_.submition_date != null)
                {
                    if (certificatesavingtest_.destination == null)
                    {
                        certificatesavingtest_.destination = "";
                    }
                    certificatesavingtest_.destination = certificatesavingtest_.destination + "@"+
                                                       certificatesavingtest_.submition_date.Value.ToString("dd MMM yy");
                }
                certificatesavingtest_.submition_date = DateTime.Today;
                db.certificatesavingtest_.Add(certificatesavingtest_);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.certificate_type = new SelectList(db.certificatetypes.Where(x => x.certificsatefor == "HR"), "Id",
                "certificate_name_");
            ViewBag.employee_id = new SelectList(afinallist.OrderBy(x => x.employee_no), "employee_id", "employee_no");
            return View();
        }

        /*
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
        }*/

        // GET: certificatesavingtest_/Delete/5
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
