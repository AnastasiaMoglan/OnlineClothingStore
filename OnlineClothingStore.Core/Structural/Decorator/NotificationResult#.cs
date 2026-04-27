namespace OnlineClothingStore.App.Structural.Decorator;

public sealed class NotificationResult
{
    private readonly List<string> _channels = new();

    public IReadOnlyList<string> Channels => _channels.AsReadOnly();

    public void AddChannel(string channel)
    {
        _channels.Add(channel);
    }
}