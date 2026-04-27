namespace OnlineClothingStore.App.Structural.Facade;

public class PaymentService
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Payment processed: {amount} MDL");
        return true;
    }
}