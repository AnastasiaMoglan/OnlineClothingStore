namespace OnlineClothingStore.Core.Behavioral.Command;

public interface ICartCommand
{
    string Description { get; }
    void Execute();
    void Undo();
}