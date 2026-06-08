using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Pharmacy.Models
{
    [Table("generic_drug_info")]
    public class Drug
    {
        [Key]
        [Column("generic_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenericId { get; set; }

        [Column("generic_name")]
        [Required]
        public string GenericName { get; set; }

        [Column("category_id")]
        [Required]
        public int CategoryId { get; set; }
    }
}