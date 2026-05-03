namespace OnlineClothingStore.Core.Behavioral.Command;

public class ClearCartCommand : ICartCommand
{
    private readonly ShoppingCart _cart;
    private List<CartItem> _backupItems = new();

    public ClearCartCommand(ShoppingCart cart)
    {
        _cart = cart;
    }

    public string Description => "Golire coș";

    public void Execute()
    {
        _backupItems = _cart.Clear();
    }

    public void Undo()
    {
        _cart.RestoreItems(_backupItems);
    }
}