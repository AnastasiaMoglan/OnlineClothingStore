namespace OnlineClothingStore.Creational.Builder;

public sealed class CustomerSupportRequestBuilder : ICustomerSupportRequestBuilder
{
    private string _orderNumber = string.Empty;
    private string _customerEmail = string.Empty;
    private string _problemType = string.Empty;
    private string _description = string.Empty;
    private string _preferredSolution = string.Empty;
    private string _contactPhone = string.Empty;
    private bool _isUrgent;
    private readonly List<string> _attachedImages = new();

    public ICustomerSupportRequestBuilder Reset()
    {
        _orderNumber = string.Empty;
        _customerEmail = string.Empty;
        _problemType = string.Empty;
        _description = string.Empty;
        _preferredSolution = string.Empty;
        _contactPhone = string.Empty;
        _isUrgent = false;
        _attachedImages.Clear();

        return this;
    }

    public ICustomerSupportRequestBuilder SetOrderNumber(string orderNumber)
    {
        _orderNumber = orderNumber;
        return this;
    }

    public ICustomerSupportRequestBuilder SetCustomerEmail(string customerEmail)
    {
        _customerEmail = customerEmail;
        return this;
    }

    public ICustomerSupportRequestBuilder SetProblemType(string problemType)
    {
        _problemType = problemType;
        return this;
    }

    public ICustomerSupportRequestBuilder SetDescription(string description)
    {
        _description = description;
        return this;
    }

    public ICustomerSupportRequestBuilder SetPreferredSolution(string preferredSolution)
    {
        _preferredSolution = preferredSolution;
        return this;
    }

    public ICustomerSupportRequestBuilder SetContactPhone(string contactPhone)
    {
        _contactPhone = contactPhone;
        return this;
    }

    public ICustomerSupportRequestBuilder MarkAsUrgent()
    {
        _isUrgent = true;
        return this;
    }

    public ICustomerSupportRequestBuilder AddAttachedImage(string imageName)
    {
        if (!string.IsNullOrWhiteSpace(imageName))
        {
            _attachedImages.Add(imageName);
        }

        return this;
    }

    public CustomerSupportRequest Build()
    {
        CustomerSupportRequest request = new(
            _orderNumber,
            _customerEmail,
            _problemType,
            _description,
            _preferredSolution,
            _contactPhone,
            _isUrgent,
            new List<string>(_attachedImages)
        );

        Reset();

        return request;
    }
}