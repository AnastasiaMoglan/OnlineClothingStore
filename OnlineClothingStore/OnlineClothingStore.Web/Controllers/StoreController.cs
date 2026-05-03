using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.App.Structural.Flyweight;
using OnlineClothingStore.App.Structural.Proxy;
using OnlineClothingStore.App.Structural.Bridge;
using OnlineClothingStore.App.Structural.Decorator;

namespace OnlineClothingStore.Web.Controllers;

public class StoreController : Controller
{
    private static readonly List<StoreProduct> Products = new List<StoreProduct>
    {
        new StoreProduct(1, "Tricou Oversize Blue", "T-Shirts", 349, "M", "Albastru", "/img/tshirt-blue.jpg"),
        new StoreProduct(2, "Jeans Slim Fit", "Jeans", 699, "L", "Denim", "/img/jeans.jpg"),
        new StoreProduct(3, "Geaca Urban Denim", "Jackets", 1199, "M", "Albastru", "/img/jacket.jpg"),
        new StoreProduct(4, "Hanorac Minimal", "Hoodies", 599, "XL", "Gri", "/img/hoodie.jpg"),
        new StoreProduct(5, "Rochie Eleganta", "Dresses", 899, "S", "Bleumarin", "/img/dress.jpg"),
        new StoreProduct(6, "Sneakers White", "Shoes", 999, "42", "Alb", "/img/sneakers.jpg")
    };

    private static readonly List<CartLine> ShoppingCart = new List<CartLine>();

    public IActionResult Index(string promoChannel = "email")
    {
        IPromotionRenderer renderer = promoChannel switch
        {
            "mobile" => new MobileAppPromotionRenderer(),
            "display" => new StoreDisplayPromotionRenderer(),
            _ => new EmailPromotionRenderer()
        };

        Promotion promotion = new FlashSalePromotion(
            "Weekend Fashion Sale",
            30,
            DateTime.Now.AddDays(2),
            renderer
        );

        ViewBag.Promotion = promotion.Publish();
        ViewBag.FeaturedProducts = Products.Take(3).ToList();
        ViewBag.PatternInfo = "Bridge: aceeasi promotie este afisata prin canale diferite.";
        ViewBag.CartCount = ShoppingCart.Sum(c => c.Quantity);

        return View();
    }

    public IActionResult Catalog(string? category)
    {
        List<StoreProduct> filteredProducts;

        if (string.IsNullOrWhiteSpace(category))
        {
            filteredProducts = Products;
        }
        else
        {
            filteredProducts = Products
                .Where(p => p.Category == category)
                .ToList();
        }

        var factory = new ProductCardStyleFactory();
        var catalog = new CatalogPage(factory);

        foreach (var product in filteredProducts)
        {
            string color = product.Category switch
            {
                "T-Shirts" => "LightBlue",
                "Jeans" => "DarkBlue",
                "Jackets" => "SteelBlue",
                "Hoodies" => "Gray",
                "Dresses" => "Navy",
                _ => "White"
            };

            catalog.AddProduct(
                new ProductCardContext(product.Name, product.Price, product.Size, product.Category),
                product.Category,
                color,
                "White",
                "Arial"
            );
        }

        ViewBag.Products = filteredProducts;
        ViewBag.Categories = Products.Select(p => p.Category).Distinct().ToList();
        ViewBag.FlyweightInfo =
            $"Flyweight: in catalog sunt {catalog.TotalCards} carduri, dar doar {catalog.SharedStyles} stiluri comune create.";
        ViewBag.CartCount = ShoppingCart.Sum(c => c.Quantity);

        return View();
    }

    public IActionResult Product(int id)
    {
        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return RedirectToAction("Catalog");
        }

        ViewBag.Product = product;
        ViewBag.CartCount = ShoppingCart.Sum(c => c.Quantity);

        return View();
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

