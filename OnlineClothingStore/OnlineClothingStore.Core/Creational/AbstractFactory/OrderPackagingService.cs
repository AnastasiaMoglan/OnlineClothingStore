namespace OnlineClothingStore.Creational.AbstractFactory;

public sealed class OrderPackagingService
{
    private readonly IOrderPackagingFactory _packagingFactory;

    public OrderPackagingService(IOrderPackagingFactory packagingFactory)
    {
        _packagingFactory = packagingFactory;
    }

    public PackagingResult PreparePackaging()
    {
        IBox box = _packagingFactory.CreateBox();
        ILabel label = _packagingFactory.CreateLabel();
        IInsert insert = _packagingFactory.CreateInsert();

        return new PackagingResult(
            _packagingFactory.GetType().Name,
            box.GetBoxDescription(),
            label.GetLabelDescription(),
            insert.GetInsertDescription()
        );
    }
}