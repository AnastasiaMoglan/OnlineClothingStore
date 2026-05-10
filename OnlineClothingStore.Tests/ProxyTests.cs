using OnlineClothingStore.App.Structural.Proxy;

namespace OnlineClothingStore.Tests.Structural;

public class ProxyTests
{
    [Fact]
    public void Proxy_Should_Allow_Admin_Access()
    {
        var realService = new SupplierPricingService();
        ISupplierPricingService proxy =
            new SupplierPricingProxy(realService, new StoreEmployee("admin", "Admin"));

        var cost = proxy.GetSupplierCost("TSHIRT-001");

        Assert.Equal(180m, cost);
        Assert.Equal(1, realService.CallCount);
    }

    [Fact]
    public void Proxy_Should_Block_Unauthorized_User()
    {
        var realService = new SupplierPricingService();
        ISupplierPricingService proxy =
            new SupplierPricingProxy(realService, new StoreEmployee("guest", "Customer"));

        Assert.Throws<UnauthorizedAccessException>(() => proxy.GetSupplierCost("TSHIRT-001"));
        Assert.Equal(0, realService.CallCount);
    }
}