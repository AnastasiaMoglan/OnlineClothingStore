namespace OnlineClothingStore.App.Structural.Decorator;

public abstract class OrderNotificationDecorator : IOrderNotification
{
    protected readonly IOrderNotification Inner;

    protected OrderNotificationDecorator(IOrderNotification inner)
    {
        Inner = inner;
    }

    public virtual NotificationResult Send(NotificationContext context)
    {
        return Inner.Send(context);
    }
}