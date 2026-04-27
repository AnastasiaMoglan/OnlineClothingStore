namespace OnlineClothingStore.App.Structural.Decorator;

public interface IOrderNotification
{
    NotificationResult Send(NotificationContext context);
}