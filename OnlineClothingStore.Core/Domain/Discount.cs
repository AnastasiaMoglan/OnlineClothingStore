namespace OnlineClothingStore.Domain;

public abstract class Discount
{
    public abstract decimal Apply(decimal price);
}
