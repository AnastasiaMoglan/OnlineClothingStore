namespace OnlineClothingStore.App.Structural.Strategy;

public class DiscountCalculator
{
    private readonly IDiscountStrategy _discountStrategy;

    public DiscountCalculator(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }

    public string StrategyName => _discountStrategy.Name;

    public string StrategyDescription => _discountStrategy.Description;

    public decimal CalculateDiscount(decimal productsTotal)
    {
        if (productsTotal <= 0)
        {
            return 0;
        }

        return _discountStrategy.CalculateDiscount(productsTotal);
    }

    public decimal CalculateTotalAfterDiscount(decimal productsTotal)
    {
        decimal discount = CalculateDiscount(productsTotal);

        return productsTotal - discount;
    }
}