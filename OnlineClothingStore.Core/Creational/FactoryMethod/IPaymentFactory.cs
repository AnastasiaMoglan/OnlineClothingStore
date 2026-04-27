using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Abstractions;

public interface IPaymentFactory
{
    Payment CreatePayment();
}