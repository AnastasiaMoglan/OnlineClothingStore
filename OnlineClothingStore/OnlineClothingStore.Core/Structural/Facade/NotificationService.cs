namespace OnlineClothingStore.App.Structural.Facade;

public class NotificationService
{
    public void SendConfirmation(string email)
    {
        Console.WriteLine($"Confirmation email sent to {email}");
    }
}