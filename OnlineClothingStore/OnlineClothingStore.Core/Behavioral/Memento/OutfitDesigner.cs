namespace OnlineClothingStore.Core.Behavioral.Memento;

// ORIGINATOR
// Obiectul principal a cărui stare se salvează și se restaurează.
public class OutfitDesigner
{
    public string Top { get; private set; } = "Tricou basic";
    public string Bottom { get; private set; } = "Jeans albaștri";
    public string Shoes { get; private set; } = "Sneakers albi";
    public string Accessory { get; private set; } = "Geantă mică";
    public string ColorPalette { get; private set; } = "Blue casual";
    public string Notes { get; private set; } = "Ținută de zi, lejeră.";

    public void UpdateOutfit(
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
    }

    public OutfitDraftMemento Save()
    {
        return new OutfitDraftMemento(
            Top,
            Bottom,
            Shoes,
            Accessory,
            ColorPalette,
            Notes);
    }

    public void Restore(OutfitDraftMemento snapshot)
    {
        Top = snapshot.Top;
        Bottom = snapshot.Bottom;
        Shoes = snapshot.Shoes;
        Accessory = snapshot.Accessory;
        ColorPalette = snapshot.ColorPalette;
        Notes = snapshot.Notes;
    }
}