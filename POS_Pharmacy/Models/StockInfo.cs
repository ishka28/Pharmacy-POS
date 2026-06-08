using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Pharmacy.Models
{
    [Table("stock_info")]
    public class StockInfo
    {
        [Key]
        [Column("stock_id")] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int StockId { get; set; }

        [Column("brand_id")]
        [Required]
        public int BrandId { get; set; }

        [Column("dose")] 
        public string Dose { get; set; }

        [Column("cost_price")]
        public decimal CostPrice { get; set; }

        [Column("sell_price")]
        public decimal SellPrice { get; set; }

        [Column("quantity")]
        public string Quantity { get; set; }

        [Column("reorder_level")]
        public string ReorderLevel { get; set; }
    }
}