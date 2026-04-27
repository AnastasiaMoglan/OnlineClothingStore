using OnlineClothingStore.App.Structural.Flyweight;

namespace OnlineClothingStore.Tests.Structural;

public class FlyweightTests
{
    [Fact]
    public void Factory_Should_Reuse_The_Same_Style_Instance()
    {
        var factory = new ProductCardStyleFactory();

        var style1 = factory.GetStyle("Tops", "Blue", "White", "Montserrat");
        var style2 = factory.GetStyle("Tops", "Blue", "White", "Montserrat");

        Assert.Same(style1, style2);
        Assert.Equal(1, factory.CreatedStylesCount);
    }

    [Fact]
    public void CatalogPage_Should_Report_Saved_Objects()
    {
        var factory = new ProductCardStyleFactory();
        var page = new CatalogPage(factory);
        page.AddProduct(new ProductCardContext("A", 100m, "M", "C1"), "Tops", "Blue", "White", "Montserrat");
        page.AddProduct(new ProductCardContext("B", 120m, "L", "C1"), "Tops", "Blue", "White", "Montserrat");
        page.AddProduct(new ProductCardContext("C", 130m, "S", "C2"), "Bottoms", "Black", "White", "Roboto");

        Assert.Equal(3, page.TotalCards);
        Assert.Equal(2, page.SharedStyles);
        Assert.Equal(1, page.SavedObjects);
    }
}