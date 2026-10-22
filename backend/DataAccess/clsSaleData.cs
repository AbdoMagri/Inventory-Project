using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsSaleData
    {
        public static bool GetSaleInfoByID(int SaleID, ref int AccountID, ref int? CustomerID,
            ref int UserID, ref DateTime SaleDate, ref decimal Total)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Sales WHERE SaleID = @SaleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SaleID", SaleID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    AccountID = (int)reader["AccountID"];
                    CustomerID = reader["CustomerID"] != DBNull.Value ? (int?)reader["CustomerID"] : null;
                    UserID = (int)reader["UserID"];
                    SaleDate = (DateTime)reader["SaleDate"];
                    Total = (decimal)reader["Total"];
                }
                reader.Close();
            }
            catch (Exception ex) { isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        public static int AddNewSale(int AccountID, int? CustomerID, int UserID,
            DateTime SaleDate, decimal Total)
        {
            int SaleID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Sales (AccountID, CustomerID, UserID, SaleDate, Total)
                             VALUES (@AccountID, @CustomerID, @UserID, @SaleDate, @Total);
                             SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@CustomerID", (object)CustomerID ?? DBNull.Value);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@SaleDate", SaleDate);
            command.Parameters.AddWithValue("@Total", Total);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    SaleID = insertedID;
            }
            catch (Exception ex) { }
            finally { connection.Close(); }
            return SaleID;
        }

        public static bool UpdateSale(int SaleID, int? CustomerID, DateTime SaleDate, decimal Total)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE Sales
                             SET CustomerID = @CustomerID,
                                 SaleDate = @SaleDate,
                                 Total = @Total
                             WHERE SaleID = @SaleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SaleID", SaleID);
            command.Parameters.AddWithValue("@CustomerID", (object)CustomerID ?? DBNull.Value);
            command.Parameters.AddWithValue("@SaleDate", SaleDate);
            command.Parameters.AddWithValue("@Total", Total);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { return false; }
            finally { connection.Close(); }
            return (rowsAffected > 0);
        }

        public static DataTable GetAllSales(int AccountID)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT s.SaleID, CustomerName = ISNULL(c.Name, 'Walk-in'), UserName = u.UserName,
                                    s.SaleDate, s.Total
                             FROM Sales s
                             LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                             INNER JOIN Users u ON s.UserID = u.UserID
                             WHERE s.AccountID = @AccountID
                             ORDER BY s.SaleDate DESC";
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

        //public static bool DeleteSale(int SaleID)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = "DELETE Sales WHERE SaleID = @SaleID";   // items are removed by ON DELETE CASCADE
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@SaleID", SaleID);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception ex) { }
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

        public static bool IsSaleExist(int SaleID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Sales WHERE SaleID = @SaleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SaleID", SaleID);
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
        public static int MakeSale(int AccountID, int UserID, int? CustomerID,
    DataTable Items, ref string ErrorMessage)
        {
            int SaleID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            SqlCommand command = new SqlCommand("sp_MakeSale", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@CustomerID", (object)CustomerID ?? DBNull.Value);

            SqlParameter itemsParam = command.Parameters.AddWithValue("@Items", Items);
            itemsParam.SqlDbType = SqlDbType.Structured;
            itemsParam.TypeName = "SaleItemList";

            SqlParameter saleIDParam = new SqlParameter("@SaleID", SqlDbType.Int);
            saleIDParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(saleIDParam);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                SaleID = (int)saleIDParam.Value;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;   // e.g. "Insufficient stock for one or more products."
            }
            finally { connection.Close(); }

            return SaleID;
        }
    }
}