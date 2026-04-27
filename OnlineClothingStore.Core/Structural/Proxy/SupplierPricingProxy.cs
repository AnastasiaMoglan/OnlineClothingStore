namespace OnlineClothingStore.App.Structural.Proxy;

public sealed class SupplierPricingProxy : ISupplierPricingService
{
    private readonly ISupplierPricingService _realService;
    private readonly StoreEmployee _employee;
    private readonly List<string> _auditLog = new();

    public SupplierPricingProxy(ISupplierPricingService realService, StoreEmployee employee)
    {
        _realService = realService;
        _employee = employee;
    }

    public IReadOnlyList<string> AuditLog => _auditLog.AsReadOnly();

    public decimal GetSupplierCost(string sku)
    {
        _auditLog.Add($"{_employee.Username} requested supplier cost for {sku}");

        var hasAccess = _employee.Role == "Admin" || _employee.Role == "Manager";
        if (!hasAccess)
            throw new UnauthorizedAccessException("Acces interzis la costurile furnizorului.");

        return _realService.GetSupplierCost(sku);
    }
}