namespace OnlineClothingStore.Core.Behavioral.Strategy;

public class NoDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal price)
    {
        return price;
    }

    public string GetDescription()
    {
        return "Fără reducere";
    }
}