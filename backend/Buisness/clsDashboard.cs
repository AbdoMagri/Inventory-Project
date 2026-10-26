using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsDashboard
    {
        // UserID = null -> whole account (Admin/Manager). UserID = x -> only that user's sales (Cashier).
        public static List<Dictionary<string, object?>> GetSummary(int AccountID, int? UserID = null)
        {
            return clsDashboardData.GetSummary(AccountID, UserID);
        }

        public static List<Dictionary<string, object?>> GetSalesByDay(int AccountID, int? UserID = null, int Days = 7)
        {
            return clsDashboardData.GetSalesByDay(AccountID, UserID, Days);
        }

        public static List<Dictionary<string, object?>> GetTopProducts(int AccountID, int? UserID = null, int Days = 30, int Top = 5)
        {
            return clsDashboardData.GetTopProducts(AccountID, UserID, Days, Top);
        }

        public static List<Dictionary<string, object?>> GetRecentSales(int AccountID, int? UserID = null, int Top = 10)
        {
            return clsDashboardData.GetRecentSales(AccountID, UserID, Top);
        }

        public static List<clsProductData.LowStockProductDTO> GetLowStockProducts(int AccountID)
        {
            return clsProduct.GetLowStockProducts(AccountID);
        }
    }
}