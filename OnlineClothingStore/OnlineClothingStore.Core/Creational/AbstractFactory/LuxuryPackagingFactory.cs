namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class LuxuryPackagingFactory : IOrderPackagingFactory
{
    public IBox CreateBox()
    {
        return new PremiumBox();
    }

    public ILabel CreateLabel()
    {
        return new GoldLabel();
    }

    public IInsert CreateInsert()
    {
        return new CareInstructionCard();
    }
}