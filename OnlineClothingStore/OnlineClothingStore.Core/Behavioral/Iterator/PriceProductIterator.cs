namespace OnlineClothingStore.Core.Behavioral.Iterator;

public class PriceProductIterator : IProductIterator
{
    private readonly IList<Product> _products;
    private readonly decimal _maxPrice;
    private int _position = -1;

    public PriceProductIterator(IList<Product> products, decimal maxPrice)
    {
        _products = products;
        _maxPrice = maxPrice;
    }

    public bool HasNext()
    {
        var nextPosition = _position + 1;

        while (nextPosition < _products.Count &&
               _products[nextPosition].Price > _maxPrice)
        {
            nextPosition++;
        }

        return nextPosition < _products.Count;
    }

    public Product Next()
    {
        _position++;

        while (_position < _products.Count &&
               _products[_position].Price > _maxPrice)
        {
            _position++;
        }

        return _products[_position];
    }

    public void Reset()
    {
        _position = -1;
    }
}