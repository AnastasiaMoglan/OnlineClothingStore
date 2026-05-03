namespace OnlineClothingStore.App.Structural.Command;

public class UpdateStockCommand : IStoreCommand
{
    private readonly CommandProduct _product;
    private readonly int _newStock;
    private readonly int _oldStock;

    public UpdateStockCommand(CommandProduct product, int newStock)
    {
        _product = product;
        _newStock = newStock < 0 ? 0 : newStock;
        _oldStock = product.StockQuantity;
    }

    public string Name => "Modificare stoc";

    public string Description =>
        $"Produsul {_product.Name}: stoc schimbat de la {_oldStock} la {_newStock}.";

    public void Execute()
    {
        _product.StockQuantity = _newStock;
    }

    public void Undo()
    {
        _product.StockQuantity = _oldStock;
    }
}