using OnlineClothingStore.Core.Behavioral.Command;

namespace OnlineClothingStore.Core.Behavioral.Memento;

public class CartSnapshot
{
    public IReadOnlyList<CartItem> Items { get; }
    public DateTime SavedAt { get; }

    public CartSnapshot(IEnumerable<CartItem> items)
    {
        Items = items.Select(x => x.Clone()).ToList().AsReadOnly();
        SavedAt = DateTime.Now;
    }

    public string GetDescription()
    {
        return $"Snapshot salvat la {SavedAt:HH:mm:ss}, produse: {Items.Count}";
    }
}