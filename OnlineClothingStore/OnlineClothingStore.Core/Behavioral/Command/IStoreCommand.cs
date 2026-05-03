namespace OnlineClothingStore.App.Structural.Command;

public interface IStoreCommand
{
    string Name { get; }

    string Description { get; }

    void Execute();

    void Undo();
}