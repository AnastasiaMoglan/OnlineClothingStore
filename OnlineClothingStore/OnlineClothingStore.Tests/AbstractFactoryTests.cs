using OnlineClothingStore.Creational.AbstractFactory;
using Xunit;

namespace OnlineClothingStore.Tests;

public class AbstractFactoryTests
{
    [Fact]
    public void VipKitFactory_Should_Apply_Discount()
    {
        IStoreKitFactory factory = new VipKitFactory();

        var discount = factory.CreateDiscount();
        var result = discount.Apply(1000m);

        Assert.True(result < 1000m);
    }

    [Fact]
    public void RegularKitFactory_Should_Not_Apply_Discount()
    {
        IStoreKitFactory factory = new RegularKitFactory();

        var discount = factory.CreateDiscount();
        var result = discount.Apply(1000m);

        Assert.Equal(1000m, result);
    }
}