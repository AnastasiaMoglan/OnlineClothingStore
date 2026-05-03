namespace OnlineClothingStore.App.Structural.Observer;

public class CustomerStockObserver : IStockObserver
{
    public int ProductId { get; }

    public string CustomerName { get; }

    public string Email { get; }

    public CustomerStockObserver(
        int productId,
        string customerName,
        string email)
    {
        ProductId = productId;
        CustomerName = customerName;
        Email = email;
    }

    public StockNotification Update(StockChangedEvent stockEvent)
    {
        string message =
            $"Salut, {CustomerName}! Stocul pentru produsul {stockEvent.ProductName} s-a modificat de la {stockEvent.OldStock} la {stockEvent.NewStock} bucati.";

        return new StockNotification(
            CustomerName,
            Email,
            stockEvent.ProductName,
            message
        );
    }
}