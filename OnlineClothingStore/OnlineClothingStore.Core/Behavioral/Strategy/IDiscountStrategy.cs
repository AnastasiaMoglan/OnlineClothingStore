namespace OnlineClothingStore.Core.Behavioral.Strategy;

public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal price);
    string GetDescription();
}