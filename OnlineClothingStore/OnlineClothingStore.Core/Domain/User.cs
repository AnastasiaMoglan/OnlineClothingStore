namespace OnlineClothingStore.Domain;

public abstract class User : Entity
{
    public string Username { get; protected set; }

    protected User(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username required");
        Username = username;
    }

    public abstract string GetRole();
}