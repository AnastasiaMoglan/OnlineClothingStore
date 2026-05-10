namespace OnlineClothingStore.App.Structural.Bridge;

public sealed class StoreDisplayPromotionRenderer : IPromotionRenderer
{
    public string Render(string title, string body, string cta)
    {
        return $"[StoreDisplay] {title} | {body} | CTA={cta}";
    }
}