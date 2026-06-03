namespace OnlineClothingStore.Creational.Prototype;

public sealed class SizeGuide
{
    public string Category { get; set; }

    public string BrandName { get; set; }

    public string Region { get; set; }

    public List<string> Sizes { get; set; }

    public Dictionary<string, string> Measurements { get; set; }

    public string Notes { get; set; }

    public SizeGuide(
        string category,
        string brandName,
        string region,
        List<string> sizes,
        Dictionary<string, string> measurements,
        string notes)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category is required.");

        if (string.IsNullOrWhiteSpace(brandName))
            throw new ArgumentException("Brand name is required.");

        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region is required.");

        Category = category;
        BrandName = brandName;
        Region = region;
        Sizes = sizes;
        Measurements = measurements;
        Notes = notes;
    }

    public override string ToString()
    {
        string sizesText = Sizes.Count == 0
            ? "No sizes"
            : string.Join(", ", Sizes);

        string measurementsText = Measurements.Count == 0
            ? "No measurements"
            : string.Join("; ", Measurements.Select(item => $"{item.Key}: {item.Value}"));

        return $"Category: {Category} | Brand: {BrandName} | Region: {Region} | " +
               $"Sizes: {sizesText} | Measurements: {measurementsText} | Notes: {Notes}";
    }
}