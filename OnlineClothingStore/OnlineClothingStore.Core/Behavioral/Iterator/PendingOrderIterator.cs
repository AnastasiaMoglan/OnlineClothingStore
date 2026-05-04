namespace OnlineClothingStore.App.Behavioral.Iterator;

public sealed class PendingOrderIterator : IOrderIterator
{
    private readonly List<OrderReviewItem> _orders;
    private int _position;

    public PendingOrderIterator(List<OrderReviewItem> orders)
    {
        _orders = orders;
        _position = 0;
    }

    public bool HasNext()
    {
        while (_position < _orders.Count)
        {
            if (_orders[_position].Status == "Pending")
            {
                return true;
            }

            _position++;
        }

        return false;
    }

    public OrderReviewItem Next()
    {
        if (!HasNext())
        {
            throw new InvalidOperationException("Nu mai există comenzi în așteptare.");
        }

        OrderReviewItem currentOrder = _orders[_position];
        _position++;

        return currentOrder;
    }
}