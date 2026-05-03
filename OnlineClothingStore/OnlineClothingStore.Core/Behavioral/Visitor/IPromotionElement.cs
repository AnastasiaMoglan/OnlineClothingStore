namespace OnlineClothingStore.App.Behavioral.Visitor;

public interface IPromotionElement
{
    string Name { get; }
    void Accept(IPromotionVisitor visitor);
}