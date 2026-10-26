using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsPurchaseData
    {
        public class PurchaseDTO
        {
            public PurchaseDTO(int purchaseID, int accountID, int supplierID, int userID, DateTime purchaseDate,
                decimal total, List<Dictionary<string, object?>> items)
            {
                PurchaseID = purchaseID; AccountID = accountID; SupplierID = supplierID; UserID = userID;
                PurchaseDate = purchaseDate; Total = total; Items = items;
            }
            
            public int PurchaseID { get; set; }
            public int AccountID { get; set; }
            public int SupplierID { get; set; }
            public int UserID { get; set; }
            public DateTime PurchaseDate { get; set; }
            public decimal Total { get; set; }
            public List<Dictionary<string, object?>> Items { get; set; }
            public string SupplierName { get; set; }
            public string UserName { get; set; }
        }
        public class PurchaseListDTO
        {
            public int PurchaseID { get; set; }
            public string SupplierName { get; set; }
            public string UserName { get; set; }
            public DateTime PurchaseDate { get; set; }
            public decimal Total { get; set; }

            public PurchaseListDTO(int purchaseID, string supplierName,
                string userName, DateTime purchaseDate, decimal total)
            {
                PurchaseID = purchaseID;
                SupplierName = supplierName;
                UserName = userName;
                PurchaseDate = purchaseDate;
                Total = total;
            }
        }
        public static bool GetPurchaseInfoByID(int PurchaseID, ref int AccountID, ref int SupplierID,
            ref int UserID, ref DateTime PurchaseDate, ref decimal Total)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Purchases WHERE PurchaseID = @PurchaseID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PurchaseID", PurchaseID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    AccountID = (int)reader["AccountID"];
                    SupplierID = (int)reader["SupplierID"];
                    UserID = (int)reader["UserID"];
                    PurchaseDate = (DateTime)reader["PurchaseDate"];
                    Total = (decimal)reader["Total"];
                }
                reader.Close();
            }
            catch (Exception){ isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        //public static int AddNewPurchase(int AccountID, int SupplierID, int UserID,
        //    DateTime PurchaseDate, decimal Total)
        //{
        //    int PurchaseID = -1;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = @"INSERT INTO Purchases (AccountID, SupplierID, UserID, PurchaseDate, Total)
        //                     VALUES (@AccountID, @SupplierID, @UserID, @PurchaseDate, @Total);
        //                     SELECT SCOPE_IDENTITY();";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@AccountID", AccountID);
        //    command.Parameters.AddWithValue("@SupplierID", SupplierID);
        //    command.Parameters.AddWithValue("@UserID", UserID);
        //    command.Parameters.AddWithValue("@PurchaseDate", PurchaseDate);
        //    command.Parameters.AddWithValue("@Total", Total);
        //    try
        //    {
        //        connection.Open();
        //        object result = command.ExecuteScalar();
        //        if (result != null && int.TryParse(result.ToString(), out int insertedID))
        //            PurchaseID = insertedID;
        //    }
        //    catch (Exception){ throw;}
        //    finally { connection.Close(); }
        //    return PurchaseID;
        //}

        //public static bool UpdatePurchase(int PurchaseID, int SupplierID, DateTime PurchaseDate, decimal Total)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = @"UPDATE Purchases
        //                     SET SupplierID = @SupplierID,
        //                         PurchaseDate = @PurchaseDate,
        //                         Total = @Total
        //                     WHERE PurchaseID = @PurchaseID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@PurchaseID", PurchaseID);
        //    command.Parameters.AddWithValue("@SupplierID", SupplierID);
        //    command.Parameters.AddWithValue("@PurchaseDate", PurchaseDate);
        //    command.Parameters.AddWithValue("@Total", Total);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception){ return false; }
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

        public static List<PurchaseListDTO> GetAllPurchases(int AccountID)
        {
            List<PurchaseListDTO> purchases = new List<PurchaseListDTO>();

            SqlConnection connection = new SqlConnection(
                clsDataAccessSettings.ConnectionString);

            string query = @"SELECT p.PurchaseID,
                            SupplierName = s.Name,
                            UserName = u.UserName,
                            p.PurchaseDate, p.Total
                     FROM Purchases p
                     INNER JOIN Suppliers s
                        ON p.SupplierID = s.SupplierID
                     INNER JOIN Users u
                        ON p.UserID = u.UserID
                     WHERE p.AccountID = @AccountID
                     ORDER BY p.PurchaseDate DESC";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    purchases.Add(new PurchaseListDTO(
                        (int)reader["PurchaseID"],
                        reader["SupplierName"].ToString(),
                        reader["UserName"].ToString(),
                        (DateTime)reader["PurchaseDate"],
                        (decimal)reader["Total"]
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

            return purchases;
        }

        //public static bool DeletePurchase(int PurchaseID)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = "DELETE Purchases WHERE PurchaseID = @PurchaseID";   // items are removed by ON DELETE CASCADE
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@PurchaseID", PurchaseID);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception){ throw;}
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

        public static bool IsPurchaseExist(int PurchaseID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Purchases WHERE PurchaseID = @PurchaseID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PurchaseID", PurchaseID);
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
        public static int MakePurchase(int AccountID, int UserID, int SupplierID,
    DataTable Items, ref string ErrorMessage)
        {
            int PurchaseID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            SqlCommand command = new SqlCommand("sp_MakePurchase", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@SupplierID", SupplierID);

            SqlParameter itemsParam = command.Parameters.AddWithValue("@Items", Items);
            itemsParam.SqlDbType = SqlDbType.Structured;
            itemsParam.TypeName = "PurchaseItemList";

            SqlParameter purchaseIDParam = new SqlParameter("@PurchaseID", SqlDbType.Int);
            purchaseIDParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(purchaseIDParam);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                PurchaseID = (int)purchaseIDParam.Value;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally { connection.Close(); }

            return PurchaseID;
        }
    }
}