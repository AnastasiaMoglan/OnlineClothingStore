namespace OnlineClothingStore.App.Behavioral.Mediator;

public class CheckoutSummaryComponent : CheckoutComponent
{
    public string Address { get; private set; } = string.Empty;
    public string DeliveryType { get; private set; } = string.Empty;
    public string PaymentMethod { get; private set; } = string.Empty;
    public decimal Total { get; private set; }

    public void Refresh(string address, string delivery, string payment, decimal total)
    {
        Address = address;
        DeliveryType = delivery;
        PaymentMethod = payment;
        Total = total;
    }
}