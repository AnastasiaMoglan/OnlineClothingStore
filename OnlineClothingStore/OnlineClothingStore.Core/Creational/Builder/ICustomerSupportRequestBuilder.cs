namespace OnlineClothingStore.Creational.Builder;

public interface ICustomerSupportRequestBuilder
{
    ICustomerSupportRequestBuilder Reset();

    ICustomerSupportRequestBuilder SetOrderNumber(string orderNumber);

    ICustomerSupportRequestBuilder SetCustomerEmail(string customerEmail);

    ICustomerSupportRequestBuilder SetProblemType(string problemType);

    ICustomerSupportRequestBuilder SetDescription(string description);

    ICustomerSupportRequestBuilder SetPreferredSolution(string preferredSolution);

    ICustomerSupportRequestBuilder SetContactPhone(string contactPhone);

    ICustomerSupportRequestBuilder MarkAsUrgent();

    ICustomerSupportRequestBuilder AddAttachedImage(string imageName);

    CustomerSupportRequest Build();
}