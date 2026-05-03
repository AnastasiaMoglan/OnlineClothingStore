namespace OnlineClothingStore.Core.Behavioral.Memento;

public class CartHistory
{
    private readonly Stack<CartSnapshot> _undoStack = new();

    public void SaveState(CartOriginator cart)
    {
        _undoStack.Push(cart.Save());
    }

    public void RestoreLastState(CartOriginator cart)
    {
        if (_undoStack.Count == 0)
        {
            Console.WriteLine("Nu există stare salvată.");
            return;
        }

        var snapshot = _undoStack.Pop();
        cart.Restore(snapshot);
    }
}