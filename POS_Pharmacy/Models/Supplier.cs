using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Pharmacy.Models
{
    [Table("supplier_information")]
    public class Supplier
    {
        [Key]
        [Column("supplier_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierId { get; set; }

        [Column("supplier_name")]
        [Required]
        public string SupplierName { get; set; }

        [Column("contact_person")]
        public string ContactPerson { get; set; }

        [Column("contact_number")]
        public string ContactNumber { get; set; }

        [Column("note")]
        public string Note { get; set; }
    }
}