namespace OnlineClothingStore.App.Structural.Observer;

public class StockNotification
{
    public string CustomerName { get; }

    public string Email { get; }

    public string ProductName { get; }

    public string Message { get; }

    public StockNotification(
        string customerName,
        string email,
        string productName,
        string message)
    {
        CustomerName = customerName;
        Email = email;
        ProductName = productName;
        Message = message;
    }
}