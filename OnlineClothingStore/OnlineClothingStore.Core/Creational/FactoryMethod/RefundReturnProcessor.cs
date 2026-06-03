namespace OnlineClothingStore.Creational.FactoryMethod;

public sealed class RefundReturnProcessor : IReturnProcessor
{
    public string ProcessReturn(string orderNumber, string productName)
    {
        return $"Retur pentru rambursare: comanda {orderNumber}, produsul {productName}. " +
               "S-a inițiat verificarea produsului și rambursarea banilor.";
    }
}