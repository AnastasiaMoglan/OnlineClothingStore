namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class CareInstructionCard : IInsert
{
    public string GetInsertDescription()
    {
        return "Card cu instructiuni de ingrijire pentru produs.";
    }
}