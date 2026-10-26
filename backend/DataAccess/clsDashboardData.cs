using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsDashboardData
    {
        private static List<Dictionary<string, object?>> _Execute(
            string ProcName, int AccountID, int? UserID, int? Days, int? Top)
        {
            List<Dictionary<string, object?>> results =
                new List<Dictionary<string, object?>>();

            using (SqlConnection connection =
                new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(ProcName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@AccountID", AccountID);
                command.Parameters.AddWithValue("@UserID",
                    (object?)UserID ?? DBNull.Value);

                if (Days.HasValue)
                    command.Parameters.AddWithValue("@Days", Days.Value);

                if (Top.HasValue)
                    command.Parameters.AddWithValue("@Top", Top.Value);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, object?> row =
                            new Dictionary<string, object?>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] =
                                reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }

                        results.Add(row);
                    }
                }
            }

            return results;
        }

        public static List<Dictionary<string, object?>> GetSummary(
            int AccountID, int? UserID)
        {
            return _Execute(
                "sp_Dashboard_Summary", AccountID, UserID, null, null);
        }

        public static List<Dictionary<string, object?>> GetSalesByDay(
            int AccountID, int? UserID, int Days)
        {
            return _Execute(
                "sp_Dashboard_SalesByDay", AccountID, UserID, Days, null);
        }

        public static List<Dictionary<string, object?>> GetTopProducts(
            int AccountID, int? UserID, int Days, int Top)
        {
            return _Execute(
                "sp_Dashboard_TopProducts", AccountID, UserID, Days, Top);
        }

        public static List<Dictionary<string, object?>> GetRecentSales(
            int AccountID, int? UserID, int Top)
        {
            return _Execute(
                "sp_Dashboard_RecentSales", AccountID, UserID, null, Top);
        }
    }
}