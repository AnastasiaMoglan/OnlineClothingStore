namespace OnlineClothingStore.App.Behavioral.Iterator;

public sealed class OrderReviewService
{
    public IReadOnlyList<OrderReviewItem> GetPendingOrdersForAdmin()
    {
        OrderReviewCollection collection = new();

        collection.AddOrder(new OrderReviewItem(
            1,
            "Ana Popescu",
            "Blue Hoodie",
            799,
            "Pending"));

        collection.AddOrder(new OrderReviewItem(
            2,
            "Maria Rusu",
            "White T-Shirt",
            299,
            "Approved"));

        collection.AddOrder(new OrderReviewItem(
            3,
            "Ion Ceban",
            "Denim Jacket",
            1199,
            "Pending"));

        collection.AddOrder(new OrderReviewItem(
            4,
            "Elena Moraru",
            "Black Dress",
            1499,
            "Cancelled"));

        collection.AddOrder(new OrderReviewItem(
            5,
            "Sofia Lupu",
            "Classic Shirt",
            499,
            "Pending"));

        IOrderIterator iterator = collection.CreateIterator();

        List<OrderReviewItem> result = new();

        while (iterator.HasNext())
        {
            result.Add(iterator.Next());
        }

        return result.AsReadOnly();
    }
}