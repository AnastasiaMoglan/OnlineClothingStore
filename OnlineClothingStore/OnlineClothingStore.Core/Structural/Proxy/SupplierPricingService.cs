namespace OnlineClothingStore.App.Structural.Proxy;

public sealed class SupplierPricingService : ISupplierPricingService
{
    private readonly Dictionary<string, decimal> _supplierCosts;

    public int CallCount { get; private set; }

    public SupplierPricingService()
    {
        _supplierCosts = new Dictionary<string, decimal>
        {
            { "TSHIRT-001", 180m },
            { "JEANS-002", 450m },
            { "JACKET-003", 700m }
        };
    }

    public decimal GetSupplierCost(string sku)
    {
        CallCount++;

        if (!_supplierCosts.ContainsKey(sku))
            throw new KeyNotFoundException($"SKU necunoscut: {sku}");

        return _supplierCosts[sku];
    }
}