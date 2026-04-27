using OnlineClothingStore.Abstractions;
using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Infrastructure;

public sealed class InMemoryProductRepository : IRepository<Product>
{
    private readonly List<Product> _products = new();

    public void Add(Product item) => _products.Add(item);

    public IEnumerable<Product> GetAll() => _products;
}