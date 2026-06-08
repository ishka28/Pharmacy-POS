using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Pharmacy.Models
{
    [Table("Purchase_info")]
    public class PurchaseInfo
    {
        [Key]
        [Column("purchase_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseID { get; set; }

        [Column("stock_id")]
        [Required]
        public int StockId { get; set; }

        [Column("po_id")]
        [Required]
        public string POId { get; set; }

        [Column("is_return")]
        [Required]
        public bool IsReturn {  get; set; }

        [Column("recieved_date")]
        [Required]
        public DateTime RecievedDate { get; set; }

        [Column("quantity_purchased")]
        [Required]
        public int QtyPurchased {  get; set; }
    }
}
