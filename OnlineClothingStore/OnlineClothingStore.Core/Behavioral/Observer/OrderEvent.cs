namespace OnlineClothingStore.Core.Behavioral.Observer;

public class OrderEvent
{
    public string OrderId { get; }
    public string CustomerEmail { get; }
    public decimal TotalAmount { get; }
    public DateTime CreatedAt { get; }

    public OrderEvent(string orderId, string customerEmail, decimal totalAmount)
    {
        OrderId = orderId;
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
        CreatedAt = DateTime.Now;
    }
}