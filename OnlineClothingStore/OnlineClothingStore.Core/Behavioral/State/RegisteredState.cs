namespace OnlineClothingStore.App.Behavioral.State;

public class RegisteredState : IUserAccountState
{
    public string StateName => "Registered";

    public void Login(UserAccountContext account)
    {
        Console.WriteLine("Utilizatorul este deja autentificat.");
    }

    public void UpgradeToPremium(UserAccountContext account)
    {
        account.TransitionTo(new PremiumState());
    }

    public void Block(UserAccountContext account)
    {
        account.TransitionTo(new BlockedState());
    }

    public bool CanBuy()
    {
        return true;
    }

    public bool CanUseWishlist()
    {
        return true;
    }

    public decimal GetDiscountPercent()
    {
        return 5;
    }
}