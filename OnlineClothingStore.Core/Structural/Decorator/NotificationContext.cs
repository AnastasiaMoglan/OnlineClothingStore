namespace OnlineClothingStore.App.Structural.Decorator;

public sealed record NotificationContext(
    string CustomerName,
    string Email,
    string PhoneNumber,
    string DeviceToken,
    string Message);