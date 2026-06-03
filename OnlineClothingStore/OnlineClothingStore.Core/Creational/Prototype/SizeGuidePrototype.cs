namespace OnlineClothingStore.Creational.Prototype;

public sealed class SizeGuidePrototype : ISizeGuidePrototype
{
    private readonly SizeGuide _sizeGuide;

    public SizeGuidePrototype(SizeGuide sizeGuide)
    {
        _sizeGuide = sizeGuide ?? throw new ArgumentNullException(nameof(sizeGuide));
    }

    public SizeGuide Clone()
    {
        return new SizeGuide(
            _sizeGuide.Category,
            _sizeGuide.BrandName,
            _sizeGuide.Region,
            new List<string>(_sizeGuide.Sizes),
            new Dictionary<string, string>(_sizeGuide.Measurements),
            _sizeGuide.Notes
        );
    }
}