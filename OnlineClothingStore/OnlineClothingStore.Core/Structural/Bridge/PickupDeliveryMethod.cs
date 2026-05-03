namespace OnlineClothingStore.App.Structural.Bridge;

public class PickupDeliveryMethod : IDeliveryMethod
{
    public string Name => "Ridicare din magazin";

    public decimal DeliveryPrice => 0;

    public string Deliver(string customerName, string address)
    {
        return $"Comanda pentru {customerName} va fi pregatita pentru ridicare din magazinul BlueWear.";
    }
}