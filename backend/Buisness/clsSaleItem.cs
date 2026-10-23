using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsSaleItem
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int SaleItemID { set; get; }
        public int SaleID { set; get; }
        public int ProductID { set; get; }
        public clsProduct ProductInfo;
        public int Qty { set; get; }
        public decimal UnitPrice { set; get; }
        public decimal UnitCost { set; get; }
        public decimal LineTotal { get { return Qty * UnitPrice; } }
        public decimal Profit { get { return Qty * (UnitPrice - UnitCost); } }

        public clsSaleItem()
        {
            this.SaleItemID = -1;
            this.Qty = 0;
            this.UnitPrice = 0;
            this.UnitCost = 0;
            Mode = enMode.AddNew;
        }

        private clsSaleItem(int SaleItemID, int SaleID, int ProductID, int Qty,
            decimal UnitPrice, decimal UnitCost)
        {
            this.SaleItemID = SaleItemID;
            this.SaleID = SaleID;
            this.ProductID = ProductID;
            this.ProductInfo = clsProduct.Find(ProductID);
            this.Qty = Qty;
            this.UnitPrice = UnitPrice;
            this.UnitCost = UnitCost;
            Mode = enMode.Update;
        }

        //private bool _AddNewSaleItem()
        //{
        //    this.SaleItemID = clsSaleItemData.AddNewSaleItem(this.SaleID, this.ProductID,
        //        this.Qty, this.UnitPrice, this.UnitCost);
        //    return (this.SaleItemID != -1);
        //}

        //private bool _UpdateSaleItem()
        //{
        //    return clsSaleItemData.UpdateSaleItem(this.SaleItemID, this.ProductID,
        //        this.Qty, this.UnitPrice, this.UnitCost);
        //}

        public static clsSaleItem Find(int SaleItemID)
        {
            int SaleID = -1, ProductID = -1, Qty = 0;
            decimal UnitPrice = 0, UnitCost = 0;

            if (clsSaleItemData.GetSaleItemInfoByID(SaleItemID, ref SaleID, ref ProductID,
                ref Qty, ref UnitPrice, ref UnitCost))
                return new clsSaleItem(SaleItemID, SaleID, ProductID, Qty, UnitPrice, UnitCost);
            else
                return null;
        }

        //public bool Save()
        //{
        //    switch (Mode)
        //    {
        //        case enMode.AddNew:
        //            if (_AddNewSaleItem())
        //            {
        //                Mode = enMode.Update;
        //                return true;
        //            }
        //            else
        //                return false;

        //        case enMode.Update:
        //            return _UpdateSaleItem();
        //    }
        //    return false;
        //}

        public static DataTable GetItemsBySaleID(int SaleID)
        {
            return clsSaleItemData.GetItemsBySaleID(SaleID);
        }

        //public static bool DeleteSaleItem(int SaleItemID)
        //{
        //    return clsSaleItemData.DeleteSaleItem(SaleItemID);
        //}
    }
}