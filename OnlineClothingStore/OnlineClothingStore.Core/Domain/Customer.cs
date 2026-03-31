namespace OnlineClothingStore.Domain;

public sealed class Customer : User
{
    public Customer(string username) : base(username) { }
    public override string GetRole() => "Customer";
}