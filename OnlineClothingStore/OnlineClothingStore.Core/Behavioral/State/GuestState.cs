namespace OnlineClothingStore.App.Behavioral.State;

public class GuestState : IUserAccountState
{
    public string StateName => "Guest";

    public void Login(UserAccountContext account)
    {
        account.TransitionTo(new RegisteredState());
    }

    public void UpgradeToPremium(UserAccountContext account)
    {
        throw new InvalidOperationException("Utilizatorul trebuie mai întâi să fie autentificat.");
    }

    public void Block(UserAccountContext account)
    {
        account.TransitionTo(new BlockedState());
    }

    public bool CanBuy()
    {
        return false;
    }

    public bool CanUseWishlist()
    {
        return false;
    }

    public decimal GetDiscountPercent()
    {
        return 0;
    }
}