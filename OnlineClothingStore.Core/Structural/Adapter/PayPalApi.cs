namespace OnlineClothingStore.App.Structural.Adapter;

// alt SDK extern - alta interfata
public class PayPalApi
{
    public bool SendPayment(decimal total, string currencyCode, string payerEmail)
    {
        return true;
    }

    public bool SendRefund(string paymentId, decimal total)
    {
        return true;
    }
}