using OnlineClothingStore.App.Factories;
using Xunit;

public class FactoryTests
{
    [Fact]
    public void ClothingFactory_Should_Create_Product_And_Discount()
    {
        IStoreFactory factory = new ClothingFactory();

        var product = factory.CreateProduct("Test", 100m);
        var discount = factory.CreateDiscount();

        Assert.NotNull(product);
        Assert.NotNull(discount);
    }
}