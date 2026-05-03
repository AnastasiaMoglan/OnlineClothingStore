namespace OnlineClothingStore.Core.Behavioral.Strategy;

public class SeasonalDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _discountPercent;
    private readonly string _seasonName;

    public SeasonalDiscountStrategy(decimal discountPercent, string seasonName)
    {
        _discountPercent = discountPercent;
        _seasonName = seasonName;
    }

    public decimal ApplyDiscount(decimal price)
    {
        return price - price * _discountPercent / 100;
    }

    public string GetDescription()
    {
        return $"Reducere sezonieră {_seasonName}: {_discountPercent}%";
    }
}