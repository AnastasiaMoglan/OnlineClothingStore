namespace OnlineClothingStore.Creational.FactoryMethod;

public sealed class SizeExchangeReturnProcessor : IReturnProcessor
{
    public string ProcessReturn(string orderNumber, string productName)
    {
        return $"Retur pentru schimb de mărime: comanda {orderNumber}, produsul {productName}. " +
               "S-a verificat disponibilitatea altei mărimi în stoc.";
    }
}