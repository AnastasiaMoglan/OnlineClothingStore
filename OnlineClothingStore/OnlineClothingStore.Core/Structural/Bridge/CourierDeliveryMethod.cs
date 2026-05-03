namespace OnlineClothingStore.App.Structural.Bridge;

public class CourierDeliveryMethod : IDeliveryMethod
{
    public string Name => "Livrare prin curier";

    public decimal DeliveryPrice => 70;

    public string Deliver(string customerName, string address)
    {
        return $"Comanda pentru {customerName} va fi livrata prin curier la adresa: {address}.";
    }
}