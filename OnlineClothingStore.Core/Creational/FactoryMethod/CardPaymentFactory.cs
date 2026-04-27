using OnlineClothingStore.Abstractions;
using OnlineClothingStore.Domain;
using OnlineClothingStore.Payments;

namespace OnlineClothingStore.Creational.FactoryMethod;

public sealed class CardPaymentFactory : IPaymentFactory
{
    public Payment CreatePayment() => new CardPayment();
}