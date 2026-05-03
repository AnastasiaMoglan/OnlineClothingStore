namespace OnlineClothingStore.App.Behavioral.State;

public class UserAccountContext
{
    public string Email { get; }
    public IUserAccountState State { get; private set; }

    public UserAccountContext(string email)
    {
        Email = email;
        State = new GuestState();
    }

    public void TransitionTo(IUserAccountState newState)
    {
        State = newState;
    }

    public void Login()
    {
        State.Login(this);
    }

    public void UpgradeToPremium()
    {
        State.UpgradeToPremium(this);
    }

    public void Block()
    {
        State.Block(this);
    }

    public bool CanBuy()
    {
        return State.CanBuy();
    }

    public bool CanUseWishlist()
    {
        return State.CanUseWishlist();
    }

    public decimal GetDiscountPercent()
    {
        return State.GetDiscountPercent();
    }
}