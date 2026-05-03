namespace OnlineClothingStore.App.Structural.Strategy;

public class VipDiscountStrategy : IDiscountStrategy
{
    public string Name => "VIP";

    public string Description => "Se aplică o reducere de 15% pentru clienții VIP.";

    public decimal CalculateDiscount(decimal productsTotal)
    {
        return productsTotal * 0.15m;
    }
}