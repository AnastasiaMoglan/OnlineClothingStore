namespace OnlineClothingStore.App.Structural.Command;

public class CommandProduct
{
    public int Id { get; }

    public string Name { get; }

    public int StockQuantity { get; set; }

    public decimal Price { get; set; }

    public CommandProduct(
        int id,
        string name,
        int stockQuantity,
        decimal price)
    {
        Id = id;
        Name = name;
        StockQuantity = stockQuantity;
        Price = price;
    }
}