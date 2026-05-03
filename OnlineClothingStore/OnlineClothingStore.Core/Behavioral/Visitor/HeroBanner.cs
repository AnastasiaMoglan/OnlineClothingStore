namespace OnlineClothingStore.App.Behavioral.Visitor;

public class HeroBanner : IPromotionElement
{
    public string Name { get; }
    public string ImageUrl { get; }
    public string Title { get; }

    public HeroBanner(string name, string imageUrl, string title)
    {
        Name = name;
        ImageUrl = imageUrl;
        Title = title;
    }

    public void Accept(IPromotionVisitor visitor)
    {
        visitor.Visit(this);
    }
}