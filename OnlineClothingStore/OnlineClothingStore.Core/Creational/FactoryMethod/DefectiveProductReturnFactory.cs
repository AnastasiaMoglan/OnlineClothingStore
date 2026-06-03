namespace OnlineClothingStore.Creational.FactoryMethod;

public sealed class DefectiveProductReturnFactory : ReturnProcessorFactory
{
    public override IReturnProcessor CreateProcessor()
    {
        return new DefectiveProductReturnProcessor();
    }
}