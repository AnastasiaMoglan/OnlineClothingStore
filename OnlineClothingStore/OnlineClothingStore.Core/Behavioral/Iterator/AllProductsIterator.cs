namespace OnlineClothingStore.Core.Behavioral.Iterator;

public class AllProductsIterator : IProductIterator
{
    private readonly IList<Product> _products;
    private int _position = -1;

    public AllProductsIterator(IList<Product> products)
    {
        _products = products;
    }

    public bool HasNext()
    {
        return _position < _products.Count - 1;
    }

    public Product Next()
    {
        _position++;
        return _products[_position];
    }

    public void Reset()
    {
        _position = -1;
    }
}