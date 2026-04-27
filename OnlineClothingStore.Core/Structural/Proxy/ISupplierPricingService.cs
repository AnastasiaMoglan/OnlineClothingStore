namespace OnlineClothingStore.App.Structural.Proxy;

public interface ISupplierPricingService
{
    decimal GetSupplierCost(string sku);
}