using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Creational.FactoryMethod;

public abstract class PaymentFactory
{
    public abstract Payment CreatePayment();

    public void Pay(decimal amount)
    {
        var payment = CreatePayment();
        payment.Pay(amount);
    }
}