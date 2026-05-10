using OnlineClothingStore.App.Structural.Bridge;

namespace OnlineClothingStore.Tests.Structural;

public class BridgeTests
{
    [Fact]
    public void FlashSalePromotion_Should_Work_With_Mobile_Renderer()
    {
        Promotion promotion = new FlashSalePromotion(
            "Flash Sale",
            20m,
            new DateTime(2026, 03, 30),
            new MobileAppPromotionRenderer());

        var result = promotion.Publish();

        Assert.Contains("[MobileApp]", result);
        Assert.Contains("Flash Sale", result);
    }
    [Fact]
    public void NewCollectionPromotion_Should_Work_With_Email_Renderer()
    {
        Promotion promotion = new NewCollectionPromotion(
            "New Collection",
            "Urban Wave",
            10,
            new EmailPromotionRenderer());

        var result = promotion.Publish();

        Assert.Contains("[Email]", result);
        Assert.Contains("Urban Wave", result);
    }
}