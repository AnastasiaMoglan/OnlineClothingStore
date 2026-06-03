namespace OnlineClothingStore.Creational.Builder;

public sealed class SupportRequestDirector
{
    private readonly ICustomerSupportRequestBuilder _builder;

    public SupportRequestDirector(ICustomerSupportRequestBuilder builder)
    {
        _builder = builder;
    }

    public CustomerSupportRequest BuildDamagedProductComplaint(
        string orderNumber,
        string customerEmail)
    {
        return _builder
            .Reset()
            .SetOrderNumber(orderNumber)
            .SetCustomerEmail(customerEmail)
            .SetProblemType("Damaged product")
            .SetDescription("The product arrived damaged and cannot be used.")
            .SetPreferredSolution("Replace the product")
            .MarkAsUrgent()
            .AddAttachedImage("damaged-product-photo.jpg")
            .Build();
    }

    public CustomerSupportRequest BuildWrongSizeComplaint(
        string orderNumber,
        string customerEmail)
    {
        return _builder
            .Reset()
            .SetOrderNumber(orderNumber)
            .SetCustomerEmail(customerEmail)
            .SetProblemType("Wrong size")
            .SetDescription("The delivered size does not match the selected size.")
            .SetPreferredSolution("Exchange for the correct size")
            .Build();
    }

    public CustomerSupportRequest BuildDeliveryDelayComplaint(
        string orderNumber,
        string customerEmail)
    {
        return _builder
            .Reset()
            .SetOrderNumber(orderNumber)
            .SetCustomerEmail(customerEmail)
            .SetProblemType("Delivery delay")
            .SetDescription("The order was not delivered within the estimated delivery time.")
            .SetPreferredSolution("Contact me with updated delivery information")
            .Build();
    }
}