namespace OnlineClothingStore.App.Behavioral.ChainOfResponsibility;

public abstract class BaseReturnHandler : IReturnHandler
{
    private IReturnHandler? _next;

    public IReturnHandler SetNext(IReturnHandler next)
    {
        _next = next;
        return next;
    }

    public abstract ReturnResult Handle(ReturnRequest request);

    protected ReturnResult PassToNext(ReturnRequest request)
    {
        if (_next != null)
        {
            return _next.Handle(request);
        }

        return new ReturnResult(
            false,
            "System",
            "Cererea de retur nu a fost procesată de niciun handler."
        );
    }
}