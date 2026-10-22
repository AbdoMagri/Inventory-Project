using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryDB_DataAccess
{
    public class clsDashboardData
    {
        private static DataTable _Execute(string ProcName, int AccountID, int? UserID, int? Days, int? Top)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            SqlCommand command = new SqlCommand(ProcName, connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@AccountID", AccountID);
            command.Parameters.AddWithValue("@UserID", (object)UserID ?? DBNull.Value);
            if (Days.HasValue) command.Parameters.AddWithValue("@Days", Days.Value);
            if (Top.HasValue) command.Parameters.AddWithValue("@Top", Top.Value);

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

        public static DataTable GetSummary(int AccountID, int? UserID)
        {
            return _Execute("sp_Dashboard_Summary", AccountID, UserID, null, null);
        }

        public static DataTable GetSalesByDay(int AccountID, int? UserID, int Days)
        {
            return _Execute("sp_Dashboard_SalesByDay", AccountID, UserID, Days, null);
        }

        public static DataTable GetTopProducts(int AccountID, int? UserID, int Days, int Top)
        {
            return _Execute("sp_Dashboard_TopProducts", AccountID, UserID, Days, Top);
        }

        public static DataTable GetRecentSales(int AccountID, int? UserID, int Top)
        {
            return _Execute("sp_Dashboard_RecentSales", AccountID, UserID, null, Top);
        }
    }
}