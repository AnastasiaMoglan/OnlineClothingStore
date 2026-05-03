namespace OnlineClothingStore.App.Structural.Command;

public class ChangePriceCommand : IStoreCommand
{
    private readonly CommandProduct _product;
    private readonly decimal _newPrice;
    private readonly decimal _oldPrice;

    public ChangePriceCommand(CommandProduct product, decimal newPrice)
    {
        _product = product;
        _newPrice = newPrice < 0 ? 0 : newPrice;
        _oldPrice = product.Price;
    }

    public string Name => "Modificare preț";

    public string Description =>
        $"Produsul {_product.Name}: preț schimbat de la {_oldPrice} MDL la {_newPrice} MDL.";

    public void Execute()
    {
        _product.Price = _newPrice;
    }

    public void Undo()
    {
        _product.Price = _oldPrice;
    }
}