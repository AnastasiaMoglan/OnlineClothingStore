namespace OnlineClothingStore.App.Structural.Composite;

public class SingleClothingItem : IStoreItem
{
    public string Name { get; }
    public decimal Price { get; }

    public SingleClothingItem(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public decimal GetPrice() => Price;

    public void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}- {Name}: {Price} MDL");
    }
}