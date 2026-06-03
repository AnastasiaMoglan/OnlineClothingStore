namespace OnlineClothingStore.Creational.FactoryMethod;

public sealed class RefundReturnFactory : ReturnProcessorFactory
{
    public override IReturnProcessor CreateProcessor()
    {
        return new RefundReturnProcessor();
    }
}