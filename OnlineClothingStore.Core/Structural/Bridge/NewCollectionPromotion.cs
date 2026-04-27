namespace OnlineClothingStore.App.Structural.Bridge;

public sealed class NewCollectionPromotion : Promotion
{
    private readonly string _collectionName;
    private readonly int _itemsCount;

    public NewCollectionPromotion(string title, string collectionName, int itemsCount, IPromotionRenderer renderer)
        : base(title, renderer)
    {
        _collectionName = collectionName;
        _itemsCount = itemsCount;
    }

    public override string Publish()
    {
        var body = $"Colectia {_collectionName} a fost lansata cu {_itemsCount} produse noi.";
        return Renderer.Render(Title, body, "Vezi colectia");
    }
}