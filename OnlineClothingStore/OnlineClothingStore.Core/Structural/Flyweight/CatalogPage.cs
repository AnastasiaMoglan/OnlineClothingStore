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

    public void AddProduct(ProductCardContext context)
    {
        ProductCardStyle style = _styleFactory.GetStyle(context.Category);

        _products.Add((context, style));
    }

    public IReadOnlyList<string> Render()
    {
        List<string> result = new();

        foreach (var product in _products)
        {
            string cardText =
                $"Produs: {product.Context.Name} | " +
                $"Categorie: {product.Context.Category} | " +
                $"Pret: {product.Context.Price} MDL | " +
                $"Marime: {product.Context.Size} | " +
                $"Stil partajat: {product.Style.Category}, " +
                $"{product.Style.BadgeColor}, " +
                $"{product.Style.TextColor}, " +
                $"{product.Style.FontFamily}, " +
                $"{product.Style.BackgroundColor}";

            result.Add(cardText);
        }

        return result.AsReadOnly();
    }
}