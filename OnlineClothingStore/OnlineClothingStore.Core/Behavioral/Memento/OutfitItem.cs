namespace OnlineClothingStore.Core.Behavioral.Memento;

public class OutfitItem
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public OutfitItem()
    {
    }

    public OutfitItem(string name, string category)
    {
        Name = name;
        Category = category;
    }
}