using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsPurchase
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public clsPurchaseData.PurchaseDTO DTO
        {
            get
            {
                return new clsPurchaseData.PurchaseDTO(this.PurchaseID, this.AccountID,
                    this.SupplierID,
                    this.UserID,
                    this.PurchaseDate, this.Total, this.GetItems());
            }
        }
        public int PurchaseID { set; get; }
        public int AccountID { set; get; }
        public int SupplierID { set; get; }
        public clsSupplier SupplierInfo;
        public int UserID { set; get; }
        public clsUser UserInfo;
        public DateTime PurchaseDate { set; get; }
        public decimal Total { set; get; }

        public clsPurchase()
        {
            this.PurchaseID = -1;
            this.PurchaseDate = DateTime.Now;
            this.Total = 0;
            Mode = enMode.AddNew;
        }

        private clsPurchase(int PurchaseID, int AccountID, int SupplierID, int UserID,
            DateTime PurchaseDate, decimal Total)
        {
            this.PurchaseID = PurchaseID;
            this.AccountID = AccountID;
            this.SupplierID = SupplierID;
            this.SupplierInfo = clsSupplier.Find(SupplierID);
            this.UserID = UserID;
            this.UserInfo = clsUser.FindByUserID(UserID);
            this.PurchaseDate = PurchaseDate;
            this.Total = Total;
            Mode = enMode.Update;
        }

        //private bool _AddNewPurchase()
        //{
        //    this.PurchaseID = clsPurchaseData.AddNewPurchase(this.AccountID, this.SupplierID,
        //        this.UserID, this.PurchaseDate, this.Total);
        //    return (this.PurchaseID != -1);
        //}

        //private bool _UpdatePurchase()
        //{
        //    return clsPurchaseData.UpdatePurchase(this.PurchaseID, this.SupplierID,
        //        this.PurchaseDate, this.Total);
        //}

        public static clsPurchase Find(int PurchaseID)
        {
            int AccountID = -1, SupplierID = -1, UserID = -1;
            DateTime PurchaseDate = DateTime.Now;
            decimal Total = 0;

            if (clsPurchaseData.GetPurchaseInfoByID(PurchaseID, ref AccountID, ref SupplierID,
                ref UserID, ref PurchaseDate, ref Total))
                return new clsPurchase(PurchaseID, AccountID, SupplierID, UserID, PurchaseDate, Total);
            else
                return null;
        }

        //public bool Save()
        //{
        //    switch (Mode)
        //    {
        //        case enMode.AddNew:
        //            if (_AddNewPurchase())
        //            {
        //                Mode = enMode.Update;
        //                return true;
        //            }
        //            else
        //                return false;

        //        case enMode.Update:
        //            return _UpdatePurchase();
        //    }
        //    return false;
        //}

        public List<Dictionary<string, object?>> GetItems()
        {
            return clsPurchaseItem.GetItemsByPurchaseID(this.PurchaseID);
        }

        public static DataTable GetAllPurchases(int AccountID)
        {
            return clsPurchaseData.GetAllPurchases(AccountID);
        }

        public static bool isPurchaseExist(int PurchaseID)
        {
            return clsPurchaseData.IsPurchaseExist(PurchaseID);
        }
        public static int MakePurchase(int AccountID, int UserID, int SupplierID,
    List<clsPurchaseItem> Items, ref string ErrorMessage)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductID", typeof(int));    // same order as the PurchaseItemList type
            dt.Columns.Add("Qty", typeof(int));
            dt.Columns.Add("UnitCost", typeof(decimal));

            foreach (clsPurchaseItem Item in Items)
                dt.Rows.Add(Item.ProductID, Item.Qty, Item.UnitCost);

            return clsPurchaseData.MakePurchase(AccountID, UserID, SupplierID, dt, ref ErrorMessage);
        }
    }
}