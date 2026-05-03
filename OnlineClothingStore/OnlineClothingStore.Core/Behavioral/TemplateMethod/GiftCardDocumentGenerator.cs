namespace OnlineClothingStore.App.Behavioral.TemplateMethod;

public class GiftCardDocumentGenerator : StoreDocumentGenerator
{
    protected override string LoadData(string documentId)
    {
        return $"Date certificat cadou {documentId}";
    }

    protected override string GenerateHeader()
    {
        return "GIFT CARD - Online Clothing Store";
    }

    protected override string GenerateContent(string data)
    {
        return $"Certificat cadou generat. {data}. Poate fi folosit la următoarea cumpărătură.";
    }

    protected override string GenerateFooter()
    {
        return "Gift card valabil 12 luni.";
    }

    protected override string Export(string document)
    {
        return $"GIFT CARD PDF:\n{document}";
    }
}