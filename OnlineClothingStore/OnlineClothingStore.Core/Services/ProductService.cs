using OnlineClothingStore.Abstractions;
using OnlineClothingStore.Creational.AbstractFactory;
using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Services;

public sealed class ProductService
{
    private readonly IRepository<Product> _repository;
    private readonly IStoreKitFactory _kitFactory;

    public ProductService(IRepository<Product> repository, IStoreKitFactory kitFactory)
    {
        _repository = repository;
        _kitFactory = kitFactory;
    }

    public Product AddProduct(string name, decimal price)
    {
        var product = _kitFactory.CreateProduct(name, price);
        _repository.Add(product);
        return product;
    }

    public decimal GetFinalPrice(Product product)
    {
        var discount = _kitFactory.CreateDiscount();

        // IMPORTANT:
        // discount must be applied to the configured product price,
        // not just the base constructor price.
        var configuredPrice = product.GetFinalPrice();

        return discount.Apply(configuredPrice);
    }
}