namespace OnlineClothingStore.Domain;

public sealed class Admin : User
{
    public Admin(string username) : base(username) { }
    public override string GetRole() => "Admin";
}