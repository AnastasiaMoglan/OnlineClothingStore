namespace OnlineClothingStore.App.Behavioral.Iterator;

public sealed class OrderReviewCollection : IOrderCollection
{
    private readonly List<OrderReviewItem> _orders = new();

    public void AddOrder(OrderReviewItem order)
    {
        _orders.Add(order);
    }

    public IReadOnlyList<OrderReviewItem> GetOrders()
    {
        return _orders.AsReadOnly();
    }

    public IOrderIterator CreateIterator()
    {
        return new PendingOrderIterator(_orders);
    }
}