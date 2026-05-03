namespace OnlineClothingStore.App.Structural.Bridge;

public class StandardDeliveryOrder : DeliveryOrder
{
    public StandardDeliveryOrder(IDeliveryMethod deliveryMethod)
        : base(deliveryMethod)
    {
    }

    public override string OrderType => "Comanda standard";

    public override decimal ServicePrice => 0;

    public override string PrepareOrder()
    {
        return "Comanda standard a fost pregatita normal, fara servicii suplimentare.";
    }
}