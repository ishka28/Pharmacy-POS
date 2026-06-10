using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Pharmacy.Models
{
    [Table("invoice_items_info")]
    public class InvoiceItem
    {
        [Key]
        [Column("invoice_item_id")]
        public int InvoiceItemId { get; set; }

        [Required]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Required]
        [Column("stock_id")]
        public int StockId { get; set; }

        [Required]
        [Column("item_code")]
        public string ItemCode { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("selling_price")]
        public decimal SellingPrice { get; set; }

        [Required]
        [Column("dose")]
        public string Dose { get; set; }

        [Required]
        [Column("direction_of_use")]
        public string DirectionOfUse { get; set; }

        [Required]
        [Column("duration_unit")]
        public string DurationUnit { get; set; }

        [Required]
        [Column("duration_value")]
        public int DurationValue { get; set; }

        [Required]
        [Column("number_of_pills")]
        public int NumberOfPills { get; set; }

        [Required]
        [Column("sub_total")]
        public decimal SubTotal { get; set; }

    }
}
