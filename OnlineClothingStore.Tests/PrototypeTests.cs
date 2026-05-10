using OnlineClothingStore.Creational.Prototype;
using OnlineClothingStore.Domain;
using Xunit;

namespace OnlineClothingStore.Tests;

public sealed class PrototypeTests
{
    [Fact]
    public void ShallowClone_Should_Share_Tags_List()
    {
        var original = new ClothingProduct(
            "Prototype Hoodie",
            900m,
            "L",
            "Black",
            "Cotton",
            false,
            null,
            true,
            new List<string> { "winter", "premium" }
        );

        var prototype = new ClothingProductPrototype(original);
        var clone = prototype.ShallowClone();

        clone.Tags.Add("shared-tag");

        Assert.Contains("shared-tag", original.Tags);
        Assert.Contains("shared-tag", clone.Tags);
    }

    [Fact]
    public void DeepClone_Should_Create_Independent_Tags_List()
    {
        var original = new ClothingProduct(
            "Prototype Hoodie",
            900m,
            "L",
            "Black",
            "Cotton",
            false,
            null,
            true,
            new List<string> { "winter", "premium" }
        );

        var prototype = new ClothingProductPrototype(original);
        var clone = prototype.DeepClone();

        clone.Tags.Add("independent-tag");

        Assert.DoesNotContain("independent-tag", original.Tags);
        Assert.Contains("independent-tag", clone.Tags);
    }

    [Fact]
    public void ShallowClone_Should_Copy_Base_Values()
    {
        var original = new ClothingProduct(
            "Oversize Hoodie",
            1000m,
            "XL",
            "Gray",
            "Cotton",
            true,
            "Street",
            true,
            new List<string> { "oversize" }
        );

        var prototype = new ClothingProductPrototype(original);
        var clone = prototype.ShallowClone();

        Assert.Equal(original.Name, clone.Name);
        Assert.Equal(original.Price, clone.Price);
        Assert.Equal(original.Size, clone.Size);
        Assert.Equal(original.Color, clone.Color);
        Assert.Equal(original.Material, clone.Material);
        Assert.Equal(original.HasCustomPrint, clone.HasCustomPrint);
        Assert.Equal(original.CustomPrintText, clone.CustomPrintText);
        Assert.Equal(original.PremiumPackaging, clone.PremiumPackaging);
    }

    [Fact]
    public void DeepClone_Should_Copy_Base_Values()
    {
        var original = new ClothingProduct(
            "Oversize Hoodie",
            1000m,
            "XL",
            "Gray",
            "Cotton",
            true,
            "Street",
            true,
            new List<string> { "oversize" }
        );

        var prototype = new ClothingProductPrototype(original);
        var clone = prototype.DeepClone();

        Assert.Equal(original.Name, clone.Name);
        Assert.Equal(original.Price, clone.Price);
        Assert.Equal(original.Size, clone.Size);
        Assert.Equal(original.Color, clone.Color);
        Assert.Equal(original.Material, clone.Material);
        Assert.Equal(original.HasCustomPrint, clone.HasCustomPrint);
        Assert.Equal(original.CustomPrintText, clone.CustomPrintText);
        Assert.Equal(original.PremiumPackaging, clone.PremiumPackaging);
    }
}