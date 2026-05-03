namespace OnlineClothingStore.App.Behavioral.State;

public class BlockedState : IUserAccountState
{
    public string StateName => "Blocked";

    public void Login(UserAccountContext account)
    {
        throw new InvalidOperationException("Contul este blocat și nu poate fi autentificat.");
    }

    public void UpgradeToPremium(UserAccountContext account)
    {
        throw new InvalidOperationException("Contul blocat nu poate fi actualizat la Premium.");
    }

    public void Block(UserAccountContext account)
    {
        Console.WriteLine("Contul este deja blocat.");
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