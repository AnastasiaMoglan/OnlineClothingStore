namespace OnlineClothingStore.App.Structural.Decorator;

public sealed class BasicOrderNotification : IOrderNotification
{
    public NotificationResult Send(NotificationContext context)
    {
        var result = new NotificationResult();
        result.AddChannel("InApp");
        return result;
    }
}