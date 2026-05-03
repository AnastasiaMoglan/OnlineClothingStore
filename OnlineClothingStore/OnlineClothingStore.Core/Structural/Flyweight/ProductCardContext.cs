namespace OnlineClothingStore.App.Structural.Flyweight;

public class ProductCardContext
{
    public string Name { get; }

    public decimal Price { get; }

    public string Size { get; }

    public string Color { get; }

    public string Category { get; }

    public ProductCardContext(
        string name,
        decimal price,
        string size,
        string color,
        string category)
    {
        Name = name;
        Price = price;
        Size = size;
        Color = color;
        Category = category;
    }
}