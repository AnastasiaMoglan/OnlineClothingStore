namespace OnlineClothingStore.App.Structural.Strategy;

public class NoDiscountStrategy : IDiscountStrategy
{
    public string Name => "Fără reducere";

    public string Description => "Comanda este calculată fără aplicarea unei reduceri.";

    public decimal CalculateDiscount(decimal productsTotal)
    {
        return 0;
    }
}