namespace OnlineClothingStore.App.Behavioral.ChainOfResponsibility;

public class ProductConditionHandler : BaseReturnHandler
{
    public override ReturnResult Handle(ReturnRequest request)
    {
        if (request.IsProductUsed)
        {
            return new ReturnResult(
                false,
                "ProductConditionHandler",
                "Retur respins: produsul a fost utilizat."
            );
        }

        if (!request.HasOriginalPackage)
        {
            return new ReturnResult(
                false,
                "ProductConditionHandler",
                "Retur respins: lipsește ambalajul original."
            );
        }

        return PassToNext(request);
    }
}