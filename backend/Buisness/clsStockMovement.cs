using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    // The stock log is append-only: records can be added, never updated or deleted
    public class clsStockMovement
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;


        public clsStockMovementData.StockMovementDTO DTO
        {
            get
            {
                return new clsStockMovementData.StockMovementDTO(this.MovementID,
                    this.ProductID,
                    this.UserID, 
                    this.MovementType.ToString(), this.Qty,
                    this.ReferenceID, this.Note, this.MovementDate);
            }
        }
        public enum enMovementType { Purchase = 0, Sale = 1, Return = 2, Adjustment = 3 };

        public int MovementID { set; get; }
        public int ProductID { set; get; }
        public clsProduct ProductInfo;
        public int UserID { set; get; }
        public clsUser UserInfo;
        public enMovementType MovementType { set; get; }
        public int Qty { set; get; }               // signed: positive = in, negative = out
        public int? ReferenceID { set; get; }      // PurchaseID / SaleID
        public string Note { set; get; }
        public DateTime MovementDate { set; get; }

        public clsStockMovement()
        {
            this.MovementID = -1;
            this.ReferenceID = null;
            this.Note = "";
            this.MovementDate = DateTime.Now;
            Mode = enMode.AddNew;
        }

        private clsStockMovement(int MovementID, int ProductID, int UserID, enMovementType MovementType,
            int Qty, int? ReferenceID, string Note, DateTime MovementDate)
        {
            this.MovementID = MovementID;
            this.ProductID = ProductID;
            this.ProductInfo = clsProduct.Find(ProductID);
            this.UserID = UserID;
            this.UserInfo = clsUser.FindByUserID(UserID);
            this.MovementType = MovementType;
            this.Qty = Qty;
            this.ReferenceID = ReferenceID;
            this.Note = Note;
            this.MovementDate = MovementDate;
            Mode = enMode.Update;
        }

        //private bool _AddNewMovement()
        //{
        //    this.MovementID = clsStockMovementData.AddNewMovement(this.ProductID, this.UserID,
        //        this.MovementType.ToString(), this.Qty, this.ReferenceID, this.Note);
        //    return (this.MovementID != -1);
        //}

        public static clsStockMovement Find(int MovementID)
        {
            int ProductID = -1, UserID = -1, Qty = 0;
            int? ReferenceID = null;
            string MovementType = "", Note = "";
            DateTime MovementDate = DateTime.Now;

            if (clsStockMovementData.GetMovementInfoByID(MovementID, ref ProductID, ref UserID,
                ref MovementType, ref Qty, ref ReferenceID, ref Note, ref MovementDate))
                return new clsStockMovement(MovementID, ProductID, UserID,
                    (enMovementType)Enum.Parse(typeof(enMovementType), MovementType),
                    Qty, ReferenceID, Note, MovementDate);
            else
                return null;
        }

        //public bool Save()
        //{
        //    switch (Mode)
        //    {
        //        case enMode.AddNew:
        //            if (_AddNewMovement())
        //            {
        //                Mode = enMode.Update;
        //                return true;
        //            }
        //            else
        //                return false;

        //        case enMode.Update:
        //            return false;   // log records are never edited
        //    }
        //    return false;
        //}

        public static List<clsStockMovementData.StockMovementDTO> GetMovementsByProductID(int ProductID)
        {
            return clsStockMovementData.GetMovementsByProductID(ProductID);
        }

        public static List<clsStockMovementData.clsStockMovement> GetAllMovements(int AccountID)
        {
            return clsStockMovementData.GetAllMovements(AccountID);
        }
        // QtyChange is signed: +5 found extra, -3 damaged/lost
        public static int AdjustStock(int AccountID, int UserID, int ProductID,
            int QtyChange, string Note, ref string ErrorMessage)
        {
            return clsStockMovementData.AdjustStock(AccountID, UserID, ProductID,
                QtyChange, Note, ref ErrorMessage);
        }

        // For a physical count: pass what you counted, the difference is calculated for you
        public static int AdjustStockToCount(int AccountID, int UserID, int ProductID,
            int CountedQty, string Note, ref string ErrorMessage)
        {
            clsProduct Product = clsProduct.Find(ProductID);

            if (Product == null)
            {
                ErrorMessage = "Product not found.";
                return -1;
            }

            return AdjustStock(AccountID, UserID, ProductID, CountedQty - Product.Quantity,
                Note, ref ErrorMessage);
        }
    }
}
