namespace OnlineClothingStore.Core.Behavioral.Strategy;

public class VipDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _discountPercent;

    public VipDiscountStrategy(decimal discountPercent)
    {
        _discountPercent = discountPercent;
    }

    public decimal ApplyDiscount(decimal price)
    {
        return price - price * _discountPercent / 100;
    }

    public string GetDescription()
    {
        return $"Reducere VIP de {_discountPercent}%";
    }
}