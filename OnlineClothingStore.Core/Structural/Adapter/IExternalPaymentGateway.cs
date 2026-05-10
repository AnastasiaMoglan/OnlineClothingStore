namespace OnlineClothingStore.App.Structural.Adapter;

public interface IExternalPaymentGateway
{
    bool Pay(string customerEmail, decimal amount, string currency);
    bool Refund(string transactionId, decimal amount, string currency);
}