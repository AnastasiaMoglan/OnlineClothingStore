namespace OnlineClothingStore.App.Behavioral.Mediator;

public class AddressComponent : CheckoutComponent
{
    public string Address { get; private set; } = string.Empty;

    public void SetAddress(string address)
    {
        Address = address;
        Mediator?.Notify(this, "AddressChanged");
    }
}