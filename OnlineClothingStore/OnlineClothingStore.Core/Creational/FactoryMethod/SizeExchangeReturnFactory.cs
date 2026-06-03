namespace OnlineClothingStore.Creational.FactoryMethod;

public sealed class SizeExchangeReturnFactory : ReturnProcessorFactory
{
    public override IReturnProcessor CreateProcessor()
    {
        return new SizeExchangeReturnProcessor();
    }
}