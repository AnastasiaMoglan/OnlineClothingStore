using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.App.Structural.Flyweight;
using OnlineClothingStore.Web.Models;

namespace OnlineClothingStore.Web.Controllers;

public class StoreController : Controller
{
    private static readonly List<StoreProduct> Products = new()
    {
        new StoreProduct(1, "Tricou Oversize Blue", "T-Shirts", 349, "M", "Albastru"),
        new StoreProduct(2, "Tricou Basic White", "T-Shirts", 279, "S", "Alb"),
        new StoreProduct(3, "Jeans Slim Fit", "Jeans", 699, "L", "Denim"),
        new StoreProduct(4, "Jeans Regular Dark", "Jeans", 749, "M", "Albastru inchis"),
        new StoreProduct(5, "Geaca Urban Denim", "Jackets", 1199, "M", "Albastru"),
        new StoreProduct(6, "Hanorac Minimal", "Hoodies", 599, "XL", "Gri"),
        new StoreProduct(7, "Hanorac Street Purple", "Hoodies", 649, "L", "Mov"),
        new StoreProduct(8, "Rochie Eleganta", "Dresses", 899, "S", "Bleumarin"),
        new StoreProduct(9, "Sneakers White", "Shoes", 999, "42", "Alb")
    };

    private static readonly List<CartLine> ShoppingCart = new();

    private static readonly List<StoreProduct> WishlistItems = new();

    private static readonly ProductCardStyleFactory StyleFactory = new();

    public IActionResult Index()
    {
        ViewBag.FeaturedProducts = Products.Take(3).ToList();
        ViewBag.Categories = Products.Select(p => p.Category).Distinct().ToList();

        AddLayoutCounters();

        return View();
    }

    public IActionResult Catalog(string? category)
    {
        List<StoreProduct> filteredProducts = string.IsNullOrWhiteSpace(category)
            ? Products
            : Products.Where(p => p.Category == category).ToList();

        List<ProductViewModel> productCards = BuildProductCards(filteredProducts);

        ViewBag.ProductCards = productCards;
        ViewBag.Categories = Products.Select(p => p.Category).Distinct().ToList();
        ViewBag.SelectedCategory = category ?? "Toate";

        ViewBag.TotalCards = productCards.Count;
        ViewBag.SharedStyles = StyleFactory.CreatedStylesCount;
        ViewBag.SavedObjects = productCards.Count - StyleFactory.CreatedStylesCount;

        ViewBag.FlyweightInfo =
            $"In catalog sunt {productCards.Count} produse afisate, dar stilurile cardurilor sunt partajate pe categorii. " +
            $"Factory-ul a creat doar {StyleFactory.CreatedStylesCount} obiecte de stil.";

        AddLayoutCounters();

        return View();
    }

    public IActionResult Product(int id)
    {
        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return RedirectToAction(nameof(Catalog));
        }

        ProductViewModel productCard = BuildProductCards(new List<StoreProduct> { product }).First();

        ViewBag.Product = productCard;

        AddLayoutCounters();

