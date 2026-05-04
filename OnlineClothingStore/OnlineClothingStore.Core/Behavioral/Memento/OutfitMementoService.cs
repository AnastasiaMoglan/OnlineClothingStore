namespace OnlineClothingStore.Core.Behavioral.Memento;

// Serviciu folosit de Controller pentru a demonstra pattern-ul în interfață.
public class OutfitMementoService
{
    public OutfitDesigner Designer { get; } = new();
    public OutfitHistory History { get; } = new();

    public void SaveCurrentState()
    {
        History.SaveState(Designer);
    }

    public void UpdateOutfit(
        string top,
        string bottom,
        string shoes,
        string accessory,
        string colorPalette,
        string notes)
    {
        Designer.UpdateOutfit(top, bottom, shoes, accessory, colorPalette, notes);
    }

    public bool Undo()
    {
        return History.Undo(Designer);
    }

    public bool Redo()
    {
        return History.Redo(Designer);
    }
}