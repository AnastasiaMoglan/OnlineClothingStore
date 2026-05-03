namespace OnlineClothingStore.Core.Behavioral.Iterator;

public class CategoryProductIterator : IProductIterator
{
    private readonly IList<Product> _products;
    private readonly string _category;
    private int _position = -1;

    public CategoryProductIterator(IList<Product> products, string category)
    {
        _products = products;
        _category = category;
    }

    public bool HasNext()
    {
        var nextPosition = _position + 1;

        while (nextPosition < _products.Count &&
               !_products[nextPosition].Category.Equals(_category, StringComparison.OrdinalIgnoreCase))
        {
            nextPosition++;
        }

        return nextPosition < _products.Count;
    }

    public Product Next()
    {
        _position++;

        while (_position < _products.Count &&
               !_products[_position].Category.Equals(_category, StringComparison.OrdinalIgnoreCase))
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