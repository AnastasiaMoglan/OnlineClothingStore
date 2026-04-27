using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Pricing;

public sealed class NoDiscount : Discount
{
    public override decimal Apply(decimal price) => price;
}