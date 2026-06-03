namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class RecycledBox : IBox
{
    public string GetBoxDescription()
    {
        return "Cutie reciclata, simpla, potrivita pentru ambalare eco.";
    }
}