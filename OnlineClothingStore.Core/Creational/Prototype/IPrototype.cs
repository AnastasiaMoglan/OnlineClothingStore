namespace OnlineClothingStore.Creational.Prototype;

public interface IPrototype<T>
{
    T ShallowClone();
    T DeepClone();
}