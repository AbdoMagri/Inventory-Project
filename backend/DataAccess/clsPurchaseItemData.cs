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

        public static List<Dictionary<string, object?>> GetItemsByPurchaseID(int PurchaseID)
        {
            List<Dictionary<string, object?>> items = new List<Dictionary<string, object?>>();
            
            string query = @"SELECT pi.PurchaseItemID, pi.ProductID, ProductName = p.Name, p.SKU,
                                    pi.Qty, pi.UnitCost, LineTotal = pi.Qty * pi.UnitCost
                             FROM PurchaseItems pi
                             INNER JOIN Products p ON pi.ProductID = p.ProductID
                             WHERE pi.PurchaseID = @PurchaseID";
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PurchaseID", PurchaseID);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, object?> item = new Dictionary<string, object?>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            item[reader.GetName(i)] =
                                reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }

                        items.Add(item);
                    }
                }
            }

            return items;
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