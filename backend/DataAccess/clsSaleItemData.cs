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
            catch (Exception){ isFound = false; }
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
        //    catch (Exception){ throw;}
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
        //    catch (Exception){ return false; }
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}


        public static List<Dictionary<string, object?>> GetItemsBySaleID(int SaleID)
        {
            List<Dictionary<string, object?>> items = new List<Dictionary<string, object?>>();

            string query = @"SELECT si.SaleItemID, si.ProductID, ProductName = p.Name, p.SKU,
                            si.Qty, si.UnitPrice, LineTotal = si.Qty * si.UnitPrice,
                            Profit = si.Qty * (si.UnitPrice - si.UnitCost)
                     FROM SaleItems si
                     INNER JOIN Products p ON si.ProductID = p.ProductID
                     WHERE si.SaleID = @SaleID";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SaleID", SaleID);

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
        //    catch (Exception){ throw;}
        //    finally { connection.Close(); }
        //    return (rowsAffected > 0);
        //}
    }
}