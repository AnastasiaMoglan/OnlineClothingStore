namespace OnlineClothingStore.App.Behavioral.Iterator;

public interface IOrderIterator
{
    bool HasNext();
    OrderReviewItem Next();
}