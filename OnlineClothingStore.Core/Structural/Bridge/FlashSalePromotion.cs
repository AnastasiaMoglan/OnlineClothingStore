namespace OnlineClothingStore.App.Structural.Bridge;

public sealed class FlashSalePromotion : Promotion
{
    private readonly decimal _discountPercent;
    private readonly DateTime _expiresAt;

    public FlashSalePromotion(string title, decimal discountPercent, DateTime expiresAt, IPromotionRenderer renderer)
        : base(title, renderer)
    {
        _discountPercent = discountPercent;
        _expiresAt = expiresAt;
    }

    public override string Publish()
    {
        var body = $"Reducere de {_discountPercent}% pana la {_expiresAt:dd.MM.yyyy}.";
        return Renderer.Render(Title, body, "Cumpara acum");
    }
}