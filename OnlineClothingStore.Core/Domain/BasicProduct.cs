namespace OnlineClothingStore.Domain;

public sealed class BasicProduct : Product
{
    public BasicProduct(string name, decimal price)
        : base(name, price)
    {
    }

    public override decimal GetFinalPrice()
    {
        return Price;
    }
}