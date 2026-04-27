using System.Text;
using OnlineClothingStore.Abstractions;
using OnlineClothingStore.Creational.AbstractFactory;
using OnlineClothingStore.Creational.Builder;
using OnlineClothingStore.Creational.FactoryMethod;
using OnlineClothingStore.Creational.Prototype;
using OnlineClothingStore.Creational.Singleton;
using OnlineClothingStore.Domain;
using OnlineClothingStore.Infrastructure;
using OnlineClothingStore.Services;

// STRUCTURAL
using OnlineClothingStore.App.Structural.Adapter;
using OnlineClothingStore.App.Structural.Composite;
using OnlineClothingStore.App.Structural.Facade;
using OnlineClothingStore.App.Structural.Flyweight;
using OnlineClothingStore.App.Structural.Decorator;
using OnlineClothingStore.App.Structural.Bridge;
using OnlineClothingStore.App.Structural.Proxy;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // ---------------------------------------------------------
        // Setup (Dependencies)
        // ---------------------------------------------------------
        var repo = new InMemoryProductRepository();

        // Abstract Factory: choose the "family" (VIP vs Regular)
        IStoreKitFactory kitFactory = new VipKitFactory();
        // IStoreKitFactory kitFactory = new RegularKitFactory();

        // Factory Method: choose payment creation strategy
        IPaymentFactory paymentFactory = new CardPaymentFactory();

        var productService = new ProductService(repo, kitFactory);
        var checkoutService = new CheckoutService(paymentFactory);

        // ---------------------------------------------------------
        // 1) ABSTRACT FACTORY DEMO (Product + Discount)
        // ---------------------------------------------------------
        var product = productService.AddProduct("Jacket", 1200m);

        var productBasePrice = product.Price;
        var productConfiguredPrice = product.GetFinalPrice();
        var finalPrice = productService.GetFinalPrice(product);

        var discountValue = productConfiguredPrice - finalPrice;

        Console.WriteLine("=== ABSTRACT FACTORY (Product + Discount) ===");
        Console.WriteLine($"KitFactory used: {kitFactory.GetType().Name}");
        Console.WriteLine($"Product created: {product.GetType().Name} - {product.Name}");
        Console.WriteLine($"Base product price: {productBasePrice:0.00}");
        Console.WriteLine($"Configured product price: {productConfiguredPrice:0.00}");
        Console.WriteLine($"Discount applied: -{discountValue:0.00}");
        Console.WriteLine($"Final price to pay: {finalPrice:0.00}");
        Console.WriteLine();

        // Put product into cart and create order with the same family discount
        var cart = new Cart();
        cart.AddProduct(product);

        var discount = kitFactory.CreateDiscount();
        var order = new Order(cart, discount);

        // ---------------------------------------------------------
        // 2) FACTORY METHOD DEMO (Payment)
        // ---------------------------------------------------------
        Console.WriteLine("=== FACTORY METHOD (Payment creation) ===");
        var payment = paymentFactory.CreatePayment();
        Console.WriteLine($"PaymentFactory used: {paymentFactory.GetType().Name}");
        Console.WriteLine($"Payment created: {payment.GetType().Name}");
        Console.WriteLine();

        Console.WriteLine("=== CHECKOUT ===");
        order.ProcessPayment(payment);
        Console.WriteLine();

        // ---------------------------------------------------------
        // 3) BUILDER DEMO (Complex clothing product)
        // ---------------------------------------------------------
        Console.WriteLine("=== BUILDER (Custom clothing product) ===");

        var builder = new CustomClothingProductBuilder();

        ClothingProduct customProduct = builder
            .Reset()
            .SetName("Custom Street Hoodie")
            .SetPrice(950m)
            .SetSize("XL")
            .SetColor("Gray")
            .SetMaterial("Cotton")
            .AddCustomPrint("Urban Style")
            .EnablePremiumPackaging()
            .Build();

        Console.WriteLine("Custom product built with fluent builder:");
        Console.WriteLine(customProduct);
        Console.WriteLine();

        var director = new ClothingProductDirector(new CustomClothingProductBuilder());

        ClothingProduct basicTShirt = director.BuildBasicTShirt();
        ClothingProduct premiumHoodie = director.BuildPremiumHoodie();

        Console.WriteLine("Products built through Director:");
        Console.WriteLine(basicTShirt);
        Console.WriteLine(premiumHoodie);

        // ---------------------------------------------------------
        // 4) PROTOTYPE DEMO (Shallow copy vs Deep copy)
        // ---------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== PROTOTYPE (Cloning clothing templates) ===");

        var hoodieTemplate = new ClothingProduct(
            "Basic Oversize Hoodie",
            1000m,
            "L",
            "Black",
            "Cotton",
            false,
            null,
            true,
            new List<string> { "oversize", "winter", "premium" }
        );

        IPrototype<ClothingProduct> hoodiePrototype =
            new ClothingProductPrototype(hoodieTemplate);

        var registry = new ProductTemplateRegistry();
        registry.Register("hoodie-template", hoodiePrototype);

        var shallowClone = registry.GetShallowClone("hoodie-template");
        shallowClone.Color = "Red";
        shallowClone.Tags.Add("limited-edition");

        var deepClone = registry.GetDeepClone("hoodie-template");
        deepClone.Color = "Blue";
        deepClone.Tags.Add("new-collection");

        Console.WriteLine("Original template:");
        Console.WriteLine(hoodieTemplate);
        Console.WriteLine();

        Console.WriteLine("Shallow clone:");
        Console.WriteLine(shallowClone);
        Console.WriteLine();

        Console.WriteLine("Deep clone:");
        Console.WriteLine(deepClone);

        // ---------------------------------------------------------
        // 5) SINGLETON DEMO (Global store configuration)
        // ---------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== SINGLETON (Store configuration) ===");

        var config1 = StoreConfiguration.Instance;
        var config2 = StoreConfiguration.Instance;

        config1.Configure("TMPPP Clothing Store", 0.19m, "MDL");

        Console.WriteLine($"Store name: {config1.StoreName}");
        Console.WriteLine($"Tax rate: {config1.TaxRate}");
        Console.WriteLine($"Currency: {config1.Currency}");
        Console.WriteLine();
        Console.WriteLine($"Are config1 and config2 the same instance? {ReferenceEquals(config1, config2)}");

        // ---------------------------------------------------------
        // 6) ADAPTER DEMO (Different payment APIs unified)
        // ---------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== ADAPTER (External payment gateways) ===");

        IExternalPaymentGateway stripeGateway = new StripePaymentAdapter(new StripeApi());
        IExternalPaymentGateway paypalGateway = new PayPalPaymentAdapter(new PayPalApi());

        var externalPaymentService1 = new ExternalPaymentService(stripeGateway);
        var externalPaymentService2 = new ExternalPaymentService(paypalGateway);

        bool stripeResult = externalPaymentService1.Checkout("client1@store.com", 1500m);
        bool paypalResult = externalPaymentService2.Checkout("client2@store.com", 800m);

        Console.WriteLine($"Gateway used: {stripeGateway.GetType().Name}");
        Console.WriteLine($"Stripe payment success: {stripeResult}");

        Console.WriteLine($"Gateway used: {paypalGateway.GetType().Name}");
        Console.WriteLine($"PayPal payment success: {paypalResult}");

        // ---------------------------------------------------------
        // 7) COMPOSITE DEMO (Products + bundles treated uniformly)
        // ---------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== COMPOSITE (Products and clothing bundles) ===");

        var tshirt = new SingleClothingItem("T-Shirt", 300m);
        var jeans = new SingleClothingItem("Jeans", 700m);
        var sneakers = new SingleClothingItem("Sneakers", 1200m);

        var summerOutfit = new ClothingBundle("Summer Outfit");
        summerOutfit.Add(tshirt);
        summerOutfit.Add(jeans);

        var premiumOutfit = new ClothingBundle("Premium Outfit");
        premiumOutfit.Add(summerOutfit);
        premiumOutfit.Add(sneakers);

        premiumOutfit.Display();
        Console.WriteLine($"Total bundle price: {premiumOutfit.GetPrice():0.00} MDL");

        // ---------------------------------------------------------
        // 8) FACADE DEMO (Simplified checkout process)
        // ---------------------------------------------------------
       
        Console.WriteLine();
        Console.WriteLine("=== FACADE (Simplified checkout) ===");

        var facade = new StoreFacade();

        var prices = new List<decimal> { 950m, 800m, 250m };

        var simpleOrder = facade.PlaceOrder("alexei@store.com", prices);

        Console.WriteLine($"Order created with total: {simpleOrder.Total} MDL");
        
        // ---------------------------------------------------------
