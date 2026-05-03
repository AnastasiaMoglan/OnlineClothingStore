namespace OnlineClothingStore.App.Structural.Strategy;

public class StudentDiscountStrategy : IDiscountStrategy
{
    public string Name => "Student";

    public string Description => "Se aplică o reducere de 10% pentru studenți.";

    public decimal CalculateDiscount(decimal productsTotal)
    {
        return productsTotal * 0.10m;
    }
}