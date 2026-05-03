namespace OnlineClothingStore.App.Structural.Proxy;

public interface IInventoryReport
{
    List<InventoryReportItem> GetReport(string userRole);

    string GetAccessMessage(string userRole);
}