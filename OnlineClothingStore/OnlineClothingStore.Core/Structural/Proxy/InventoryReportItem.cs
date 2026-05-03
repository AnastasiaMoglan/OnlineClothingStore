namespace OnlineClothingStore.App.Structural.Proxy;

public class InventoryReportItem
{
    public string ProductName { get; }

    public string Category { get; }

    public decimal SellingPrice { get; }

    public int StockQuantity { get; }

    public string SupplierName { get; }

    public decimal PurchasePrice { get; }

    public decimal EstimatedProfit { get; }

    public bool IsSensitiveDataVisible { get; }

    public InventoryReportItem(
        string productName,
        string category,
        decimal sellingPrice,
        int stockQuantity,
        string supplierName,
        decimal purchasePrice,
        bool isSensitiveDataVisible)
    {
        ProductName = productName;
        Category = category;
        SellingPrice = sellingPrice;
        StockQuantity = stockQuantity;
        SupplierName = supplierName;
        PurchasePrice = purchasePrice;
        IsSensitiveDataVisible = isSensitiveDataVisible;

        EstimatedProfit = (sellingPrice - purchasePrice) * stockQuantity;
    }
}