using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace POS_Pharmacy.Models
{
    [Table("invoice_info")]
    public class Invoice
    {
        [Key]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Required]
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; }

        [Required]
        [Column("invoice_date")]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [Column("doctor_name")]
        public string DoctorName { get; set; }

        [Required]
        [Column("patient_name")]
        public string PatientName { get; set; }

        [Required]
        [Column("patient_mobile")]
        public string PatientMobile { get; set; }

        [Required]
        [Column("dob")]
        public DateTime PatientDOB { get; set; }

        [Required]
        [Column("gender")]
        public string Gender { get; set; }

        [Required]
        [Column("total_amount")]
        public decimal InvoiceTotal { get; set; }

    }
}
