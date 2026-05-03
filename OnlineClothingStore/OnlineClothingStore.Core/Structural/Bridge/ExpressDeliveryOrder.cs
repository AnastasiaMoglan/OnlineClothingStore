namespace OnlineClothingStore.App.Structural.Bridge;

public class ExpressDeliveryOrder : DeliveryOrder
{
    public ExpressDeliveryOrder(IDeliveryMethod deliveryMethod)
        : base(deliveryMethod)
    {
    }

    public override string OrderType => "Comanda expres";

    public override decimal ServicePrice => 80;

    public override string PrepareOrder()
    {
        return "Comanda expres a fost prioritizata si va fi procesata mai rapid.";
    }
}