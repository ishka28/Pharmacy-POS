using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using POS_Pharmacy.Models;
using POS_Pharmacy.Data;

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
        public IActionResult Index()
        {
            // join generic drugs to their brands and then to their active stock rows
            var stockData = (from generic in _context.Set<Drug>()
                             join brand in _context.Set<BrandInfo>()
                                on generic.GenericId equals brand.GenericId
                             join stock in _context.Set<StockInfo>()
                                on brand.BrandId equals stock.BrandId into stockGroup
                             from subStock in stockGroup.DefaultIfEmpty()
                             select new
                             {
                                 StockId = subStock != null ? subStock.StockId : 0,
                                 BrandId = brand.BrandId,
                                 BrandName = brand.BrandName,
                                 GenericName = generic.GenericName,
                                 Dose = subStock != null ? subStock.Dose : "N/A",
                                 Quantity = subStock != null ? (subStock.Quantity != null ? Convert.ToInt32(subStock.Quantity) : 0) : 0,
                                 SellPrice = subStock != null ? subStock.SellPrice : 0
                             }).ToList();

            // extract unique generic names 
            ViewBag.GenericNames = stockData
                                    .Select(s => s.GenericName)
                                    .Distinct()
                                    .OrderBy(g => g)
                                    .ToList();

            ViewBag.AllStockJson = JsonSerializer.Serialize(stockData);

            // load a fresh view model with an automated tracking invoice number
            var model = new InvoiceViewModel
            {
                InvoiceNo = "INV-" + DateTime.Now.Ticks.ToString().Substring(10),
                Date = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveInvoice([FromBody] InvoiceViewModel model)
        {
            // verify incoming data is not null
            if (model == null || model.BillingItems == null || model.BillingItems.Count == 0)
            {
                return Json(new { success = false, message = "No prescription items found to process." });
            }

            // open database transaction to run safe operations together
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // save invoice record into invoice table
                Invoice mainInvoice = new Invoice
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

                _context.Set<Invoice>().Add(mainInvoice);
                await _context.SaveChangesAsync();

                // process billing items list and adjust stock balances
                foreach (var item in model.BillingItems)
                {
                    int.TryParse(item.NumberOfPills, out int pillsCount);
                    decimal.TryParse(item.SellingPrice, out decimal sellPrice);
                    decimal.TryParse(item.SubTotal, out decimal itemSubTotal);

                    if (int.TryParse(item.ItemCode, out int targetStockId))
                    {
                        var stockRow = await _context.Set<StockInfo>().FirstOrDefaultAsync(s => s.StockId == targetStockId);

                        if (stockRow != null)
                        {
                            if (int.TryParse(stockRow.Quantity, out int currentStockQuantity))
                            {
                                // If requested pills are more than available stock quantity
                                if (currentStockQuantity < pillsCount)
                                {
                                    // Must safely roll back the transaction before returning, or it saves anyway
                                    await dbTransaction.RollbackAsync();
                                    return Json(new { success = false, message = $"Warning: Out of stock! Only {currentStockQuantity} pills available for {item.Description}." });
                                }

                                // Deduct stock safely
                                stockRow.Quantity = (currentStockQuantity - pillsCount).ToString();
                                _context.Set<StockInfo>().Update(stockRow);
                            }
                        }
                        else
                        {
                            // If the item code lookup failed entirely in the database database
                            await dbTransaction.RollbackAsync();
                            return Json(new { success = false, message = $"Product validation failed for Item Code: {item.ItemCode}" });
                        }
                    }

                    // create item details row+
                    InvoiceItem itemRow = new InvoiceItem
                    {
                        InvoiceId = mainInvoice.InvoiceId,
                        StockId = targetStockId,
                        ItemCode = item.ItemCode,
                        Description = item.Description,
                        SellingPrice = Convert.ToDecimal(item.SellingPrice),
                        Dose = item.Dose,
                        DirectionOfUse = item.DirectionOfUse,
                        DurationUnit = item.DurationUnit,
                        DurationValue = item.DurationValue,
                        NumberOfPills = pillsCount,
                        SubTotal = Convert.ToDecimal(item.SubTotal)
                    };

                    _context.Set<InvoiceItem>().Add(itemRow);
                }

                // save changes and commit the active database transaction
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                // generate new code 
                string nextInvoiceNo = "INV-" + DateTime.Now.Ticks.ToString().Substring(10);

                return Json(new { success = true, newInvoiceNo = nextInvoiceNo });
            }
            catch (Exception error)
            {
                // rollback entirely if code calculation fails midway
                await dbTransaction.RollbackAsync();
                return Json(new { success = false, message = "System database exception failure: " + error.Message });
            }
        }
    }
}