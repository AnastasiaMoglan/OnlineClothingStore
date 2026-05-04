namespace OnlineClothingStore.App.Behavioral.Iterator;

public interface IOrderCollection
{
    IOrderIterator CreateIterator();
}