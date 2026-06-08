using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Pharmacy.Models
{
    [Table("brand_info")]
    public class BrandInfo
    {
        [Key]
        [Column("brand_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BrandId { get; set; }

        [Column("generic_id")]
        [Required]
        public int GenericId { get; set; }

        [Column("brand_name")]
        [Required]
        public string BrandName { get; set; }
    }
}