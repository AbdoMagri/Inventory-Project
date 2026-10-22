using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsDashboard
    {
        // UserID = null -> whole account (Admin/Manager). UserID = x -> only that user's sales (Cashier).
        public static DataTable GetSummary(int AccountID, int? UserID = null)
        {
            return clsDashboardData.GetSummary(AccountID, UserID);
        }

        public static DataTable GetSalesByDay(int AccountID, int? UserID = null, int Days = 7)
        {
            return clsDashboardData.GetSalesByDay(AccountID, UserID, Days);
        }

        public static DataTable GetTopProducts(int AccountID, int? UserID = null, int Days = 30, int Top = 5)
        {
            return clsDashboardData.GetTopProducts(AccountID, UserID, Days, Top);
        }

        public static DataTable GetRecentSales(int AccountID, int? UserID = null, int Top = 10)
        {
            return clsDashboardData.GetRecentSales(AccountID, UserID, Top);
        }

        public static DataTable GetLowStockProducts(int AccountID)
        {
            return clsProduct.GetLowStockProducts(AccountID);
        }
    }
}