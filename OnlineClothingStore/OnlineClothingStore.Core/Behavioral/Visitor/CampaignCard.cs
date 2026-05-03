namespace OnlineClothingStore.App.Behavioral.Visitor;

public class CampaignCard : IPromotionElement
{
    public string Name { get; }
    public string CampaignName { get; }
    public int Priority { get; }

    public CampaignCard(string name, string campaignName, int priority)
    {
        Name = name;
        CampaignName = campaignName;
        Priority = priority;
    }

    public void Accept(IPromotionVisitor visitor)
    {
        visitor.Visit(this);
    }
}