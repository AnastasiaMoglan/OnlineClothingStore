namespace OnlineClothingStore.Creational.FactoryMethod;

public abstract class ReturnProcessorFactory
{
    public abstract IReturnProcessor CreateProcessor();

    public string HandleReturn(string orderNumber, string productName)
    {
        var processor = CreateProcessor();
        return processor.ProcessReturn(orderNumber, productName);
    }
}