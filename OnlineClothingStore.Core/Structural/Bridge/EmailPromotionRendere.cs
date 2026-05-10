namespace OnlineClothingStore.App.Structural.Bridge;

public sealed class EmailPromotionRenderer : IPromotionRenderer
{
    public string Render(string title, string body, string cta)
    {
        return $"[Email] {title} | {body} | CTA={cta}";
    }
}