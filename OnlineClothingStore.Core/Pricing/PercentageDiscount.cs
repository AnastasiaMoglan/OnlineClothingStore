using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Pricing;

public sealed class PercentageDiscount : Discount
{
    private readonly decimal _percent;

    public PercentageDiscount(decimal percent)
    {
        if (percent < 0 || percent > 1) throw new ArgumentException("Percent 0..1");
        _percent = percent;
    }

    public override decimal Apply(decimal price) => price - price * _percent;
}