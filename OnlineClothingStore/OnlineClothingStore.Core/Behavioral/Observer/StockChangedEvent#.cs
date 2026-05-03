namespace OnlineClothingStore.App.Structural.Observer;

public class StockChangedEvent
{
    public int ProductId { get; }

    public string ProductName { get; }

    public int OldStock { get; }

    public int NewStock { get; }

    public StockChangedEvent(
        int productId,
        string productName,
        int oldStock,
        int newStock)
    {
        ProductId = productId;
        ProductName = productName;
        OldStock = oldStock;
        NewStock = newStock;
    }
}