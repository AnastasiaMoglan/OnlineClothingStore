namespace OnlineClothingStore.App.Structural.Strategy;

public interface IDiscountStrategy
{
    string Name { get; }

    string Description { get; }

    decimal CalculateDiscount(decimal productsTotal);
}