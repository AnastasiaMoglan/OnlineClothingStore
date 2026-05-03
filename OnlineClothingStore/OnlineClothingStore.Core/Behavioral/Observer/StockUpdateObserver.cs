namespace OnlineClothingStore.Core.Behavioral.Observer;

public class StockUpdateObserver : IOrderObserver
{
    public void Update(OrderEvent orderEvent)
    {
        Console.WriteLine(
            $"Stoc actualizat după comanda {orderEvent.OrderId}."
        );
    }
}