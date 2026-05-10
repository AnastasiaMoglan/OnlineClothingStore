using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Creational.Builder;

public sealed class CustomClothingProductBuilder : IClothingProductBuilder
{
    private string _name = string.Empty;
    private decimal _price;
    private string _size = "M";
    private string _color = "Black";
    private string _material = "Cotton";
    private bool _hasCustomPrint;
    private string? _customPrintText;
    private bool _premiumPackaging;

    public IClothingProductBuilder Reset()
    {
        _name = string.Empty;
        _price = 0;
        _size = "M";
        _color = "Black";
        _material = "Cotton";
        _hasCustomPrint = false;
        _customPrintText = null;
        _premiumPackaging = false;
        return this;
    }

    public IClothingProductBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    public IClothingProductBuilder SetPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public IClothingProductBuilder SetSize(string size)
    {
        _size = size;
        return this;
    }

    public IClothingProductBuilder SetColor(string color)
    {
        _color = color;
        return this;
    }

    public IClothingProductBuilder SetMaterial(string material)
    {
        _material = material;
        return this;
    }

    public IClothingProductBuilder AddCustomPrint(string text)
    {
        _hasCustomPrint = true;
        _customPrintText = text;
        return this;
    }

    public IClothingProductBuilder EnablePremiumPackaging()
    {
        _premiumPackaging = true;
        return this;
    }

    public ClothingProduct Build()
    {
        if (string.IsNullOrWhiteSpace(_name))
            throw new InvalidOperationException("Product name is required.");

        if (_price <= 0)
            throw new InvalidOperationException("Price must be greater than 0.");

        var product = new ClothingProduct(
            _name,
            _price,
            _size,
            _color,
            _material,
            _hasCustomPrint,
            _customPrintText,
            _premiumPackaging
        );

        Reset();
        return product;
    }
}