using OnlineClothingStore.Domain;
using OnlineClothingStore.Pricing;

namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class VipKitFactory : IStoreKitFactory
{
    public Product CreateProduct(string name, decimal price)
        => new ClothingProduct(
            name,
            price,
            "L",
            "Black",
            "Premium Cotton",
            false,
            null,
            true
        );

    public Discount CreateDiscount()
        => new PercentageDiscount(0.15m); // 15%
}