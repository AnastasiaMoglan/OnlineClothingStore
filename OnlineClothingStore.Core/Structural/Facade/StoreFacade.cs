namespace OnlineClothingStore.App.Structural.Facade;

public class StoreFacade
{
    private readonly CartService _cartService;
    private readonly PaymentService _paymentService;
    private readonly NotificationService _notificationService;

    public StoreFacade()
    {
        _cartService = new CartService();
        _paymentService = new PaymentService();
        _notificationService = new NotificationService();
    }

    public SimpleOrder PlaceOrder(string email, List<decimal> prices)
    {
        var total = _cartService.CalculateTotal(prices);

        var paymentSuccess = _paymentService.ProcessPayment(total);

        if (!paymentSuccess)
            throw new Exception("Payment failed");

        _notificationService.SendConfirmation(email);

        return new SimpleOrder(total);
    }
}