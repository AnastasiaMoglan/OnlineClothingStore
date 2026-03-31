namespace OnlineClothingStore.Abstractions;

public interface IRepository<T>
{
    void Add(T item);
    IEnumerable<T> GetAll();
}