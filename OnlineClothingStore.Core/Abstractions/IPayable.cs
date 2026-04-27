using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Abstractions;

public interface IPayable
{
    void ProcessPayment(Payment payment);
}