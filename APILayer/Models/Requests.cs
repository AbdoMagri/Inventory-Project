namespace InventoryAPI.Models
{
    public class SaleItemRequest
    {
        public int ProductID { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class MakeSaleRequest
    {
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public int? CustomerID { get; set; }
        public List<SaleItemRequest> Items { get; set; } = new();
    }

    public class PurchaseItemRequest
    {
        public int ProductID { get; set; }
        public int Qty { get; set; }
        public decimal UnitCost { get; set; }
    }
    public class MakePurchaseRequest
    {
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public int SupplierID { get; set; }
        public List<PurchaseItemRequest> Items { get; set; } = new();
    }

    public class AdjustStockRequest
    {
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int QtyChange { get; set; }
        public string Note { get; set; } = "";
    }
    public class AdjustToCountRequest
    {
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int CountedQty { get; set; }
        public string Note { get; set; } = "";
    }
}