namespace OnlineClothingStore.App.Behavioral.Visitor;

public class PromoCode : IPromotionElement
{
    public string Name { get; }
    public string Code { get; }
    public decimal DiscountPercent { get; }

    public PromoCode(string name, string code, decimal discountPercent)
    {
        Name = name;
        Code = code;
        DiscountPercent = discountPercent;
    }

    public void Accept(IPromotionVisitor visitor)
    {
        visitor.Visit(this);
    }
}