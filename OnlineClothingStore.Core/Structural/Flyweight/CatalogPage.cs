namespace OnlineClothingStore.App.Structural.Flyweight;

public sealed class CatalogPage
{
    private readonly ProductCardStyleFactory _styleFactory;
    private readonly List<(ProductCardContext Context, ProductCardStyle Style)> _products = new();

    public CatalogPage(ProductCardStyleFactory styleFactory)
    {
        _styleFactory = styleFactory;
    }

public int TotalCards => _products.Count;
public int SharedStyles => _styleFactory.CreatedStylesCount;
public int SavedObjects => TotalCards - SharedStyles;

public void AddProduct(
    ProductCardContext context,
    string category,
    string badgeColor,
    string fontColor,
    string fontFamily)
{
    var style = _styleFactory.GetStyle(category, badgeColor, fontColor, fontFamily);
    _products.Add((context, style));
}

public IReadOnlyList<string> Render()
{
    return _products
        .Select(x => x.Style.Render(x.Context))
        .ToList()
        .AsReadOnly();
}
}