namespace OnlineClothingStore.App.Structural.Bridge;

public interface IDeliveryMethod
{
    string Name { get; }

    decimal DeliveryPrice { get; }

    string Deliver(string customerName, string address);
}