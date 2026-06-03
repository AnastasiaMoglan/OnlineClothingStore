namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class EcoLabel : ILabel
{
    public string GetLabelDescription()
    {
        return "Eticheta eco, tiparita pe hartie reciclata.";
    }
}