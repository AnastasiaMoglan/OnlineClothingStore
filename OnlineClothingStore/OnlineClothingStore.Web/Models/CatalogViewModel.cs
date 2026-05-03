using System.Collections.Generic;

namespace OnlineClothingStore.Web.Models;

public class CatalogViewModel
{
    public List<ProductViewModel> Products { get; set; } = new();

    public int TotalProducts { get; set; }

    public int SharedStyleObjects { get; set; }

    public int SavedObjects { get; set; }
}