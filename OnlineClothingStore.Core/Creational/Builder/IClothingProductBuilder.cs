using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Creational.Builder;

public interface IClothingProductBuilder
{
    IClothingProductBuilder Reset();
    IClothingProductBuilder SetName(string name);
    IClothingProductBuilder SetPrice(decimal price);
    IClothingProductBuilder SetSize(string size);
    IClothingProductBuilder SetColor(string color);
    IClothingProductBuilder SetMaterial(string material);
    IClothingProductBuilder AddCustomPrint(string text);
    IClothingProductBuilder EnablePremiumPackaging();
    ClothingProduct Build();
}