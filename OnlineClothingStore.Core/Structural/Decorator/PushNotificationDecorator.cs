namespace OnlineClothingStore.App.Structural.Decorator;

public sealed class PushNotificationDecorator : OrderNotificationDecorator
{
    public PushNotificationDecorator(IOrderNotification inner) : base(inner) { }

    public override NotificationResult Send(NotificationContext context)
    {
        var result = base.Send(context);
        result.AddChannel($"Push:{context.DeviceToken}");
        return result;
    }
}