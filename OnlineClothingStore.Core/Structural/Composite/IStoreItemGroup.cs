namespace OnlineClothingStore.App.Structural.Composite;

public interface IStoreItemGroup : IStoreItem
{
    void Add(IStoreItem item);
    void Remove(IStoreItem item);
    IReadOnlyCollection<IStoreItem> GetChildren();
}