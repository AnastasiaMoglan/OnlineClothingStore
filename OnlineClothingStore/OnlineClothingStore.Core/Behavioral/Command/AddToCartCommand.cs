namespace OnlineClothingStore.Core.Behavioral.Command;

public class AddToCartCommand : ICartCommand
{
    private readonly ShoppingCart _cart;
    private readonly CartItem _item;

    public AddToCartCommand(ShoppingCart cart, CartItem item)
    {
        _cart = cart;
        _item = item;
    }

    public string Description => $"Adăugare produs în coș: {_item.ProductName}";

    public void Execute()
    {
        _cart.AddItem(_item);
    }

    public void Undo()
    {
        _cart.RemoveItem(_item.ProductId);
    }
}