using OnlineClothingStore.App.Structural.Composite;
using Xunit;

namespace OnlineClothingStore.Tests.Structural;

public class CompositeTests
{
    [Fact]
    public void SingleClothingItem_Should_Return_Its_Own_Price()
    {
        // Arrange
        var item = new SingleClothingItem("T-Shirt", 300m);

        // Act
        var price = item.GetPrice();

        // Assert
        Assert.Equal(300m, price);
    }

    [Fact]
    public void ClothingBundle_Should_Return_Sum_Of_Child_Items()
    {
        // Arrange
        var tshirt = new SingleClothingItem("T-Shirt", 300m);
        var jeans = new SingleClothingItem("Jeans", 700m);

        var bundle = new ClothingBundle("Summer Outfit");
        bundle.Add(tshirt);
        bundle.Add(jeans);

        // Act
        var total = bundle.GetPrice();

        // Assert
        Assert.Equal(1000m, total);
    }

    [Fact]
    public void Nested_ClothingBundle_Should_Return_Total_Price_Recursively()
    {
        // Arrange
        var tshirt = new SingleClothingItem("T-Shirt", 300m);
        var jeans = new SingleClothingItem("Jeans", 700m);
        var sneakers = new SingleClothingItem("Sneakers", 1200m);

        var summerOutfit = new ClothingBundle("Summer Outfit");
        summerOutfit.Add(tshirt);
        summerOutfit.Add(jeans);

        var premiumOutfit = new ClothingBundle("Premium Outfit");
        premiumOutfit.Add(summerOutfit);
        premiumOutfit.Add(sneakers);

        // Act
        var total = premiumOutfit.GetPrice();

        // Assert
        Assert.Equal(2200m, total);
    }
}