        return View();
    }

    public IActionResult AdminLogin()
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role == "Manager")
        {
            return RedirectToAction(nameof(Admin));
        }

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult Login(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            role = "Customer";
        }

        HttpContext.Session.SetString("role", role);

        if (role == "Manager")
        {
            return RedirectToAction(nameof(Admin));
        }

        return RedirectToAction(nameof(AdminLogin));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Admin()
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role != "Manager")
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        ViewBag.Role = role;
        ViewBag.CategoryStyles = GetCategoryStyles();
        ViewBag.FlyweightAdminInfo =
            "Adminul modifica un singur stil partajat pentru o categorie. Toate produsele din acea categorie vor folosi noul stil.";

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult UpdateCategoryStyle(CategoryStyleEditViewModel model)
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role != "Manager")
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        StyleFactory.UpdateStyle(
            model.Category,
            model.BadgeColor,
            model.TextColor,
            model.FontFamily,
            model.BackgroundColor);

        return RedirectToAction(nameof(Catalog));
    }

    public IActionResult AddToCart(int id)
    {
        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product != null)
        {
            CartLine? existing = ShoppingCart.FirstOrDefault(c => c.Product.Id == id);

            if (existing == null)
            {
                ShoppingCart.Add(new CartLine(product, 1));
            }
            else
            {
                existing.Quantity++;
            }
        }

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult Cart()
    {
        ViewBag.Cart = ShoppingCart;
        ViewBag.Total = ShoppingCart.Sum(c => c.Product.Price * c.Quantity);

        AddLayoutCounters();

        return View();
    }

    public IActionResult IncreaseQuantity(int id)
    {
        CartLine? line = ShoppingCart.FirstOrDefault(c => c.Product.Id == id);

        if (line != null)
        {
            line.Quantity++;
        }

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult DecreaseQuantity(int id)
    {
        CartLine? line = ShoppingCart.FirstOrDefault(c => c.Product.Id == id);

        if (line != null)
        {
            line.Quantity--;

            if (line.Quantity <= 0)
            {
                ShoppingCart.Remove(line);
            }
        }

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult RemoveFromCart(int id)
    {
        CartLine? line = ShoppingCart.FirstOrDefault(c => c.Product.Id == id);

        if (line != null)
        {
            ShoppingCart.Remove(line);
        }

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult ClearCart()
    {
        ShoppingCart.Clear();

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult Wishlist()
    {
        ViewBag.Wishlist = WishlistItems;

        AddLayoutCounters();

        return View();
    }

    public IActionResult AddToWishlist(int id)
    {
        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product != null && WishlistItems.All(p => p.Id != id))
        {
            WishlistItems.Add(product);
        }

        return RedirectToAction(nameof(Wishlist));
    }

    public IActionResult RemoveFromWishlist(int id)
    {
        StoreProduct? product = WishlistItems.FirstOrDefault(p => p.Id == id);

        if (product != null)
        {
            WishlistItems.Remove(product);
        }

        return RedirectToAction(nameof(Wishlist));
    }

    public IActionResult Checkout()
    {
        ViewBag.Cart = ShoppingCart;
        ViewBag.Total = ShoppingCart.Sum(c => c.Product.Price * c.Quantity);

        AddLayoutCounters();

        return View();
    }

    private static List<ProductViewModel> BuildProductCards(List<StoreProduct> products)
    {
        List<ProductViewModel> cards = new();

        foreach (StoreProduct product in products)
        {
            ProductCardStyle style = StyleFactory.GetStyle(product.Category);

            cards.Add(new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Size = product.Size,
                Color = product.Color,

                BadgeColor = style.BadgeColor,
                TextColor = style.TextColor,
                FontFamily = style.FontFamily,
                BackgroundColor = style.BackgroundColor,

                SharedStyleId = RuntimeHelpers.GetHashCode(style)
            });
        }

        return cards;
    }

    private static List<CategoryStyleEditViewModel> GetCategoryStyles()
    {
        List<CategoryStyleEditViewModel> styles = new();

        foreach (string category in Products.Select(p => p.Category).Distinct())
        {
            ProductCardStyle style = StyleFactory.GetStyle(category);

            styles.Add(new CategoryStyleEditViewModel
            {
                Category = style.Category,
                BadgeColor = style.BadgeColor,
                TextColor = style.TextColor,
                FontFamily = style.FontFamily,
                BackgroundColor = style.BackgroundColor
            });
        }

        return styles;
    }

    private void AddLayoutCounters()
    {
        ViewBag.CartCount = ShoppingCart.Sum(c => c.Quantity);
        ViewBag.WishlistCount = WishlistItems.Count;
    }

    public class StoreProduct
    {
        public int Id { get; }

        public string Name { get; }

        public string Category { get; }

        public decimal Price { get; }

        public string Size { get; }

        public string Color { get; }

        public StoreProduct(
            int id,
            string name,
            string category,
            decimal price,
            string size,
            string color)
        {
            Id = id;
            Name = name;
            Category = category;
            Price = price;
            Size = size;
            Color = color;
        }
    }

    public class CartLine
    {
        public StoreProduct Product { get; }

        public int Quantity { get; set; }

        public CartLine(StoreProduct product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}