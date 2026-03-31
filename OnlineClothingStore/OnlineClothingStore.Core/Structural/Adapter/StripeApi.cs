namespace OnlineClothingStore.App.Structural.Adapter;

// simulare SDK extern - nu il modificam
public class StripeApi
{
    public string CreateCharge(string email, long amountInCents, string currency)
    {
        return $"stripe_{Guid.NewGuid():N}";
    }

    public bool CreateRefund(string chargeId, long amountInCents)
    {
        return true;
    }
}