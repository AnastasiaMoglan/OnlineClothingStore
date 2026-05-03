namespace OnlineClothingStore.Core.Behavioral.Iterator;

public interface IProductIterator
{
    bool HasNext();
    Product Next();
    void Reset();
}