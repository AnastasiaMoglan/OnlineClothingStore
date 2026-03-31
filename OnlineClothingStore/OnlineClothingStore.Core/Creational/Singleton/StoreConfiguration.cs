namespace OnlineClothingStore.Creational.Singleton;

public sealed class StoreConfiguration
{
    private static readonly Lazy<StoreConfiguration> _instance =
        new Lazy<StoreConfiguration>(() => new StoreConfiguration());

    public static StoreConfiguration Instance => _instance.Value;

    public string StoreName { get; private set; }
    public decimal TaxRate { get; private set; }
    public string Currency { get; private set; }

    private StoreConfiguration()
    {
        StoreName = "Online Clothing Store";
        TaxRate = 0.19m;
        Currency = "MDL";
    }

    public void Configure(string storeName, decimal taxRate, string currency)
    {
        StoreName = storeName;
        TaxRate = taxRate;
        Currency = currency;
    }
}