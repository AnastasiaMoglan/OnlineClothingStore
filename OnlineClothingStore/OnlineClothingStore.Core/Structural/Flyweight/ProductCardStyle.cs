namespace OnlineClothingStore.App.Structural.Flyweight;

public class ProductCardStyle
{
    public string Category { get; }

    public string BadgeColor { get; private set; }

    public string TextColor { get; private set; }

    public string FontFamily { get; private set; }

    public string BackgroundColor { get; private set; }

    public ProductCardStyle(
        string category,
        string badgeColor,
        string textColor,
        string fontFamily,
        string backgroundColor)
    {
        Category = category;
        BadgeColor = badgeColor;
        TextColor = textColor;
        FontFamily = fontFamily;
        BackgroundColor = backgroundColor;
    }

    public void UpdateStyle(
        string badgeColor,
        string textColor,
        string fontFamily,
        string backgroundColor)
    {
        BadgeColor = badgeColor;
        TextColor = textColor;
        FontFamily = fontFamily;
        BackgroundColor = backgroundColor;
    }
}