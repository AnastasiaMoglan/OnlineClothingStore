namespace OnlineClothingStore.Domain;

public abstract class Product : Entity
{
    public string Name { get; }
    public decimal Price { get; }

    protected Product(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.");

        if (price <= 0)
            throw new ArgumentException("Price must be greater than 0.");

        Name = name;
        Price = price;
    }

    public abstract decimal GetFinalPrice();
}