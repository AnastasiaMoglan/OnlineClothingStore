namespace OnlineClothingStore.App.Behavioral.State;

public interface IUserAccountState
{
    string StateName { get; }

    void Login(UserAccountContext account);
    void UpgradeToPremium(UserAccountContext account);
    void Block(UserAccountContext account);

    bool CanBuy();
    bool CanUseWishlist();
    decimal GetDiscountPercent();
}