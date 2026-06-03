namespace OnlineClothingStore.Creational.AbstractFactory;

public interface IOrderPackagingFactory
{
    IBox CreateBox();

    ILabel CreateLabel();

    IInsert CreateInsert();
}