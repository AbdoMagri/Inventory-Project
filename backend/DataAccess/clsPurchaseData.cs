using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsPurchaseData
    {
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
            catch (Exception ex) { isFound = false; }
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
        //    catch (Exception ex) { }
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
        //    catch (Exception ex) { return false; }
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

        public static DataTable GetAllPurchases(int AccountID)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT p.PurchaseID, SupplierName = s.Name, UserName = u.UserName,
                                    p.PurchaseDate, p.Total
                             FROM Purchases p
                             INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
                             INNER JOIN Users u ON p.UserID = u.UserID
                             WHERE p.AccountID = @AccountID
                             ORDER BY p.PurchaseDate DESC";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) { dt.Load(reader); }
                reader.Close();
            }
            catch (Exception ex) { }
            finally { connection.Close(); }
            return dt;
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
        //    catch (Exception ex) { }
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
            catch (Exception ex) { isFound = false; }
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