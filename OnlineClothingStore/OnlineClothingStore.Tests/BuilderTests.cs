using OnlineClothingStore.Creational.Builder;
using Xunit;

namespace OnlineClothingStore.Tests;

public sealed class BuilderTests
{
    [Fact]
    public void Build_Should_Create_Valid_ClothingProduct()
    {
        var builder = new CustomClothingProductBuilder();

        var product = builder
            .Reset()
            .SetName("Test Hoodie")
            .SetPrice(700m)
            .SetSize("L")
            .SetColor("Black")
            .SetMaterial("Cotton")
            .AddCustomPrint("TMPPP")
            .EnablePremiumPackaging()
            .Build();

        Assert.Equal("Test Hoodie", product.Name);
        Assert.Equal(700m, product.Price);
        Assert.Equal("L", product.Size);
        Assert.Equal("Black", product.Color);
        Assert.Equal("Cotton", product.Material);
        Assert.True(product.HasCustomPrint);
        Assert.Equal("TMPPP", product.CustomPrintText);
        Assert.True(product.PremiumPackaging);
        Assert.Equal(850m, product.GetFinalPrice());
    }

    [Fact]
    public void Build_Without_Name_Should_Throw_InvalidOperationException()
    {
        var builder = new CustomClothingProductBuilder();

        Assert.Throws<InvalidOperationException>(() =>
            builder
                .Reset()
                .SetPrice(500m)
                .SetSize("M")
                .SetColor("Blue")
                .SetMaterial("Cotton")
                .Build());
    }

    [Fact]
    public void Build_Without_Valid_Price_Should_Throw_InvalidOperationException()
    {
        var builder = new CustomClothingProductBuilder();

        Assert.Throws<InvalidOperationException>(() =>
            builder
                .Reset()
                .SetName("Invalid Product")
                .SetPrice(0m)
                .SetSize("M")
                .SetColor("Blue")
                .SetMaterial("Cotton")
                .Build());
    }

    [Fact]
    public void Director_Should_Build_Standard_Products()
    {
        var director = new ClothingProductDirector(new CustomClothingProductBuilder());

        var basic = director.BuildBasicTShirt();
        var premium = director.BuildPremiumHoodie();

        Assert.Equal("Basic T-Shirt", basic.Name);
        Assert.Equal(300m, basic.GetFinalPrice());

        Assert.Equal("Premium Hoodie", premium.Name);
        Assert.True(premium.PremiumPackaging);
        Assert.Equal(850m, premium.GetFinalPrice());
    }
}