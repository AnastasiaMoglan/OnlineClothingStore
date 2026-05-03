namespace OnlineClothingStore.App.Behavioral.Mediator;

public interface ICheckoutMediator
{
    void Notify(CheckoutComponent sender, string eventName);
}