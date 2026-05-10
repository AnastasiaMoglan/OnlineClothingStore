using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Payments;

public sealed class CardPayment : Payment
{
    public override void Pay(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be > 0");

        Console.WriteLine($"[PAYMENT] Paid {amount:0.00} MDL by card.");
    }
}