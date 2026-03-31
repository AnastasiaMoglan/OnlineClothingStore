using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Creational.Builder;

public sealed class ClothingProductDirector
{
    private readonly IClothingProductBuilder _builder;

    public ClothingProductDirector(IClothingProductBuilder builder)
    {
        _builder = builder;
    }

    public ClothingProduct BuildBasicTShirt()
    {
        return _builder
            .Reset()
            .SetName("Basic T-Shirt")
            .SetPrice(300m)
            .SetSize("M")
            .SetColor("White")
            .SetMaterial("Cotton")
            .Build();
    }

    public ClothingProduct BuildPremiumHoodie()
    {
        return _builder
            .Reset()
            .SetName("Premium Hoodie")
            .SetPrice(800m)
            .SetSize("L")
            .SetColor("Black")
            .SetMaterial("Wool")
            .EnablePremiumPackaging()
            .Build();
    }
}