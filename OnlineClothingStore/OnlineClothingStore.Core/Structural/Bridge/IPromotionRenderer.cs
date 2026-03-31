namespace OnlineClothingStore.App.Structural.Bridge;

public interface IPromotionRenderer
{
    string Render(string title, string body, string cta);
}