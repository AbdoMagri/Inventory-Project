using System;
using System.Collections.Generic;

namespace InventoryDB_Business
{
    public static class clsPermissions
    {
        public enum enRole { Admin = 1, Manager = 2, Cashier = 3, Storekeeper = 4 }

        public enum enPermission
        {
            ManageUsers, EditAccount, ManageProducts, ManageCategories, ManageSuppliers, ManageCustomers,
            MakeSale, MakePurchase, AdjustStock,
            ViewAllSales, ViewPurchases, ViewStockLog, ViewCostAndProfit
        }

        private static readonly Dictionary<enRole, HashSet<enPermission>> _Map =
            new Dictionary<enRole, HashSet<enPermission>>
        {
            { enRole.Admin, new HashSet<enPermission> {
    enPermission.ManageUsers, enPermission.EditAccount, enPermission.ManageProducts, enPermission.ManageCategories,
    enPermission.ManageSuppliers, enPermission.ManageCustomers, enPermission.MakeSale,
    enPermission.MakePurchase, enPermission.AdjustStock, enPermission.ViewAllSales,
    enPermission.ViewPurchases, enPermission.ViewStockLog, enPermission.ViewCostAndProfit } },

            { enRole.Manager, new HashSet<enPermission> {
                enPermission.ManageProducts, enPermission.ManageCategories, enPermission.ManageSuppliers,
                enPermission.ManageCustomers, enPermission.MakeSale, enPermission.MakePurchase,
                enPermission.AdjustStock, enPermission.ViewAllSales, enPermission.ViewPurchases,
                enPermission.ViewStockLog, enPermission.ViewCostAndProfit } },

            { enRole.Cashier, new HashSet<enPermission> {
                enPermission.ManageCustomers, enPermission.MakeSale } },

            { enRole.Storekeeper, new HashSet<enPermission> {
                enPermission.ManageProducts, enPermission.ManageCategories, enPermission.ManageSuppliers,
                enPermission.MakePurchase, enPermission.AdjustStock, enPermission.ViewPurchases,
                enPermission.ViewStockLog } }
        };

        public static bool HasPermission(int RoleID, enPermission Permission)
        {
            enRole Role = (enRole)RoleID;
            return _Map.ContainsKey(Role) && _Map[Role].Contains(Permission);
        }
    }
}