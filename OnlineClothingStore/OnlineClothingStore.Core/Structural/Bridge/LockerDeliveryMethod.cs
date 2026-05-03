namespace OnlineClothingStore.App.Structural.Bridge;

public class LockerDeliveryMethod : IDeliveryMethod
{
    public string Name => "Livrare la locker";

    public decimal DeliveryPrice => 45;

    public string Deliver(string customerName, string address)
    {
        return $"Comanda pentru {customerName} va fi livrata la lockerul selectat: {address}.";
    }
}