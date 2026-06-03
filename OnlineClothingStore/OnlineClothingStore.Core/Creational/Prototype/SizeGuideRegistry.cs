namespace OnlineClothingStore.Creational.Prototype;

public sealed class SizeGuideRegistry
{
    private readonly Dictionary<string, ISizeGuidePrototype> _templates = new();

    public void Register(string key, ISizeGuidePrototype prototype)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is required.", nameof(key));

        _templates[key] = prototype ?? throw new ArgumentNullException(nameof(prototype));
    }

    public SizeGuide GetClone(string key)
    {
        if (!_templates.TryGetValue(key, out ISizeGuidePrototype? prototype))
            throw new KeyNotFoundException($"Size guide template '{key}' was not found.");

        return prototype.Clone();
    }
}