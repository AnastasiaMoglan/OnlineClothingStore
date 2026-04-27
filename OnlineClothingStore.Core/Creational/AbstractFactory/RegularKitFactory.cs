using OnlineClothingStore.Domain;
using OnlineClothingStore.Pricing;

namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class RegularKitFactory : IStoreKitFactory
{
    public Product CreateProduct(string name, decimal price)
        => new BasicProduct(name, price);

    public Discount CreateDiscount()
        => new NoDiscount();
}