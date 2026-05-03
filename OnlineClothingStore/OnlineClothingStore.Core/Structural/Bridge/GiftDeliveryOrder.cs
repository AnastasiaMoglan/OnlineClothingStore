namespace OnlineClothingStore.App.Structural.Bridge;

public class GiftDeliveryOrder : DeliveryOrder
{
    public GiftDeliveryOrder(IDeliveryMethod deliveryMethod)
        : base(deliveryMethod)
    {
    }

    public override string OrderType => "Comanda cadou";

    public override decimal ServicePrice => 120;

    public override string PrepareOrder()
    {
        return "Comanda cadou a fost ambalata special si include mesaj personalizat.";
    }
}