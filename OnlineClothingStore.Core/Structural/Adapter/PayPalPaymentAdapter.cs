namespace OnlineClothingStore.App.Structural.Adapter;

public class PayPalPaymentAdapter : IExternalPaymentGateway
{
    private readonly PayPalApi _payPalApi;

    public PayPalPaymentAdapter(PayPalApi payPalApi)
    {
        _payPalApi = payPalApi;
    }

    public bool Pay(string customerEmail, decimal amount, string currency)
    {
        return _payPalApi.SendPayment(amount, currency, customerEmail);
    }

    public bool Refund(string transactionId, decimal amount, string currency)
    {
        return _payPalApi.SendRefund(transactionId, amount);
    }
}