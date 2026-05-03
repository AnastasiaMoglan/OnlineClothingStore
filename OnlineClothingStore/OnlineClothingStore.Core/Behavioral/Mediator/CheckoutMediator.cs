namespace OnlineClothingStore.App.Behavioral.Mediator;

public class CheckoutMediator : ICheckoutMediator
{
    private readonly AddressComponent _addressComponent;
    private readonly DeliveryComponent _deliveryComponent;
    private readonly PaymentComponent _paymentComponent;
    private readonly CheckoutSummaryComponent _summaryComponent;
    private readonly decimal _baseTotal;

    public CheckoutMediator(
        AddressComponent addressComponent,
        DeliveryComponent deliveryComponent,
        PaymentComponent paymentComponent,
        CheckoutSummaryComponent summaryComponent,
        decimal baseTotal)
    {
        _addressComponent = addressComponent;
        _deliveryComponent = deliveryComponent;
        _paymentComponent = paymentComponent;
        _summaryComponent = summaryComponent;
        _baseTotal = baseTotal;

        _addressComponent.SetMediator(this);
        _deliveryComponent.SetMediator(this);
        _paymentComponent.SetMediator(this);
        _summaryComponent.SetMediator(this);
    }

    public void Notify(CheckoutComponent sender, string eventName)
    {
        decimal total = _baseTotal + _deliveryComponent.DeliveryPrice;

        _summaryComponent.Refresh(
            _addressComponent.Address,
            _deliveryComponent.DeliveryType,
            _paymentComponent.PaymentMethod,
            total
        );

        Console.WriteLine($"Checkout actualizat după evenimentul: {eventName}");
    }
}