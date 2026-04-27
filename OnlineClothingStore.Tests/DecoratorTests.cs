using OnlineClothingStore.App.Structural.Decorator;

namespace OnlineClothingStore.Tests.Structural;

public class DecoratorTests
{
    [Fact]
    public void BasicNotification_Should_Use_Only_InApp_Channel()
    {
        IOrderNotification notification = new BasicOrderNotification();

        var result = notification.Send(new NotificationContext("Ana", "ana@test.com", "123", "token", "Test"));

        Assert.Single(result.Channels);
        Assert.Contains("InApp", result.Channels);
    }

    [Fact]
    public void Decorators_Should_Add_Channels_Dynamically()
    {
        IOrderNotification notification =
            new PushNotificationDecorator(
                new SmsNotificationDecorator(
                    new EmailNotificationDecorator(
                        new BasicOrderNotification())));
        var result = notification.Send(new NotificationContext("Ana", "ana@test.com", "123", "token", "Test"));

        Assert.Equal(4, result.Channels.Count);
        Assert.Contains(result.Channels, x => x.StartsWith("Email:"));
        Assert.Contains(result.Channels, x => x.StartsWith("SMS:"));
        Assert.Contains(result.Channels, x => x.StartsWith("Push:"));
    }
}
                        