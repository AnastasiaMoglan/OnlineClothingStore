namespace OnlineClothingStore.App.Structural.Proxy;

public class InventoryReportProxy : IInventoryReport
{
    private readonly RealInventoryReport _realInventoryReport = new();

    public List<InventoryReportItem> GetReport(string userRole)
    {
        if (userRole == "Manager")
        {
            return _realInventoryReport.GetReport(userRole);
        }

        List<InventoryReportItem> publicItems = _realInventoryReport
            .GetReport(userRole)
            .Select(item => new InventoryReportItem(
                productName: item.ProductName,
                category: item.Category,
                sellingPrice: item.SellingPrice,
                stockQuantity: item.StockQuantity,
                supplierName: "Acces restrictionat",
                purchasePrice: 0,
                isSensitiveDataVisible: false))
            .ToList();

        return publicItems;
    }

    public string GetAccessMessage(string userRole)
    {
        if (userRole == "Manager")
        {
            return "Ai rol de Manager. Proxy permite accesul la datele complete despre stocuri, furnizori si profit.";
        }

        return "Nu ai rol de Manager. Proxy ascunde datele sensibile si afiseaza doar informatiile publice.";
    }
}