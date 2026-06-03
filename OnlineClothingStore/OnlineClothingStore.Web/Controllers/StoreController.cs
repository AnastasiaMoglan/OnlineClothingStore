using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineClothingStore.App.Behavioral.Iterator;
using OnlineClothingStore.App.Structural.Bridge;
using OnlineClothingStore.App.Structural.Decorator;
using OnlineClothingStore.App.Structural.Flyweight;
using OnlineClothingStore.App.Structural.Observer;
using OnlineClothingStore.App.Structural.Strategy;
using OnlineClothingStore.Web.Data;
using OnlineClothingStore.Web.Models;
using OnlineClothingStore.Creational.AbstractFactory;
using OnlineClothingStore.Creational.Builder;
using OnlineClothingStore.Creational.FactoryMethod;
using OnlineClothingStore.Creational.Prototype;
using PrototypeSizeGuide = OnlineClothingStore.Creational.Prototype.SizeGuide;

namespace OnlineClothingStore.Web.Controllers;

public class StoreController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    private List<StoreProduct> Products => _context.Products.ToList();

    private static readonly ProductCardStyleFactory StyleFactory = new();

    private static readonly AdminCommandInvoker CommandInvoker = new();

    // ============================================================
    // MEMENTO PATTERN - DATE STATICE PENTRU DEMONSTRARE
    // ============================================================

    private static readonly OutfitDesigner _outfitDesigner = new();

    private static readonly OutfitHistory _outfitHistory = new();

    public StoreController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        ViewBag.FeaturedProducts = Products.Take(3).ToList();
        ViewBag.Categories = Products.Select(p => p.Category).Distinct().ToList();

        AddLayoutCounters();

        return View();
    }

    public IActionResult Catalog(
        string? category,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy)
    {
        NormalizePriceRange(ref minPrice, ref maxPrice);

        StoreProductCatalog catalog = new(Products);
        IStoreProductIterator catalogIterator = catalog.CreateIterator(
            category,
            minPrice,
            maxPrice,
            sortBy);
        List<StoreProduct> filteredProducts = new();

        while (catalogIterator.HasNext())
        {
            filteredProducts.Add(catalogIterator.Next());
        }

        List<ProductViewModel> productCards = BuildProductCards(filteredProducts);

        ViewBag.ProductCards = productCards;
        ViewBag.Categories = Products.Select(p => p.Category).Distinct().ToList();
        ViewBag.SelectedCategory = category ?? "Toate";
        ViewBag.SelectedMinPrice = minPrice;
        ViewBag.SelectedMaxPrice = maxPrice;
        ViewBag.SelectedSortBy = sortBy ?? "none";

        ViewBag.TotalCards = productCards.Count;
        ViewBag.SharedStyles = StyleFactory.CreatedStylesCount;
        ViewBag.SavedObjects = productCards.Count - StyleFactory.CreatedStylesCount;
        ViewBag.IteratorVisitedCount = catalogIterator.VisitedCount;
        ViewBag.IteratorReturnedCount = productCards.Count;
        ViewBag.IteratorFilterLabel = BuildIteratorFilterLabel(
            category,
            minPrice,
            maxPrice,
            sortBy);

        ViewBag.FlyweightInfo =
            $"In catalog sunt {productCards.Count} produse afisate, dar stilurile cardurilor sunt partajate pe categorii. " +
            $"Factory-ul a creat doar {StyleFactory.CreatedStylesCount} obiecte de stil.";

        ViewBag.IteratorInfo =
            "Catalogul foloseste Iterator pentru a parcurge produsele pe rand si pentru a afisa doar produsele care respecta filtrul ales.";
        ViewBag.IteratorRoles =
            "StoreProductCatalog este colectia, ProductCatalogIterator parcurge produsele, iar Catalog foloseste doar metoda HasNext / Next.";

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
        ViewBag.Observers = GetStockObservers();
        ViewBag.StockNotifications = GetStockNotifications();
        ViewBag.CommandHistory = CommandInvoker.History;
        ViewBag.CommandLastMessage = CommandInvoker.LastMessage;
        ViewBag.ReturnRequests = GetReturnRequests();
        ViewBag.SupportRequests = GetCustomerSupportRequests();

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
            LogAdminCommand(command, isUndo: false);
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Admin));
    }

    public IActionResult AddToCart(int id)
    {
        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product != null && product.StockQuantity > 0)
        {
            CartItem? existing = _context.CartItems.FirstOrDefault(c => c.ProductId == id);

            if (existing == null)
            {
                _context.CartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = 1
                });
            }
            else if (existing.Quantity < product.StockQuantity)
            {
                existing.Quantity++;
            }

            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult Cart()
    {
        List<CartLine> cart = GetCartLines();

        ViewBag.Cart = cart;
        ViewBag.Total = cart.Sum(c => c.Product.Price * c.Quantity);

        AddLayoutCounters();

        return View();
    }

    public IActionResult IncreaseQuantity(int id)
    {
        CartItem? line = _context.CartItems
            .Include(c => c.Product)
            .FirstOrDefault(c => c.ProductId == id);

        if (line != null)
        {
            if (line.Quantity < line.Product.StockQuantity)
            {
                line.Quantity++;
                _context.SaveChanges();
            }
        }

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult DecreaseQuantity(int id)
    {
        CartItem? line = _context.CartItems.FirstOrDefault(c => c.ProductId == id);

        if (line != null)
        {
            line.Quantity--;

            if (line.Quantity <= 0)
            {
                _context.CartItems.Remove(line);
            }

            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult RemoveFromCart(int id)
    {
        CartItem? line = _context.CartItems.FirstOrDefault(c => c.ProductId == id);

        if (line != null)
        {
            _context.CartItems.Remove(line);
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult ClearCart()
    {
        _context.CartItems.RemoveRange(_context.CartItems);
        _context.SaveChanges();

        return RedirectToAction(nameof(Cart));
    }

    public IActionResult Wishlist()
    {
        ViewBag.Wishlist = GetWishlistProducts();

        AddLayoutCounters();

        return View();
    }

    public IActionResult AddToWishlist(int id)
    {
        StoreProduct? product = Products.FirstOrDefault(p => p.Id == id);

        if (product != null && !_context.WishlistItems.Any(p => p.ProductId == id))
        {
            _context.WishlistItems.Add(new WishlistItem
            {
                ProductId = product.Id
            });
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Wishlist));
    }

    public IActionResult RemoveFromWishlist(int id)
    {
        WishlistItem? item = _context.WishlistItems.FirstOrDefault(p => p.ProductId == id);

        if (item != null)
        {
            _context.WishlistItems.Remove(item);
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Wishlist));
    }

    public IActionResult Support()
    {
        ConfigureSupportViewData(
            orderNumber: "",
            customerEmail: "",
            contactPhone: "",
            problemType: "Damaged product",
            preferredSolution: "Replace the product",
            description: "",
            attachedImages: "",
            isUrgent: false,
            wasSubmitted: false,
            message: ""
        );

        AddLayoutCounters();

        return View();
    }

    public IActionResult SizeGuide(
        string? category,
        string? region,
        string? fit,
        decimal? chest,
        decimal? waist,
        decimal? hips,
        decimal? footLength)
    {
        string selectedCategory = NormalizeSizeGuideCategory(category);
        string selectedRegion = NormalizeSizeGuideRegion(region);
        string selectedFit = NormalizeSizeGuideFit(fit);

        PrototypeSizeGuide? guide = null;
        string recommendedSize = "Alege categoria";

        if (!string.IsNullOrWhiteSpace(selectedCategory))
        {
            guide = CreateSizeGuideClone(selectedCategory);
            guide.Region = selectedRegion;
            guide.BrandName = $"BlueWear {GetFitLabel(selectedFit)}";
            guide.Sizes = BuildRegionAwareSizes(guide.Category, selectedRegion);
            guide.Notes = BuildSizeGuideNotes(selectedCategory, selectedFit, chest, waist, hips, footLength);
            recommendedSize = BuildSizeRecommendation(selectedCategory, selectedRegion, chest, waist, hips, footLength);
        }

        ViewBag.SizeGuide = guide;
        ViewBag.SizeGuideCategories = GetSizeGuideCategories();
        ViewBag.SizeGuideRegions = new List<string> { "EU", "US", "UK" };
        ViewBag.SelectedCategory = selectedCategory;
        ViewBag.SelectedRegion = selectedRegion;
        ViewBag.SelectedFit = selectedFit;
        ViewBag.Chest = chest;
        ViewBag.Waist = waist;
        ViewBag.Hips = hips;
        ViewBag.FootLength = footLength;
        ViewBag.RecommendedSize = recommendedSize;

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult Support(
        string orderNumber,
        string customerEmail,
        string contactPhone,
        string problemType,
        string preferredSolution,
        string description,
        string attachedImages,
        bool isUrgent)
    {
        orderNumber = string.IsNullOrWhiteSpace(orderNumber)
            ? "Comanda neindicata"
            : orderNumber;

        customerEmail = string.IsNullOrWhiteSpace(customerEmail)
            ? "email-neindicat@bluewear.local"
            : customerEmail;

        contactPhone = string.IsNullOrWhiteSpace(contactPhone)
            ? "Telefon neindicat"
            : contactPhone;

        problemType = string.IsNullOrWhiteSpace(problemType)
            ? "Other"
            : problemType;

        preferredSolution = string.IsNullOrWhiteSpace(preferredSolution)
            ? "Contact me with a solution"
            : preferredSolution;

        description = string.IsNullOrWhiteSpace(description)
            ? "Clientul a trimis o cerere fara descriere suplimentara."
            : description;

        List<string> images = SplitAttachedImages(attachedImages);

        ICustomerSupportRequestBuilder builder = new CustomerSupportRequestBuilder()
            .Reset()
            .SetOrderNumber(orderNumber)
            .SetCustomerEmail(customerEmail)
            .SetProblemType(problemType)
            .SetDescription(description)
            .SetPreferredSolution(preferredSolution)
            .SetContactPhone(contactPhone);

        if (isUrgent)
        {
            builder.MarkAsUrgent();
        }

        foreach (string image in images)
        {
            builder.AddAttachedImage(image);
        }

        CustomerSupportRequest request = builder.Build();

        _context.CustomerSupportRequests.Add(new CustomerSupportRequestRecord
        {
            OrderNumber = request.OrderNumber,
            CustomerEmail = request.CustomerEmail,
            ContactPhone = request.ContactPhone,
            ProblemType = request.ProblemType,
            Description = request.Description,
            PreferredSolution = request.PreferredSolution,
            IsUrgent = request.IsUrgent,
            AttachedImages = string.Join(", ", request.AttachedImages)
        });

        _context.SaveChanges();

        ConfigureSupportViewData(
            orderNumber: orderNumber,
            customerEmail: customerEmail,
            contactPhone: contactPhone,
            problemType: problemType,
            preferredSolution: preferredSolution,
            description: description,
            attachedImages: attachedImages,
            isUrgent: isUrgent,
            wasSubmitted: true,
            message: "Cererea a fost trimisa catre Customer Support."
        );

        AddLayoutCounters();

        return View();
    }

    public IActionResult Checkout()
    {
        List<CartLine> cart = GetCartLines();
        decimal productsTotal = cart.Sum(c => c.Product.Price * c.Quantity);

        IDiscountStrategy discountStrategy = CreateDiscountStrategy("none");
        DiscountCalculator discountCalculator = new(discountStrategy);

        decimal discountValue = discountCalculator.CalculateDiscount(productsTotal);
        decimal totalAfterDiscount = discountCalculator.CalculateTotalAfterDiscount(productsTotal);
        PackagingCheckoutOption packaging = CreatePackagingCheckoutOption("standard");

        ViewBag.Cart = cart;
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
            packagingPrice: packaging.Price,
            finalTotal: totalAfterDiscount + packaging.Price,
            orderTypeName: "",
            deliveryMethodName: "",
            preparationResult: "",
            deliveryResult: "",
            wasCalculated: false
        );

        ConfigurePackagingViewData(packaging);

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult Checkout(
        string customerName,
        string address,
        string orderType,
        string deliveryMethod,
        string discountType,
        string packagingType)
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

        packagingType = string.IsNullOrWhiteSpace(packagingType)
            ? "standard"
            : packagingType;

        List<CartLine> cart = GetCartLines();
        decimal productsTotal = cart.Sum(c => c.Product.Price * c.Quantity);

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
        PackagingCheckoutOption packaging = CreatePackagingCheckoutOption(packagingType);
        decimal finalTotal = totalAfterDiscount + servicePrice + deliveryPrice + packaging.Price;

        ViewBag.Cart = cart;
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
            packagingPrice: packaging.Price,
            finalTotal: finalTotal,
            orderTypeName: deliveryOrder.OrderType,
            deliveryMethodName: deliveryOrder.GetDeliveryMethodName(),
            preparationResult: preparationResult,
            deliveryResult: deliveryResult,
            wasCalculated: true
        );

        ConfigurePackagingViewData(packaging);

        AddLayoutCounters();

        return View();
    }

    private static PackagingCheckoutOption CreatePackagingCheckoutOption(string packagingType)
    {
        packagingType = string.IsNullOrWhiteSpace(packagingType)
            ? "standard"
            : packagingType;

        IOrderPackagingFactory factory = packagingType switch
        {
            "eco" => new EcoPackagingFactory(),
            "luxury" => new LuxuryPackagingFactory(),
            _ => new StandardPackagingFactory()
        };

        OrderPackagingService packagingService = new(factory);
        PackagingResult result = packagingService.PreparePackaging();

        return new PackagingCheckoutOption(
            Type: packagingType switch
            {
                "eco" => "eco",
                "luxury" => "luxury",
                _ => "standard"
            },
            Name: packagingType switch
            {
                "eco" => "Eco Smart Pack",
                "luxury" => "Luxury Gift Wrap",
                _ => "Ambalare standard"
            },
            Price: packagingType switch
            {
                "eco" => 45m,
                "luxury" => 120m,
                _ => 0m
            },
            ImageUrl: packagingType switch
            {
                "eco" => "https://forestpackage.com/wp-content/uploads/2023/02/apparel-box.jpg",
                "luxury" => "https://image.made-in-china.com/2f0j00zLqoaWkGvUcb/Custom-Logo-Printed-Purple-Premium-Schmuck-Box-Jewelry-Packaging-Pouch-and-Boxes-Slide-Drawer-Jewelry-Box-Packaging.jpg",
                _ => "https://www.top-packaging.com/uploadfile/201908/12/61cc3bf8fc5f8a029e9d37fc90cb14ed_medium.jpg"
            },
            BoxDescription: result.BoxDescription,
            LabelDescription: result.LabelDescription,
            InsertDescription: result.InsertDescription
        );
    }

    private void ConfigurePackagingViewData(PackagingCheckoutOption packaging)
    {
        ViewBag.PackagingType = packaging.Type;
        ViewBag.PackagingName = packaging.Name;
        ViewBag.PackagingPrice = packaging.Price;
        ViewBag.PackagingImageUrl = packaging.ImageUrl;
        ViewBag.PackagingBoxDescription = packaging.BoxDescription;
        ViewBag.PackagingLabelDescription = packaging.LabelDescription;
        ViewBag.PackagingInsertDescription = packaging.InsertDescription;
    }

    private sealed record PackagingCheckoutOption(
        string Type,
        string Name,
        decimal Price,
        string ImageUrl,
        string BoxDescription,
        string LabelDescription,
        string InsertDescription);

    // ============================================================
    // FACTORY METHOD PATTERN
    // ============================================================

    public IActionResult FactoryMethod()
    {
        return RedirectToAction(nameof(Returns));
    }

    public IActionResult Returns()
    {
        ConfigureFactoryMethodViewData(
            orderNumber: "",
            productName: "",
            customerEmail: "",
            phoneNumber: "",
            returnType: "refund",
            resultMessage: string.Empty,
            returnReason: "",
            requestId: null,
            wasProcessed: false
        );

        AddLayoutCounters();

        return View();
    }

    [HttpPost]
    public IActionResult Returns(
        string orderNumber,
        string productName,
        string customerEmail,
        string phoneNumber,
        string returnReason,
        string returnType)
    {
        orderNumber = string.IsNullOrWhiteSpace(orderNumber)
            ? "Comanda neindicata"
            : orderNumber;

        productName = string.IsNullOrWhiteSpace(productName)
            ? "Produs din comanda"
            : productName;

        customerEmail = string.IsNullOrWhiteSpace(customerEmail)
            ? "email-neindicat@bluewear.local"
            : customerEmail;

        phoneNumber = string.IsNullOrWhiteSpace(phoneNumber)
            ? "Telefon neindicat"
            : phoneNumber;

        returnReason = string.IsNullOrWhiteSpace(returnReason)
            ? "Clientul a solicitat retur fara detalii suplimentare."
            : returnReason;

        returnType = string.IsNullOrWhiteSpace(returnType)
            ? "refund"
            : returnType;

        ReturnProcessorFactory factory = CreateReturnProcessorFactory(returnType);

        string resultMessage = factory.HandleReturn(orderNumber, productName);

        ReturnRequestRecord request = new()
        {
            OrderNumber = orderNumber,
            ProductName = productName,
            CustomerEmail = customerEmail,
            PhoneNumber = phoneNumber,
            ReturnReason = returnReason,
            ReturnType = GetReturnTypeLabel(returnType),
            ProcessingMessage = resultMessage
        };

        _context.ReturnRequests.Add(request);
        _context.SaveChanges();

        ConfigureFactoryMethodViewData(
            orderNumber: orderNumber,
            productName: productName,
            customerEmail: customerEmail,
            phoneNumber: phoneNumber,
            returnType: returnType,
            resultMessage: resultMessage,
            returnReason: returnReason,
            requestId: request.Id,
            wasProcessed: true
        );

        AddLayoutCounters();

        return View();
    }

    private static ReturnProcessorFactory CreateReturnProcessorFactory(string returnType)
    {
        return returnType switch
        {
            "size" => new SizeExchangeReturnFactory(),
            "defective" => new DefectiveProductReturnFactory(),
            _ => new RefundReturnFactory()
        };
    }

    private void ConfigureFactoryMethodViewData(
        string orderNumber,
        string productName,
        string customerEmail,
        string phoneNumber,
        string returnType,
        string resultMessage,
        string returnReason,
        int? requestId,
        bool wasProcessed)
    {
        ViewBag.OrderNumber = orderNumber;
        ViewBag.ProductName = productName;
        ViewBag.CustomerEmail = customerEmail;
        ViewBag.PhoneNumber = phoneNumber;
        ViewBag.ReturnType = returnType;
        ViewBag.ReturnReason = returnReason;
        ViewBag.ResultMessage = resultMessage;
        ViewBag.ReturnRequestId = requestId;
        ViewBag.WasProcessed = wasProcessed;
    }

    [HttpPost]
    public IActionResult RegisterReturnRequest(int id)
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role != "Manager")
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        ReturnRequestRecord? request = _context.ReturnRequests.FirstOrDefault(item => item.Id == id);

        if (request != null)
        {
            request.Status = "Inregistrat";
            request.RegisteredAt = DateTime.Now;
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Admin));
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

        _context.SimpleNotifications.Add(new SimpleNotificationRecord
        {
            CustomerName = customerName,
            Email = email,
            PhoneNumber = phoneNumber,
            DeviceToken = deviceToken,
            Message = message,
            UseEmail = useEmail,
            UseSms = useSms,
            UsePush = usePush,
            Channels = string.Join(", ", result.Channels),
            DecoratorChain = string.Join(" -> ", decoratorChain)
        });
        _context.SaveChanges();

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
        string deliveryMethod,
        string packagingType)
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

        packagingType = string.IsNullOrWhiteSpace(packagingType)
            ? "standard"
            : packagingType;

        List<CartLine> cart = GetCartLines();
        decimal productsTotal = cart.Sum(c => c.Product.Price * c.Quantity);

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
        PackagingCheckoutOption packaging = CreatePackagingCheckoutOption(packagingType);
        decimal finalTotal = totalAfterDiscount + servicePrice + deliveryPrice + packaging.Price;

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
            packagingPrice: packaging.Price,
            finalTotal: finalTotal,
            orderTypeName: deliveryOrder.OrderType,
            deliveryMethodName: deliveryOrder.GetDeliveryMethodName(),
            preparationResult: preparationResult,
            deliveryResult: deliveryResult,
            wasCalculated: true
        );

        ConfigurePackagingViewData(packaging);

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
        decimal packagingPrice,
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
        ViewBag.PackagingPrice = packagingPrice;
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

        bool alreadyExists = _context.StockSubscriptions.Any(subscription =>
            subscription.ProductId == product.Id &&
            subscription.Email == email);

        if (!alreadyExists)
        {
            _context.StockSubscriptions.Add(new StockSubscription
            {
                ProductId = product.Id,
                CustomerName = customerName,
                Email = email
            });
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(StockAlerts));
    }

    [HttpPost]
    public IActionResult UnsubscribeStockAlert(
        int productId,
        string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            StockSubscription? subscription = _context.StockSubscriptions.FirstOrDefault(existing =>
                existing.Email == email &&
                existing.ProductId == productId);

            if (subscription != null)
            {
                _context.StockSubscriptions.Remove(subscription);
                _context.SaveChanges();
            }
        }

        return RedirectToAction(nameof(Admin));
    }

    public IActionResult ClearStockNotifications()
    {
        _context.StockNotifications.RemoveRange(_context.StockNotifications);
        _context.SaveChanges();

        return RedirectToAction(nameof(Admin));
    }

    private void NotifyStockObservers(StoreProduct product, int oldStock, int newStock)
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

        List<CustomerStockObserver> observers = _context.StockSubscriptions
            .Where(subscription => subscription.ProductId == product.Id)
            .AsEnumerable()
            .Select(subscription => new CustomerStockObserver(
                subscription.ProductId,
                subscription.CustomerName,
                subscription.Email))
            .ToList();

        List<StockNotification> notifications = observers
            .Select(observer => observer.Update(stockEvent))
            .ToList();

        if (notifications.Count > 0)
        {
            List<StockNotificationRecord> records = notifications
                .Select(notification => new StockNotificationRecord
                {
                    ProductId = product.Id,
                    CustomerName = notification.CustomerName,
                    Email = notification.Email,
                    ProductName = notification.ProductName,
                    Message = notification.Message
                })
                .ToList();

            _context.StockNotifications.AddRange(records);
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
            LogAdminCommand(command, isUndo: false);
            _context.SaveChanges();
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
            LogAdminCommand(command, isUndo: false);
            _context.SaveChanges();
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

        IAdminCommand? command = CommandInvoker.UndoLastCommand();

        if (command != null)
        {
            _context.Products.Update(command.Product);
            LogAdminCommand(command, isUndo: true);
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(AdminCommands));
    }

    // ============================================================
    // MEMENTO PATTERN
    // ============================================================

    public IActionResult Memento()
    {
        LoadOutfitDraftFromDatabase();

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
        SaveCurrentOutfitDraft();

        TempData["MementoMessage"] =
            "Tinuta a fost modificata. Pentru a pastra aceasta versiune, apasa Salveaza snapshot.";

        return RedirectToAction(nameof(Memento));
    }

    [HttpPost]
    public IActionResult SaveOutfitSnapshot()
    {
        _outfitHistory.SaveState(_outfitDesigner);
        SaveOutfitSnapshotRecord("Save");

        TempData["MementoMessage"] =
            "Snapshot salvat. Starea curenta a tinutei a fost memorata.";

        return RedirectToAction(nameof(Memento));
    }

    [HttpPost]
    public IActionResult UndoOutfit()
    {
        bool restored = _outfitHistory.Undo(_outfitDesigner);

        if (restored)
        {
            SaveCurrentOutfitDraft();
            SaveOutfitSnapshotRecord("Undo");
        }

        TempData["MementoMessage"] = restored
            ? "Undo realizat. Tinuta a revenit la o stare salvata anterior."
            : "Nu exista snapshot-uri pentru Undo.";

        return RedirectToAction(nameof(Memento));
    }

    [HttpPost]
    public IActionResult RedoOutfit()
    {
        bool restored = _outfitHistory.Redo(_outfitDesigner);

        if (restored)
        {
            SaveCurrentOutfitDraft();
            SaveOutfitSnapshotRecord("Redo");
        }

        TempData["MementoMessage"] = restored
            ? "Redo realizat. Tinuta a fost restaurata inainte."
            : "Nu exista snapshot-uri pentru Redo.";

        return RedirectToAction(nameof(Memento));
    }

    private void ConfigureMementoViewData(string message)
    {
        List<string> snapshotLabels = _context.OutfitSnapshots
            .OrderByDescending(snapshot => snapshot.CreatedAt)
            .Take(10)
            .AsEnumerable()
            .Select(snapshot => $"{snapshot.ActionType} la {snapshot.CreatedAt:HH:mm:ss}")
            .ToList();

        ViewBag.OutfitTop = _outfitDesigner.Top;
        ViewBag.OutfitBottom = _outfitDesigner.Bottom;
        ViewBag.OutfitShoes = _outfitDesigner.Shoes;
        ViewBag.OutfitAccessory = _outfitDesigner.Accessory;
        ViewBag.OutfitColorPalette = _outfitDesigner.ColorPalette;
        ViewBag.OutfitNotes = _outfitDesigner.Notes;

        ViewBag.UndoCount = _outfitHistory.UndoCount;
        ViewBag.RedoCount = _outfitHistory.RedoCount;
        ViewBag.SnapshotLabels = snapshotLabels;

        ViewBag.MementoMessage = TempData["MementoMessage"] ?? message;

        ViewBag.MementoInfo =
            "Memento salveaza starea interna a obiectului OutfitDesigner intr-un snapshot, fara ca View-ul sau Controller-ul sa modifice direct continutul snapshot-ului.";

        ViewBag.MementoRoles =
            "OutfitDesigner este Originator, OutfitDraftMemento este Memento, iar OutfitHistory este Caretaker.";
    }

    private void LoadOutfitDraftFromDatabase()
    {
        OutfitDraftRecord? draft = _context.OutfitDrafts
            .OrderByDescending(item => item.UpdatedAt)
            .FirstOrDefault();

        if (draft == null)
        {
            SaveCurrentOutfitDraft();
            return;
        }

        _outfitDesigner.UpdateOutfit(
            draft.Top,
            draft.Bottom,
            draft.Shoes,
            draft.Accessory,
            draft.ColorPalette,
            draft.Notes
        );
    }

    private void SaveCurrentOutfitDraft()
    {
        OutfitDraftRecord? draft = _context.OutfitDrafts.FirstOrDefault();

        if (draft == null)
        {
            draft = new OutfitDraftRecord();
            _context.OutfitDrafts.Add(draft);
        }

        draft.Top = _outfitDesigner.Top;
        draft.Bottom = _outfitDesigner.Bottom;
        draft.Shoes = _outfitDesigner.Shoes;
        draft.Accessory = _outfitDesigner.Accessory;
        draft.ColorPalette = _outfitDesigner.ColorPalette;
        draft.Notes = _outfitDesigner.Notes;
        draft.UpdatedAt = DateTime.Now;

        _context.SaveChanges();
    }

    private void SaveOutfitSnapshotRecord(string actionType)
    {
        _context.OutfitSnapshots.Add(new OutfitSnapshotRecord
        {
            Top = _outfitDesigner.Top,
            Bottom = _outfitDesigner.Bottom,
            Shoes = _outfitDesigner.Shoes,
            Accessory = _outfitDesigner.Accessory,
            ColorPalette = _outfitDesigner.ColorPalette,
            Notes = _outfitDesigner.Notes,
            ActionType = actionType
        });

        _context.SaveChanges();
    }

    // ============================================================
    // ITERATOR PATTERN
    // ============================================================

    public IActionResult Iterator()
    {
        string role = HttpContext.Session.GetString("role") ?? "Guest";

        if (role != "Manager")
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        OrderReviewService service = new();
        IReadOnlyList<OrderReviewItem> pendingOrders = service.GetPendingOrdersForAdmin();

        ViewBag.IteratorInfo =
            "Iterator permite parcurgerea comenzilor una cate una, fara ca pagina web sau controllerul sa cunoasca structura interna a colectiei.";

        ViewBag.IteratorRoles =
            "OrderReviewCollection este colectia, PendingOrderIterator este iteratorul concret, iar OrderReviewService foloseste iteratorul pentru a returna doar comenzile cu status Pending.";

        ViewBag.IteratorResult =
            $"Iteratorul a parcurs colectia de comenzi si a returnat {pendingOrders.Count} comenzi in asteptare.";

        AddLayoutCounters();

        return View(pendingOrders);
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

    private static string BuildIteratorFilterLabel(
        string? category,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy)
    {
        List<string> filters = new();

        if (!string.IsNullOrWhiteSpace(category))
        {
            filters.Add($"categoria {category}");
        }

        if (minPrice.HasValue && maxPrice.HasValue)
        {
            filters.Add($"pret intre {minPrice.Value} si {maxPrice.Value} MDL");
        }
        else if (minPrice.HasValue)
        {
            filters.Add($"pret minim {minPrice.Value} MDL");
        }
        else if (maxPrice.HasValue)
        {
            filters.Add($"pret maxim {maxPrice.Value} MDL");
        }

        if (sortBy == "colorAsc")
        {
            filters.Add("culoare A-Z");
        }
        else if (sortBy == "colorDesc")
        {
            filters.Add("culoare Z-A");
        }

        return filters.Count == 0
            ? "toate produsele"
            : string.Join(" + ", filters);
    }

    private static void NormalizePriceRange(ref decimal? minPrice, ref decimal? maxPrice)
    {
        if (minPrice.HasValue && minPrice.Value < 0)
        {
            minPrice = 0;
        }

        if (maxPrice.HasValue && maxPrice.Value < 0)
        {
            maxPrice = 0;
        }

        if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
        {
            (minPrice, maxPrice) = (maxPrice, minPrice);
        }
    }

    private static List<string> GetSizeGuideCategories()
    {
        return new List<string>
        {
            "T-Shirts",
            "Hoodies",
            "Jeans",
            "Jackets",
            "Dresses",
            "Shoes"
        };
    }

    private static PrototypeSizeGuide CreateSizeGuideClone(string category)
    {
        SizeGuideRegistry registry = new();

        registry.Register("T-Shirts", new SizeGuidePrototype(new PrototypeSizeGuide(
            "T-Shirts",
            "BlueWear Regular",
            "EU",
            new List<string> { "XS", "S", "M", "L", "XL" },
            new Dictionary<string, string>
            {
                ["Bust"] = "82-90 / 90-98 / 98-106 / 106-114 / 114-122 cm",
                ["Umeri"] = "36-38 / 38-40 / 40-42 / 42-44 / 44-46 cm",
                ["Lungime"] = "60-62 / 62-64 / 64-66 / 66-68 / 68-70 cm"
            },
            "Alege marimea dupa bust; pentru tricouri lejere poti urca o marime."
        )));

        registry.Register("Hoodies", new SizeGuidePrototype(new PrototypeSizeGuide(
            "Hoodies",
            "BlueWear Comfort",
            "EU",
            new List<string> { "XS", "S", "M", "L", "XL", "XXL" },
            new Dictionary<string, string>
            {
                ["Bust"] = "88-96 / 96-104 / 104-112 / 112-120 / 120-128 / 128-136 cm",
                ["Maneca"] = "58-60 / 60-62 / 62-64 / 64-66 / 66-68 / 68-70 cm",
                ["Lungime"] = "63-65 / 65-67 / 67-69 / 69-71 / 71-73 / 73-75 cm"
            },
            "Hanoracele sunt gandite pentru stratificare; lasa spatiu peste tricou."
        )));

        registry.Register("Jeans", new SizeGuidePrototype(new PrototypeSizeGuide(
            "Jeans",
            "BlueWear Denim",
            "EU",
            new List<string> { "26", "28", "30", "32", "34", "36" },
            new Dictionary<string, string>
            {
                ["Talie"] = "66-70 / 70-74 / 74-78 / 78-84 / 84-90 / 90-96 cm",
                ["Sold"] = "88-92 / 92-96 / 96-100 / 100-106 / 106-112 / 112-118 cm",
                ["Interior picior"] = "76 / 78 / 80 / 82 / 84 / 86 cm"
            },
            "Pentru jeans, talia si soldul sunt cele mai importante masuratori."
        )));

        registry.Register("Jackets", new SizeGuidePrototype(new PrototypeSizeGuide(
            "Jackets",
            "BlueWear Layers",
            "EU",
            new List<string> { "S", "M", "L", "XL", "XXL" },
            new Dictionary<string, string>
            {
                ["Bust"] = "92-100 / 100-108 / 108-116 / 116-124 / 124-132 cm",
                ["Umeri"] = "40-42 / 42-44 / 44-46 / 46-48 / 48-50 cm",
                ["Lungime"] = "66-68 / 68-70 / 70-72 / 72-74 / 74-76 cm"
            },
            "Pentru jachete, alege marimea care lasa loc pentru un strat subtire dedesubt."
        )));

        registry.Register("Dresses", new SizeGuidePrototype(new PrototypeSizeGuide(
            "Dresses",
            "BlueWear Studio",
            "EU",
            new List<string> { "XS", "S", "M", "L", "XL" },
            new Dictionary<string, string>
            {
                ["Bust"] = "80-86 / 86-92 / 92-98 / 98-106 / 106-114 cm",
                ["Talie"] = "62-68 / 68-74 / 74-80 / 80-88 / 88-96 cm",
                ["Sold"] = "86-92 / 92-98 / 98-104 / 104-112 / 112-120 cm"
            },
            "Pentru rochii, verifica bustul, talia si soldul impreuna."
        )));

        registry.Register("Shoes", new SizeGuidePrototype(new PrototypeSizeGuide(
            "Shoes",
            "BlueWear Steps",
            "EU",
            new List<string> { "36", "37", "38", "39", "40", "41", "42", "43" },
            new Dictionary<string, string>
            {
                ["Lungime talpa"] = "23.0 / 23.7 / 24.4 / 25.0 / 25.7 / 26.4 / 27.0 / 27.7 cm",
                ["Latime"] = "Standard",
                ["Recomandare"] = "Masora piciorul la finalul zilei pentru precizie mai buna."
            },
            "Pentru incaltaminte, lungimea talpii decide marimea."
        )));

        return registry.GetClone(category);
    }

    private static string NormalizeSizeGuideCategory(string? category)
    {
        return GetSizeGuideCategories().Contains(category)
            ? category!
            : string.Empty;
    }

    private static string NormalizeSizeGuideRegion(string? region)
    {
        return region switch
        {
            "US" => "US",
            "UK" => "UK",
            _ => "EU"
        };
    }

    private static string NormalizeSizeGuideFit(string? fit)
    {
        return fit switch
        {
            "slim" => "slim",
            "relaxed" => "relaxed",
            "oversized" => "oversized",
            _ => "regular"
        };
    }

    private static string GetFitLabel(string fit)
    {
        return fit switch
        {
            "slim" => "Slim Fit",
            "relaxed" => "Relaxed Fit",
            "oversized" => "Oversized Fit",
            _ => "Regular Fit"
        };
    }

    private static List<string> BuildRegionAwareSizes(string category, string region)
    {
        if (category != "Shoes")
        {
            return category switch
            {
                "Jeans" => new List<string> { "26", "28", "30", "32", "34", "36" },
                "Hoodies" => new List<string> { "XS", "S", "M", "L", "XL", "XXL" },
                "Jackets" => new List<string> { "S", "M", "L", "XL", "XXL" },
                _ => new List<string> { "XS", "S", "M", "L", "XL" }
            };
        }

        return region switch
        {
            "US" => new List<string> { "5.5", "6.5", "7.5", "8.5", "9.5", "10.5", "11.5", "12.5" },
            "UK" => new List<string> { "3.5", "4.5", "5.5", "6.5", "7.5", "8.5", "9.5", "10.5" },
            _ => new List<string> { "36", "37", "38", "39", "40", "41", "42", "43" }
        };
    }

    private static string BuildSizeGuideNotes(
        string category,
        string fit,
        decimal? chest,
        decimal? waist,
        decimal? hips,
        decimal? footLength)
    {
        string fitAdvice = fit switch
        {
            "slim" => "Croiala slim sta aproape de corp; daca esti intre doua marimi, alege marimea mai mare.",
            "relaxed" => "Croiala relaxed lasa mai mult spatiu pentru miscare si layering.",
            "oversized" => "Croiala oversized este vizibil lejera; ramai la marimea ta daca vrei efectul dorit.",
            _ => "Croiala regular pastreaza echilibrul intre confort si forma."
        };

        bool hasBodyMeasurements = chest.HasValue || waist.HasValue || hips.HasValue || footLength.HasValue;
        string measurementAdvice = hasBodyMeasurements
            ? "Am luat in calcul masuratorile introduse pentru o recomandare rapida."
            : "Completeaza masuratorile pentru o recomandare mai precisa.";

        string categoryAdvice = category switch
        {
            "Jeans" => "La denim, prioritatea este talia; soldul confirma daca modelul ramane confortabil.",
            "Shoes" => "Pentru pantofi, masoara lungimea piciorului in centimetri si compara cu tabelul.",
            "Dresses" => "Pentru rochii, verifica talia si soldul inainte de a confirma marimea.",
            "Jackets" => "Pentru jachete, lasa 2-4 cm extra daca porti hanorac dedesubt.",
            "Hoodies" => "Pentru hanorace, alege lejer daca preferi maneci si umeri relaxati.",
            _ => "Pentru topuri, bustul si umerii dau cea mai buna orientare."
        };

        return $"{fitAdvice} {measurementAdvice} {categoryAdvice}";
    }

    private static string BuildSizeRecommendation(
        string category,
        string region,
        decimal? chest,
        decimal? waist,
        decimal? hips,
        decimal? footLength)
    {
        if (category == "Shoes" && footLength.HasValue)
        {
            decimal value = footLength.Value;

            if (value <= 23.0m) return ConvertShoeSize("36", region);
            if (value <= 23.7m) return ConvertShoeSize("37", region);
            if (value <= 24.4m) return ConvertShoeSize("38", region);
            if (value <= 25.0m) return ConvertShoeSize("39", region);
            if (value <= 25.7m) return ConvertShoeSize("40", region);
            if (value <= 26.4m) return ConvertShoeSize("41", region);
            if (value <= 27.0m) return ConvertShoeSize("42", region);
            return ConvertShoeSize("43", region) + "+";
        }

        if (category == "Jeans")
        {
            decimal? denimReference = waist ?? hips;

            if (!denimReference.HasValue)
            {
                return "Adauga masuratori";
            }

            decimal denimValue = denimReference.Value;

            if (denimValue <= 70m) return "26";
            if (denimValue <= 74m) return "28";
            if (denimValue <= 78m) return "30";
            if (denimValue <= 84m) return "32";
            if (denimValue <= 90m) return "34";
            return "36+";
        }

        decimal? reference = category switch
        {
            "Dresses" => new[] { chest, waist, hips }.Where(value => value.HasValue).Max(),
            _ => chest ?? waist ?? hips
        };

        if (!reference.HasValue)
        {
            return "Adauga masuratori";
        }

        decimal sizeValue = reference.Value;

        if (sizeValue <= 88m) return "XS";
        if (sizeValue <= 96m) return "S";
        if (sizeValue <= 104m) return "M";
        if (sizeValue <= 112m) return "L";
        if (sizeValue <= 122m) return "XL";
        return "XXL";
    }

    private static string ConvertShoeSize(string euSize, string region)
    {
        return region switch
        {
            "US" => euSize switch
            {
                "36" => "5.5",
                "37" => "6.5",
                "38" => "7.5",
                "39" => "8.5",
                "40" => "9.5",
                "41" => "10.5",
                "42" => "11.5",
                _ => "12.5"
            },
            "UK" => euSize switch
            {
                "36" => "3.5",
                "37" => "4.5",
                "38" => "5.5",
                "39" => "6.5",
                "40" => "7.5",
                "41" => "8.5",
                "42" => "9.5",
                _ => "10.5"
            },
            _ => euSize
        };
    }

    public interface IStoreProductIterator
    {
        int VisitedCount { get; }

        bool HasNext();

        StoreProduct Next();
    }

    public sealed class StoreProductCatalog
    {
        private readonly List<StoreProduct> _products;

        public StoreProductCatalog(List<StoreProduct> products)
        {
            _products = products;
        }

        public IStoreProductIterator CreateIterator(
            string? category,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy)
        {
            return new ProductCatalogIterator(
                _products,
                category,
                minPrice,
                maxPrice,
                sortBy);
        }
    }

    public sealed class ProductCatalogIterator : IStoreProductIterator
    {
        private readonly List<StoreProduct> _products;
        private readonly string? _category;
        private readonly decimal? _minPrice;
        private readonly decimal? _maxPrice;
        private int _position;

        public int VisitedCount { get; private set; }

        public ProductCatalogIterator(
            List<StoreProduct> products,
            string? category,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy)
        {
            _products = SortProducts(products, sortBy);
            _category = string.IsNullOrWhiteSpace(category) ? null : category;
            _minPrice = minPrice;
            _maxPrice = maxPrice;
            _position = 0;
        }

        public bool HasNext()
        {
            while (_position < _products.Count)
            {
                StoreProduct product = _products[_position];

                if (MatchesFilter(product))
                {
                    return true;
                }

                _position++;
                VisitedCount++;
            }

            return false;
        }

        public StoreProduct Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("Nu mai exista produse pentru filtrul selectat.");
            }

            StoreProduct currentProduct = _products[_position];
            _position++;
            VisitedCount++;

            return currentProduct;
        }

        private bool MatchesFilter(StoreProduct product)
        {
            bool matchesCategory = _category == null || product.Category == _category;
            bool matchesMinPrice = !_minPrice.HasValue || product.Price >= _minPrice.Value;
            bool matchesMaxPrice = !_maxPrice.HasValue || product.Price <= _maxPrice.Value;

            return matchesCategory && matchesMinPrice && matchesMaxPrice;
        }

        private static List<StoreProduct> SortProducts(List<StoreProduct> products, string? sortBy)
        {
            return sortBy switch
            {
                "colorAsc" => products
                    .OrderBy(product => product.Color)
                    .ThenBy(product => product.Name)
                    .ToList(),
                "colorDesc" => products
                    .OrderByDescending(product => product.Color)
                    .ThenBy(product => product.Name)
                    .ToList(),
                _ => products.ToList()
            };
        }
    }

    private List<CategoryStyleEditViewModel> GetCategoryStyles()
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
        ViewBag.CartCount = _context.CartItems.Sum(c => (int?)c.Quantity) ?? 0;
        ViewBag.WishlistCount = _context.WishlistItems.Count();
    }

    private List<CartLine> GetCartLines()
    {
        return _context.CartItems
            .Include(item => item.Product)
            .OrderBy(item => item.Id)
            .Select(item => new CartLine(item.Product, item.Quantity))
            .ToList();
    }

    private List<StoreProduct> GetWishlistProducts()
    {
        return _context.WishlistItems
            .Include(item => item.Product)
            .OrderBy(item => item.Id)
            .Select(item => item.Product)
            .ToList();
    }

    private List<IStockObserver> GetStockObservers()
    {
        return _context.StockSubscriptions
            .OrderBy(subscription => subscription.Id)
            .AsEnumerable()
            .Select(subscription => new CustomerStockObserver(
                subscription.ProductId,
                subscription.CustomerName,
                subscription.Email))
            .Cast<IStockObserver>()
            .ToList();
    }

    private List<StockNotification> GetStockNotifications()
    {
        return _context.StockNotifications
            .OrderByDescending(notification => notification.CreatedAt)
            .AsEnumerable()
            .Select(notification => new StockNotification(
                notification.CustomerName,
                notification.Email,
                notification.ProductName,
                notification.Message))
            .ToList();
    }

    private List<ReturnRequestRecord> GetReturnRequests()
    {
        return _context.ReturnRequests
            .OrderBy(request => request.Status == "Inregistrat")
            .ThenByDescending(request => request.CreatedAt)
            .ToList();
    }

    private List<CustomerSupportRequestRecord> GetCustomerSupportRequests()
    {
        return _context.CustomerSupportRequests
            .OrderBy(request => request.Status == "Raspuns trimis")
            .ThenByDescending(request => request.IsUrgent)
            .ThenByDescending(request => request.CreatedAt)
            .ToList();
    }

    private void ConfigureSupportViewData(
        string orderNumber,
        string customerEmail,
        string contactPhone,
        string problemType,
        string preferredSolution,
        string description,
        string attachedImages,
        bool isUrgent,
        bool wasSubmitted,
        string message)
    {
        ViewBag.OrderNumber = orderNumber;
        ViewBag.CustomerEmail = customerEmail;
        ViewBag.ContactPhone = contactPhone;
        ViewBag.ProblemType = problemType;
        ViewBag.PreferredSolution = preferredSolution;
        ViewBag.Description = description;
        ViewBag.AttachedImages = attachedImages;
        ViewBag.IsUrgent = isUrgent;
        ViewBag.SupportWasSubmitted = wasSubmitted;
        ViewBag.SupportMessage = message;
    }

    private static List<string> SplitAttachedImages(string attachedImages)
    {
        if (string.IsNullOrWhiteSpace(attachedImages))
        {
            return new List<string>();
        }

        return attachedImages
            .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToList();
    }

    private static string GetReturnTypeLabel(string returnType)
    {
        return returnType switch
        {
            "size" => "Schimb de marime",
            "defective" => "Produs defect",
            _ => "Rambursare"
        };
    }

    private void LogAdminCommand(IAdminCommand command, bool isUndo)
    {
        _context.AdminCommandLogs.Add(new AdminCommandLog
        {
            ProductId = command.Product.Id,
            CommandName = command.Name,
            Description = command.Description,
            IsUndo = isUndo
        });
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
        StoreProduct Product { get; }

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

        public StoreProduct Product => _product;

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

        public StoreProduct Product => _product;

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

        public IAdminCommand? UndoLastCommand()
        {
            if (_history.Count == 0)
            {
                LastMessage = "Nu exista comenzi pentru Undo.";
                return null;
            }

            IAdminCommand command = _history.Pop();
            command.Undo();

            LastMessage = $"Undo: {command.Description}";
            return command;
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
