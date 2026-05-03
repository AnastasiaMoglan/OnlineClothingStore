namespace OnlineClothingStore.App.Structural.Command;

public class StoreCommandInvoker
{
    private readonly Stack<IStoreCommand> _history = new();

    public IReadOnlyList<IStoreCommand> History => _history.ToList();

    public string LastMessage { get; private set; } = "Nu a fost executată nicio comandă.";

    public void ExecuteCommand(IStoreCommand command)
    {
        command.Execute();
        _history.Push(command);
        LastMessage = $"Executat: {command.Description}";
    }

    public void UndoLastCommand()
    {
        if (_history.Count == 0)
        {
            LastMessage = "Nu există comenzi pentru Undo.";
            return;
        }

        IStoreCommand command = _history.Pop();
        command.Undo();
        LastMessage = $"Undo: {command.Description}";
    }
}