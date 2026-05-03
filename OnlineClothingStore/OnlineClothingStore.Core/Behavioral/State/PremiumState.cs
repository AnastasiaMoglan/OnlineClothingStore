namespace OnlineClothingStore.App.Behavioral.State;

public class PremiumState : IUserAccountState
{
    public string StateName => "Premium";

    public void Login(UserAccountContext account)
    {
        Console.WriteLine("Utilizatorul premium este deja autentificat.");
    }

    public void UpgradeToPremium(UserAccountContext account)
    {
        Console.WriteLine("Utilizatorul este deja Premium.");
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
        return 15;
    }
}