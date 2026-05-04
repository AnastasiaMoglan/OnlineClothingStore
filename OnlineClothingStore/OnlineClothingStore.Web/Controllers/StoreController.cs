using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.App.Structural.Bridge;
using OnlineClothingStore.App.Structural.Decorator;
using OnlineClothingStore.App.Structural.Flyweight;
using OnlineClothingStore.App.Structural.Observer;
using OnlineClothingStore.App.Structural.Strategy;
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

    private static readonly ProductStockSubject StockSubject = new();

    private static readonly List<StockNotification> StockNotifications = new();

    private static readonly AdminCommandInvoker CommandInvoker = new();

    // ============================================================
    // MEMENTO PATTERN - DATE STATICE PENTRU DEMONSTRARE
    // ============================================================

    private static readonly OutfitDesigner _outfitDesigner = new();

    private static readonly OutfitHistory _outfitHistory = new();

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
        ViewBag.Observers = StockSubject.GetObservers();
        ViewBag.StockNotifications = StockNotifications;
        ViewBag.CommandHistory = CommandInvoker.History;
        ViewBag.CommandLastMessage = CommandInvoker.LastMessage;

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
            IAdminCommand command = new UpdateProductStockCommand(
                product,
                stockQuantity,
                NotifyStockObservers
            );

            CommandInvoker.ExecuteCommand(command);
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

        IDiscountStrategy discountStrategy = CreateDiscountStrategy("none");
        DiscountCalculator discountCalculator = new(discountStrategy);

        decimal discountValue = discountCalculator.CalculateDiscount(productsTotal);
        decimal totalAfterDiscount = discountCalculator.CalculateTotalAfterDiscount(productsTotal);

        ViewBag.Cart = ShoppingCart;
        ViewBag.Total = productsTotal;

        ConfigureStrategyViewData(
            discountType: "none",
            discountValue: discountValue,
            totalAfterDiscount: totalAfterDiscount,
            discountName: discountCalculator.StrategyName,
            discountDescription: discountCalculator.StrategyDescription
        );

        ConfigureBridgeViewData(
            customerName: "Ana",
            address: "Chisinau, bd. Stefan cel Mare 1",
            orderType: "standard",
            deliveryMethod: "courier",
            productsTotal: productsTotal,
            servicePrice: 0,
            deliveryPrice: 0,
            finalTotal: totalAfterDiscount,
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
        string deliveryMethod,
        string discountType)
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

        discountType = string.IsNullOrWhiteSpace(discountType)
            ? "none"
            : discountType;

        decimal productsTotal = ShoppingCart.Sum(c => c.Product.Price * c.Quantity);

        IDiscountStrategy discountStrategy = CreateDiscountStrategy(discountType);
        DiscountCalculator discountCalculator = new(discountStrategy);

        decimal discountValue = discountCalculator.CalculateDiscount(productsTotal);
        decimal totalAfterDiscount = discountCalculator.CalculateTotalAfterDiscount(productsTotal);

        IDeliveryMethod selectedDeliveryMethod = CreateDeliveryMethod(deliveryMethod);
        DeliveryOrder deliveryOrder = CreateDeliveryOrder(orderType, selectedDeliveryMethod);

        string preparationResult = deliveryOrder.PrepareOrder();
        string deliveryResult = deliveryOrder.CompleteDelivery(customerName, address);

        decimal servicePrice = deliveryOrder.ServicePrice;
        decimal deliveryPrice = deliveryOrder.GetDeliveryPrice();
        decimal finalTotal = totalAfterDiscount + servicePrice + deliveryPrice;

        ViewBag.Cart = ShoppingCart;
        ViewBag.Total = productsTotal;

        ConfigureStrategyViewData(
            discountType: discountType,
            discountValue: discountValue,
            totalAfterDiscount: totalAfterDiscount,
            discountName: discountCalculator.StrategyName,
            discountDescription: discountCalculator.StrategyDescription
        );

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

        IDiscountStrategy discountStrategy = CreateDiscountStrategy("none");
        DiscountCalculator discountCalculator = new(discountStrategy);

        decimal discountValue = discountCalculator.CalculateDiscount(productsTotal);
        decimal totalAfterDiscount = discountCalculator.CalculateTotalAfterDiscount(productsTotal);

        IDeliveryMethod selectedDeliveryMethod = CreateDeliveryMethod(deliveryMethod);
        DeliveryOrder deliveryOrder = CreateDeliveryOrder(orderType, selectedDeliveryMethod);

        string preparationResult = deliveryOrder.PrepareOrder();
        string deliveryResult = deliveryOrder.CompleteDelivery(customerName, address);

        decimal servicePrice = deliveryOrder.ServicePrice;
        decimal deliveryPrice = deliveryOrder.GetDeliveryPrice();
        decimal finalTotal = totalAfterDiscount + servicePrice + deliveryPrice;

        ConfigureStrategyViewData(
            discountType: "none",
            discountValue: discountValue,
            totalAfterDiscount: totalAfterDiscount,
            discountName: discountCalculator.StrategyName,
            discountDescription: discountCalculator.StrategyDescription
        );

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
    // STRATEGY PATTERN
    // ============================================================

    private static IDiscountStrategy CreateDiscountStrategy(string discountType)
    {
        return discountType switch
        {
            "newCustomer" => new NewCustomerDiscountStrategy(),
            "student" => new StudentDiscountStrategy(),
            "vip" => new VipDiscountStrategy(),
            _ => new NoDiscountStrategy()
        };
    }

    private void ConfigureStrategyViewData(
        string discountType,
        decimal discountValue,
        decimal totalAfterDiscount,
        string discountName,
        string discountDescription)
    {
        ViewBag.DiscountType = discountType;
        ViewBag.DiscountValue = discountValue;
        ViewBag.TotalAfterDiscount = totalAfterDiscount;
        ViewBag.DiscountName = discountName;
        ViewBag.DiscountDescription = discountDescription;

        ViewBag.StrategyInfo =
            "Strategy permite alegerea algoritmului de reducere fara modificarea codului principal de checkout.";
    }

    // ============================================================
    // OBSERVER PATTERN
    // ============================================================

    public IActionResult StockAlerts()
    {
        ViewBag.Products = Products;

        ViewBag.ObserverInfo =
            "Observer permite clientilor sa se aboneze la modificarile de stoc. Cand managerul modifica stocul in Admin, abonatii produsului sunt notificati automat.";

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult SubscribeStockAlert(
        int productId,
        string customerName,
        string email)
    {
        StoreProduct? product = Products.FirstOrDefault(p => p.Id == productId);

        if (product == null)
        {
            return RedirectToAction(nameof(StockAlerts));
        }

        customerName = string.IsNullOrWhiteSpace(customerName)
            ? "Client BlueWear"
            : customerName;

        email = string.IsNullOrWhiteSpace(email)
            ? "client@bluewear.com"
            : email;

        CustomerStockObserver observer = new(
            product.Id,
            customerName,
            email
        );

        StockSubject.Attach(observer);

        return RedirectToAction(nameof(StockAlerts));
    }

    [HttpPost]
    public IActionResult UnsubscribeStockAlert(
        int productId,
        string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            StockSubject.Detach(email, productId);
        }

        return RedirectToAction(nameof(Admin));
    }

    public IActionResult ClearStockNotifications()
    {
        StockNotifications.Clear();

        return RedirectToAction(nameof(Admin));
    }

    private static void NotifyStockObservers(StoreProduct product, int oldStock, int newStock)
    {
        if (oldStock == newStock)
        {
            return;
        }

        StockChangedEvent stockEvent = new(
            product.Id,
            product.Name,
            oldStock,
            newStock
        );

        List<StockNotification> notifications = StockSubject.Notify(stockEvent);

        if (notifications.Count > 0)
        {
            StockNotifications.AddRange(notifications);
        }
    }

    // ============================================================
    // COMMAND PATTERN
    // ============================================================

    public IActionResult AdminCommands()
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role != "Manager")
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        ViewBag.Products = Products;
        ViewBag.CommandHistory = CommandInvoker.History;
        ViewBag.CommandLastMessage = CommandInvoker.LastMessage;

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult ExecuteStockCommand(int id, int stockQuantity)
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role != "Manager")
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product != null)
        {
            IAdminCommand command = new UpdateProductStockCommand(
                product,
                stockQuantity,
                NotifyStockObservers
            );

            CommandInvoker.ExecuteCommand(command);
        }

        return RedirectToAction(nameof(AdminCommands));
    }

    [HttpPost]
    public IActionResult ExecutePriceCommand(int id, decimal price)
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role != "Manager")
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product != null)
        {
            IAdminCommand command = new ChangeProductPriceCommand(
                product,
                price
            );

            CommandInvoker.ExecuteCommand(command);
        }

        return RedirectToAction(nameof(AdminCommands));
    }

    [HttpPost]
    public IActionResult UndoLastAdminCommand()
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role != "Manager")
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        CommandInvoker.UndoLastCommand();

        return RedirectToAction(nameof(AdminCommands));
    }

    // ============================================================
    // MEMENTO PATTERN
    // ============================================================

    public IActionResult Memento()
    {
        ConfigureMementoViewData(
            message: "Modifica tinuta, salveaza snapshot-uri si testeaza Undo / Redo."
        );

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult UpdateOutfitDraft(
        string top,
        string bottom,
        string shoes,
        string accessory,
        string colorPalette,
        string notes)
    {
        top = string.IsNullOrWhiteSpace(top) ? "Tricou basic" : top;
        bottom = string.IsNullOrWhiteSpace(bottom) ? "Jeans albastri" : bottom;
        shoes = string.IsNullOrWhiteSpace(shoes) ? "Sneakers albi" : shoes;
        accessory = string.IsNullOrWhiteSpace(accessory) ? "Geanta mica" : accessory;
        colorPalette = string.IsNullOrWhiteSpace(colorPalette) ? "Blue casual" : colorPalette;
        notes = string.IsNullOrWhiteSpace(notes) ? "Tinuta de zi, lejera." : notes;

        _outfitDesigner.UpdateOutfit(
            top,
            bottom,
            shoes,
            accessory,
            colorPalette,
            notes
        );

        TempData["MementoMessage"] =
            "Tinuta a fost modificata. Pentru a pastra aceasta versiune, apasa Salveaza snapshot.";

        return RedirectToAction(nameof(Memento));
    }

    [HttpPost]
    public IActionResult SaveOutfitSnapshot()
    {
        _outfitHistory.SaveState(_outfitDesigner);

        TempData["MementoMessage"] =
            "Snapshot salvat. Starea curenta a tinutei a fost memorata.";

        return RedirectToAction(nameof(Memento));
    }

    [HttpPost]
    public IActionResult UndoOutfit()
    {
        bool restored = _outfitHistory.Undo(_outfitDesigner);

        TempData["MementoMessage"] = restored
            ? "Undo realizat. Tinuta a revenit la o stare salvata anterior."
            : "Nu exista snapshot-uri pentru Undo.";

        return RedirectToAction(nameof(Memento));
    }

    [HttpPost]
    public IActionResult RedoOutfit()
    {
        bool restored = _outfitHistory.Redo(_outfitDesigner);

        TempData["MementoMessage"] = restored
            ? "Redo realizat. Tinuta a fost restaurata inainte."
            : "Nu exista snapshot-uri pentru Redo.";

        return RedirectToAction(nameof(Memento));
    }

    private void ConfigureMementoViewData(string message)
    {
        ViewBag.OutfitTop = _outfitDesigner.Top;
        ViewBag.OutfitBottom = _outfitDesigner.Bottom;
        ViewBag.OutfitShoes = _outfitDesigner.Shoes;
        ViewBag.OutfitAccessory = _outfitDesigner.Accessory;
        ViewBag.OutfitColorPalette = _outfitDesigner.ColorPalette;
        ViewBag.OutfitNotes = _outfitDesigner.Notes;

        ViewBag.UndoCount = _outfitHistory.UndoCount;
        ViewBag.RedoCount = _outfitHistory.RedoCount;
        ViewBag.SnapshotLabels = _outfitHistory.GetHistoryLabels();

        ViewBag.MementoMessage = TempData["MementoMessage"] ?? message;

        ViewBag.MementoInfo =
            "Memento salveaza starea interna a obiectului OutfitDesigner intr-un snapshot, fara ca View-ul sau Controller-ul sa modifice direct continutul snapshot-ului.";

        ViewBag.MementoRoles =
            "OutfitDesigner este Originator, OutfitDraftMemento este Memento, iar OutfitHistory este Caretaker.";
    }

    // ============================================================
    // PROXY PATTERN
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
                FontFamily = style.FontFamily,
                TextColor = style.TextColor,
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

        public decimal Price { get; set; }

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

    public interface IAdminCommand
    {
        string Name { get; }

        string Description { get; }

        void Execute();

        void Undo();
    }

    public class UpdateProductStockCommand : IAdminCommand
    {
        private readonly StoreProduct _product;
        private readonly int _oldStock;
        private readonly int _newStock;
        private readonly Action<StoreProduct, int, int> _onStockChanged;

        public UpdateProductStockCommand(
            StoreProduct product,
            int newStock,
            Action<StoreProduct, int, int> onStockChanged)
        {
            _product = product;
            _oldStock = product.StockQuantity;
            _newStock = newStock < 0 ? 0 : newStock;
            _onStockChanged = onStockChanged;
        }

        public string Name => "Modificare stoc";

        public string Description =>
            $"Produsul {_product.Name}: stoc schimbat de la {_oldStock} la {_newStock}.";

        public void Execute()
        {
            _product.StockQuantity = _newStock;
            _onStockChanged(_product, _oldStock, _newStock);
        }

        public void Undo()
        {
            int currentStock = _product.StockQuantity;
            _product.StockQuantity = _oldStock;
            _onStockChanged(_product, currentStock, _oldStock);
        }
    }

    public class ChangeProductPriceCommand : IAdminCommand
    {
        private readonly StoreProduct _product;
        private readonly decimal _oldPrice;
        private readonly decimal _newPrice;

        public ChangeProductPriceCommand(StoreProduct product, decimal newPrice)
        {
            _product = product;
            _oldPrice = product.Price;
            _newPrice = newPrice < 0 ? 0 : newPrice;
        }

        public string Name => "Modificare pret";

        public string Description =>
            $"Produsul {_product.Name}: pret schimbat de la {_oldPrice} MDL la {_newPrice} MDL.";

        public void Execute()
        {
            _product.Price = _newPrice;
        }

        public void Undo()
        {
            _product.Price = _oldPrice;
        }
    }

    public class AdminCommandInvoker
    {
        private readonly Stack<IAdminCommand> _history = new();

        public IReadOnlyList<IAdminCommand> History => _history.ToList();

        public string LastMessage { get; private set; } = "Nu a fost executata nicio comanda.";

        public void ExecuteCommand(IAdminCommand command)
        {
            command.Execute();
            _history.Push(command);
            LastMessage = $"Executat: {command.Description}";
        }

        public void UndoLastCommand()
        {
            if (_history.Count == 0)
            {
                LastMessage = "Nu exista comenzi pentru Undo.";
                return;
            }

            IAdminCommand command = _history.Pop();
            command.Undo();

            LastMessage = $"Undo: {command.Description}";
        }
    }

    // ============================================================
    // CLASE PENTRU MEMENTO PATTERN
    // ============================================================

    public class OutfitDesigner
    {
        public string Top { get; private set; } = "Tricou basic BlueWear";
        public string Bottom { get; private set; } = "Jeans slim fit";
        public string Shoes { get; private set; } = "Sneakers white";
        public string Accessory { get; private set; } = "Geanta casual";
        public string ColorPalette { get; private set; } = "Albastru + Alb";
        public string Notes { get; private set; } = "Tinuta casual pentru oras.";

        public void UpdateOutfit(
            string top,
            string bottom,
            string shoes,
            string accessory,
            string colorPalette,
            string notes)
        {
            Top = top;
            Bottom = bottom;
            Shoes = shoes;
            Accessory = accessory;
            ColorPalette = colorPalette;
            Notes = notes;
        }

        public OutfitDraftMemento Save()
        {
            return new OutfitDraftMemento(
                Top,
                Bottom,
                Shoes,
                Accessory,
                ColorPalette,
                Notes
            );
        }

        public void Restore(OutfitDraftMemento snapshot)
        {
            Top = snapshot.Top;
            Bottom = snapshot.Bottom;
            Shoes = snapshot.Shoes;
            Accessory = snapshot.Accessory;
            ColorPalette = snapshot.ColorPalette;
            Notes = snapshot.Notes;
        }
    }

    public sealed class OutfitDraftMemento
    {
        internal string Top { get; }
        internal string Bottom { get; }
        internal string Shoes { get; }
        internal string Accessory { get; }
        internal string ColorPalette { get; }
        internal string Notes { get; }

        public DateTime SavedAt { get; }

        internal OutfitDraftMemento(
            string top,
            string bottom,
            string shoes,
            string accessory,
            string colorPalette,
            string notes)
        {
            Top = top;
            Bottom = bottom;
            Shoes = shoes;
            Accessory = accessory;
            ColorPalette = colorPalette;
            Notes = notes;
            SavedAt = DateTime.Now;
        }

        public string GetLabel()
        {
            return $"Snapshot salvat la {SavedAt:HH:mm:ss}";
        }
    }

    public class OutfitHistory
    {
        private readonly Stack<OutfitDraftMemento> _undoStack = new();
        private readonly Stack<OutfitDraftMemento> _redoStack = new();

        public int UndoCount => _undoStack.Count;

        public int RedoCount => _redoStack.Count;

        public void SaveState(OutfitDesigner designer)
        {
            _undoStack.Push(designer.Save());
            _redoStack.Clear();
        }

        public bool Undo(OutfitDesigner designer)
        {
            if (_undoStack.Count == 0)
            {
                return false;
            }

            _redoStack.Push(designer.Save());

            OutfitDraftMemento previousState = _undoStack.Pop();
            designer.Restore(previousState);

            return true;
        }

        public bool Redo(OutfitDesigner designer)
        {
            if (_redoStack.Count == 0)
            {
                return false;
            }

            _undoStack.Push(designer.Save());

            OutfitDraftMemento nextState = _redoStack.Pop();
            designer.Restore(nextState);

            return true;
        }

        public List<string> GetHistoryLabels()
        {
            return _undoStack
                .Select(snapshot => snapshot.GetLabel())
                .ToList();
        }
    }
}