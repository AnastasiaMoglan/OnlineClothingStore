namespace OnlineClothingStore.App.Behavioral.Visitor;

public class MarketingValidationVisitor : IPromotionVisitor
{
    private readonly List<string> _messages = new();

    public void Visit(HeroBanner banner)
    {
        if (string.IsNullOrWhiteSpace(banner.ImageUrl))
        {
            _messages.Add($"Bannerul {banner.Name} nu are imagine.");
        }

        if (banner.Title.Length < 5)
        {
            _messages.Add($"Bannerul {banner.Name} are titlu prea scurt.");
        }
    }

    public void Visit(PromoCode code)
    {
        if (code.DiscountPercent <= 0)
        {
            _messages.Add($"Codul promoțional {code.Code} nu are reducere validă.");
        }

        if (code.DiscountPercent > 50)
        {
            _messages.Add($"Codul promoțional {code.Code} are reducere prea mare.");
        }
    }

    public void Visit(CampaignCard card)
    {
        if (card.Priority < 1 || card.Priority > 5)
        {
            _messages.Add($"Campania {card.CampaignName} are prioritate invalidă.");
        }
    }

    public IReadOnlyList<string> GetMessages()
    {
        return _messages.AsReadOnly();
    }
}