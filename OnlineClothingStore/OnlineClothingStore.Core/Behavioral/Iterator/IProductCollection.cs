namespace OnlineClothingStore.Core.Behavioral.Iterator;

public interface IProductCollection
{
    IProductIterator CreateIterator();
    IProductIterator CreateCategoryIterator(string category);
    IProductIterator CreatePriceIterator(decimal maxPrice);
}