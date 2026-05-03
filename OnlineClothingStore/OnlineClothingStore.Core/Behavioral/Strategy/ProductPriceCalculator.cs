namespace OnlineClothingStore.Core.Behavioral.Strategy;

public class ProductPriceCalculator
{
    private IDiscountStrategy _discountStrategy;

    public ProductPriceCalculator(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }

    public void SetDiscountStrategy(IDiscountStrategy strategy)
    {
        _discountStrategy = strategy;
    }

    public decimal CalculateFinalPrice(decimal price)
    {
        return _discountStrategy.ApplyDiscount(price);
    }

    public string GetDiscountInfo()
    {
        return _discountStrategy.GetDescription();
    }
}