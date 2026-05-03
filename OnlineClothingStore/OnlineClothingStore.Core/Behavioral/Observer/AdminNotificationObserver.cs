namespace OnlineClothingStore.Core.Behavioral.Observer;

public class AdminNotificationObserver : IOrderObserver
{
    public void Update(OrderEvent orderEvent)
    {
        Console.WriteLine(
            $"Admin notificat: comandă nouă {orderEvent.OrderId}, total {orderEvent.TotalAmount} lei."
        );
    }
}