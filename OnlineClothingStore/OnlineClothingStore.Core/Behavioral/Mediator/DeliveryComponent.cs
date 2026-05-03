namespace OnlineClothingStore.App.Behavioral.Mediator;

public class DeliveryComponent : CheckoutComponent
{
    public string DeliveryType { get; private set; } = "Standard";
    public decimal DeliveryPrice { get; private set; }

    public void SelectDelivery(string type, decimal price)
    {
        DeliveryType = type;
        DeliveryPrice = price;
        Mediator?.Notify(this, "DeliveryChanged");
    }
}