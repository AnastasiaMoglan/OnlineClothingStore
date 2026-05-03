namespace OnlineClothingStore.App.Structural.Observer;

public interface IStockObserver
{
    int ProductId { get; }

    string CustomerName { get; }

    string Email { get; }

    StockNotification Update(StockChangedEvent stockEvent);
}