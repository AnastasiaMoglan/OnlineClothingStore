namespace OnlineClothingStore.Core.Behavioral.Command;

public class RemoveFromCartCommand : ICartCommand
{
    private readonly ShoppingCart _cart;
    private readonly int _productId;
    private CartItem? _removedItem;

    public RemoveFromCartCommand(ShoppingCart cart, int productId)
    {
        _cart = cart;
        _productId = productId;
    }

    public string Description => $"Ștergere produs din coș cu ID {_productId}";

    public void Execute()
    {
        _removedItem = _cart.RemoveItem(_productId);
    }

    public void Undo()
    {
        if (_removedItem != null)
        {
            _cart.AddItem(_removedItem);
        }
    }
}