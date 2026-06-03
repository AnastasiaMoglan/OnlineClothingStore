namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class ThankYouCard : IInsert
{
    public string GetInsertDescription()
    {
        return "Card simplu de multumire inclus in colet.";
    }
}