// 9) FLYWEIGHT DEMO
// ---------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== FLYWEIGHT (Shared catalog styles) ===");

        var styleFactory = new ProductCardStyleFactory();
        var catalogPage = new CatalogPage(styleFactory);

        catalogPage.AddProduct(
            new ProductCardContext("Basic T-Shirt", 299m, "M", "Spring Essentials"),
            "Tops", "Blue", "White", "Montserrat");

        catalogPage.AddProduct(
            new ProductCardContext("Oversize T-Shirt", 349m, "L", "Spring Essentials"),
            "Tops", "Blue", "White", "Montserrat");
        catalogPage.AddProduct(
            new ProductCardContext("Slim Jeans", 799m, "32", "Denim Days"),
            "Bottoms", "Black", "White", "Roboto");

        catalogPage.AddProduct(
            new ProductCardContext("Regular Jeans", 899m, "34", "Denim Days"),
            "Bottoms", "Black", "White", "Roboto");

        foreach (var line in catalogPage.Render())
        {
            Console.WriteLine(line);
        }

        Console.WriteLine($"Carduri totale: {catalogPage.TotalCards}");
        Console.WriteLine($"Stiluri partajate create: {catalogPage.SharedStyles}");
        Console.WriteLine($"Obiecte economisite: {catalogPage.SavedObjects}");
        
        // ---------------------------------------------------------
