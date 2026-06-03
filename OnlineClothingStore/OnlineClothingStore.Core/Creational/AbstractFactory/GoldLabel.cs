namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class GoldLabel : ILabel
{
    public string GetLabelDescription()
    {
        return "Eticheta aurie premium cu aspect elegant.";
    }
}