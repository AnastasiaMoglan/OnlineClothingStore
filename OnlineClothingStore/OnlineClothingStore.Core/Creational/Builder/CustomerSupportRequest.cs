namespace OnlineClothingStore.Creational.Builder;

public sealed class CustomerSupportRequest
{
    public string OrderNumber { get; }

    public string CustomerEmail { get; }

    public string ProblemType { get; }

    public string Description { get; }

    public string PreferredSolution { get; }

    public string ContactPhone { get; }

    public bool IsUrgent { get; }

    public IReadOnlyList<string> AttachedImages { get; }

    public CustomerSupportRequest(
        string orderNumber,
        string customerEmail,
        string problemType,
        string description,
        string preferredSolution,
        string contactPhone,
        bool isUrgent,
        IReadOnlyList<string> attachedImages)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Order number is required.");

        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email is required.");

        if (string.IsNullOrWhiteSpace(problemType))
            throw new ArgumentException("Problem type is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");

        OrderNumber = orderNumber;
        CustomerEmail = customerEmail;
        ProblemType = problemType;
        Description = description;
        PreferredSolution = string.IsNullOrWhiteSpace(preferredSolution)
            ? "Contact me with a solution"
            : preferredSolution;

        ContactPhone = string.IsNullOrWhiteSpace(contactPhone)
            ? "Not provided"
            : contactPhone;

        IsUrgent = isUrgent;
        AttachedImages = attachedImages;
    }

    public override string ToString()
    {
        string urgency = IsUrgent ? "Urgent" : "Normal";
        string images = AttachedImages.Count == 0
            ? "No attachments"
            : string.Join(", ", AttachedImages);

        return $"Order: {OrderNumber} | Email: {CustomerEmail} | Problem: {ProblemType} | " +
               $"Solution: {PreferredSolution} | Phone: {ContactPhone} | Priority: {urgency} | " +
               $"Images: {images}";
    }
}