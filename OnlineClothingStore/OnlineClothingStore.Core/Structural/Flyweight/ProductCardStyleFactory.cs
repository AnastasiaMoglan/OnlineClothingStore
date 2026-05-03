namespace OnlineClothingStore.App.Structural.Flyweight;

public class ProductCardStyleFactory
{
    private readonly Dictionary<string, ProductCardStyle> _styles = new();

    public ProductCardStyle GetStyle(string category)
    {
        if (_styles.TryGetValue(category, out ProductCardStyle? existingStyle))
        {
            return existingStyle;
        }

        ProductCardStyle newStyle = category switch
        {
            "T-Shirts" => new ProductCardStyle(
                category,
                "#2563eb",
                "#ffffff",
                "Arial",
                "#eff6ff"),

            "Jeans" => new ProductCardStyle(
                category,
                "#1e3a8a",
                "#ffffff",
                "Arial",
                "#f8fafc"),

            "Jackets" => new ProductCardStyle(
                category,
                "#334155",
                "#ffffff",
                "Arial",
                "#f1f5f9"),

            "Hoodies" => new ProductCardStyle(
                category,
                "#7c3aed",
                "#ffffff",
                "Arial",
                "#f5f3ff"),

            "Dresses" => new ProductCardStyle(
                category,
                "#be185d",
                "#ffffff",
                "Arial",
                "#fdf2f8"),

            "Shoes" => new ProductCardStyle(
                category,
                "#059669",
                "#ffffff",
                "Arial",
                "#ecfdf5"),

            _ => new ProductCardStyle(
                category,
                "#64748b",
                "#ffffff",
                "Arial",
                "#f8fafc")
        };

        _styles[category] = newStyle;

        return newStyle;
    }

    public void UpdateStyle(
        string category,
        string badgeColor,
        string textColor,
        string fontFamily,
        string backgroundColor)
    {
        ProductCardStyle style = GetStyle(category);

        style.UpdateStyle(
            badgeColor,
            textColor,
            fontFamily,
            backgroundColor);
    }

    public int CreatedStylesCount => _styles.Count;
}