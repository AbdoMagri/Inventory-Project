using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsSaleItemData
    {
        public static bool GetSaleItemInfoByID(int SaleItemID, ref int SaleID, ref int ProductID,
            ref int Qty, ref decimal UnitPrice, ref decimal UnitCost)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM SaleItems WHERE SaleItemID = @SaleItemID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SaleItemID", SaleItemID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    SaleID = (int)reader["SaleID"];
                    ProductID = (int)reader["ProductID"];
                    Qty = (int)reader["Qty"];
                    UnitPrice = (decimal)reader["UnitPrice"];
                    UnitCost = (decimal)reader["UnitCost"];
                }
                reader.Close();
            }
            catch (Exception ex) { isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        //public static int AddNewSaleItem(int SaleID, int ProductID, int Qty, decimal UnitPrice, decimal UnitCost)
        //{
        //    int SaleItemID = -1;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = @"INSERT INTO SaleItems (SaleID, ProductID, Qty, UnitPrice, UnitCost)
        //                     VALUES (@SaleID, @ProductID, @Qty, @UnitPrice, @UnitCost);
        //                     SELECT SCOPE_IDENTITY();";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@SaleID", SaleID);
        //    command.Parameters.AddWithValue("@ProductID", ProductID);
        //    command.Parameters.AddWithValue("@Qty", Qty);
        //    command.Parameters.AddWithValue("@UnitPrice", UnitPrice);
        //    command.Parameters.AddWithValue("@UnitCost", UnitCost);
        //    try
        //    {
        //        connection.Open();
        //        object result = command.ExecuteScalar();
        //        if (result != null && int.TryParse(result.ToString(), out int insertedID))
        //            SaleItemID = insertedID;
        //    }
        //    catch (Exception ex) { }
        //    finally { connection.Close(); }
        //    return SaleItemID;
        //}

        //public static bool UpdateSaleItem(int SaleItemID, int ProductID, int Qty, decimal UnitPrice, decimal UnitCost)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = @"UPDATE SaleItems
        //                     SET ProductID = @ProductID,
        //                         Qty = @Qty,
        //                         UnitPrice = @UnitPrice,
        //                         UnitCost = @UnitCost
        //                     WHERE SaleItemID = @SaleItemID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@SaleItemID", SaleItemID);
        //    command.Parameters.AddWithValue("@ProductID", ProductID);
        //    command.Parameters.AddWithValue("@Qty", Qty);
        //    command.Parameters.AddWithValue("@UnitPrice", UnitPrice);
        //    command.Parameters.AddWithValue("@UnitCost", UnitCost);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception ex) { return false; }
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}

        public static DataTable GetItemsBySaleID(int SaleID)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT si.SaleItemID, si.ProductID, ProductName = p.Name, p.SKU,
                                    si.Qty, si.UnitPrice, LineTotal = si.Qty * si.UnitPrice,
                                    Profit = si.Qty * (si.UnitPrice - si.UnitCost)
                             FROM SaleItems si
                             INNER JOIN Products p ON si.ProductID = p.ProductID
                             WHERE si.SaleID = @SaleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SaleID", SaleID);
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

        //public static bool DeleteSaleItem(int SaleItemID)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = "DELETE SaleItems WHERE SaleItemID = @SaleItemID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@SaleItemID", SaleItemID);
        //    try
        //    {
        //        connection.Open();
        //        rowsAffected = command.ExecuteNonQuery();
        //    }
        //    catch (Exception ex) { }
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}
    }
}