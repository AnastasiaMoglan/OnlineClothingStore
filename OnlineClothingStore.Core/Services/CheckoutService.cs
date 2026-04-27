using OnlineClothingStore.Abstractions;
using OnlineClothingStore.Services;

namespace OnlineClothingStore.Services;

public sealed class CheckoutService
{
    private readonly IPaymentFactory _paymentFactory;

    public CheckoutService(IPaymentFactory paymentFactory)
        => _paymentFactory = paymentFactory;

    public void Checkout(Order order)
    {
        var payment = _paymentFactory.CreatePayment();
        order.ProcessPayment(payment);
    }
}