namespace OnlineClothingStore.Core.Behavioral.Memento;

// CARETAKER
// Gestionează istoricul de stări, dar nu modifică direct conținutul snapshot-ului.
public class OutfitHistory
{
    private readonly Stack<OutfitDraftMemento> _undoStack = new();
    private readonly Stack<OutfitDraftMemento> _redoStack = new();

    public int UndoCount => _undoStack.Count;
    public int RedoCount => _redoStack.Count;

    public void SaveState(OutfitDesigner designer)
    {
        _undoStack.Push(designer.Save());
        _redoStack.Clear();
    }

    public bool Undo(OutfitDesigner designer)
    {
        if (_undoStack.Count == 0)
            return false;

        _redoStack.Push(designer.Save());

        OutfitDraftMemento previousState = _undoStack.Pop();
        designer.Restore(previousState);

        return true;
    }

    public bool Redo(OutfitDesigner designer)
    {
        if (_redoStack.Count == 0)
            return false;

        _undoStack.Push(designer.Save());

        OutfitDraftMemento nextState = _redoStack.Pop();
        designer.Restore(nextState);

        return true;
    }

    public List<string> GetHistoryLabels()
    {
        return _undoStack
            .Select(snapshot => snapshot.GetLabel())
            .ToList();
    }
}