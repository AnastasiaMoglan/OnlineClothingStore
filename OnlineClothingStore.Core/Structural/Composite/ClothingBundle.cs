namespace OnlineClothingStore.App.Structural.Composite;

public class ClothingBundle : IStoreItemGroup
{
    private readonly List<IStoreItem> _items = new();

    public string Name { get; }

    public ClothingBundle(string name)
    {
        Name = name;
    }

    public void Add(IStoreItem item)
    {
        _items.Add(item);
    }

    public void Remove(IStoreItem item)
    {
        _items.Remove(item);
    }

    public IReadOnlyCollection<IStoreItem> GetChildren() => _items.AsReadOnly();

    public decimal GetPrice()
    {
        return _items.Sum(i => i.GetPrice());
    }

    public void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}+ {Name}: {GetPrice()} MDL");
        foreach (var item in _items)
        {
            item.Display(depth + 1);
        }
    }
}