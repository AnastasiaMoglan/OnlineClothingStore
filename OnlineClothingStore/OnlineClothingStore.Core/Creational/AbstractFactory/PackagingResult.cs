namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class PackagingResult
{
    public string FactoryName { get; }

    public string BoxDescription { get; }

    public string LabelDescription { get; }

    public string InsertDescription { get; }

    public PackagingResult(
        string factoryName,
        string boxDescription,
        string labelDescription,
        string insertDescription)
    {
        FactoryName = factoryName;
        BoxDescription = boxDescription;
        LabelDescription = labelDescription;
        InsertDescription = insertDescription;
    }
}