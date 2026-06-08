using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POS_Pharmacy.Data;
using POS_Pharmacy.Models;

namespace POS_Pharmacy.Controllers
{
    public class DrugRegister
    {
        private readonly PharmacyDatabase _db;

        public DrugRegister(PharmacyDatabase db)
        {
            _db = db;
        }

        public async Task<bool> RegisterDrugTransactionAsync(string category, string genericName, string brandName, string dose)
        {
            var connection = (SqlConnection)_db.Database.GetDbConnection();

            // CATEGORY INFORMATION
            string query = "select * from category_information where category_name='" + category + "'";

            SqlCommand com = new SqlCommand(query, connection);

            connection.Open();
            DataTable tb = new DataTable();
            tb.Load(com.ExecuteReader());
            connection.Close();

            string category_id = "";
            if (tb.Rows.Count > 0)
            {
                category_id = tb.Rows[0]["category_id"].ToString();
                System.Diagnostics.Debug.WriteLine("Category exists: " + category);
            }
            else

            {
                System.Diagnostics.Debug.WriteLine("Category not found: " + category);

                com.CommandText = "INSERT INTO category_information (category_name) OUTPUT INSERTED.category_id VALUES ('" + category + "')";

                connection.Open();
                category_id = com.ExecuteScalar().ToString();
                connection.Close();
            }

            // DRUG INFORMATION
            string drugQuery = "select * from generic_drug_info where generic_name='" + genericName + "'";
            com.CommandText = drugQuery;

            connection.Open();
            DataTable tbDrug = new DataTable();
            tbDrug.Load(com.ExecuteReader());
            connection.Close();

            string generic_id = "";
            if (tbDrug.Rows.Count > 0)
            {
                generic_id = tbDrug.Rows[0]["generic_id"].ToString();
                System.Diagnostics.Debug.WriteLine("Drug exists: " + genericName);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Drug not found: " + genericName);
                com.CommandText = "INSERT INTO generic_drug_info (generic_name, category_id) OUTPUT INSERTED.generic_id VALUES ('" + genericName + "', (SELECT TOP 1 category_id FROM category_information WHERE category_name = '" + category + "'))";
               
                connection.Open();
                generic_id = com.ExecuteScalar().ToString();
                connection.Close();
            }

            // BRAND INFORMATION
            string Brandquery = "select * from brand_info where brand_name='" + brandName + "'";
            com.CommandText = Brandquery;

            connection.Open();
            DataTable tbBrand = new DataTable();
            tbBrand.Load(com.ExecuteReader());
            connection.Close();

            string brand_id = "";
            if (tbBrand.Rows.Count > 0)
            {
                brand_id = tbBrand.Rows[0]["brand_id"].ToString();
                System.Diagnostics.Debug.WriteLine("Brand exists: " + brandName);
            } 
            else
            {
                System.Diagnostics.Debug.WriteLine("Brand not found: " + brandName);
                com.CommandText = "INSERT INTO brand_info (brand_name, generic_id) OUTPUT INSERTED.brand_id VALUES ('" + brandName + "', (SELECT TOP 1 generic_id FROM generic_drug_info WHERE generic_name = '" + genericName + "'))";
                
                connection.Open();
                brand_id = com.ExecuteScalar().ToString();
                connection.Close();
            }

            // STOCK INFORMATION
            string Stockquery = "select * from stock_info where brand_id=(SELECT TOP 1 brand_id FROM brand_info WHERE brand_name = '" + brandName + "') AND dose='" + dose + "'";
            com.CommandText = Stockquery;

            connection.Open();
            DataTable tbStock = new DataTable();
            tbStock.Load(com.ExecuteReader());
            connection.Close();

            string stock_id = "";
            if (tbStock.Rows.Count > 0)
            {
                stock_id = tbStock.Rows[0]["stock_id"].ToString();
                System.Diagnostics.Debug.WriteLine("Stock exists for: " + brandName + " " + dose);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Stock not found for: " + brandName + " " + dose);
                com.CommandText = "INSERT INTO stock_info(brand_id, cost_price, sell_price, quantity, reorder_level, dose) OUTPUT INSERTED.stock_id VALUES ((SELECT TOP 1 brand_id FROM brand_info WHERE brand_name = '" + brandName + "'), 0, 0, 0, 0, '" + dose + "')";
                
                connection.Open();
                stock_id = com.ExecuteScalar().ToString();
                connection.Close();
            }

            System.Diagnostics.Debug.WriteLine(tb.Rows.Count.ToString());
            return true;
        }
    }
}