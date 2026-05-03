namespace OnlineClothingStore.Core.Behavioral.Iterator;

public class Product
{
    public int Id { get; }
    public string Name { get; }
    public string Category { get; }
    public decimal Price { get; }
    public string Size { get; }

    public Product(int id, string name, string category, decimal price, string size)
    {
        Id = id;
        Name = name;
        Category = category;
        Price = price;
        Size = size;
    }
}