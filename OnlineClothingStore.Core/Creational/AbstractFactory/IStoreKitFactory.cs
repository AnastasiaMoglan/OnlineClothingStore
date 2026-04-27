using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Creational.AbstractFactory;

public interface IStoreKitFactory
{
    Product CreateProduct(string name, decimal price);
    Discount CreateDiscount();
}