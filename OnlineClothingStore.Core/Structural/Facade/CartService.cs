namespace OnlineClothingStore.App.Structural.Facade;

public class CartService
{
    public decimal CalculateTotal(List<decimal> prices)
    {
        return prices.Sum();
    }
}