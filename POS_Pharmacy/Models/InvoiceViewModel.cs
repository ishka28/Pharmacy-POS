using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace POS_Pharmacy.Models
{
    public class InvoiceViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; } = DateTime.Today;

        [Required]
        public string InvoiceNo { get; set; }

        [Required]
        public string DoctorName { get; set; }

        [Required]
        [Display(Name ="Patient Name")]
        public string PatientName { get; set; }

        [Required]
        [Display(Name = "Patient Mobile")]
        public string PatientMobile{ get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; } = DateTime.Today;
         
        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        public decimal InvoiceTotal { get; set; } 

        public List<BillingItemViewModel> BillingItems { get; set; } = new List<BillingItemViewModel>();
    }

    public class BillingItemViewModel
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string SellingPrice { get; set; }
        public string Dose { get; set; }
        public string DirectionOfUse { get; set; } // dropdown Nocte, Mane, TDS, SOS, Q4H, Q6H
        public string DurationUnit { get; set; }  // dropdown days weeks months
        public int DurationValue { get; set; }
        public string NumberOfPills { get; set; } 
        public string SubTotal { get; set; }
    }
    public class PurchasingViewModel
    {
        public string POId { get; set; } = "PO-2026-001";
        public string TransactionType { get; set; } = "Purchase";

        [Required]
        public DateTime ReceivedDate { get; set; } = DateTime.Today;

        [Required]
        public string SupplierName { get; set; }

        [Required]
        public string SupplierInvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string SupplierNote { get; set; }

        public decimal TotalIntakeCost { get; set; }
        public List<PurchaseLineItemViewModel> PurchaseItems { get; set; } = new List<PurchaseLineItemViewModel>();
    }

    public class PurchaseLineItemViewModel
    {
        public string ItemCode { get; set; }
        public string Category { get; set; }
        public string GenericName { get; set; }
        public string BrandName { get; set; }
        public string Dose { get; set; }
        public int QtyRecv { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal CostPrice { get; set; }
        public decimal UnitCost { get; set; }
        public string ExpiryDate { get; set; }
        public decimal LineTotal { get; set; }
    }

    namespace POS_Pharmacy.Models
    {
        public class RegisterViewModel
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Mobile { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginViewModel
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}

