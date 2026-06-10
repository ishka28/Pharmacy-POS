using Microsoft.AspNetCore.Mvc;
using POS_Pharmacy.Models;
using System;
using POS_Pharmacy.Data;
using System.Transactions;

namespace POS_Pharmacy.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly PharmacyDatabase _context;

        public InvoiceController(PharmacyDatabase context)
        {
            _context = context;
        }

        [HttpGet]
        // Display the invoice creation form with a generated invoice number
        public IActionResult Index()
        {
            var model = new InvoiceViewModel
            {
                InvoiceNo = GenerateSequentialInvoiceNumber(),
                Date = DateTime.Today
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveInvoice([FromBody] InvoiceViewModel model)
        {
            // verify the incoming form data exists
            if (model == null)
            {
                return Json(new { success = false, message = "We didn't recieve any form data. Please refresh and try again." });
            }

            // make sure user didn't submit empty items
            if (model.BillingItems == null || model.BillingItems.Count == 0)
            {
                return Json(new { success = false, message = "Please add atleast one item to the table before saving." });
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // create master invoice records
                    var invoiceMaster = new Invoice
                    {
                        InvoiceNumber = model.InvoiceNo,
                        InvoiceDate = model.Date ?? DateTime.Today,
                        DoctorName = model.DoctorName,
                        PatientName = model.PatientName,
                        PatientMobile = model.PatientMobile,
                        PatientDOB = model.DOB ?? DateTime.Today,
                        Gender = model.Gender,
                        InvoiceTotal = model.InvoiceTotal
                    };

                    _context.Invoices.Add(invoiceMaster);
                    _context.SaveChanges();

                    foreach (var item in model.BillingItems)
                    {
                        int.TryParse(item.NumberOfPills, out int pillsCount);
                        decimal.TryParse(item.SellingPrice, out decimal sellPrice);
                        decimal.TryParse(item.SubTotal, out decimal itemSubTotal);

                        if (int.TryParse(item.ItemCode, out int targetStockId))
                        {
                            var stockRow = _context.Stocks.FirstOrDefault(s => s.StockId == targetStockId);

                            if (stockRow != null)
                            {
                                if (int.TryParse(stockRow. Quantity, out int currentStockQuantity))
                                {
                                    if (currentStockQuantity < pillsCount)
                                    {
                                        return Json(new { success = false, message = $"Insufficient stock for {item.Description}. Available inventory limit: {currentStockQuantity}" });
                                    }
                                    stockRow.Quantity = (currentStockQuantity - pillsCount).ToString();
                                    _context.Stocks.Update(stockRow);
                                }
                            }
                        }
                        // create the item records
                        var invoiceDetail = new InvoiceItem
                        {
                            InvoiceId = invoiceMaster.InvoiceId,
                            ItemCode = item.ItemCode,
                            Description = item.Description,
                            SellingPrice = sellPrice,
                            Dose = item.Dose,
                            DirectionOfUse = item.DirectionOfUse,

                            DurationValue = item.DurationValue,
                            DurationUnit = item.DurationUnit,

                            NumberOfPills = pillsCount,
                            SubTotal = itemSubTotal
                        };

                        _context.InvoiceItems.Add(invoiceDetail);
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    return Json(new { success = true, newInvoiceNo = GenerateSequentialInvoiceNumber() });
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Something went wrong on the server while trying to save this invoice." + ex.Message });
                }
            }
        }

// Helper method to generate a sequential invoice number 
private string GenerateSequentialInvoiceNumber()
{
    return $"INV-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
}
}
}
