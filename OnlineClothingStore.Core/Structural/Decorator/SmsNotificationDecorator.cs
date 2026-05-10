namespace OnlineClothingStore.App.Structural.Decorator;

public sealed class SmsNotificationDecorator : OrderNotificationDecorator
{
    public SmsNotificationDecorator(IOrderNotification inner) : base(inner) { }

    public override NotificationResult Send(NotificationContext context)
    {
        var result = base.Send(context);
        result.AddChannel($"SMS:{context.PhoneNumber}");
        return result;
    }
}