namespace OnlineClothingStore.Core.Behavioral.Command;

public class CartCommandInvoker
{
    private readonly Stack<ICartCommand> _history = new();

    public void ExecuteCommand(ICartCommand command)
    {
        command.Execute();
        _history.Push(command);
    }

    public void UndoLastCommand()
    {
        if (_history.Count == 0)
        {
            Console.WriteLine("Nu există acțiuni de anulat.");
            return;
        }

        var command = _history.Pop();
        command.Undo();
    }
}