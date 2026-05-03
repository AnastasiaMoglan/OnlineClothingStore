namespace OnlineClothingStore.Web.Models;

public class ProductViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Size { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string BadgeColor { get; set; } = string.Empty;

    public string TextColor { get; set; } = string.Empty;

    public string FontFamily { get; set; } = string.Empty;

    public string BackgroundColor { get; set; } = string.Empty;

    public int SharedStyleId { get; set; }
}