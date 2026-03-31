using OnlineClothingStore.Creational.Singleton;
using Xunit;

namespace OnlineClothingStore.Tests;

public sealed class SingletonTests
{
    [Fact]
    public void Instance_Should_Return_Same_Object()
    {
        var config1 = StoreConfiguration.Instance;
        var config2 = StoreConfiguration.Instance;

        Assert.True(ReferenceEquals(config1, config2));
    }

    [Fact]
    public void Configure_Should_Update_Same_Shared_Instance()
    {
        var config1 = StoreConfiguration.Instance;
        var config2 = StoreConfiguration.Instance;

        config1.Configure("TMPPP Clothing Store", 0.19m, "MDL");

        Assert.Equal("TMPPP Clothing Store", config2.StoreName);
        Assert.Equal(0.19m, config2.TaxRate);
        Assert.Equal("MDL", config2.Currency);
    }
}