        return RedirectToAction("Cart");
    }

    public IActionResult RemoveFromCart(int id)
    {
        CartLine? item = ShoppingCart.FirstOrDefault(c => c.Product.Id == id);

        if (item != null)
        {
            ShoppingCart.Remove(item);
        }

        return RedirectToAction("Cart");
    }

    public IActionResult IncreaseQuantity(int id)
    {
        CartLine? item = ShoppingCart.FirstOrDefault(c => c.Product.Id == id);

        if (item != null)
        {
            item.Quantity++;
        }

        return RedirectToAction("Cart");
    }

    public IActionResult DecreaseQuantity(int id)
    {
        CartLine? item = ShoppingCart.FirstOrDefault(c => c.Product.Id == id);

        if (item != null)
        {
            item.Quantity--;

            if (item.Quantity <= 0)
            {
                ShoppingCart.Remove(item);
            }
        }

        return RedirectToAction("Cart");
    }

    public IActionResult ClearCart()
    {
        ShoppingCart.Clear();
        return RedirectToAction("Cart");
    }

    public IActionResult Cart()
    {
        ViewBag.Cart = ShoppingCart;
        ViewBag.Total = ShoppingCart.Sum(c => c.Product.Price * c.Quantity);
        ViewBag.CartCount = ShoppingCart.Sum(c => c.Quantity);

        return View();
    }

    public IActionResult Checkout(bool email = true, bool sms = true, bool push = false)
    {
        IOrderNotification notification = new BasicOrderNotification();

        if (email)
        {
            notification = new EmailNotificationDecorator(notification);
        }

        if (sms)
        {
            notification = new SmsNotificationDecorator(notification);
        }

        if (push)
        {
            notification = new PushNotificationDecorator(notification);
        }

        var context = new NotificationContext(
            "Ana Popescu",
            "ana@mail.com",
            "+373 60000000",
            "DEVICE-TOKEN-123",
            "Comanda ta din magazinul Online Clothing Store a fost confirmata."
        );

        var result = notification.Send(context);

        ViewBag.Cart = ShoppingCart;
        ViewBag.Total = ShoppingCart.Sum(c => c.Product.Price * c.Quantity);
        ViewBag.NotificationChannels = result.Channels;
        ViewBag.DecoratorInfo = "Decorator: notificarea de baza a fost extinsa cu Email, SMS si Push.";
        ViewBag.CartCount = ShoppingCart.Sum(c => c.Quantity);

        return View();
    }

    public IActionResult Login(string role = "Customer")
    {
        HttpContext.Session.SetString("role", role);
        return RedirectToAction("Admin");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    public IActionResult Admin(string sku = "TSHIRT-001")
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        var realService = new SupplierPricingService();
        var employee = new StoreEmployee("employee01", role);
        var proxy = new SupplierPricingProxy(realService, employee);

        try
        {
            decimal supplierCost = proxy.GetSupplierCost(sku);
            ViewBag.SupplierResult = $"SKU: {sku} | Supplier cost: {supplierCost} MDL";
            ViewBag.Access = true;
        }
        catch (Exception ex)
        {
            ViewBag.SupplierResult = ex.Message;
            ViewBag.Access = false;
        }

        ViewBag.Role = role;
        ViewBag.ProxyInfo = "Proxy: accesul la costurile furnizorului este permis doar pentru rolurile autorizate.";
        ViewBag.CartCount = ShoppingCart.Sum(c => c.Quantity);

        return View();
    }

    public class StoreProduct
    {
        public int Id { get; }
        public string Name { get; }
        public string Category { get; }
        public decimal Price { get; }
        public string Size { get; }
        public string Color { get; }
        public string Image { get; }

        public StoreProduct(
            int id,
            string name,
            string category,
            decimal price,
            string size,
            string color,
            string image)
        {
            Id = id;
            Name = name;
            Category = category;
            Price = price;
            Size = size;
            Color = color;
            Image = image;
        }
    }

    public class CartLine
    {
        public StoreProduct Product { get; set; }
        public int Quantity { get; set; }

        public CartLine(StoreProduct product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}