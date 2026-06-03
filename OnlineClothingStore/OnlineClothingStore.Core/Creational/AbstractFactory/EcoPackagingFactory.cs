namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class EcoPackagingFactory : IOrderPackagingFactory
{
    public IBox CreateBox()
    {
        return new RecycledBox();
    }

    public ILabel CreateLabel()
    {
        return new EcoLabel();
    }

    public IInsert CreateInsert()
    {
        return new ThankYouCard();
    }
}