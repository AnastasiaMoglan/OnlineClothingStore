namespace OnlineClothingStore.Core.Behavioral.Observer;

public interface IOrderObserver
{
    void Update(OrderEvent orderEvent);
}