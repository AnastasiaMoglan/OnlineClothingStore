using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Creational.Prototype;

public sealed class ClothingProductPrototype : IPrototype<ClothingProduct>
{
    private readonly ClothingProduct _product;

    public ClothingProductPrototype(ClothingProduct product)
    {
        _product = product ?? throw new ArgumentNullException(nameof(product));
    }

    public ClothingProduct ShallowClone()
    {
        return new ClothingProduct(
            _product.Name,
            _product.Price,
            _product.Size,
            _product.Color,
            _product.Material,
            _product.HasCustomPrint,
            _product.CustomPrintText,
            _product.PremiumPackaging,
            _product.Tags // same reference -> shallow copy
        );
    }

    public ClothingProduct DeepClone()
    {
        return new ClothingProduct(
            _product.Name,
            _product.Price,
            _product.Size,
            _product.Color,
            _product.Material,
            _product.HasCustomPrint,
            _product.CustomPrintText,
            _product.PremiumPackaging,
            new List<string>(_product.Tags) // new list -> deep copy
        );
    }
}