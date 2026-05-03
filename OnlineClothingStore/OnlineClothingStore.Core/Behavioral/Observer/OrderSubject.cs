namespace OnlineClothingStore.Core.Behavioral.Observer;

public class OrderSubject
{
    private readonly List<IOrderObserver> _observers = new();

    public void Subscribe(IOrderObserver observer)
    {
        _observers.Add(observer);
    }

    public void Unsubscribe(IOrderObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(OrderEvent orderEvent)
    {
        foreach (var observer in _observers)
        {
            observer.Update(orderEvent);
        }
    }

    public OrderEvent PlaceOrder(string customerEmail, decimal totalAmount)
    {
        var orderEvent = new OrderEvent(
            orderId: $"ORD-{Guid.NewGuid().ToString()[..8]}",
            customerEmail: customerEmail,
            totalAmount: totalAmount
        );

        Notify(orderEvent);

        return orderEvent;
    }
}