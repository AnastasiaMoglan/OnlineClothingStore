using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.App.Structural.Bridge;
using OnlineClothingStore.App.Structural.Decorator;
using OnlineClothingStore.App.Structural.Flyweight;
using OnlineClothingStore.App.Structural.Proxy;
using OnlineClothingStore.Web.Models;

namespace OnlineClothingStore.Web.Controllers;

public class StoreController : Controller
{
    private static readonly List<StoreProduct> Products = new()
    {
        new StoreProduct(1, "Tricou Oversize Blue", "T-Shirts", 349, "M", "Albastru", 25),
        new StoreProduct(2, "Tricou Basic White", "T-Shirts", 279, "S", "Alb", 40),
        new StoreProduct(3, "Jeans Slim Fit", "Jeans", 699, "L", "Denim", 18),
        new StoreProduct(4, "Jeans Regular Dark", "Jeans", 749, "M", "Albastru inchis", 14),
        new StoreProduct(5, "Geaca Urban Denim", "Jackets", 1199, "M", "Albastru", 9),
        new StoreProduct(6, "Hanorac Minimal", "Hoodies", 599, "XL", "Gri", 30),
        new StoreProduct(7, "Hanorac Street Purple", "Hoodies", 649, "L", "Mov", 16),
        new StoreProduct(8, "Rochie Eleganta", "Dresses", 899, "S", "Bleumarin", 11),
        new StoreProduct(9, "Sneakers White", "Shoes", 999, "42", "Alb", 20)
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
        ViewBag.Products = Products;
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

    [HttpPost]
    public IActionResult UpdateProductStock(int id, int stockQuantity)
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role != "Manager")
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product != null)
        {
            product.StockQuantity = stockQuantity < 0 ? 0 : stockQuantity;
        }

        return RedirectToAction(nameof(Admin));
    }

    public IActionResult AddToCart(int id)
    {
        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product != null && product.StockQuantity > 0)
        {
            CartLine? existing = ShoppingCart.FirstOrDefault(c => c.Product.Id == id);

            if (existing == null)
            {
                ShoppingCart.Add(new CartLine(product, 1));
            }
            else if (existing.Quantity < product.StockQuantity)
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
            if (line.Quantity < line.Product.StockQuantity)
            {
                line.Quantity++;
            }
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
        decimal productsTotal = ShoppingCart.Sum(c => c.Product.Price * c.Quantity);

        ViewBag.Cart = ShoppingCart;
        ViewBag.Total = productsTotal;

        ConfigureBridgeViewData(
            customerName: "Ana",
            address: "Chisinau, bd. Stefan cel Mare 1",
            orderType: "standard",
            deliveryMethod: "courier",
            productsTotal: productsTotal,
            servicePrice: 0,
            deliveryPrice: 0,
            finalTotal: productsTotal,
            orderTypeName: "",
            deliveryMethodName: "",
            preparationResult: "",
            deliveryResult: "",
            wasCalculated: false
        );

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult Checkout(
        string customerName,
        string address,
        string orderType,
        string deliveryMethod)
    {
        customerName = string.IsNullOrWhiteSpace(customerName)
            ? "Client BlueWear"
            : customerName;

        address = string.IsNullOrWhiteSpace(address)
            ? "Adresa nu a fost indicata"
            : address;

        orderType = string.IsNullOrWhiteSpace(orderType)
            ? "standard"
            : orderType;

        deliveryMethod = string.IsNullOrWhiteSpace(deliveryMethod)
            ? "courier"
            : deliveryMethod;

        decimal productsTotal = ShoppingCart.Sum(c => c.Product.Price * c.Quantity);

        IDeliveryMethod selectedDeliveryMethod = CreateDeliveryMethod(deliveryMethod);
        DeliveryOrder deliveryOrder = CreateDeliveryOrder(orderType, selectedDeliveryMethod);

        string preparationResult = deliveryOrder.PrepareOrder();
        string deliveryResult = deliveryOrder.CompleteDelivery(customerName, address);

        decimal servicePrice = deliveryOrder.ServicePrice;
        decimal deliveryPrice = deliveryOrder.GetDeliveryPrice();
        decimal finalTotal = deliveryOrder.CalculateTotal(productsTotal);

        ViewBag.Cart = ShoppingCart;
        ViewBag.Total = productsTotal;

        ConfigureBridgeViewData(
            customerName: customerName,
            address: address,
            orderType: orderType,
            deliveryMethod: deliveryMethod,
            productsTotal: productsTotal,
            servicePrice: servicePrice,
            deliveryPrice: deliveryPrice,
            finalTotal: finalTotal,
            orderTypeName: deliveryOrder.OrderType,
            deliveryMethodName: deliveryOrder.GetDeliveryMethodName(),
            preparationResult: preparationResult,
            deliveryResult: deliveryResult,
            wasCalculated: true
        );

        AddLayoutCounters();

        return View();
    }

    // ============================================================
    // DECORATOR PATTERN
    // Functionalitate adaugata fara a modifica Flyweight.
    // Demonstreaza:
    // BasicOrderNotification + EmailNotificationDecorator
    // + SmsNotificationDecorator + PushNotificationDecorator
    // ============================================================

    public IActionResult Decorator()
    {
        ConfigureDecoratorViewData(
            customerName: "Ana",
            email: "ana@bluewear.com",
            phoneNumber: "+37360000000",
            deviceToken: "device-token-123",
            message: "Comanda ta BlueWear a fost confirmata.",
            useEmail: true,
            useSms: true,
            usePush: false,
            wasSent: false,
            channels: new List<string>(),
            decoratorChain: new List<string>()
        );

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult Decorator(
        string customerName,
        string email,
        string phoneNumber,
        string deviceToken,
        string message,
        bool useEmail,
        bool useSms,
        bool usePush)
    {
        customerName = string.IsNullOrWhiteSpace(customerName)
            ? "Client BlueWear"
            : customerName;

        email = string.IsNullOrWhiteSpace(email)
            ? "client@bluewear.com"
            : email;

        phoneNumber = string.IsNullOrWhiteSpace(phoneNumber)
            ? "+37360000000"
            : phoneNumber;

        deviceToken = string.IsNullOrWhiteSpace(deviceToken)
            ? "device-token-123"
            : deviceToken;

        message = string.IsNullOrWhiteSpace(message)
            ? "Comanda ta BlueWear a fost confirmata."
            : message;

        List<string> decoratorChain = new()
        {
            "Notificare de baza"
        };

        IOrderNotification notification = new BasicOrderNotification();

        if (useEmail)
        {
            notification = new EmailNotificationDecorator(notification);
            decoratorChain.Add("Email");
        }

        if (useSms)
        {
            notification = new SmsNotificationDecorator(notification);
            decoratorChain.Add("SMS");
        }

        if (usePush)
        {
            notification = new PushNotificationDecorator(notification);
            decoratorChain.Add("Push");
        }

        NotificationContext context = new(
            customerName,
            email,
            phoneNumber,
            deviceToken,
            message
        );

        NotificationResult result = notification.Send(context);

        ConfigureDecoratorViewData(
            customerName: customerName,
            email: email,
            phoneNumber: phoneNumber,
            deviceToken: deviceToken,
            message: message,
            useEmail: useEmail,
            useSms: useSms,
            usePush: usePush,
            wasSent: true,
            channels: result.Channels.ToList(),
            decoratorChain: decoratorChain
        );

        AddLayoutCounters();

        return View();
    }

    private void ConfigureDecoratorViewData(
        string customerName,
        string email,
        string phoneNumber,
        string deviceToken,
        string message,
        bool useEmail,
        bool useSms,
        bool usePush,
        bool wasSent,
        List<string> channels,
        List<string> decoratorChain)
    {
        ViewBag.CustomerName = customerName;
        ViewBag.Email = email;
        ViewBag.PhoneNumber = phoneNumber;
        ViewBag.DeviceToken = deviceToken;
        ViewBag.Message = message;

        ViewBag.UseEmail = useEmail;
        ViewBag.UseSms = useSms;
        ViewBag.UsePush = usePush;

        ViewBag.WasSent = wasSent;
        ViewBag.Channels = channels;
        ViewBag.DecoratorChain = decoratorChain;

        ViewBag.DecoratorInfo =
            "Notificarile pot fi trimise prin Email, SMS si Push.";
    }

    // ============================================================
    // BRIDGE PATTERN
    // Functionalitate adaugata fara a modifica Flyweight sau Decorator.
    // Demonstreaza separarea dintre:
    // 1. Tipul comenzii: Standard / Express / Cadou
    // 2. Metoda de livrare: Curier / Magazin / Locker
    // ============================================================

    public IActionResult Delivery()
    {
        return RedirectToAction(nameof(Checkout));
    }

    [HttpPost]
    public IActionResult Delivery(
        string customerName,
        string address,
        string orderType,
        string deliveryMethod)
    {
        customerName = string.IsNullOrWhiteSpace(customerName)
            ? "Client BlueWear"
            : customerName;

        address = string.IsNullOrWhiteSpace(address)
            ? "Adresa nu a fost indicata"
            : address;

        orderType = string.IsNullOrWhiteSpace(orderType)
            ? "standard"
            : orderType;

        deliveryMethod = string.IsNullOrWhiteSpace(deliveryMethod)
            ? "courier"
            : deliveryMethod;

        decimal productsTotal = ShoppingCart.Sum(c => c.Product.Price * c.Quantity);

        IDeliveryMethod selectedDeliveryMethod = CreateDeliveryMethod(deliveryMethod);

        DeliveryOrder deliveryOrder = CreateDeliveryOrder(orderType, selectedDeliveryMethod);

        string preparationResult = deliveryOrder.PrepareOrder();
        string deliveryResult = deliveryOrder.CompleteDelivery(customerName, address);

        decimal servicePrice = deliveryOrder.ServicePrice;
        decimal deliveryPrice = deliveryOrder.GetDeliveryPrice();
        decimal finalTotal = deliveryOrder.CalculateTotal(productsTotal);

        ConfigureBridgeViewData(
            customerName: customerName,
            address: address,
            orderType: orderType,
            deliveryMethod: deliveryMethod,
            productsTotal: productsTotal,
            servicePrice: servicePrice,
            deliveryPrice: deliveryPrice,
            finalTotal: finalTotal,
            orderTypeName: deliveryOrder.OrderType,
            deliveryMethodName: deliveryOrder.GetDeliveryMethodName(),
            preparationResult: preparationResult,
            deliveryResult: deliveryResult,
            wasCalculated: true
        );

        AddLayoutCounters();

        return View();
    }

    private static IDeliveryMethod CreateDeliveryMethod(string deliveryMethod)
    {
        return deliveryMethod switch
        {
            "pickup" => new PickupDeliveryMethod(),
            "locker" => new LockerDeliveryMethod(),
            _ => new CourierDeliveryMethod()
        };
    }

    private static DeliveryOrder CreateDeliveryOrder(string orderType, IDeliveryMethod deliveryMethod)
    {
        return orderType switch
        {
            "express" => new ExpressDeliveryOrder(deliveryMethod),
            "gift" => new GiftDeliveryOrder(deliveryMethod),
            _ => new StandardDeliveryOrder(deliveryMethod)
        };
    }

    private void ConfigureBridgeViewData(
        string customerName,
        string address,
        string orderType,
        string deliveryMethod,
        decimal productsTotal,
        decimal servicePrice,
        decimal deliveryPrice,
        decimal finalTotal,
        string orderTypeName,
        string deliveryMethodName,
        string preparationResult,
        string deliveryResult,
        bool wasCalculated)
    {
        ViewBag.CustomerName = customerName;
        ViewBag.Address = address;

        ViewBag.OrderType = orderType;
        ViewBag.DeliveryMethod = deliveryMethod;

        ViewBag.ProductsTotal = productsTotal;
        ViewBag.ServicePrice = servicePrice;
        ViewBag.DeliveryPrice = deliveryPrice;
        ViewBag.FinalTotal = finalTotal;

        ViewBag.OrderTypeName = orderTypeName;
        ViewBag.DeliveryMethodName = deliveryMethodName;

        ViewBag.PreparationResult = preparationResult;
        ViewBag.DeliveryResult = deliveryResult;

        ViewBag.WasCalculated = wasCalculated;

        ViewBag.BridgeInfo =
            "Bridge separa tipul comenzii de metoda de livrare. Tipul comenzii si metoda de livrare pot varia independent.";
    }

    // ============================================================
    // PROXY PATTERN
    // Functionalitate adaugata fara a modifica Flyweight,
    // Decorator sau Bridge.
    // Demonstreaza controlul accesului la raportul de stocuri.
    // ============================================================

    public IActionResult Inventory()
    {
        return RedirectToAction(nameof(Admin));
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

        public int StockQuantity { get; set; }

        public StoreProduct(
            int id,
            string name,
            string category,
            decimal price,
            string size,
            string color,
            int stockQuantity)
        {
            Id = id;
            Name = name;
            Category = category;
            Price = price;
            Size = size;
            Color = color;
            StockQuantity = stockQuantity;
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
