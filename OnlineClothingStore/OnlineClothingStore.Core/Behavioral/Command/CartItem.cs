namespace OnlineClothingStore.Core.Behavioral.Command;

public class CartItem
{
    public int ProductId { get; }
    public string ProductName { get; }
    public decimal Price { get; }
    public int Quantity { get; private set; }

    public CartItem(int productId, string productName, decimal price, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int quantity)
    {
        Quantity += quantity;
    }

    public CartItem Clone()
    {
        return new CartItem(ProductId, ProductName, Price, Quantity);
    }
}