namespace OnlineClothingStore.Core.Behavioral.Observer;

public class CustomerNotificationObserver : IOrderObserver
{
    public void Update(OrderEvent orderEvent)
    {
        Console.WriteLine(
            $"Email către client: Comanda {orderEvent.OrderId} a fost plasată. Total: {orderEvent.TotalAmount} lei."
        );
    }
}