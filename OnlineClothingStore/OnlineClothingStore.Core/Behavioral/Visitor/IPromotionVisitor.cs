namespace OnlineClothingStore.App.Behavioral.Visitor;

public interface IPromotionVisitor
{
    void Visit(HeroBanner banner);
    void Visit(PromoCode code);
    void Visit(CampaignCard card);
}