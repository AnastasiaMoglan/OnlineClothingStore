namespace OnlineClothingStore.App.Structural.Flyweight;

public sealed record ProductCardContext(
    string ProductName,
    decimal Price,
    string Size,
    string CollectionName);