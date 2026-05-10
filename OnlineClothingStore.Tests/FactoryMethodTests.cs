using OnlineClothingStore.Abstractions;
using OnlineClothingStore.Creational.FactoryMethod;
using Xunit;

namespace OnlineClothingStore.Tests;

public class FactoryMethodTests
{
    [Fact]
    public void CardPaymentFactory_Should_Create_CardPayment()
    {
        IPaymentFactory factory = new CardPaymentFactory();
        var payment = factory.CreatePayment();

        Assert.Contains("CardPayment", payment.GetType().Name);
    }
}