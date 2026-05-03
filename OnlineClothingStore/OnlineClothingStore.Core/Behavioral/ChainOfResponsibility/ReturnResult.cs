namespace OnlineClothingStore.App.Behavioral.ChainOfResponsibility;

public class ReturnResult
{
    public bool Approved { get; }
    public string ProcessedBy { get; }
    public string Reason { get; }

    public ReturnResult(bool approved, string processedBy, string reason)
    {
        Approved = approved;
        ProcessedBy = processedBy;
        Reason = reason;
    }
}