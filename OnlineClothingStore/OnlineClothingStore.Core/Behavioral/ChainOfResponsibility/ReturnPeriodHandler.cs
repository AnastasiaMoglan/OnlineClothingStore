namespace OnlineClothingStore.App.Behavioral.ChainOfResponsibility;

public class ReturnPeriodHandler : BaseReturnHandler
{
    private const int MaxReturnDays = 14;

    public override ReturnResult Handle(ReturnRequest request)
    {
        if (request.DaysAfterPurchase > MaxReturnDays)
        {
            return new ReturnResult(
                false,
                "ReturnPeriodHandler",
                $"Retur respins: termenul de {MaxReturnDays} zile a fost depășit."
            );
        }

        return PassToNext(request);
    }
}