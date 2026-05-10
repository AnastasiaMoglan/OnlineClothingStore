namespace OnlineClothingStore.App.Structural.Composite;

public interface IStoreItem
{
    string Name { get; }
    decimal GetPrice();
    void Display(int depth = 0);
}