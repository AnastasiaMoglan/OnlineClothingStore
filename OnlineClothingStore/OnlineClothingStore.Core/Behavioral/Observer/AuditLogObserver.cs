namespace OnlineClothingStore.Core.Behavioral.Observer;

public class AuditLogObserver : IOrderObserver
{
    public void Update(OrderEvent orderEvent)
    {
        Console.WriteLine(
            $"AUDIT: {orderEvent.CreatedAt} | {orderEvent.OrderId} | {orderEvent.CustomerEmail} | {orderEvent.TotalAmount} lei"
        );
    }
}