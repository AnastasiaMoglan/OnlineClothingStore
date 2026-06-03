namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class PremiumBox : IBox
{
    public string GetBoxDescription()
    {
        return "Cutie premium rigida, potrivita pentru comenzi cadou.";
    }
}