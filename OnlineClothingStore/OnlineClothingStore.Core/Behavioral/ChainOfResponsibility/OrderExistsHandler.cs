namespace OnlineClothingStore.App.Behavioral.ChainOfResponsibility;

public class OrderExistsHandler : BaseReturnHandler
{
    public override ReturnResult Handle(ReturnRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderId))
        {
            return new ReturnResult(
                false,
                "OrderExistsHandler",
                "Retur respins: comanda nu există."
            );
        }

        return PassToNext(request);
    }
}