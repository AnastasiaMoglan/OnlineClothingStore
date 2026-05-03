namespace OnlineClothingStore.Core.Behavioral.Command;

public class ShoppingCart
{
    private readonly List<CartItem> _items = new();

    public void AddItem(CartItem item)
    {
        var existingItem = _items.FirstOrDefault(x => x.ProductId == item.ProductId);

        if (existingItem == null)
        {
            _items.Add(item);
        }
        else
        {
            existingItem.IncreaseQuantity(item.Quantity);
        }
    }

    public CartItem? RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
        {
            return null;
        }

        _items.Remove(item);
        return item;
    }

    public List<CartItem> Clear()
    {
        var backup = _items.Select(x => x.Clone()).ToList();
        _items.Clear();
        return backup;
    }

    public void RestoreItems(List<CartItem> items)
    {
        _items.Clear();
        _items.AddRange(items.Select(x => x.Clone()));
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