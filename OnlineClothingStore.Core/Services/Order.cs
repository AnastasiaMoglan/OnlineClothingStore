using OnlineClothingStore.Abstractions;
using OnlineClothingStore.Domain;

namespace OnlineClothingStore.Services;

public class Order : IPayable
{
    public Cart Cart { get; }
    public Discount Discount { get; }

    public Order(Cart cart, Discount discount)
    {
        Cart = cart;
        Discount = discount;
    }

    public void ProcessPayment(Payment payment)
    {
        var total = Discount.Apply(Cart.GetTotal());
        payment.Pay(total);
    }
}