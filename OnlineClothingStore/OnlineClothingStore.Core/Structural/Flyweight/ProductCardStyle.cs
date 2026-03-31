namespace OnlineClothingStore.App.Structural.Flyweight;

public sealed class ProductCardStyle
{
    public string Category { get; }
    public string BadgeColor { get; }
    public string FontColor { get; }
    public string FontFamily { get; }

    public ProductCardStyle(string category, string badgeColor, string fontColor, string fontFamily)
    {
        Category = category;
        BadgeColor = badgeColor;
        FontColor = fontColor;
        FontFamily = fontFamily;
    }

    public string Render(ProductCardContext context)
    {
        return $"[{Category}] {context.ProductName} | {context.Size} | {context.CollectionName} | " +
               $"{context.Price:0.00} MDL | badge={BadgeColor} | font={FontFamily}/{FontColor}";
    }
}