// 10) DECORATOR DEMO
// ---------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== DECORATOR (Extensible notifications) ===");

        IOrderNotification notification =
            new PushNotificationDecorator(
                new SmsNotificationDecorator(
                    new EmailNotificationDecorator(
                        new BasicOrderNotification())));

        var notificationContext = new NotificationContext(
            "Ana",
            "ana@store.com",
            "+37360000000",
            "device-token-123",
            "Comanda ta a fost expediata.");

        var notificationResult = notification.Send(notificationContext);

        Console.WriteLine("Canale folosite:");
        foreach (var channel in notificationResult.Channels)
        {
            Console.WriteLine(channel);
        }
        
        // ---------------------------------------------------------
// 11) BRIDGE DEMO
// ---------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== BRIDGE (Promotions + renderers) ===");

        Promotion flashSaleMobile = new FlashSalePromotion(
            "Flash Sale Weekend",
            25m,
            new DateTime(2026, 03, 30),
            new MobileAppPromotionRenderer());

        Promotion newCollectionEmail = new NewCollectionPromotion(
            "New Urban Collection",
            "Urban Wave",
            18,
            new EmailPromotionRenderer());

        Console.WriteLine(flashSaleMobile.Publish());
        Console.WriteLine(newCollectionEmail.Publish());

        // ---------------------------------------------------------
// 12) PROXY DEMO
// ---------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== PROXY (Protected supplier costs) ===");

        var realPricingService = new SupplierPricingService();
        var adminProxy = new SupplierPricingProxy(realPricingService, new StoreEmployee("admin1", "Admin"));

        Console.WriteLine($"Cost furnizor JACKET-003: {adminProxy.GetSupplierCost("JACKET-003")} MDL");

        try
        {
            var customerProxy = new SupplierPricingProxy(realPricingService, new StoreEmployee("guest1", "Customer"));
            Console.WriteLine(customerProxy.GetSupplierCost("TSHIRT-001"));
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Proxy blocked access: {ex.Message}");
        }
        
        }
    
    
    }
