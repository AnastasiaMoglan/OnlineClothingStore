using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Creational.Prototype;

public sealed class ProductTemplateRegistry
{
    private readonly Dictionary<string, IPrototype<ClothingProduct>> _templates = new();

    public void Register(string key, IPrototype<ClothingProduct> prototype)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty.", nameof(key));

        _templates[key] = prototype;
    }

    public ClothingProduct GetShallowClone(string key)
    {
        if (!_templates.TryGetValue(key, out var prototype))
            throw new KeyNotFoundException($"Prototype '{key}' was not found.");

        return prototype.ShallowClone();
    }

    public ClothingProduct GetDeepClone(string key)
    {
        if (!_templates.TryGetValue(key, out var prototype))
            throw new KeyNotFoundException($"Prototype '{key}' was not found.");

        return prototype.DeepClone();
    }
}