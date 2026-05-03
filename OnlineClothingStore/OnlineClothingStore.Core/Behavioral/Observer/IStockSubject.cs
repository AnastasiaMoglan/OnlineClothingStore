namespace OnlineClothingStore.App.Structural.Observer;

public interface IStockSubject
{
    void Attach(IStockObserver observer);

    void Detach(string email, int productId);

    List<StockNotification> Notify(StockChangedEvent stockEvent);

    List<IStockObserver> GetObservers();
}