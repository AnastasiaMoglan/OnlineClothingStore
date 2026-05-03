using OnlineClothingStore.Core.Behavioral.Command;

namespace OnlineClothingStore.Core.Behavioral.Memento;

public class CartOriginator
{
    private readonly List<CartItem> _items = new();

    public void AddItem(CartItem item)
    {
        _items.Add(item);
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item != null)
        {
            _items.Remove(item);
        }
    }

    public CartSnapshot Save()
    {
        return new CartSnapshot(_items);
    }

    public void Restore(CartSnapshot snapshot)
    {
        _items.Clear();
        _items.AddRange(snapshot.Items.Select(x => x.Clone()));
    }

    public IReadOnlyList<CartItem> GetItems()
    {
        return _items.AsReadOnly();
    }

    public decimal GetTotal()
    {
        return _items.Sum(x => x.Price * x.Quantity);
    }
}