using System;
using Microsoft.EntityFrameworkCore;
using POS_Pharmacy.Models;

namespace POS_Pharmacy.Data
{
    public class PharmacyDatabase : DbContext
    {
        public PharmacyDatabase(DbContextOptions<PharmacyDatabase> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Drug> Drugs { get; set; }
        public DbSet<BrandInfo> Brands { get; set; }
        public DbSet<StockInfo> Stocks { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseInfo> Purchases { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

    }
}