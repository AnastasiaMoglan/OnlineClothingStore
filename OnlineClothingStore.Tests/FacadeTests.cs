using OnlineClothingStore.App.Structural.Facade;
using Xunit;

namespace OnlineClothingStore.Tests.Structural;

public class FacadeTests
{
    [Fact]
    public void StoreFacade_Should_Create_Order_With_Correct_Total()
    {
        var facade = new StoreFacade();
        var prices = new List<decimal> { 950m, 800m, 250m };

        var order = facade.PlaceOrder("alexei@store.com", prices);

        Assert.NotNull(order);
        Assert.Equal(2000m, order.Total);
    }
}