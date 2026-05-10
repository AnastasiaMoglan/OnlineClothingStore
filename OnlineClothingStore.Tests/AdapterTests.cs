using OnlineClothingStore.App.Structural.Adapter;
using Xunit;

namespace OnlineClothingStore.Tests.Structural;

public class AdapterTests
{
    [Fact]
    public void StripeAdapter_Should_Process_Payment_Successfully()
    {
        // Arrange
        IExternalPaymentGateway gateway = new StripePaymentAdapter(new StripeApi());

        // Act
        var result = gateway.Pay("client@store.com", 1500m, "MDL");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void PayPalAdapter_Should_Process_Payment_Successfully()
    {
        // Arrange
        IExternalPaymentGateway gateway = new PayPalPaymentAdapter(new PayPalApi());

        // Act
        var result = gateway.Pay("client@store.com", 800m, "MDL");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ExternalPaymentService_Should_Work_With_Any_Adapter()
    {
        // Arrange
        IExternalPaymentGateway gateway = new StripePaymentAdapter(new StripeApi());
        var service = new ExternalPaymentService(gateway);

        // Act
        var result = service.Checkout("client@store.com", 1000m);

        // Assert
        Assert.True(result);
    }
}