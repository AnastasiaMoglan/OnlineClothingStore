namespace OnlineClothingStore.App.Structural.Bridge;

public sealed class MobileAppPromotionRenderer : IPromotionRenderer
{
    public string Render(string title, string body, string cta)
    {
        return $"[MobileApp] {title} | {body} | CTA={cta}";
    }
}