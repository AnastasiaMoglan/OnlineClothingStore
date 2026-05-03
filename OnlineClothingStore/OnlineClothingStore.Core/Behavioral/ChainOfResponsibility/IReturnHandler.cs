namespace OnlineClothingStore.App.Behavioral.ChainOfResponsibility;

public interface IReturnHandler
{
    IReturnHandler SetNext(IReturnHandler next);
    ReturnResult Handle(ReturnRequest request);
}