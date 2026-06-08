using Microsoft.AspNetCore.Mvc;
using POS_Pharmacy.Data;
using POS_Pharmacy.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace POS_Pharmacy.Controllers
{
    public class HomeController : Controller
    {
        private readonly PharmacyDatabase _context;

        public HomeController(PharmacyDatabase context)
        {
            _context = context;
        }

        // GET: /Home/Purchasing
        [HttpGet]
        public IActionResult Purchasing()
        {
            // 1. Fetch distinct Generic Drug Names from the database
            var genericList = _context.Drugs
                .Select(d => d.GenericName)
                .Distinct()
                .OrderBy(g => g)
                .ToList();
            ViewBag.GenericDrugList = genericList;

            // 2. Fetch Supplier Names from the database 
            var supplierList = _context.Suppliers
                .Select(s => s.SupplierName)
                .Distinct()
                .OrderBy(s => s)
                .ToList();
            ViewBag.SupplierList = supplierList;

            // 3. Initialize a clean instance of the view model 
            var model = new PurchasingViewModel();
            Random rand = new Random();
            int randomNumber = rand.Next(100, 999);
            model.POId = $"PO-{DateTime.Now:yyyyMMdd}-{randomNumber}";
            model.ReceivedDate = DateTime.Today;

            return View(model);
        }

        // POST: /Home/SavePurchase
        [HttpPost]
        public IActionResult SavePurchase(PurchasingViewModel model, bool IsReturn)
        {
            // check if user actually added any value table
            if (model.PurchaseItems != null && model.PurchaseItems.Count > 0)
            {
                // loop through every row
                foreach (var item in model.PurchaseItems)
                {
                    // find the brand record for the current item
                    var brandRecord = _context.Brands.FirstOrDefault(b => b.BrandName == item.BrandName);
                    if (brandRecord != null)
                    {
                        // find the stock record for the current brand and dose
                        var stockRow = _context.Stocks.FirstOrDefault(s =>
                        s.BrandId == brandRecord.BrandId &&
                        s.Dose == item.Dose);

                        // if a stock record exists, update the quantity and prices based on the purchase details
                        if (stockRow != null)
                        {
                            int currentStock = 0;
                            int.TryParse(stockRow.Quantity, out currentStock);

                            int incomingQuantity = item.QtyRecv;

                            // if this is a return transaction, we need to subtract the incoming quantity from the current stock, otherwise we add it
                            if (IsReturn == true)
                            {
                                stockRow.Quantity = (currentStock - incomingQuantity).ToString();

                            }
                            else
                            {
                                stockRow.Quantity = (currentStock + incomingQuantity).ToString();
                            }

                            // update the cost price and selling price in the stock record 
                            stockRow.CostPrice = item.CostPrice;
                            stockRow.SellPrice = item.SellingPrice;

                            _context.Stocks.Update(stockRow);

                            // create a purchase record for this transaction
                            var purchaseRecord = new PurchaseInfo
                            {
                                // generate a unique purchase ID for this transaction 
                                POId = model.POId,
                                StockId = stockRow.StockId,
                                IsReturn = IsReturn,
                                RecievedDate = model.ReceivedDate,
                                QtyPurchased = item.QtyRecv
                            };

                            _context.Purchases.Add(purchaseRecord);
                        }
                    }
                    }
                    // comit all changes to the database at once after processing all rows
                    _context.SaveChanges();

                    // refresh the page to clear the form and show updated stock levels if user wants to add another purchase
                    return RedirectToAction("Purchasing");
                }
                // fallback error handling if no items were added to the purchase
                ViewBag.GenericDrugList = _context.Drugs.Select(d => d.GenericName).Distinct().OrderBy(g => g).ToList();
                ViewBag.SupplierList = _context.Suppliers.Select(s => s.SupplierName).Distinct().OrderBy(s => s).ToList();
                return View("Purchasing", model);
            }

            // GET: /Home/GetDrugsByGeneric
            [HttpGet]
            public JsonResult GetDrugsByGeneric(string genericName)
            {
            // find the drug record that matches the selected generic name
            var drug = _context.Drugs.FirstOrDefault(d => d.GenericName == genericName);
                if (drug == null)
                {
                    return Json(null);
                }
            // find all brand records that are associated with the generic drug
            var matchingBrands = _context.Brands
                    .Where(b => b.GenericId == drug.GenericId)
                    .ToList();

                var brandOptionsList = new List<object>();
            // loop through each matching brand and find the corresponding stock records to get the available doses for that brand
            for (var i = 0; i < matchingBrands.Count; i++)
                {
                    var currentBrand = matchingBrands[i];

                // for the current brand, find all stock records that match the brand ID and extract the dose information to populate the dropdown options for that brand
                var stockItems = _context.Stocks
                        .Where(s => s.BrandId == currentBrand.BrandId)
                        .Select(s => s.Dose)
                        .ToList();

                // loop through each stock item for the current brand and add an entry to the brand options list that includes the brand name and the available dose for that stock item
                for (var j = 0; j < stockItems.Count; j++)
                    {
                        brandOptionsList.Add(new
                        {
                            // combine the brand name and dose into a single string to be used as the display value for the dropdown option
                            brandName = currentBrand.BrandName,
                            dose = stockItems[j]
                        });
                    }
                }

                return Json(new
                {
                    // return the generic drug ID as a 3-digit string to be used as the item code in the purchase line item
                    itemCode = drug.GenericId.ToString("D3"),
                    category = "CATEGORY ID: " + drug.CategoryId,
                    brands = brandOptionsList
                });
            }

        // GET: /Home/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Home/Register
        [HttpPost]
        public IActionResult Register(User registrationData)
        {
            var checkUser = _context.Users.FirstOrDefault(u => u.Username == registrationData.Username || u.Email == registrationData.Email);

            if (checkUser != null)
            {
                ViewBag.Error = "Username or Email address is already registered.";
                return View();
            }
            registrationData.Attempt = 0;
            _context.Users.Add(registrationData);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        // GET: /Home/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Home/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password configuration.";
                return View();
            }

            if (user.Attempt >= 3)
            {
                ViewBag.Error = "This profile is permanently locked due to 3 invalid password attempts. Contact Admin.";
                return View();
            }

            if (user.Password == password)
            {
                user.Attempt = 0;
                _context.SaveChanges();
                return RedirectToAction("Purchasing");
            }
            else
            {
                user.Attempt += 1;
                _context.SaveChanges();
                int remainingStrikes = 3 - user.Attempt;

                if (user.Attempt >= 3)
                {
                    ViewBag.Error = "You have entered an incorrect password 3 times. This profile is now blocked.";
                }
                else
                {
                    ViewBag.Error = $"Incorrect password. You have {remainingStrikes} attempts remaining before structural block.";
                }

                return View();
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}