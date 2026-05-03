namespace OnlineClothingStore.App.Structural.Observer;

public class ProductStockSubject : IStockSubject
{
    private readonly List<IStockObserver> _observers = new();

    public void Attach(IStockObserver observer)
    {
        bool alreadyExists = _observers.Any(existing =>
            existing.Email == observer.Email &&
            existing.ProductId == observer.ProductId);

        if (!alreadyExists)
        {
            _observers.Add(observer);
        }
    }

    public void Detach(string email, int productId)
    {
        IStockObserver? observer = _observers.FirstOrDefault(existing =>
            existing.Email == email &&
            existing.ProductId == productId);

        if (observer != null)
        {
            _observers.Remove(observer);
        }
    }

    public List<StockNotification> Notify(StockChangedEvent stockEvent)
    {
        List<StockNotification> notifications = new();

        foreach (IStockObserver observer in _observers)
        {
            if (observer.ProductId == stockEvent.ProductId)
            {
                StockNotification notification = observer.Update(stockEvent);
                notifications.Add(notification);
            }
        }

        return notifications;
    }

    public List<IStockObserver> GetObservers()
    {
        return _observers.ToList();
    }
}