namespace OnlineClothingStore.App.Behavioral.ChainOfResponsibility;

public class FinalReturnApprovalHandler : BaseReturnHandler
{
    public override ReturnResult Handle(ReturnRequest request)
    {
        return new ReturnResult(
            true,
            "FinalReturnApprovalHandler",
            $"Retur aprobat pentru produsul: {request.ProductName}."
        );
    }
}