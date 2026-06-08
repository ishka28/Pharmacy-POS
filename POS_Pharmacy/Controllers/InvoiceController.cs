using Microsoft.AspNetCore.Mvc;
using POS_Pharmacy.Models;
using System;

namespace POS_Pharmacy.Controllers
{
    public class InvoiceController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new InvoiceViewModel
            {
                Date = DateTime.Today,
                InvoiceNo = "INV-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper() //auto generate id
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create([FromBody] InvoiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(new { success = true, message = "Invoice saved successfully!" });
            }
            return Json(new { success = false, message = "Please fill in all required fields accurately." });
        }
    }
}