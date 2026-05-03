namespace OnlineClothingStore.App.Behavioral.Mediator;

public abstract class CheckoutComponent
{
    protected ICheckoutMediator? Mediator;

    public void SetMediator(ICheckoutMediator mediator)
    {
        Mediator = mediator;
    }
}