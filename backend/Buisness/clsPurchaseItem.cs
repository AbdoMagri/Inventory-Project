using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsPurchaseItem
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int PurchaseItemID { set; get; }
        public int PurchaseID { set; get; }
        public int ProductID { set; get; }
        public clsProduct ProductInfo;
        public int Qty { set; get; }
        public decimal UnitCost { set; get; }
        public decimal LineTotal { get { return Qty * UnitCost; } }

        public clsPurchaseItem()
        {
            this.PurchaseItemID = -1;
            this.Qty = 0;
            this.UnitCost = 0;
            Mode = enMode.AddNew;
        }

        private clsPurchaseItem(int PurchaseItemID, int PurchaseID, int ProductID, int Qty, decimal UnitCost)
        {
            this.PurchaseItemID = PurchaseItemID;
            this.PurchaseID = PurchaseID;
            this.ProductID = ProductID;
            this.ProductInfo = clsProduct.Find(ProductID);
            this.Qty = Qty;
            this.UnitCost = UnitCost;
            Mode = enMode.Update;
        }

        //private bool _AddNewPurchaseItem()
        //{
        //    this.PurchaseItemID = clsPurchaseItemData.AddNewPurchaseItem(this.PurchaseID,
        //        this.ProductID, this.Qty, this.UnitCost);
        //    return (this.PurchaseItemID != -1);
        //}

        //private bool _UpdatePurchaseItem()
        //{
        //    return clsPurchaseItemData.UpdatePurchaseItem(this.PurchaseItemID,
        //        this.ProductID, this.Qty, this.UnitCost);
        //}

        public static clsPurchaseItem Find(int PurchaseItemID)
        {
            int PurchaseID = -1, ProductID = -1, Qty = 0;
            decimal UnitCost = 0;

            if (clsPurchaseItemData.GetPurchaseItemInfoByID(PurchaseItemID, ref PurchaseID,
                ref ProductID, ref Qty, ref UnitCost))
                return new clsPurchaseItem(PurchaseItemID, PurchaseID, ProductID, Qty, UnitCost);
            else
                return null;
        }

        //public bool Save()
        //{
        //    switch (Mode)
        //    {
        //        case enMode.AddNew:
        //            if (_AddNewPurchaseItem())
        //            {
        //                Mode = enMode.Update;
        //                return true;
        //            }
        //            else
        //                return false;

        //        case enMode.Update:
        //            return _UpdatePurchaseItem();
        //    }
        //    return false;
        //}

        public static DataTable GetItemsByPurchaseID(int PurchaseID)
        {
            return clsPurchaseItemData.GetItemsByPurchaseID(PurchaseID);
        }

        //public static bool DeletePurchaseItem(int PurchaseItemID)
        //{
        //    return clsPurchaseItemData.DeletePurchaseItem(PurchaseItemID);
        //}
    }
}