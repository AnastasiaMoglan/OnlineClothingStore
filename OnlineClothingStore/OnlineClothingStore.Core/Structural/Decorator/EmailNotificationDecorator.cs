namespace OnlineClothingStore.App.Structural.Decorator;

public sealed class EmailNotificationDecorator : OrderNotificationDecorator
{
    public EmailNotificationDecorator(IOrderNotification inner) : base(inner) { }

    public override NotificationResult Send(NotificationContext context)
    {
        var result = base.Send(context);
        result.AddChannel($"Email:{context.Email}");
        return result;
    }
}