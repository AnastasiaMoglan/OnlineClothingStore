namespace OnlineClothingStore.Domain;

public abstract class Payment
{
    public abstract void Pay(decimal amount);
}