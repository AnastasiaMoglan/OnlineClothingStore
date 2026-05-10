namespace OnlineClothingStore.App.Structural.Adapter;

public class StripePaymentAdapter : IExternalPaymentGateway
{
    private readonly StripeApi _stripeApi;

    public StripePaymentAdapter(StripeApi stripeApi)
    {
        _stripeApi = stripeApi;
    }

    public bool Pay(string customerEmail, decimal amount, string currency)
    {
        var cents = (long)(amount * 100);
        var chargeId = _stripeApi.CreateCharge(customerEmail, cents, currency);
        return !string.IsNullOrWhiteSpace(chargeId);
    }

    public bool Refund(string transactionId, decimal amount, string currency)
    {
        var cents = (long)(amount * 100);
        return _stripeApi.CreateRefund(transactionId, cents);
    }
}