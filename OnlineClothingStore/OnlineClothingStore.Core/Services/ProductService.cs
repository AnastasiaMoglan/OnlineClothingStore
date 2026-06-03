using OnlineClothingStore.Abstractions;
using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Services;

public sealed class ProductService
{
    private readonly IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public Product AddProduct(Product product)
    {
        _repository.Add(product);
        return product;
    }

    public decimal GetFinalPrice(Product product)
    {
        return product.GetFinalPrice();
    }
}