namespace OnlineClothingStore.App.Behavioral.Iterator;

public sealed class OrderReviewItem
{
    public int Id { get; }
    public string CustomerName { get; }
    public string ProductName { get; }
    public decimal TotalPrice { get; }
    public string Status { get; }

    public OrderReviewItem(
        int id,
        string customerName,
        string productName,
        decimal totalPrice,
        string status)
    {
        Id = id;
        CustomerName = customerName;
        ProductName = productName;
        TotalPrice = totalPrice;
        Status = status;
    }
}