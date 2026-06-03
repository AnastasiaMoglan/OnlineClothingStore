namespace OnlineClothingStore.Creational.FactoryMethod;

public interface IReturnProcessor
{
    string ProcessReturn(string orderNumber, string productName);
}