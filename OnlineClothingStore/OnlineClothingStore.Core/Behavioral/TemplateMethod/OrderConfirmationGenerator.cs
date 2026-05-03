namespace OnlineClothingStore.App.Behavioral.TemplateMethod;

public class OrderConfirmationGenerator : StoreDocumentGenerator
{
    protected override string LoadData(string documentId)
    {
        return $"Date confirmare pentru comanda {documentId}";
    }

    protected override string GenerateHeader()
    {
        return "CONFIRMARE COMANDĂ";
    }

    protected override string GenerateContent(string data)
    {
        return $"Comanda a fost confirmată. {data}. Clientul va primi detaliile livrării.";
    }

    protected override string GenerateFooter()
    {
        return "Mulțumim că ai ales magazinul nostru.";
    }

    protected override string Export(string document)
    {
        return $"EMAIL CONFIRMATION:\n{document}";
    }
}