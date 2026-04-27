namespace OnlineClothingStore.Domain;

public sealed class ClothingProduct : Product
{
    public string Size { get; }
    public string Color { get; set; }
    public string Material { get; }
    public bool HasCustomPrint { get; }
    public string? CustomPrintText { get; }
    public bool PremiumPackaging { get; }
    public List<string> Tags { get; }

    public ClothingProduct(
        string name,
        decimal price,
        string size,
        string color,
        string material,
        bool hasCustomPrint = false,
        string? customPrintText = null,
        bool premiumPackaging = false,
        List<string>? tags = null)
        : base(name, price)
    {
        if (string.IsNullOrWhiteSpace(size))
            throw new ArgumentException("Size cannot be empty.");

        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Color cannot be empty.");

        if (string.IsNullOrWhiteSpace(material))
            throw new ArgumentException("Material cannot be empty.");

        Size = size;
        Color = color;
        Material = material;
        HasCustomPrint = hasCustomPrint;
        CustomPrintText = customPrintText;
        PremiumPackaging = premiumPackaging;
        Tags = tags ?? new List<string>();
    }

    public override decimal GetFinalPrice()
    {
        decimal finalPrice = Price;

        if (HasCustomPrint)
            finalPrice += 100m;

        if (PremiumPackaging)
            finalPrice += 50m;

        return finalPrice;
    }

    public override string ToString()
    {
        var tagsText = Tags.Count > 0 ? string.Join(", ", Tags) : "No tags";

        return $"{Name} | Size: {Size} | Color: {Color} | Material: {Material} | " +
               $"CustomPrint: {(HasCustomPrint ? CustomPrintText : "No")} | " +
               $"PremiumPackaging: {PremiumPackaging} | Tags: {tagsText} | " +
               $"FinalPrice: {GetFinalPrice():0.00} MDL";
    }
}