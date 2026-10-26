using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsProduct
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public clsProductData.ProductListDTO DTO
        {
            get
            {
                return new clsProductData.ProductListDTO(
                    this.ProductID,
                    this.Name,
                    this.SKU,
                    this.CategoryInfo.Name,
                    this.UnitInfo.Name,
                    this.CostPrice,
                    this.SellPrice,
                    this.MinStock,
                    this.Quantity,
                    this.IsActive,
                    this.ImagePath
                );
            }
        }
        public int ProductID { set; get; }
        public int AccountID { set; get; }
        public int? CategoryID { set; get; }
        public clsCategory CategoryInfo;
        public int UnitID { set; get; }
        public clsUnit UnitInfo;
        public string Name { set; get; }
        public string SKU { set; get; }
        public decimal CostPrice { set; get; }
        public decimal SellPrice { set; get; }
        public int MinStock { set; get; }
        public int Quantity { private set; get; }   // changes only through ChangeQuantity
        public bool IsActive { set; get; }
        public string ImagePath { set; get; }

        public clsProduct()
        {
            this.ProductID = -1;
            this.CategoryID = null;
            this.Name = "";
            this.SKU = "";
            this.CostPrice = 0;
            this.SellPrice = 0;
            this.MinStock = 0;
            this.Quantity = 0;
            this.IsActive = true; this.ImagePath = "";
            Mode = enMode.AddNew;
        }

        public clsProduct(clsProductData.ProductDTO DTO)
        {
            this.ProductID = DTO.ProductID;
            this.AccountID = DTO.AccountID;
            this.CategoryID = DTO.CategoryID;
            this.CategoryInfo = DTO.CategoryID.HasValue ? clsCategory.Find(DTO.CategoryID.Value) : null;
            this.UnitID = DTO.UnitID;
            this.UnitInfo = clsUnit.Find(DTO.UnitID);
            this.Name = DTO.Name;
            this.SKU = DTO.SKU;
            this.CostPrice = DTO.CostPrice;
            this.SellPrice = DTO.SellPrice;
            this.MinStock = DTO.MinStock;
            this.Quantity = DTO.Quantity;
            this.IsActive = DTO.IsActive;
            this.ImagePath = DTO.ImagePath;
            Mode = enMode.Update;
        }
        private clsProduct(int ProductID, int AccountID, int? CategoryID, int UnitID, string Name,
            string SKU, decimal CostPrice, decimal SellPrice, int MinStock, int Quantity, bool IsActive, string ImagePath)
        {
            this.ProductID = ProductID;
            this.AccountID = AccountID;
            this.CategoryID = CategoryID;
            this.CategoryInfo = CategoryID.HasValue ? clsCategory.Find(CategoryID.Value) : null;
            this.UnitID = UnitID;
            this.UnitInfo = clsUnit.Find(UnitID);
            this.Name = Name;
            this.SKU = SKU;
            this.CostPrice = CostPrice;
            this.SellPrice = SellPrice;
            this.MinStock = MinStock;
            this.Quantity = Quantity;
            this.IsActive = IsActive;
            this.ImagePath = ImagePath;
            Mode = enMode.Update;
        }

        private bool _AddNewProduct()
        {
            this.ProductID = clsProductData.AddNewProduct(this.AccountID, this.CategoryID, this.UnitID,
                this.Name, this.SKU, this.CostPrice, this.SellPrice, this.MinStock, this.IsActive, this.ImagePath);
            return (this.ProductID != -1);
        }

        private bool _UpdateProduct()
        {
            return clsProductData.UpdateProduct(this.ProductID, this.CategoryID, this.UnitID,
                this.Name, this.SKU, this.CostPrice, this.SellPrice, this.MinStock, this.IsActive, this.ImagePath);
        }

        public static clsProduct Find(int ProductID)
        {
            int AccountID = -1, UnitID = -1, MinStock = 0, Quantity = 0;
            int? CategoryID = null;
            string Name = "", SKU = "";
            decimal CostPrice = 0, SellPrice = 0;
            bool IsActive = false;
            string ImagePath = "";

            if (clsProductData.GetProductInfoByID(ProductID, ref AccountID, ref CategoryID, ref UnitID,
                ref Name, ref SKU, ref CostPrice, ref SellPrice, ref MinStock, ref Quantity, ref IsActive, ref ImagePath))
                return new clsProduct(ProductID, AccountID, CategoryID, UnitID, Name, SKU,
                    CostPrice, SellPrice, MinStock, Quantity, IsActive, ImagePath);
            else
                return null;
        }

        public static clsProduct FindBySKU(int AccountID, string SKU)
        {
            int ProductID = -1, UnitID = -1, MinStock = 0, Quantity = 0;
            int? CategoryID = null;
            string Name = "";
            decimal CostPrice = 0, SellPrice = 0;
            bool IsActive = false;
            string ImagePath = "";

            if (clsProductData.GetProductInfoBySKU(AccountID, SKU, ref ProductID, ref CategoryID, ref UnitID,
                ref Name, ref CostPrice, ref SellPrice, ref MinStock, ref Quantity, ref IsActive, ref ImagePath))
                return new clsProduct(ProductID, AccountID, CategoryID, UnitID, Name, SKU,
                    CostPrice, SellPrice, MinStock, Quantity, IsActive, ImagePath);
            else
                return null;
        }

        // QtyChange is signed: +10 stock in, -3 stock out
        //public bool ChangeQuantity(int QtyChange)
        //{
        //    if (Mode != enMode.Update)
        //        return false;

        //    if (clsProductData.UpdateProductQuantity(this.ProductID, QtyChange))
        //    {
        //        this.Quantity += QtyChange;
        //        return true;
        //    }
        //    return false;
        //}

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewProduct())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateProduct();
            }
            return false;
        }

        public static List<clsProductData.ProductListDTO> GetAllProducts(int AccountID)
        {
            return clsProductData.GetAllProducts(AccountID);
        }

        public static List<clsProductData.LowStockProductDTO> GetLowStockProducts(int AccountID)
        {
            return clsProductData.GetLowStockProducts(AccountID);
        }

        public static bool DeleteProduct(int ProductID)
        {
            return clsProductData.DeleteProduct(ProductID);
        }

        public static bool isProductExist(int ProductID)
        {
            return clsProductData.IsProductExist(ProductID);
        }

        public static bool isSKUExist(int AccountID, string SKU)
        {
            return clsProductData.IsSKUExist(AccountID, SKU);
        }
    }
}