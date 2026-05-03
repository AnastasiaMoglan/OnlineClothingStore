namespace OnlineClothingStore.App.Structural.Proxy;

public class RealInventoryReport : IInventoryReport
{
    private readonly List<InventoryReportItem> _items = new()
    {
        new InventoryReportItem(
            "Tricou Oversize Blue",
            "T-Shirts",
            349,
            25,
            "Blue Textile SRL",
            180,
            true),

        new InventoryReportItem(
            "Tricou Basic White",
            "T-Shirts",
            279,
            40,
            "Cotton Wear Moldova",
            130,
            true),

        new InventoryReportItem(
            "Jeans Slim Fit",
            "Jeans",
            699,
            18,
            "Denim Factory SRL",
            390,
            true),

        new InventoryReportItem(
            "Jeans Regular Dark",
            "Jeans",
            749,
            14,
            "Urban Denim Supplier",
            420,
            true),

        new InventoryReportItem(
            "Geaca Urban Denim",
            "Jackets",
            1199,
            9,
            "Blue Jacket Partner",
            760,
            true),

        new InventoryReportItem(
            "Hanorac Minimal",
            "Hoodies",
            599,
            30,
            "SoftWear Studio",
            310,
            true),

        new InventoryReportItem(
            "Hanorac Street Purple",
            "Hoodies",
            649,
            16,
            "Street Fashion SRL",
            350,
            true),

        new InventoryReportItem(
            "Rochie Eleganta",
            "Dresses",
            899,
            11,
            "Elegant Line Moldova",
            520,
            true),

        new InventoryReportItem(
            "Sneakers White",
            "Shoes",
            999,
            20,
            "Urban Shoes Import",
            610,
            true)
    };

    public List<InventoryReportItem> GetReport(string userRole)
    {
        return _items;
    }

    public string GetAccessMessage(string userRole)
    {
        return "Managerul are acces complet la raportul real de stocuri.";
    }
}