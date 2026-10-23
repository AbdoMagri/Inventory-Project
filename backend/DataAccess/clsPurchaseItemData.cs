using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsPurchaseItemData
    {
        public static bool GetPurchaseItemInfoByID(int PurchaseItemID, ref int PurchaseID,
            ref int ProductID, ref int Qty, ref decimal UnitCost)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM PurchaseItems WHERE PurchaseItemID = @PurchaseItemID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PurchaseItemID", PurchaseItemID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    PurchaseID = (int)reader["PurchaseID"];
                    ProductID = (int)reader["ProductID"];
                    Qty = (int)reader["Qty"];
                    UnitCost = (decimal)reader["UnitCost"];
                }
                reader.Close();
            }
            catch (Exception ex) { isFound = false; }
            finally { connection.Close(); }
            return isFound;
        }

        //public static int AddNewPurchaseItem(int PurchaseID, int ProductID, int Qty, decimal UnitCost)
        //{
        //    int PurchaseItemID = -1;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = @"INSERT INTO PurchaseItems (PurchaseID, ProductID, Qty, UnitCost)
        //                     VALUES (@PurchaseID, @ProductID, @Qty, @UnitCost);
        //                     SELECT SCOPE_IDENTITY();";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@PurchaseID", PurchaseID);
        //    command.Parameters.AddWithValue("@ProductID", ProductID);
        //    command.Parameters.AddWithValue("@Qty", Qty);
        //    command.Parameters.AddWithValue("@UnitCost", UnitCost);
        //    try
        //    {
        //        connection.Open();
        //        object result = command.ExecuteScalar();
        //        if (result != null && int.TryParse(result.ToString(), out int insertedID))
        //            PurchaseItemID = insertedID;
        //    }
        //    catch (Exception ex) { }
        //    finally { connection.Close(); }
        //    return PurchaseItemID;
        //}

        //public static bool UpdatePurchaseItem(int PurchaseItemID, int ProductID, int Qty, decimal UnitCost)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = @"UPDATE PurchaseItems
        //                     SET ProductID = @ProductID,
        //                         Qty = @Qty,
        //                         UnitCost = @UnitCost
        //                     WHERE PurchaseItemID = @PurchaseItemID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@PurchaseItemID", PurchaseItemID);
        //    command.Parameters.AddWithValue("@ProductID", ProductID);
        //    command.Parameters.AddWithValue("@Qty", Qty);
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

        public static DataTable GetItemsByPurchaseID(int PurchaseID)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT pi.PurchaseItemID, pi.ProductID, ProductName = p.Name, p.SKU,
                                    pi.Qty, pi.UnitCost, LineTotal = pi.Qty * pi.UnitCost
                             FROM PurchaseItems pi
                             INNER JOIN Products p ON pi.ProductID = p.ProductID
                             WHERE pi.PurchaseID = @PurchaseID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PurchaseID", PurchaseID);
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

        //public static bool DeletePurchaseItem(int PurchaseItemID)
        //{
        //    int rowsAffected = 0;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = "DELETE PurchaseItems WHERE PurchaseItemID = @PurchaseItemID";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@PurchaseItemID", PurchaseItemID);
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