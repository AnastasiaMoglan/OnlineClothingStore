namespace OnlineClothingStore.App.Structural.Bridge;

public abstract class Promotion
{
    protected readonly IPromotionRenderer Renderer;
    public string Title { get; }

    protected Promotion(string title, IPromotionRenderer renderer)
    {
        Title = title;
        Renderer = renderer;
    }

    public abstract string Publish();
}