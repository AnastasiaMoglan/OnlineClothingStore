namespace OnlineClothingStore.App.Behavioral.Visitor;

public class PromotionCostVisitor : IPromotionVisitor
{
    private decimal _totalCost;

    public void Visit(HeroBanner banner)
    {
        _totalCost += 300;
    }

    public void Visit(PromoCode code)
    {
        _totalCost += code.DiscountPercent * 10;
    }

    public void Visit(CampaignCard card)
    {
        _totalCost += card.Priority * 50;
    }

    public decimal GetTotalCost()
    {
        return _totalCost;
    }
}