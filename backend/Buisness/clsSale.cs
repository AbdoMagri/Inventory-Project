using InventoryDB_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using static InventoryDB_DataAccess.clsSaleData;

namespace InventoryDB_Business
{
    public class clsSale
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public clsSaleData.SaleDTO DTO
        {
            get
            {
                return new clsSaleData.SaleDTO(this.SaleID, this.AccountID,
                    this.CustomerID,
                    this.UserID, 
                    this.SaleDate, this.Total, this.GetItems());
            }
        }
        public int SaleID { set; get; }
        public int AccountID { set; get; }
        public int? CustomerID { set; get; }      // null = walk-in customer
        public clsCustomer CustomerInfo;
        public int UserID { set; get; }
        public clsUser UserInfo;
        public DateTime SaleDate { set; get; }
        public decimal Total { set; get; }

        public clsSale()
        {
            this.SaleID = -1;
            this.CustomerID = null;
            this.SaleDate = DateTime.Now;
            this.Total = 0;
            Mode = enMode.AddNew;
        }

        private clsSale(int SaleID, int AccountID, int? CustomerID, int UserID,
            DateTime SaleDate, decimal Total)
        {
            this.SaleID = SaleID;
            this.AccountID = AccountID;
            this.CustomerID = CustomerID;
            this.CustomerInfo = CustomerID.HasValue ? clsCustomer.Find(CustomerID.Value) : null;
            this.UserID = UserID;
            this.UserInfo = clsUser.FindByUserID(UserID);
            this.SaleDate = SaleDate;
            this.Total = Total;
            Mode = enMode.Update;
        }

        //private bool _AddNewSale()
        //{
        //    this.SaleID = clsSaleData.AddNewSale(this.AccountID, this.CustomerID,
        //        this.UserID, this.SaleDate, this.Total);
        //    return (this.SaleID != -1);
        //}

        //private bool _UpdateSale()
        //{
        //    return clsSaleData.UpdateSale(this.SaleID, this.CustomerID, this.SaleDate, this.Total);
        //}

        //public static clsSale Find(int SaleID)
        //{
        //    int AccountID = -1, UserID = -1;
        //    int? CustomerID = null;
        //    DateTime SaleDate = DateTime.Now;
        //    decimal Total = 0;

        //    if (clsSaleData.GetSaleInfoByID(SaleID, ref AccountID, ref CustomerID,
        //        ref UserID, ref SaleDate, ref Total))
        //        return new clsSale(SaleID, AccountID, CustomerID, UserID, SaleDate, Total);
        //    else
        //        return null;
        //}

        //public bool Save()
        //{
        //    switch (Mode)
        //    {
        //        case enMode.AddNew:
        //            if (_AddNewSale())
        //            {
        //                Mode = enMode.Update;
        //                return true;
        //            }
        //            else
        //                return false;

        //        case enMode.Update:
        //            return _UpdateSale();
        //    }
        //    return false;
        //}

        public static clsSale Find(int SaleID)
        {
            int AccountID = -1,  UserID = -1;
            int? CustomerID = null;
            DateTime SaleDate = DateTime.Now;
            decimal Total = 0;

            if (clsSaleData.GetSaleInfoByID(SaleID, ref AccountID, ref CustomerID,
                ref UserID, ref SaleDate, ref Total))
                return new clsSale(SaleID, AccountID, CustomerID, UserID, SaleDate, Total);
            else
                return null;
        }

        public List<Dictionary<string, object?>> GetItems()
        {
            return clsSaleItem.GetItemsBySaleID(this.SaleID);
        }

        public static List<SaleDTO> GetAllSales(int AccountID)
        {
            return clsSaleData.GetAllSales(AccountID);
        }

        public static bool isSaleExist(int SaleID)
        {
            return clsSaleData.IsSaleExist(SaleID);
        }
        public static int MakeSale(int AccountID, int UserID, int? CustomerID,
    List<clsSaleItem> Items, ref string ErrorMessage)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductID", typeof(int));    // same order as the SaleItemList type
            dt.Columns.Add("Qty", typeof(int));
            dt.Columns.Add("UnitPrice", typeof(decimal));

            foreach (clsSaleItem Item in Items)
                dt.Rows.Add(Item.ProductID, Item.Qty, Item.UnitPrice);

            return clsSaleData.MakeSale(AccountID, UserID, CustomerID, dt, ref ErrorMessage);
        }
    }
}