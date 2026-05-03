namespace OnlineClothingStore.App.Structural.Bridge;

public abstract class DeliveryOrder
{
    protected readonly IDeliveryMethod DeliveryMethod;

    protected DeliveryOrder(IDeliveryMethod deliveryMethod)
    {
        DeliveryMethod = deliveryMethod;
    }

    public abstract string OrderType { get; }

    public abstract decimal ServicePrice { get; }

    public abstract string PrepareOrder();

    public decimal CalculateTotal(decimal productsTotal)
    {
        return productsTotal + ServicePrice + DeliveryMethod.DeliveryPrice;
    }

    public string CompleteDelivery(string customerName, string address)
    {
        return DeliveryMethod.Deliver(customerName, address);
    }

    public string GetDeliveryMethodName()
    {
        return DeliveryMethod.Name;
    }

    public decimal GetDeliveryPrice()
    {
        return DeliveryMethod.DeliveryPrice;
    }
}