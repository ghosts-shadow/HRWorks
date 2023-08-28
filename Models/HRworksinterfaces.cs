using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    /*public class HRworksinterfaces
    {
    }*/
    public interface ICertificate
    {
         int Id { get; set; }
         int employee_id { get; set; }
         int certificate_type { get; set; }
         string destination { get; set; }
         Nullable<System.DateTime> submition_date { get; set; }
         string cs_gr { get; set; }
         string status { get; set; }
         string approved_by { get; set; }
         System.DateTime modifieddate_by { get; set; }
         string submited_by { get; set; }
         certificatetype certificatetype { get; set; }
         master_file master_file { get; set; }
    }
    public class CertificateAdapter : ICertificate
    {
        private readonly certificatesavinggrove _certificate;

        public CertificateAdapter(certificatesavinggrove certificate)
        {
            _certificate = certificate;
        }

        public int Id { get => _certificate.Id; set => _certificate.Id = value; }
        public int employee_id { get => _certificate.employee_id; set => _certificate.employee_id = value; }
        public int certificate_type { get => _certificate.certificate_type; set => _certificate.certificate_type = value; }
        public string destination { get => _certificate.destination; set => _certificate.destination = value; }
        public Nullable<System.DateTime> submition_date { get => _certificate.submition_date; set => _certificate.submition_date = value; }
        public string cs_gr { get => _certificate.cs_gr; set => _certificate.cs_gr = value; }
        public string status { get => _certificate.status; set => _certificate.status = value; }
        public string approved_by { get => _certificate.approved_by; set => _certificate.approved_by = value; }
        public System.DateTime modifieddate_by { get => _certificate.modifieddate_by; set => _certificate.modifieddate_by = value; }
        public string submited_by { get => _certificate.submited_by; set => _certificate.submited_by = value; }
        public certificatetype certificatetype { get => _certificate.certificatetype; set => _certificate.certificatetype = value; }
        public master_file master_file { get => _certificate.master_file; set => _certificate.master_file = value; }
    }
    public class CertificateTestAdapter : ICertificate
    {
        private readonly certificatesavingtest_ _certificateTest;

        public CertificateTestAdapter(certificatesavingtest_ certificateTest)
        {
            _certificateTest = certificateTest;
        }

        public int Id { get => _certificateTest.Id; set => _certificateTest.Id = value; }
        public int employee_id { get => _certificateTest.employee_id; set => _certificateTest.employee_id = value; }
        public int certificate_type { get => _certificateTest.certificate_type; set => _certificateTest.certificate_type = value; }
        public string destination { get => _certificateTest.destination; set => _certificateTest.destination = value; }
        public Nullable<System.DateTime> submition_date { get => _certificateTest.submition_date; set => _certificateTest.submition_date = value; }
        public string cs_gr { get => _certificateTest.cs_gr; set => _certificateTest.cs_gr = value; }
        public string status { get => _certificateTest.status; set => _certificateTest.status = value; }
        public string approved_by { get => _certificateTest.approved_by; set => _certificateTest.approved_by = value; }
        public System.DateTime modifieddate_by { get => _certificateTest.modifieddate_by; set => _certificateTest.modifieddate_by = value; }
        public string submited_by { get => _certificateTest.submited_by; set => _certificateTest.submited_by = value; }
        public certificatetype certificatetype { get => _certificateTest.certificatetype; set => _certificateTest.certificatetype = value; }
        public master_file master_file { get => _certificateTest.master_file; set => _certificateTest.master_file = value; }
    }


}