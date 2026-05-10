namespace OnlineClothingStore.Domain;

public sealed class Cart
{
    private readonly List<Product> _products = new();
    public IReadOnlyList<Product> Products => _products;

    public void AddProduct(Product product)
    {
        _products.Add(product ?? throw new ArgumentNullException(nameof(product)));
    }

    public decimal GetTotal() => _products.Sum(p => p.GetFinalPrice());
}
