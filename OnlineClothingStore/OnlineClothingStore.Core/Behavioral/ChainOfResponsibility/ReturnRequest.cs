namespace OnlineClothingStore.App.Behavioral.ChainOfResponsibility;

public class ReturnRequest
{
    public string OrderId { get; }
    public string ProductName { get; }
    public int DaysAfterPurchase { get; }
    public bool IsProductUsed { get; }
    public bool HasOriginalPackage { get; }

    public ReturnRequest(
        string orderId,
        string productName,
        int daysAfterPurchase,
        bool isProductUsed,
        bool hasOriginalPackage)
    {
        OrderId = orderId;
        ProductName = productName;
        DaysAfterPurchase = daysAfterPurchase;
        IsProductUsed = isProductUsed;
        HasOriginalPackage = hasOriginalPackage;
    }
}