using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POS_Pharmacy.Data;
using POS_Pharmacy.Models;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace POS_Pharmacy.Controllers
{
    [Route("Drug")]
    public class DrugController : Controller
    {
        private readonly PharmacyDatabase _db;
        private readonly string _connectionString;

        public DrugController(PharmacyDatabase db)
        {
            _db = db;
            _connectionString = _db.Database.GetConnectionString();
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(string itemCode, string category, string genericName, string brandName, string dose)
        {
            if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(genericName) || string.IsNullOrWhiteSpace(brandName))
            {
                ModelState.AddModelError("", "Required fields cannot be empty.");
                return View();
            }

            try
            {
                var register = new DrugRegister(_db);
                bool success = await register.RegisterDrugTransactionAsync(category, genericName, brandName, dose);

                if (success)
                {
                    TempData["SuccessMessage"] = "Drug registered successfully!";
                    return RedirectToAction("Create");
                }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError("", "Save Failed: " + innerMessage);
            }
            return View();
        }

        [HttpPost]
        [Route("UploadDrugFile")] 
        public async Task<IActionResult> UploadExcel(IFormFile drugFile)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (drugFile != null && drugFile.Length > 0)
            {
                try
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    var filePath = Path.Combine(uploadPath, drugFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await drugFile.CopyToAsync(stream);
                    }

                    var register = new DrugRegister(_db);
                    using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            do
                            {
                                bool isHeaderSkipped = false;
                                while (reader.Read())
                                {
                                    if (!isHeaderSkipped)
                                    {
                                        isHeaderSkipped = true;
                                        continue;
                                    }
                                    if (reader.GetValue(0) == null || string.IsNullOrWhiteSpace(reader.GetValue(0).ToString()))
                                    {
                                        continue;
                                    }
                                    string itemCode = reader.GetValue(0)?.ToString()?.Trim() ?? "";
                                    string category = reader.GetValue(1)?.ToString()?.Trim() ?? "";
                                    string genericName = reader.GetValue(2)?.ToString()?.Trim() ?? "";
                                    string brandName = reader.GetValue(3)?.ToString()?.Trim() ?? "";
                                    string dose = reader.GetValue(4)?.ToString()?.Trim() ?? "";

                                    await register.RegisterDrugTransactionAsync(category, genericName, brandName, dose);
                                }
                            }
                            while (reader.NextResult());
                        }
                    }

                    TempData["SuccessMessage"] = "Document imported successfully!";
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError("", "Import Failed: " + innerMessage);
                }
            }
            else
            {
                ModelState.AddModelError("", "No file selected for upload.");
            }
            return View("Create");
        }
    }
}