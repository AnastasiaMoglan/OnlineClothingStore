namespace OnlineClothingStore.Creational.FactoryMethod;

public sealed class DefectiveProductReturnProcessor : IReturnProcessor
{
    public string ProcessReturn(string orderNumber, string productName)
    {
        return $"Retur pentru produs defect: comanda {orderNumber}, produsul {productName}. " +
               "Cererea a fost trimisă către verificarea calității.";
    }
}