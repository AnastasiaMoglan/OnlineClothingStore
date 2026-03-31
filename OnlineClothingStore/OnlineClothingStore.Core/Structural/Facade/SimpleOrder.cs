namespace OnlineClothingStore.App.Structural.Facade;

public class SimpleOrder
{
    public decimal Total { get; }

    public SimpleOrder(decimal total)
    {
        Total = total;
    }
}