namespace OnlineClothingStore.App.Structural.Flyweight;

public sealed class ProductCardStyleFactory
{
    private readonly Dictionary<string, ProductCardStyle> _styles = new();

    public int CreatedStylesCount => _styles.Count;

    public ProductCardStyle GetStyle(string category, string badgeColor, string fontColor, string fontFamily)
    {
        var key = $"{category}|{badgeColor}|{fontColor}|{fontFamily}";

        if (!_styles.ContainsKey(key))
        {
            _styles[key] = new ProductCardStyle(category, badgeColor, fontColor, fontFamily);
        }

        return _styles[key];
    }
}