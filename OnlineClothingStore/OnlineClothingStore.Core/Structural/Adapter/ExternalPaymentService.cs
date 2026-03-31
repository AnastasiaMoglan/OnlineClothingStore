namespace OnlineClothingStore.App.Structural.Adapter;

public class ExternalPaymentService
{
    private readonly IExternalPaymentGateway _gateway;

    public ExternalPaymentService(IExternalPaymentGateway gateway)
    {
        _gateway = gateway;
    }

    public bool Checkout(string email, decimal total)
    {
        return _gateway.Pay(email, total, "MDL");
    }
}