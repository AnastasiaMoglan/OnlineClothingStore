namespace OnlineClothingStore.Core.Behavioral.Memento;

// MEMENTO
// Salvează starea unei ținute la un anumit moment.
public sealed class OutfitDraftMemento
{
    internal string Top { get; }
    internal string Bottom { get; }
    internal string Shoes { get; }
    internal string Accessory { get; }
    internal string ColorPalette { get; }
    internal string Notes { get; }

    public DateTime SavedAt { get; }

    internal OutfitDraftMemento(
        string top,
        string bottom,
        string shoes,
        string accessory,
        string colorPalette,
        string notes)
    {
        Top = top;
        Bottom = bottom;
        Shoes = shoes;
        Accessory = accessory;
        ColorPalette = colorPalette;
        Notes = notes;
        SavedAt = DateTime.Now;
    }

    public string GetLabel()
    {
        return $"Snapshot salvat la {SavedAt:HH:mm:ss}";
    }
}