using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsProductData
    {
        public class ProductDTO
        {
            public ProductDTO(int productID, int accountID, int? categoryID, int unitID, string name,
                string sku, decimal costPrice, decimal sellPrice, int minStock, int quantity,
                bool isActive, string imagePath)
            {
                ProductID = productID; AccountID = accountID; CategoryID = categoryID; UnitID = unitID;
                Name = name; SKU = sku; CostPrice = costPrice; SellPrice = sellPrice;
                MinStock = minStock; Quantity = quantity; IsActive = isActive; ImagePath = imagePath;
            }
           
            public int ProductID { get; set; }
            public int AccountID { get; set; }
            public int? CategoryID { get; set; }
            public string CategoryName { get; set; }
            public string UnitName { get; set; }
            public int UnitID { get; set; }
            public string Name { get; set; }
            public string SKU { get; set; }
            public decimal CostPrice { get; set; }
            public decimal SellPrice { get; set; }
            public int MinStock { get; set; }
            public int Quantity { get; set; }
            public bool IsActive { get; set; }
            public string ImagePath { get; set; }
        }
        public class ProductListDTO
        {
            public int ProductID { get; set; }
            public string Name { get; set; }
            public string SKU { get; set; }
            public string CategoryName { get; set; }
            public string UnitName { get; set; }
            public decimal CostPrice { get; set; }
            public decimal SellPrice { get; set; }
            public int MinStock { get; set; }
            public int Quantity { get; set; }
            public bool IsActive { get; set; }
            public string ImagePath { get; set; }

            public ProductListDTO(
                int productID,
                string name,
                string sku,
                string categoryName,
                string unitName,
                decimal costPrice,
                decimal sellPrice,
                int minStock,
                int quantity,
                bool isActive,
                string imagePath)
            {
                ProductID = productID;
                Name = name;
                SKU = sku;
                CategoryName = categoryName;
                UnitName = unitName;
                CostPrice = costPrice;
                SellPrice = sellPrice;
                MinStock = minStock;
                Quantity = quantity;
                IsActive = isActive;
                ImagePath = imagePath;
            }
        }
        public class LowStockProductDTO
        {
            public int ProductID { get; set; }
            public string Name { get; set; }
            public string SKU { get; set; }
            public int MinStock { get; set; }
            public int Quantity { get; set; }

            public LowStockProductDTO(int productID, string name, string sku,
                int minStock, int quantity)
            {
                ProductID = productID;
                Name = name;
                SKU = sku;
                MinStock = minStock;
                Quantity = quantity;
            }
        }
        public static bool GetProductInfoByID(int ProductID, ref int AccountID, ref int? CategoryID,
            ref int UnitID, ref string Name, ref string SKU, ref decimal CostPrice,
            ref decimal SellPrice, ref int MinStock, ref int Quantity, ref bool IsActive, ref string ImagePath)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Products WHERE ProductID = @ProductID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductID", ProductID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    AccountID = (int)reader["AccountID"];
                    CategoryID = reader["CategoryID"] != DBNull.Value ? (int?)reader["CategoryID"] : null;
                    UnitID = (int)reader["UnitID"];
                    Name = (string)reader["Name"];
                    SKU = (string)reader["SKU"];
                    CostPrice = (decimal)reader["CostPrice"];
                    SellPrice = (decimal)reader["SellPrice"];
                    MinStock = (int)reader["MinStock"];
                    Quantity = (int)reader["Quantity"];
                    IsActive = (bool)reader["IsActive"];
                    ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : "";
                }
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        // Used for barcode/SKU scan at the cashier
        public static bool GetProductInfoBySKU(int AccountID, string SKU, ref int ProductID, ref int? CategoryID,
            ref int UnitID, ref string Name, ref decimal CostPrice, ref decimal SellPrice,
            ref int MinStock, ref int Quantity, ref bool IsActive, ref string ImagePath)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Products WHERE AccountID = @AccountID AND SKU = @SKU";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@SKU", SKU);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    ProductID = (int)reader["ProductID"];
                    CategoryID = reader["CategoryID"] != DBNull.Value ? (int?)reader["CategoryID"] : null;
                    UnitID = (int)reader["UnitID"];
                    Name = (string)reader["Name"];
                    CostPrice = (decimal)reader["CostPrice"];
                    SellPrice = (decimal)reader["SellPrice"];
                    MinStock = (int)reader["MinStock"];
                    Quantity = (int)reader["Quantity"];
                    IsActive = (bool)reader["IsActive"];
                    ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : "";
                }
                reader.Close();
            }
            catch (Exception){ isFound = false; throw; }
            finally { connection.Close(); }
            return isFound;
        }

        // Quantity starts at 0. Stock changes only through UpdateProductQuantity + a StockMovement row
        public static int AddNewProduct(int AccountID, int? CategoryID, int UnitID, string Name,
            string SKU, decimal CostPrice, decimal SellPrice, int MinStock, bool IsActive, string ImagePath)
        {
            int ProductID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Products (AccountID, CategoryID, UnitID, Name, SKU, CostPrice, SellPrice, MinStock, IsActive, ImagePath)
                 VALUES (@AccountID, @CategoryID, @UnitID, @Name, @SKU, @CostPrice, @SellPrice, @MinStock, @IsActive, @ImagePath);
                 SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@CategoryID", (object)CategoryID ?? DBNull.Value);
            command.Parameters.AddWithValue("@UnitID", UnitID);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@SKU", SKU);
            command.Parameters.AddWithValue("@CostPrice", CostPrice);
            command.Parameters.AddWithValue("@SellPrice", SellPrice);
            command.Parameters.AddWithValue("@MinStock", MinStock);
            command.Parameters.AddWithValue("@IsActive", IsActive);
            command.Parameters.AddWithValue("@ImagePath",
    string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    ProductID = insertedID;
            }
            catch (Exception){ throw;}
            finally { connection.Close(); }
            return ProductID;
        }

        public static bool UpdateProduct(int ProductID, int? CategoryID, int UnitID, string Name,
            string SKU, decimal CostPrice, decimal SellPrice, int MinStock, bool IsActive, string ImagePath)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE Products
                             SET CategoryID = @CategoryID,
                                 UnitID = @UnitID,
                                 Name = @Name,
                                 SKU = @SKU,
                                 CostPrice = @CostPrice,
                                 SellPrice = @SellPrice,
                                 MinStock = @MinStock,
                                 IsActive = @IsActive,
                                 ImagePath = @ImagePath
                             WHERE ProductID = @ProductID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductID", ProductID);
            command.Parameters.AddWithValue(
    "@CategoryID",
    CategoryID.HasValue ? (object)CategoryID.Value : DBNull.Value
);
            command.Parameters.AddWithValue("@UnitID", UnitID);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@SKU", SKU);
            command.Parameters.AddWithValue("@CostPrice", CostPrice);
            command.Parameters.AddWithValue("@SellPrice", SellPrice);
            command.Parameters.AddWithValue("@MinStock", MinStock);
            command.Parameters.AddWithValue("@IsActive", IsActive);
            command.Parameters.AddWithValue("@ImagePath",
    string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception){ return false; }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        // QtyChange is signed: +10 stock in, -3 stock out
        //public static bool UpdateProductQuantity(int ProductID, int QtyChange)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = "UPDATE Products SET Quantity = Quantity + @QtyChange WHERE ProductID = @ProductID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@ProductID", ProductID);
        //    command.Parameters.AddWithValue("@QtyChange", QtyChange);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception){ return false; }   // CK_Products_Stock fails here if stock would go below 0
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

        public static List<ProductListDTO> GetAllProducts(int AccountID)
        {
            List<ProductListDTO> products = new List<ProductListDTO>();

            SqlConnection connection = new SqlConnection(
                clsDataAccessSettings.ConnectionString);

            string query = @"SELECT p.ProductID, p.Name, p.SKU,
                            CategoryName = c.Name,
                            UnitName = u.Name,
                            p.CostPrice, p.SellPrice, p.MinStock,
                            p.Quantity, p.IsActive, p.ImagePath
                     FROM Products p
                     LEFT JOIN Categories c
                        ON p.CategoryID = c.CategoryID
                     INNER JOIN Units u
                        ON p.UnitID = u.UnitID
                     WHERE p.AccountID = @AccountID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new ProductListDTO(
                        (int)reader["ProductID"],
                        reader["Name"].ToString(),
                        reader["SKU"].ToString(),
                        reader["CategoryName"] == DBNull.Value
                            ? null
                            : reader["CategoryName"].ToString(),
                        reader["UnitName"].ToString(),
                        (decimal)reader["CostPrice"],
                        (decimal)reader["SellPrice"],
                        (int)reader["MinStock"],
                        (int)reader["Quantity"],
                        (bool)reader["IsActive"],
                        reader["ImagePath"] == DBNull.Value
                            ? null
                            : reader["ImagePath"].ToString()
                    ));
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return products;
        }
        public static List<LowStockProductDTO> GetLowStockProducts(int AccountID)
        {
            List<LowStockProductDTO> products = new List<LowStockProductDTO>();

            SqlConnection connection = new SqlConnection(
                clsDataAccessSettings.ConnectionString);

            string query = @"SELECT ProductID, Name, SKU, MinStock, Quantity
                     FROM Products
                     WHERE AccountID = @AccountID
                       AND IsActive = 1
                       AND Quantity <= MinStock";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new LowStockProductDTO(
                        (int)reader["ProductID"],
                        reader["Name"].ToString(),
                        reader["SKU"].ToString(),
                        (int)reader["MinStock"],
                        (int)reader["Quantity"]
                    ));
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return products;
        }
        public static bool DeleteProduct(int ProductID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "DELETE Products WHERE ProductID = @ProductID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductID", ProductID);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception){ throw;}   // fails if the product already has sales/purchases: use IsActive = 0 instead
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static bool IsProductExist(int ProductID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Products WHERE ProductID = @ProductID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductID", ProductID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        public static bool IsSKUExist(int AccountID, string SKU)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Products WHERE AccountID = @AccountID AND SKU = @SKU";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@SKU", SKU);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }
    }
}