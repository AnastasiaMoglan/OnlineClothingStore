namespace OnlineClothingStore.App.Behavioral.Mediator;

public class PaymentComponent : CheckoutComponent
{
    public string PaymentMethod { get; private set; } = "Not selected";

    public void SelectPayment(string method)
    {
        PaymentMethod = method;
        Mediator?.Notify(this, "PaymentChanged");
    }
}