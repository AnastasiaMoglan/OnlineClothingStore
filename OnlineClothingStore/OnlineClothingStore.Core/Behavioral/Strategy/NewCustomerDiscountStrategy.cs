namespace OnlineClothingStore.App.Structural.Strategy;

public class NewCustomerDiscountStrategy : IDiscountStrategy
{
    public string Name => "Client nou";

    public string Description => "Se aplică o reducere de 5% pentru clienții noi.";

    public decimal CalculateDiscount(decimal productsTotal)
    {
        return productsTotal * 0.05m;
    }
}